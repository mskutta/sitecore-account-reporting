using Sitecore.Analytics.Aggregation.Data.Model;

namespace Skutta.AccountReporting.Aggregation.Pipelines
{
    public class ContactsByAccountFact : Fact<ContactsByAccountKey, ContactsByAccountValue>
    {
        public ContactsByAccountFact() : base(ContactsByAccountValue.Reduce)
        {

        }

        public override string TableName
        {
            get { return "ContactsByAccount"; }
        }
    }
}
