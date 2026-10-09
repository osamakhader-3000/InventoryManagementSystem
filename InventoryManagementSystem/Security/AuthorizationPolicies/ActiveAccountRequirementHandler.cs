using Microsoft.AspNetCore.Authorization;
using SecurityApplicationCore.Models;

namespace SecurityApplicationPresentation.AuthorizationPolicies
{
    public class ActiveAccountHandler : AuthorizationHandler<ActiveAccountRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            ActiveAccountRequirement requirement)
        {
            var claim = context.User.FindFirst(nameof(UserAccountStatus));

            if (claim != null && claim.Value == ((byte)UserAccountStatus.Active).ToString())
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
