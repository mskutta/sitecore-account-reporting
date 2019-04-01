using Sitecore.Diagnostics;

namespace Skutta.AccountReporting.Lookups.KickFire
{
    internal static class KickFireGeoIpCacheManager
    {
        private static object _syncLock;

        private static KickFireGeoIpCache _geoIpCache;

        public static KickFireGeoIpCache GeoIpCache
        {
            get
            {
                return _geoIpCache;
            }
            set
            {
                Assert.ArgumentNotNull(value, "value");
                lock (_syncLock)
                {
                    _geoIpCache = value;
                }
            }
        }

        static KickFireGeoIpCacheManager()
        {
            _syncLock = new object();
            _geoIpCache = new KickFireGeoIpCache();
        }
    }
}
