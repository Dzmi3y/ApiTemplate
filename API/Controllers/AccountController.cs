using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Core.DTOs;
using Core.Interfaces;
using Database.Entities;
using Microsoft.AspNetCore.Identity;
using API.Contracts.Requests;
using API.Contracts.Responses;
using Core.Models;




namespace API.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]

    public class AccountController : ApiControllerBase
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IIssueTokenService _issueTokenService;
        private readonly IRefreshTokenService _refreshTokenService;

        public AccountController(
            ILogger<AccountController> logger,
            IConfiguration configuration,
            IUserService userService,
            IPasswordHasher<User> passwordHasher,
            IIssueTokenService issueTokenService,
            IRefreshTokenService refreshTokenService
            )
        {
            _logger = logger;
            _configuration = configuration;
            _userService = userService;
            _passwordHasher = passwordHasher;
            _issueTokenService = issueTokenService;
            _refreshTokenService = refreshTokenService;
        }

        [AllowAnonymous]
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK)]
        [SwaggerResponse(StatusCodes.Status409Conflict, Type = typeof(string))]
        [SwaggerOperation(Summary = "Registration by email")]
        public async Task<IActionResult> SignUp([FromBody] SignUpRequest request)
        {
            var existing = await _userService.GetUserByEmailAsync(request.Email);
            if (existing != null)
                return Conflict("User already exists");

            var passwordHash = _passwordHasher.HashPassword(null, request.Password);
            var confirmationToken = Guid.NewGuid().ToString();
            var userId = await _userService.RegisterAccountAsync(new AccountUserDto
            {
                Email = request.Email,
                PasswordHash = passwordHash,
                Name = request.Name,
                ConfirmationToken = confirmationToken
            });

            return Ok();
        }

        [NonAction]
        private IActionResult CreateAuthResponse(AuthenticationResult tokenIssueServiceResponse)
        {
            if (tokenIssueServiceResponse.Error.HasValue)
                return Conflict(tokenIssueServiceResponse.Error.ToString());

            return Ok(new AuthSuccessResponse
            {
                AccessToken = tokenIssueServiceResponse.AccessToken,
                ExpiresIn = tokenIssueServiceResponse.ExpiresIn,
                RefreshToken = tokenIssueServiceResponse.RefreshToken
            });
        }


        [AllowAnonymous]
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(AuthSuccessResponse))]
        [SwaggerResponse(StatusCodes.Status409Conflict, Type = typeof(string))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Type = typeof(string))]
        [SwaggerOperation(Summary = "Endpoint for log in")]
        public async Task<IActionResult> LocalSignIn([FromBody] SignInRequest request)
        {
            var user = await _userService.GetUserByEmailAsync(request.Email);

            if (user == null)
                return NotFound("User does not exist");

            if (!user.Confirmed)
                return Conflict("Registration wasn't completed");

            var passwordVerifyResult = _passwordHasher.VerifyHashedPassword(null, user.PasswordHash, request.Password);
            if (passwordVerifyResult == PasswordVerificationResult.Failed)
                return Conflict("Password is wrong");

            var tokenIssueServiceResponse = await _issueTokenService.GenerateAuthenticationResult(user);
            return CreateAuthResponse(tokenIssueServiceResponse);
        }

        [HttpPost]
        [AllowAnonymous] 
        [SwaggerResponse(StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Log out from the system")]
        public async Task<IActionResult> SignOut([FromBody] SignOutRequest request)
        {
            await _refreshTokenService.SetAsInvalidatedAsync(request.RefreshToken);
            return Ok();
        }

        [HttpPost]
        [AllowAnonymous]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(AuthSuccessResponse))]
        [SwaggerResponse(StatusCodes.Status409Conflict, Type = typeof(string))]
        [SwaggerOperation(Summary = "Make a new token (Refresh)")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
        {
            var tokenIssueServiceResponse = await _issueTokenService.RefreshTokenAsync(request.RefreshToken);
            return CreateAuthResponse(tokenIssueServiceResponse);
        }

        [HttpDelete]
        [SwaggerResponse(StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Remove a current user from the system")]
        public async Task<IActionResult> RemoveAccount()
        {
            await _userService.DeleteAccountAsync(UserId);
            return Ok();
        }


    }

}
