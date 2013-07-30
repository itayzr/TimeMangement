using System;
using System.Net.Sockets;
using Raven.Client;
using Raven.Client.Document;

namespace TimeMangement.Helpers
{
    public class DocumentStoreHolder
    {
        private static IDocumentStore _store;
        public static IDocumentStore Store
        {
            get
            {
                if (_store == null)
                    throw new InvalidOperationException("RavenDB is not reachable!");
                return _store;
            }
            set { _store = value; }
        }

        public const string BuildsOld = "builds.HibernatingRhinos.com";
        public const string CompanyWebsite = "HibernatingRhinos.com";
        public const string Orders = "HibernatingRhinos.Orders";
        public const string TimeMangement = "TimeMangement";

        public static void Initialize()
        {
            try
            {
                Store = new DocumentStore { Url = "http://localhost:8080" };
              //  SetupConventions(Store.Conventions);

                Store.Initialize();

        //        IndexCreation.CreateIndexes(typeof(ContentSearchIndex).Assembly, Store);

            //    ConfigureVersioning();
            }
            catch (Exception e)
            {
                if (RedirectToErrorPageIfRavenDbIsDown(e) == false)
                    throw;
            }
        }

        //private static void ConfigureVersioning()
        //{
        //    using (var s = _store.OpenSession())
        //    {
        //        var item = s.Load<dynamic>("Raven/Versioning/DefaultConfiguration");
        //        if (item != null) return;

        //        s.Store(new
        //        {
        //            Exclude = true,
        //            Id = "Raven/Versioning/DefaultConfiguration",
        //        });
        //        s.SaveChanges();
        //    }
        //}

        //private static void SetupConventions(DocumentConvention conventions)
        //{
        //    conventions.RegisterIdConvention<BuildDownload>((s, commands, entity) => string.Format("{0}/{1}/{2}", conventions.GetTypeTagName(entity.GetType()), entity.ProductName, entity.Version));
        //    conventions.RegisterIdConvention<TeamCityBuild>((s, commands, entity) => string.Format("{0}/{1}/{2}", conventions.GetTypeTagName(entity.GetType()), entity.BuildName, entity.Version));
        //    conventions.RegisterIdConvention<Project>((s, commands, entity) => string.Format("{0}/{1}", conventions.GetTypeTagName(entity.GetType()), entity.TeamCityId));
        //    conventions.RegisterIdConvention<ProductContent>((s, commands, content) => string.Format("products/{0}/learn/{1}/{2}", content.Product, content.ContentType, content.Slug));
        //    conventions.RegisterIdConvention<Product>((s, commands, product) => string.Format("products/{0}", product.ShortName.ToLowerInvariant()));

        //    var generator = new MultiTypeHiLoKeyGenerator(5);
        //    conventions.DocumentKeyGenerator = (s, commands, entity) =>
        //    {
        //        var product = entity as Product;
        //        if (product != null)
        //        {
        //            var id = Store.Conventions.GetTypeTagName(entity.GetType()) + "/" + product.ShortName;
        //            return id;
        //        }
        //        return generator.GenerateDocumentKey(commands, conventions, entity);
        //    };
        //}

        public static void Shutdown()
        {
            if (_store != null && !_store.WasDisposed)
                _store.Dispose();
        }

        private static bool RedirectToErrorPageIfRavenDbIsDown(Exception e)
        {
            var socketException = e.InnerException as SocketException;
            if (socketException == null)
                return false;

            switch (socketException.SocketErrorCode)
            {
                case SocketError.AddressNotAvailable:
                case SocketError.NetworkDown:
                case SocketError.NetworkUnreachable:
                case SocketError.ConnectionAborted:
                case SocketError.ConnectionReset:
                case SocketError.TimedOut:
                case SocketError.ConnectionRefused:
                case SocketError.HostDown:
                case SocketError.HostUnreachable:
                case SocketError.HostNotFound:
                    return true;
                default:
                    return false;
            }
        }

        public static IDocumentSession OpenSession()
        {
            var session = Store.OpenSession();
            session.Advanced.UseOptimisticConcurrency = true;
            return session;
        }
    }
}