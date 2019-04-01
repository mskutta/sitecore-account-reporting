using System;
using Skutta.AccountReporting.Reports;
using Sitecore.sitecore.admin;

namespace Skutta.AccountReporting.Web.sitecore.admin
{
    public partial class CompanyAnalyticsReports : AdminPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            CheckSecurity(true);
        }

        protected void RunAll_Click(object sender, EventArgs e)
        {
            ReportRunner.Run(true);
        }
    }
}