using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using SecurityApplicationCore.Models;

namespace SecurityApplicationPresentation.AuthorizationPolicies
{


    public class EmailVerifiedHandler : AuthorizationHandler<EmailVerifiedRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            EmailVerifiedRequirement requirement)
        {
            var claim = context.User.FindFirst(nameof(SecurityApplicationCore.Models.UserEmailVerificationStatus));

            if (claim != null && claim.Value == ((byte)SecurityApplicationCore.Models.UserEmailVerificationStatus.Verified).ToString())
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }

}

