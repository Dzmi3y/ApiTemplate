using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;

namespace API.Controllers
{
    public abstract class ApiControllerBase : ControllerBase
    {
        protected Guid UserId => Guid.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub));
    }

}
