using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Play.Inventory.Service.Auth {
    public class MatchingUserIdRequirement : AuthorizationHandler<MatchingUserIdRequirement>, IAuthorizationRequirement {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MatchingUserIdRequirement requirement) {
            // id from jwt token
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if(userId == null) {
                context.Fail();
                return Task.CompletedTask;
            }

            // get resourceUserId from request
            if (context.Resource is HttpContext httpContext) {
                var resourceUserId = httpContext.Request.Query["userId"];

                // jwt user id matches id of the resource owner
                if(resourceUserId == userId) {
                    context.Succeed(requirement);
                } else {
                    context.Fail();
                }
            } else {
                context.Fail();
            }

            return Task.CompletedTask;
        }
    }
}