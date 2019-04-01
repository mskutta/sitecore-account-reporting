using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Analytics.Aggregation.Data.Model.Dimensions;
using Sitecore.Analytics.Aggregation.Pipeline;
using Sitecore.Diagnostics;

namespace Skutta.AccountReporting.Aggregation.Pipelines
{
    public class PageViewsByAccountProcessor : AggregationProcessor
    {
        protected override void OnProcess(AggregationPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");

            var visit = args.Context.Visit;
            var itemIds = new List<Guid>();
            if (visit.Pages != null && 0 < visit.Pages.Count)
            {
                var accountId = UpdateAccountDimension(args);
                var items = args.GetDimension<Items>();
                var fact = args.GetFact<PageViewsByAccountFact>();

                foreach (var page in visit.Pages)
                {
                    if (page.Item.Id == Guid.Empty)
                        continue;

                    var itemId = items.Add(page.Item.Id, page.Url.Path);

                    var key = new PageViewsByAccountKey
                    {
                        AccountId = accountId,
                        Date = args.DateTimeStrategy.Translate(page.DateTime),
                        ItemId = itemId
                    };
                    var value = new PageViewsByAccountValue
                    {
                        Bounces = (visit.Pages.Count == 1) ? 1 : 0, // If only this page visited, it is a bounce
                        Conversions = page.PageEvents.Count(x => x.IsGoal),
                        Duration = page.Duration,
                        Value = (itemIds.Contains(itemId) ? 0 : visit.Value), // Only add value once to any given page.
                        Views = 1,
                        Visits = (itemIds.Contains(itemId)) ? 0 : 1 // Only add a visit once to any given page.
                    };
                    fact.Emit(key, value);
                    itemIds.Add(itemId);
                }
            }
        }
    }
}
