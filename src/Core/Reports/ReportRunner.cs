using Skutta.AccountReporting.Data;
using Sitecore.Analytics.Reporting;
using Sitecore.Configuration;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.ExperienceAnalytics.Api.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace Skutta.AccountReporting.Reports
{
    public class ReportRunner
    {

        private const string REPORT_QUERY = @"
SELECT TOP 100 [DimensionKeyId], [DimensionKey] [Key], SUM([Visits]) Visits, SUM([Value]) Value, SUM([Bounces]) Bounces, SUM([Conversions]) Conversions, SUM([TimeOnSite]) TimeOnSite, SUM([Pageviews]) Pageviews, SUM([Count]) Count
FROM [ReportDataView]
WHERE [SegmentId] = @SegmentId AND [Date] BETWEEN @StartDate AND @EndDate
GROUP BY [DimensionKeyId], [DimensionKey]
ORDER BY Pageviews DESC
";

        public static void Run(bool isManualRun)
        {
            if (isManualRun)
                Log.Info("Beginning Experience Analytics Report.  This was triggered manually.", typeof(ReportRunner));
            else
                Log.Info("Beginning Experience Analytics Report.  This is a scheduled run.", typeof(ReportRunner));

            try
            {
                using (new ReportContext(language: "en"))
                {
                    var reportsFolder = Sitecore.Context.Database.GetItem("/sitecore/system/Marketing Control Panel/Experience Analytics/Reports");
                    if (reportsFolder == null)
                        throw new ApplicationException("Reports folder could not be found. /sitecore/system/Marketing Control Panel/Experience Analytics/Reports");

                    foreach(var report in reportsFolder.Children.OfType<Item>())
                    {
                        Run(report);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("An unhandled error occurred within the Task", ex, typeof(ReportRunner));
            }
            Log.Info("Exporter Run Complete at " + DateTime.Now, typeof(ReportRunner));
        }

        private static void Run(Item report)
        {
            var name = report.Fields["Name"].Value;
            var description = report.Fields["Description"].Value;
            int days;
            if (!int.TryParse(report.Fields["Days"].Value, out days))
                days = 1;

            var disableSafeMode = Settings.GetSetting("Skutta.AccountReporting.DisableSafeMode") == "true";
            var emailFrom = disableSafeMode ? report.Fields["EmailFrom"].Value : Settings.GetSetting("Skutta.AccountReporting.SafeModeEmailFrom");
            var emailTo = disableSafeMode ? report.Fields["EmailTo"].Value : Settings.GetSetting("Skutta.AccountReporting.SafeModeEmailTo");

            var endDate = DateTime.Now.Date.AddDays(-1);
            var startDate = DateTime.Now.Date.AddDays(-days - 1);

            var title = (startDate == endDate) ? string.Format("{0} ({1})", name, startDate.ToString("d")) : string.Format("{0} ({1} - {2})", name, startDate.ToString("d"), endDate.ToString("d"));

            var html = new StringBuilder();
            html.AppendFormat("<h1>{0}</h1>", title);
            html.AppendFormat("<p>{0}</p>", description);

            var reportSegments = report.Children.OfType<Item>();

            foreach (var reportSegment in reportSegments)
            {
                var segments = ((MultilistField)reportSegment.Fields["Segment"]).GetItems();

                var dashboardUrl = reportSegment.Fields["DashboardUrl"].Value;
                var detailUrl = reportSegment.Fields["DetailUrl"].Value;

                var url = dashboardUrl
                    .Replace("{datefrom}", startDate.ToString("dd-MM-yyyy"))
                    .Replace("{dateto}", endDate.ToString("dd-MM-yyyy"));

                foreach (var segment in segments)
                {
                    html.AppendFormat("<h2><a href=\"{0}\">{1}</a></h2>", url, segment.DisplayName);
                    var reportResponse = ExecuteQuery(segment.ID.Guid, startDate, endDate + new TimeSpan(23, 59, 59));
                    html.Append(FormatResults(reportResponse, startDate, endDate, detailUrl));
                }
            }

            Send(emailFrom, emailTo, title, html.ToString());
        }

        private static ReportResponse ExecuteQuery(Guid segmentId, DateTime startDate, DateTime endDate)
        {
            var parameters = new Dictionary<string, object>()
            {
                { "@SegmentId", segmentId },
                { "@StartDate", startDate },
                { "@EndDate", endDate }
            };
            var query = new ReportDataQuery(REPORT_QUERY, parameters);
            var cachingPolicy = new CachingPolicy { ExpirationPeriod = TimeSpan.Zero };
            return ReportDataService.ExecuteQuery(query, cachingPolicy);
        }

        private static string FormatResults(ReportResponse reportResponse, DateTime dateFrom, DateTime dateTo, string detailUrl)
        {
            var html = new StringBuilder();
            html.Append("<table><tr>");
            html.Append("<th style=\"padding: 5px; font-weight: bold; text-align: left;\">Key</th>");
            html.Append("<th style=\"padding: 5px; font-weight: bold; text-align: left;\">Visits</th>");
            html.Append("<th style=\"padding: 5px; font-weight: bold; text-align: left;\">Value per visit</th>");
            html.Append("<th style=\"padding: 5px; font-weight: bold; text-align: left;\">Average duration</th>");
            html.Append("<th style=\"padding: 5px; font-weight: bold; text-align: left;\">Bounce rate</th>");
            html.Append("<th style=\"padding: 5px; font-weight: bold; text-align: left;\">Conversion rate</th>");
            html.Append("<th style=\"padding: 5px; font-weight: bold; text-align: left;\">Pageviews per visit</th>");
            html.Append("</tr>");

            var reportRows = reportResponse.Data.Content;
            foreach (var reportRow in reportRows)
            {
                var url = detailUrl
                    .Replace("{key}", (reportRow.DimensionKeyId.HasValue) ? reportRow.DimensionKeyId.Value.ToString() : "")
                    .Replace("{datefrom}", dateFrom.ToString("dd-MM-yyyy"))
                    .Replace("{dateto}", dateTo.ToString("dd-MM-yyyy"));

                html.Append("<tr>");
                html.AppendFormat("<td style=\"padding: 5px;\"><a href=\"{0}\">{1}</a></td>", url, reportRow.Key);
                html.AppendFormat("<td style=\"padding: 5px; text-align: center;\">{0}</td>", reportRow.Visits);
                html.AppendFormat("<td style=\"padding: 5px; text-align: center;\">{0}</td>", reportRow.ValuePerVisit);

                var t = TimeSpan.FromSeconds(reportRow.AvgVisitDuration);
                if (t.Hours > 0)
                    html.AppendFormat("<td style=\"padding: 5px; text-align: center;\">{0:D1}hour {1:D1}min {2:D1}s</td>", t.Hours, t.Minutes, t.Seconds);
                else if (t.Minutes > 0)
                    html.AppendFormat("<td style=\"padding: 5px; text-align: center;\">{0:D1}min {1:D1}s</td>", t.Minutes, t.Seconds);
                else if (t.Seconds > 0)
                    html.AppendFormat("<td style=\"padding: 5px; text-align: center;\">{0:D1}s</td>", t.Seconds);
                else
                    html.AppendFormat("<td style=\"padding: 5px; text-align: center;\">0s</td>");

                html.AppendFormat("<td style=\"padding: 5px; text-align: center;\">{0:0.00}%</td>", reportRow.BounceRate * 100);
                html.AppendFormat("<td style=\"padding: 5px; text-align: center;\">{0:0.00}%</td>", reportRow.ConversionRate * 100);
                html.AppendFormat("<td style=\"padding: 5px; text-align: center;\">{0}</td>", reportRow.AvgVisitPageViews);
                html.Append("</tr>");
            }
            html.Append("</table>");
            return html.ToString();
        }

        private static void Send(string from, string to, string subject, string body)
        {
            var mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(from);

            var toAddressList = new List<string>();
            foreach (var address in to.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (!toAddressList.Contains(address))
                {
                    mailMessage.To.Add(address);
                    toAddressList.Add(address);
                }
            }

            mailMessage.Subject = subject;
            mailMessage.Body = body;
            mailMessage.IsBodyHtml = true;

            try
            {
                Sitecore.MainUtil.SendMail(mailMessage);
            }
            catch (Exception ex)
            {
                Log.Error("Error sending email.", ex, typeof(ReportRunner));
            }
        }
    }
}
