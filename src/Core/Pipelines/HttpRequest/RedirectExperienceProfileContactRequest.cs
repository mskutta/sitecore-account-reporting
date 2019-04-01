using Sitecore.Pipelines.HttpRequest;
using System;

namespace Skutta.AccountReporting.Pipelines.HttpRequest
{
    public class RedirectExperienceProfileContactRequest
    {
        public void Process(HttpRequestArgs args)
        {
            if (args.Url.FilePath.IndexOf("/sitecore/client/Applications/ExperienceProfile/contact", StringComparison.CurrentCultureIgnoreCase) == -1)
                return;

            if (args.Url.QueryString.IndexOf("key=", StringComparison.CurrentCultureIgnoreCase) == -1)
                return;

            // Handle coming from Experience Analytics where key is used on the querystring
            var url = args.Url.FilePathWithQueryString.Replace("key", "cid");

            args.Context.Response.Redirect(url);
            args.AbortPipeline();
        }
    }
}
