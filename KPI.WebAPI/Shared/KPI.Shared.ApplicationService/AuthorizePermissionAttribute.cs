using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPI.Shared.ApplicationService
{
    public class AuthorizePermissionAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string _permission;
        public AuthorizePermissionAttribute(string permission) => _permission = permission;

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var userClaims = context.HttpContext.User;
            if (!userClaims.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var permissionsClaim = userClaims.Claims
                .FirstOrDefault(c => c.Type == "permissions")?.Value;

            if (string.IsNullOrEmpty(permissionsClaim) ||
                !permissionsClaim.Split(',').Contains(_permission))
            {
                context.Result = new ForbidResult();
            }
        }
    }

}
