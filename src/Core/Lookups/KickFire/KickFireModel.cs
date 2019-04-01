using Newtonsoft.Json;
using System.Collections.Generic;

namespace Skutta.AccountReporting.Lookups.KickFire
{
    public class KickFireModel
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("results")]
        public int Results { get; set; }

        [JsonProperty("data")]
        public List<KickFireCompanyModel> Data { get; set; }
    }
}
