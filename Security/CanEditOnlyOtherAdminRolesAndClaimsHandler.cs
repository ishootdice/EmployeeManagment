using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EmployeeManagment.Security
{
    public class CanEditOnlyOtherAdminRolesAndClaimsHandler : AuthorizationHandler<ManageAdminRolesAndClaimsRequirement>
    {
        private readonly IHttpContextAccessor contextAccessor;

        public CanEditOnlyOtherAdminRolesAndClaimsHandler(IHttpContextAccessor contextAccessor)
        {
            this.contextAccessor = contextAccessor;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            ManageAdminRolesAndClaimsRequirement requirement)
        {
            var loggedInAdminId = context.User.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value.ToString();

            string request = contextAccessor.HttpContext.Request.Path.Value;

            string adminIdBeingEdited = request.Split('/')[3];

            if (context.User.IsInRole("Admin")
                 && context.User.HasClaim(c => c.Type == "Edit Role" && c.Value == "true")
                 && adminIdBeingEdited.ToLower() != loggedInAdminId.ToLower())
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}

