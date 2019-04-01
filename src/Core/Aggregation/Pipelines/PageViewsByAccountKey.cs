using System;
using Sitecore.Analytics.Aggregation.Data.Model;

namespace Skutta.AccountReporting.Aggregation.Pipelines
{
    public class PageViewsByAccountKey : DictionaryKey
    {
        public Guid AccountId { get; set; }
        public DateTime Date { get; set; }
        public Guid ItemId { get; set; }
    }
}
