using System.Linq;
using Sitecore.Analytics.Aggregation.Data.Model.Dimensions;
using Sitecore.Analytics.Aggregation.Pipeline;
using Sitecore.Diagnostics;

namespace Skutta.AccountReporting.Aggregation.Pipelines
{
    public class ContactsByAccountProcessor : AggregationProcessor
    {
        protected override void OnProcess(AggregationPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");

            var visit = args.Context.Visit;
            if (visit.Pages != null && 0 < visit.Pages.Count)
            {
                var accountId = UpdateAccountDimension(args);
                var contactId = visit.ContactId;
                var items = args.GetDimension<Items>();
                var fact = args.GetFact<ContactsByAccountFact>();

                var key = new ContactsByAccountKey
                {
                    AccountId = accountId,
                    ContactId = contactId,
                    Date = args.DateTimeStrategy.Translate(visit.StartDateTime),
                };
                var value = new ContactsByAccountValue
                {
                    Bounces = (visit.VisitPageCount == 1) ? 1 : 0, // If only this page visited, it is a bounce
                    Conversions = visit.Pages.Sum(page => page.PageEvents.Count(x => x.IsGoal)),
                    Duration = visit.Pages.Sum(page => page.Duration),
                    Value = visit.Value,
                    Views = visit.VisitPageCount,
                    Visits = 1
                };
                fact.Emit(key, value);
            }
        }
    }
}
