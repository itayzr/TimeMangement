using System;
using Raven.Client;
using TimeMangement.Models;

namespace TimeMangement.Helpers
{
    public static class AccountHelpers
    {
        public static User GetUser(this IDocumentSession session, string login, bool cache = true)
        {
            User user;
            var disposable = cache
                                ? session.Advanced.DocumentStore.AggressivelyCacheFor(TimeSpan.FromMinutes(5))
                                : session.Advanced.DocumentStore.DisableAggressiveCaching();

            using (disposable)
            {
                user = session.Load<User>(User.FullId(login));
            }
            return user;
        }
    }
}