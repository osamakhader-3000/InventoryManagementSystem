using Microsoft.AspNetCore.Authorization;
using SecurityApplicationCore.Models;

namespace SecurityApplicationPresentation.AuthorizationPolicies
{
    public class AdminLevelRequirementHandler : AuthorizationHandler<AdminLevelRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminLevelRequirement requirement)
        {
            var claim = context.User.FindFirst(nameof(UserRole));

            if (claim != null && claim.Value == ((byte)UserRole.Administrator).ToString())
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
