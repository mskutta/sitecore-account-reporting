using System.Web.Mvc;
using System.Web.Routing;
using Sitecore.Pipelines;

namespace Skutta.AccountReporting.Pipelines.Initialize
{
    public class InitializeRoutes : Sitecore.Mvc.Pipelines.Loader.InitializeRoutes
    {
        public override void Process(PipelineArgs args)
        {
            RouteTable.Routes.MapRoute(
                "VisitGeoData", // Route name
                "api/visit/GeoData",
                new { controller = "Visit", action = "GeoData" },
                new[] { "Skutta.AccountReporting.Controllers" });

            RouteTable.Routes.MapRoute(
                "VisitEnd",
                "api/visit/End",
                new { controller = "Visit", action = "End" },
                new[] { "Skutta.AccountReporting.Controllers" });
        }
    }
}
