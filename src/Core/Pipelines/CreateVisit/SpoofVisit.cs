using Sitecore.Analytics.Pipelines.CreateVisits;
using System.Net;

namespace Skutta.AccountReporting.Pipelines.CreateVisit
{
    public class SpoofVisit : CreateVisitProcessor
    {
        public override void Process(CreateVisitArgs args)
        {
            if (args.Request.Params["geoip"] != null)
            {
                var geoip = args.Request.Params["geoip"];
                var ipAddress = IPAddress.Parse(geoip);
                args.Interaction.Ip = ipAddress.GetAddressBytes();
            }

            if (args.Request.Params["identifier"] != null)
            {
                var identifier = args.Request.Params["identifier"];
                args.Session.Identify(identifier);
            }
        }
    }
}
