namespace Skutta.AccountReporting.Reports
{
    public class Task : DailyTaskBase
    {
        protected override void OnRun()
        {
            ReportRunner.Run(false);
        }
    }
}
