using Sitecore.Analytics.Reporting;
using Sitecore.Common;
using Sitecore.ExperienceAnalytics.Api;
using Sitecore.ExperienceAnalytics.Api.Response;
using System;
using System.Collections.Generic;
using System.Data;

namespace Skutta.AccountReporting.Data
{
    // Modeled after internal: Sitecore.ExperienceAnalytics.Api.ReportDataService
    public class ReportDataService
    {
        public static ReportResponse ExecuteQuery(ReportDataQuery queryData, CachingPolicy cachingPolicy, string keyTranslationField = null, string keyTranslationDefault = null)
        {
            var reportResponse = new ReportResponse();
            var totalRecordCount = 0;
            var translations = new Dictionary<string, string>();

            var reportingDataProvider = ApiContainer.Configuration.GetReportingDataProvider();

            var reportRows = new List<ReportRow>();
            var dataTable = reportingDataProvider.GetData("reporting", queryData, cachingPolicy).GetDataTable();
            foreach (DataRow dataRow in dataTable.Rows)
            {
                var key = GetStringValue(dataRow, "[Key]");
                var dimensionKeyId = GetInt64Value(dataRow, "[DimensionKeyId]");
                var bounces = GetIntValue(dataRow, "[Bounces]");
                var conversions = GetIntValue(dataRow, "[Conversions]");
                var timeOnSite = GetInt64Value(dataRow, "[TimeOnSite]");
                var value = GetIntValue(dataRow, "[Value]");
                var pageViews = GetIntValue(dataRow, "[PageViews]");
                var visits = GetIntValue(dataRow, "[Visits]");
                var date = GetDateValue(dataRow, "[Date]");
                var count = GetIntValue(dataRow, "[Count]");

                var reportRow = new ReportRow
                {
                    Key = key,
                    DimensionKeyId = dimensionKeyId,
                    Bounces = bounces,
                    Conversions = conversions,
                    TimeOnSite = timeOnSite,
                    PageViews = pageViews,
                    Visits = visits,
                    Value = value,
                    Date = date,
                    Count = count,
                    ValuePerVisit = Math.Round((double)value / visits, 2),
                    BounceRate = Math.Round((double)bounces / visits, 4),
                    AvgVisitDuration = Math.Round((double)timeOnSite / visits, 2),
                    ConversionRate = Math.Round((double)conversions / visits, 2),
                    AvgVisitPageViews = Math.Round((double)pageViews / visits, 2),
                    AvgVisitCount = Math.Round((double)count / visits, 2),
                    AvgPageCount = Math.Round((double)count / pageViews, 2),
                    AvgCountValue = (count == 0) ? 0 : Math.Round((double)value / count, 2)
                };
                reportResponse.Data.AddRow(reportRow);

                if (keyTranslationField != null)
                {
                    var translation = GetStringValue(dataRow, keyTranslationField) ?? keyTranslationDefault;
                    translations.Add(reportRow.Key, translation);
                }

                totalRecordCount++;
            }

            if (keyTranslationField != null)
                reportResponse.Data.Localization.AddField("key", translations);
            reportResponse.TotalRecordCount = totalRecordCount;
            reportResponse.TimeResolution = "c"; // w

            return reportResponse;
        }

        private static DateTime? GetDateValue(DataRow row, string field)
        {
            var name = TrimName(field);
            if (!row.Table.Columns.Contains(name))
                return null;

            object item = row[name];
            if (DBNull.Value.Equals(item))
                return null;
            return new DateTime?(((DateTime)item).SpecifyKind(DateTimeKind.Local));
        }

        private static double GetDoubleValue(DataRow row, string field, int numberOfPlaces = 2)
        {
            var name = TrimName(field);
            if (!row.Table.Columns.Contains(name))
                return 0;

            object item = row[name];
            return Math.Round((DBNull.Value.Equals(item) ? 0 : Convert.ToDouble(item)), numberOfPlaces);
        }

        private static Guid GetGuidValue(DataRow row, string field)
        {
            var name = TrimName(field);
            if (!row.Table.Columns.Contains(name))
                return Guid.Empty;

            object item = row[name];
            if (DBNull.Value.Equals(item))
                return Guid.Empty;
            return (Guid)item;
        }

        private static long GetInt64Value(DataRow row, string field)
        {
            var name = TrimName(field);
            if (!row.Table.Columns.Contains(name))
                return 0;

            object item = row[name];
            if (!DBNull.Value.Equals(item))
                return Convert.ToInt64(item);
            return 0;
        }

        private static int GetIntValue(DataRow row, string field)
        {
            var name = TrimName(field);
            if (!row.Table.Columns.Contains(name))
                return 0;

            object item = row[name];
            if (DBNull.Value.Equals(item))
                return 0;
            return Convert.ToInt32(item);
        }

        private static string GetStringValue(DataRow row, string field)
        {
            var name = TrimName(field);
            if (!row.Table.Columns.Contains(name))
                return string.Empty;

            object item = row[name];
            if (DBNull.Value.Equals(item))
                return null;

            if (item is Guid)
                return ((Guid)item).ToString("N");
            return (string) item;
        }

        private static string TrimName(string field)
        {
            return field.TrimStart(new char[] { '[' }).TrimEnd(new char[] { ']' });
        }
    }
}
