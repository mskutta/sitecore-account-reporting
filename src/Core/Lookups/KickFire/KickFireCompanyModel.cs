using Newtonsoft.Json;

namespace Skutta.AccountReporting.Lookups.KickFire
{
    public class KickFireCompanyModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("website")]
        public string Website { get; set; }

        [JsonProperty("street")]
        public string Street { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("regionShort")]
        public string RegionShort { get; set; }

        [JsonProperty("region")]
        public string Region { get; set; }

        [JsonProperty("postal")]
        public string Postal { get; set; }

        [JsonProperty("countryShort")]
        public string CountryShort { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("employees")]
        public string Employees { get; set; }

        [JsonProperty("revenue")]
        public string Revenue { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("sicCode")]
        public string SicCode { get; set; }

        [JsonProperty("latitude")]
        public string Latitude { get; set; }

        [JsonProperty("longitude")]
        public string Longitude { get; set; }

        [JsonProperty("stockSymbol")]
        public string StockSymbol { get; set; }

        [JsonProperty("facebook")]
        public string Facebook { get; set; }

        [JsonProperty("twitter")]
        public string Twitter { get; set; }

        [JsonProperty("linkedIn")]
        public string LinkedIn { get; set; }

        [JsonProperty("linkedInID")]
        public string LinkedInID { get; set; }

        [JsonProperty("isISP")]
        public int IsISP { get; set; }

        [JsonProperty("confidence")]
        public int Confidence { get; set; }
    }
}
