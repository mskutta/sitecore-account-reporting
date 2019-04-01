using System.Web.Http;
using Sitecore.Pipelines;

namespace Skutta.AccountReporting.Pipelines.Initialize
{
    public class WebApiInitializer : Sitecore.Mvc.Pipelines.Loader.InitializeRoutes
    {
        public override void Process(PipelineArgs args)
        {
            GlobalConfiguration.Configure(Configure);
        }

        protected void Configure(HttpConfiguration configuration)
        {
            var routes = configuration.Routes;

            routes.MapHttpRoute(
                "ContactsByBusinessName",
                "sitecore/api/ao/aggregates/contactsbybusinessname/{keys}",
                new { controller = "ContactsByBusinessName", action = "Get" });

            routes.MapHttpRoute(
                "PageViewsByBusinessName",
                "sitecore/api/ao/aggregates/pageviewsbybusinessname/{keys}",
                new { controller = "PageViewsByBusinessName", action = "Get" });
        }
    }
}
