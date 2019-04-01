using Sitecore.Analytics;
using Sitecore.Diagnostics;
using System.Web.Mvc;

namespace Skutta.AccountReporting.Controllers
{
    public class VisitController : Controller
    {
        [HttpGet]
        public JsonResult GeoData()
        {
            var currentTracker = Tracker.Current;
            var geoData = currentTracker.Interaction.GeoData;
            return Json(geoData, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult End()
        {
            Session.Abandon();
            Log.Info("Closing xDB session", this);
            return Json(new { Message = "xDB Session Closed" }, JsonRequestBehavior.AllowGet);
        }
    }
}
