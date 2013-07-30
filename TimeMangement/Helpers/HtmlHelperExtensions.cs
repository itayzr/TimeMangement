using System.IO;
using System.Web.Mvc;

namespace TimeMangement.Helpers
{
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString ToJson(this HtmlHelper helper, object instance)
        {
            var stringWriter = new StringWriter();
            DocumentStoreHolder.Store.Conventions.CreateSerializer().Serialize(stringWriter, instance);
            return MvcHtmlString.Create(stringWriter.GetStringBuilder().ToString());
        }
    }
}