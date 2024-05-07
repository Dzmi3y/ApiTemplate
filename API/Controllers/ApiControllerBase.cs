using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;

namespace API.Controllers
{
    public abstract class ApiControllerBase : ControllerBase
    {
        protected Guid UserId => Guid.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub));
    }

}
