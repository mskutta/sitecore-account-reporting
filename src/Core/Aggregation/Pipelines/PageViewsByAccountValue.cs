﻿using Sitecore.Analytics.Aggregation.Data.Model;

namespace Skutta.AccountReporting.Aggregation.Pipelines
{
    public class PageViewsByAccountValue : DictionaryValue
    {
        internal static PageViewsByAccountValue Reduce(PageViewsByAccountValue left, PageViewsByAccountValue right)
        {
            var viewValue = new PageViewsByAccountValue();

            viewValue.Bounces = left.Bounces + right.Bounces;
            viewValue.Conversions = left.Conversions + right.Conversions;
            viewValue.Duration = left.Duration + right.Duration;
            viewValue.Value = left.Value + right.Value;
            viewValue.Views = left.Views + right.Views;
            viewValue.Visits = left.Visits + right.Visits;

            return viewValue;
        }

        public long Bounces { get; set; }
        public long Conversions { get; set; }
        public long Duration { get; set; }
        public long Value { get; set; }
        public long Views { get; set; }
        public long Visits { get; set; }
    }
}
