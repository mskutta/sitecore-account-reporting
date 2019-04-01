using Newtonsoft.Json;
using Sitecore.Analytics.Model;
using Sitecore.CES.Client;
using Sitecore.CES.Discovery;
using Sitecore.CES.GeoIp;
using Sitecore.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Skutta.AccountReporting.Lookups.KickFire
{
    public class KickFireProvider : SitecoreProvider
    {
        private List<string> _countries = new List<string>();

        public KickFireProvider(ResourceConnector<WhoIsInformation> geoIpConnector) : base(geoIpConnector)
        {
        }

        public KickFireProvider(ResourceConnector<WhoIsInformation> geoIpConnector, EndpointSource endpointSource) : base(geoIpConnector, endpointSource)
        {
        }

        public void AddCountry(string country)
        {
            _countries.Add(country);
        }

        public string ApiUrl { get; set; }

        public string ApiKey { get; set; }

        public override WhoIsInformation GetInformationByIp(string ip)
        {
            Assert.ArgumentNotNullOrEmpty(ip, "ip");

            // Retrieve from cache
            KickFireGeoIpCache geoIpCache = KickFireGeoIpCacheManager.GeoIpCache;
            var whoIsInformation = geoIpCache.Get(ip);
            if (whoIsInformation != null)
                return whoIsInformation;

            // Retrieve from Sitecore Provider
            whoIsInformation = base.GetInformationByIp(ip);

            // Skip further processing if not in configured country
            if (!_countries.Any() || _countries.Contains(whoIsInformation.Country))
            {
                // Suppliment with KickFire
                var company = GetCompany(ip);
                if (company != null)
                {
                    // Suppliment with business details
                    whoIsInformation.BusinessName = company.Name;
                    if (company.IsISP == 1)
                        whoIsInformation.Isp = company.Name;
                    whoIsInformation.Dns = company.Website;
                    whoIsInformation.Url = company.Website;
                }
            }
                    
            geoIpCache.Add(ip, whoIsInformation);
            return whoIsInformation;
        }

        private KickFireCompanyModel GetCompany(string ip)
        {
            if (ip == "127.0.0.1")
                return null;

            Assert.IsNotNullOrEmpty(ApiUrl, "ApiUrl cannot be empty. Please update the settings file.");
            Assert.IsNotNullOrEmpty(ApiKey, "ApiKey cannot be empty. Please update the settings file.");

            var url = string.Format("{0}?ip={1}&key={2}", ApiUrl, ip, ApiKey);

            Log.Info("KickFire: Calling api for " + ip, "KickFire");
            try
            {
                var webRequest = WebRequest.Create(url);

                string json;
                using (var response = webRequest.GetResponse())
                {
                    using (var streamReader = new StreamReader(response.GetResponseStream(), new UTF8Encoding()))
                    {
                        json = streamReader.ReadToEnd();
                        Log.Info("KickFire: API call for " + ip + ": " + json, "KickFire");
                        var kickFire = JsonConvert.DeserializeObject<KickFireModel>(json);
                        return kickFire.Data.FirstOrDefault();
                    }
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
