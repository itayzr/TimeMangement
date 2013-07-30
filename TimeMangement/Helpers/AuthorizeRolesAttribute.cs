using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using TimeMangement.Models;

namespace TimeMangement.Helpers
{
    /// <summary>
    /// Supports enum roles, by default show the page to registered users of Standard role and up
    /// </summary>
    public class AuthorizeRolesAttribute : AuthorizeAttribute
    {
        private readonly UserRole _role;

        public AuthorizeRolesAttribute(UserRole role)
        {
            _role = role;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (base.AuthorizeCore(httpContext))
            {
                using (var session = DocumentStoreHolder.Store.OpenSession())
                {
                    var user = session.GetUser(httpContext.User.Identity.Name);
                    if (user.Roles != null && user.Roles.Any(role => role == _role))
                        return true;
                }
            }

            return false;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.User.Identity.IsAuthenticated) // user is logged in, but doesn't have perms
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
		                {
		                    {"controller", "Account"},
		                    {"action", "PermissionDenied"},
							{"area", ""},
							{"ReturnUrl", filterContext.HttpContext.Request.RawUrl}
		                });
            }
            else
            {
                filterContext.Result = new HttpUnauthorizedResult(); // intercepted later on and causes a redirect to the login page
            }
        }
    }
}