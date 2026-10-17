using Microsoft.AspNetCore.Authorization;
using SecurityApplicationCore.Models;

namespace SecurityApplicationPresentation.AuthorizationPolicies
{
    public class AdminLevelRequirementHandler : AuthorizationHandler<AdminLevelRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminLevelRequirement requirement)
        {
            if (context.User.IsInRole(Roles.Admin))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
