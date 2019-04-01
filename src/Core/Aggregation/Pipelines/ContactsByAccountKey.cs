using System;
using Sitecore.Analytics.Aggregation.Data.Model;

namespace Skutta.AccountReporting.Aggregation.Pipelines
{
    public class ContactsByAccountKey : DictionaryKey
    {
        public Guid AccountId { get; set; }
        public Guid ContactId { get; set; }
        public DateTime Date { get; set; }
    }
}
