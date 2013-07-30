using System.Web.Mvc;
using System.Xml.Linq;
using Raven.Client;
using Rhino.ServiceBus;
using TimeMangement.Helpers;
using TimeMangement.Tasks;

namespace TimeMangement.Controllers
{
    public abstract class RavenController : Controller
    {
        public static IOnewayBus Bus { get; set; }
        public IDocumentSession RavenSession { get; protected set; }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            RavenSession = DocumentStoreHolder.Store.OpenSession(DocumentStoreHolder.TimeMangement);
            RavenSession.Advanced.UseOptimisticConcurrency = true;
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.IsChildAction)
                return;

           // ViewBag.MainMenu = new MenuHelper(Url, RavenSession).MainMenu();

            using (RavenSession)
            {
                if (filterContext.Exception != null)
                    return;

                if (RavenSession != null)
                    RavenSession.SaveChanges();
            }

            TaskExecutor.StartExecuting();
        }

        protected HttpStatusCodeResult HttpNotModified()
        {
            return new HttpStatusCodeResult(304);
        }

        //protected ActionResult Xml(XDocument xml, string etag)
        //{
        //    return new XmlResult(xml, etag);
        //}
    }
}