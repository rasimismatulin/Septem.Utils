using IdentityServer4.Models;
using IdentityServer4.Services;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Septem.IdentityServer.Api.Services
{
    public class ProfileService : IProfileService
    {
        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var userNameClaim = context.Subject.Claims.FirstOrDefault(x => x.Type == Shared.Claims.UsernameClaim);
            var fullnameClaim = context.Subject.Claims.FirstOrDefault(x => x.Type == Shared.Claims.FullnameClaim);
            var userRoleClaim = context.Subject.Claims.FirstOrDefault(x => x.Type == Shared.Claims.UserRoleClaim);
            var userLanguage = context.Subject.Claims.FirstOrDefault(x => x.Type == Shared.Claims.UserLanguage);

            var claims = new List<Claim>
            {
                userNameClaim, fullnameClaim, userRoleClaim, userLanguage
            };

            context.IssuedClaims.AddRange(claims.Where(x => x != default));

            return Task.CompletedTask;
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            context.IsActive = true;
            return Task.CompletedTask;
        }
    }
}
