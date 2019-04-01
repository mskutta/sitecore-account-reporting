using System;
using Sitecore.Analytics.Aggregation.Data.Model;
using Sitecore.ExperienceAnalytics.Aggregation.Dimensions;

namespace Skutta.AccountReporting.Aggregation.Dimensions
{
    public class ByBusinessName : VisitDimensionBase
    {
        public ByBusinessName(Guid dimensionId)
            : base(dimensionId)
        {
        }
        public override string GetKey(IVisitAggregationContext context)
        {
            return (context.Visit.GeoData != null && !string.IsNullOrEmpty(context.Visit.GeoData.BusinessName)) ? context.Visit.GeoData.BusinessName : null;
        }

        public override bool HasDimensionKey(IVisitAggregationContext context)
        {
            return (context.Visit.GeoData != null && !string.IsNullOrEmpty(context.Visit.GeoData.BusinessName));
        }
    }
}
