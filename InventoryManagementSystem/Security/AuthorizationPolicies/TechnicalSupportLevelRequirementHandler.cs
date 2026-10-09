using Microsoft.AspNetCore.Authorization;
using SecurityApplicationCore.Models;

namespace SecurityApplicationPresentation.AuthorizationPolicies
{
    public class TechnicalSupportLevelRequirementHandler : AuthorizationHandler<TechnicalSupportLevelRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, TechnicalSupportLevelRequirement requirement)
        {
            var claim = context.User.FindFirst(nameof(UserRole));

            if (claim != null && (claim.Value == ((byte)UserRole.Administrator).ToString() || claim.Value ==  ((byte)UserRole.TechnicalSupport).ToString()))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;

        }
    }
}
