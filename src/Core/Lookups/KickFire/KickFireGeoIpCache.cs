using Sitecore.Analytics.Model;
using Sitecore.Caching;
using Sitecore.CES.GeoIp.Configuration;
using Sitecore.Diagnostics;
using Sitecore.Reflection;

namespace Skutta.AccountReporting.Lookups.KickFire
{
    public class KickFireGeoIpCache : CustomCache
    {
        public KickFireGeoIpCache() : this(Settings.GeoIpCacheSize)
        {
        }

        public KickFireGeoIpCache(long cacheSize) : base("KickFireGeoIpCache", cacheSize)
        {
        }

        public virtual void Add(string ip, WhoIsInformation value)
        {
            Assert.ArgumentNotNull(ip, "ip");
            Assert.ArgumentNotNull(value, "value");
            if (!Enabled)
                return;
            InnerCache.Add(ip, value, Settings.GeoIpCacheExpiryTime);
        }

        public virtual WhoIsInformation Get(string ip)
        {
            Assert.ArgumentNotNull(ip, "ip");
            return base.GetObject(ip) as WhoIsInformation;
        }

        protected virtual long GetDataLength(WhoIsInformation whoIsInformation)
        {
            int num = TypeUtil.SizeOfString(whoIsInformation.AreaCode) + TypeUtil.SizeOfString(whoIsInformation.BusinessName) + TypeUtil.SizeOfString(whoIsInformation.City) + TypeUtil.SizeOfString(whoIsInformation.Country) + TypeUtil.SizeOfString(whoIsInformation.Dns) + TypeUtil.SizeOfString(whoIsInformation.Isp) + TypeUtil.SizeOfString(whoIsInformation.MetroCode) + TypeUtil.SizeOfString(whoIsInformation.PostalCode) + TypeUtil.SizeOfString(whoIsInformation.Region) + TypeUtil.SizeOfString(whoIsInformation.Url) + 16 + 1;
            return (long)num;
        }
    }
}
