﻿using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Skutta.AccountReporting.Data;
using Sitecore.Analytics.Reporting;
using Sitecore.ExperienceAnalytics.Api.Response;
using Sitecore.Services.Infrastructure.Web.Http;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Web.Http;
using System.Web.Http.Results;

namespace Skutta.AccountReporting.Controllers
{
    public class ContactsByBusinessNameController : ServicesApiController
    {
        private const string REPORT_QUERY = @"
WITH [BaseResults] AS
(
  SELECT TOP 100 f.[ContactId] [Key], SUM(f.[Bounces]) [Bounces], SUM(f.[Conversions]) [Conversions], SUM(f.[Duration]) / 1000 [TimeOnSite], SUM(f.[Value]) [Value], SUM(f.[Views]) [PageViews], SUM(f.[Visits]) [Visits]
  FROM [Fact_ContactsByAccount] f
    JOIN [Accounts] a
      JOIN [DimensionKeys] d ON a.[BusinessName] = d.[DimensionKey]
    ON f.[AccountId] = a.[AccountId]
    JOIN [Contacts] c ON f.[ContactId] = c.[ContactId]
  WHERE d.[DimensionKeyId] = @DimensionKeyId
    AND f.[Date] BETWEEN @StartDate AND @EndDate
    AND c.[IntegrationLevel] > 0
  GROUP BY f.[ContactId]
  ORDER BY [PageViews] DESC
)

SELECT 
  b.*, 
  dbo.GetTagValue([ContactTags], 'Full name') [Name]
FROM [BaseResults] b
  JOIN [Contacts] c ON b.[Key] = c.[ContactId]
";

        [HttpGet]
        public IHttpActionResult Get(string keys = null, string dateFrom = null, string dateTo = null)
        {
            DateTime startDate;
            if (!DateTime.TryParseExact(dateFrom, "dd-MM-yyyy", new CultureInfo("en-US"), DateTimeStyles.None, out startDate))
                startDate = DateTime.MinValue;

            DateTime endDate;
            if (!DateTime.TryParseExact(dateTo, "dd-MM-yyyy", new CultureInfo("en-US"), DateTimeStyles.None, out endDate))
                endDate = DateTime.MaxValue;
            else
                endDate = endDate.Date + new TimeSpan(23, 59, 59);

            var parameters = new Dictionary<string, object>()
            {
                { "@DimensionKeyId", keys },
                { "@StartDate", startDate },
                { "@EndDate", endDate }
            };
            var query = new ReportDataQuery(REPORT_QUERY, parameters);
            var cachingPolicy = new CachingPolicy { ExpirationPeriod = TimeSpan.Zero };
            var response = ReportDataService.ExecuteQuery(query, cachingPolicy, "[Name]", "Unknown");

            var serializerSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
            return new JsonResult<ReportResponse>(response, serializerSettings, Encoding.UTF8, this);
        }
    }
}
