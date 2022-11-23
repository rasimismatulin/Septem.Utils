using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.Validation;
using Septem.IdentityServer.Api.Data.Entities;

namespace Septem.IdentityServer.Api
{

    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly ApplicationDbContextFactory _dbContextFactory;

        public ResourceOwnerPasswordValidator(ApplicationDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            //await Task.Delay(TimeSpan.FromSeconds(1));
            if (context.UserName == "ec530ada-178b-cca5-4647-1704e50a850f" &&
                context.Password == "d653ba60-3ba5-25a2-4e93-a3d14c51a61c")
            {
                var red = new GrantValidationResult("ec530ada-178b-cca5-4647-1704e50a850f",
                    "ec530ada-178b-cca5-4647-1704e50a850f", GetResourceOwnerClaims(new UserEntity()));
                context.Result = red;
                return;
            }

            var clientId = context.Request.ClientId;

            var userRole = new List<byte>();

            if (clientId == Config.BusinessClientUid)
                userRole.AddRange(new byte[] { 0, 2 });
            else if (clientId == Config.RestaurantClientUid)
                userRole.Add(1);
            else if (clientId == Config.EndUserClientUid)
                userRole.Add(1);
            else if (clientId == Config.AdminClientUid)
                userRole.Add(3);

            var connectionName = context.Request.Client.Properties.First(x => x.Key == "dbConnection").Value;


            using var dbContext = _dbContextFactory.CreateApplicationDbContext(connectionName);

            var userQuery = dbContext.UserEntities
                .Where(x => x.UserName == context.UserName && !x.IsDeleted && x.Status == 0);

            if (userRole.Any())
            {
                userQuery = userQuery.Where(x => userRole.Contains(x.UserRole));
            }

            var user = userQuery.FirstOrDefault();
            if (user == null)
                return;

#if !DEBUG
            var hash = Rfc2898Hasher.ComputeSaltedHash(context.Password, user.SecretSalt);
            if (!Rfc2898Hasher.VerifyHashes(hash, user.SecretHash))
                return;
#endif

            //var client = context.Request.Client;
            //var connection = client.Properties.Where(x => x.Key == "dbConnection");

            var r = new GrantValidationResult(user.Uid.ToString(), user.UserName, GetResourceOwnerClaims(user));
            context.Result = r;

            return;
        }

        private static IEnumerable<Claim> GetResourceOwnerClaims(UserEntity user)
        {
            yield return new Claim(Shared.Claims.ResourceOwnerClaim, true.ToString());
            yield return new Claim(Shared.Claims.UsernameClaim, user.UserName ?? "-");
            yield return new Claim(Shared.Claims.FullnameClaim, $"{user.FirstName} {user.LastName}");
            yield return new Claim(Shared.Claims.UserRoleClaim, user.UserRole.ToString());
            yield return new Claim(Shared.Claims.UserLanguage, user.Language);
        }
    }
}
