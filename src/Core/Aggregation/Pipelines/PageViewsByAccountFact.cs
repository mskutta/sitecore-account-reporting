using Sitecore.Analytics.Aggregation.Data.Model;

namespace Skutta.AccountReporting.Aggregation.Pipelines
{
    public class PageViewsByAccountFact : Fact<PageViewsByAccountKey, PageViewsByAccountValue>
    {
        public PageViewsByAccountFact() : base(PageViewsByAccountValue.Reduce)
        {

        }

        public override string TableName
        {
            get { return "PageViewsByAccount"; }
        }
    }
}
