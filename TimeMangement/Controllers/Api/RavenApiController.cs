using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using Raven.Client;
using Rhino.ServiceBus;
using TimeMangement.Helpers;
using TimeMangement.Tasks;

namespace TimeMangement.Controllers.Api
{
    public class RavenApiController : ApiController
    {
        public static IOnewayBus Bus { get; set; }
        public IDocumentSession RavenSession { get; protected set; }

        public override Task<HttpResponseMessage> ExecuteAsync(HttpControllerContext controllerContext,
                                                               CancellationToken cancellationToken)
        {
            RavenSession = DocumentStoreHolder.Store.OpenSession(DocumentStoreHolder.TimeMangement);
            RavenSession.Advanced.UseOptimisticConcurrency = true;

            return base.ExecuteAsync(controllerContext, cancellationToken)
                       .ContinueWith(task =>
                           {
                               if (task.IsFaulted == false)
                               {
                                   using (RavenSession)
                                   {
                                       if (RavenSession != null)
                                           RavenSession.SaveChanges();
                                   }

                                   TaskExecutor.StartExecuting();
                               }
                               return task;
                           }).Unwrap();
        }
    }
}