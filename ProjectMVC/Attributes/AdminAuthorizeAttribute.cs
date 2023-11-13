using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ProjectMVC.Attributes
{
    public class AdminAuthorizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var userRole = context.HttpContext.Session.GetInt32("UserRole");
            if (!userRole.HasValue || userRole.Value != 2) 
            {
                context.Result = new RedirectToActionResult("Index", "Home", null);
            }
        }
    }
}
