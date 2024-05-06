using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using API.Contracts;
using Core.DTOs;
using Core.Interfaces;
using Database.Entities;
using Microsoft.AspNetCore.Identity;



namespace API.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]

    public class AccountController : ControllerBase
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private readonly IPasswordHasher<User> _passwordHasher;

        public AccountController(
            ILogger<AccountController> logger,
            IConfiguration configuration,
            IUserService userService,
            IPasswordHasher<User> passwordHasher
            )
        {
            _logger = logger;
            _configuration = configuration;
            _userService = userService;
            _passwordHasher = passwordHasher;

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

    }

}
