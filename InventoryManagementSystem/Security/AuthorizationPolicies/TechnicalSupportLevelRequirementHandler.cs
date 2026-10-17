using Microsoft.AspNetCore.Authorization;
using SecurityApplicationCore.Models;

namespace SecurityApplicationPresentation.AuthorizationPolicies
{
    public class TechnicalSupportLevelRequirementHandler : AuthorizationHandler<TechnicalSupportLevelRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, TechnicalSupportLevelRequirement requirement)
        {
            if (context.User.IsInRole(Roles.Admin) || context.User.IsInRole(Roles.TechnicalSupport))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;

        }
    }
}
