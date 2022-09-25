using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Sheca.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class Protect : Attribute, IAuthorizationFilter
    {
     
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            //// skip authorization if action is decorated with [AllowAnonymous] attribute
            //var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
            //if (allowAnonymous)
            //    return;

            //// authorization
            //var user = context.HttpContext.Items["User"] as User;
            //if (user == null)
            //    context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
        }
    }
}
