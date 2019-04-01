using System.Globalization;
using Sitecore.Caching;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Diagnostics;
using System;
using System.Threading;

namespace Skutta.AccountReporting.Reports
{
    public abstract class DailyTaskBase
    {
        private TimeSpan? _runTime;
        private DateTime _lastRunTime;
        private DateTime _nextRunTime;

        //Adding Hourly Support
        private int _hoursToNextRun = 0;

        private string _lastRunDatePropertyName;

        public string RunTime { get; set; }
        public string SyncDatabase { get; set; }
        public string HoursToNextRun { get; set; }

        protected abstract void OnRun();

        public void Run()
        {
            if (!_runTime.HasValue)
            {
                DateTime tempTime;
                if (DateTime.TryParse(RunTime, out tempTime))
                {
                    _runTime = tempTime.TimeOfDay;
                    _nextRunTime = GetNextRuntime();
                    _lastRunTime = DateTime.Now;
                    _lastRunDatePropertyName = "AbsoluteTimer_" + GetType().FullName;
                }
            }

            var syncDatabase = Database.GetDatabase(SyncDatabase);

            if (_hoursToNextRun == 0)
            {
                int tempHours;
                if (int.TryParse(HoursToNextRun, out tempHours))
                {

                    _runTime = DateTime.Now.TimeOfDay;
                    _lastRunDatePropertyName = "AbsoluteTimer_" + GetType().FullName;
                    //Need to clear cache
                    ClearPropertyCache(syncDatabase);
                    _nextRunTime = GetNextRuntimeAddHours(syncDatabase, tempHours);
                    _lastRunTime = GetLastRunDate(syncDatabase);
                    _hoursToNextRun = tempHours;
                }
            }


            if (!_runTime.HasValue) return;

            var now = DateTime.Now;

            //See if the job has run between now and the last time the absolute timer was run.
            if (_lastRunTime.CompareTo(_nextRunTime) <= 0 && now.CompareTo(_nextRunTime) >= 0)
            {
                //Update the time of the last run to prevent the job from running again if it is a long running job
                _lastRunTime = now;

                if (!string.IsNullOrEmpty(SyncDatabase) && !CanRunSingleInstanceJob())
                    return;

                try
                {
                    OnRun();
                }
                finally
                {
                    if (_hoursToNextRun > 0)
                        _nextRunTime = GetNextRuntimeAddHours(syncDatabase, _hoursToNextRun);
                    else
                    {
                        _nextRunTime = GetNextRuntime();
                    }
                }
            }

            _lastRunTime = now;
        }

        private DateTime GetNextRuntime()
        {
            if (_runTime.HasValue)
            {
                var runtime = DateTime.Now.Date + _runTime.Value;
                if (runtime.CompareTo(DateTime.Now) <= 0)
                {
                    runtime = runtime.AddDays(1);
                }

                return runtime;
            }
            return DateTime.MinValue;
        }

        private bool CanRunSingleInstanceJob()
        {
            if (string.IsNullOrEmpty(SyncDatabase))
                return true;

            var syncDatabase = Database.GetDatabase(SyncDatabase);

            if (syncDatabase != null)
            {
                ClearPropertyCache(syncDatabase);

                //Get the time of the last run 
                var lastRunDate = GetLastRunDate(syncDatabase);

                if (lastRunDate.CompareTo(DateTime.Now.AddHours(-1)) > 0)
                    return false;

                //If it hasn't been run within an hour, then it is eligible to be run
                //Create a signature so we can see if another server is trying to run it
                var signature = string.Format("{0}|{1:N}", DateTime.Now.ToString(CultureInfo.InvariantCulture), Guid.NewGuid());

                //Set the run time  
                syncDatabase.Properties[_lastRunDatePropertyName] = signature;

                ClearPropertyCache(syncDatabase);

                //Wait for a random time to make sure no other server is trying to run
                Thread.Sleep((new Random()).Next(1000, 3000));

                if (syncDatabase.Properties[_lastRunDatePropertyName] != signature)
                    return false;
                return true;
            }

            Log.Warn("Can not open the Sync Database", this);

            return false;
        }

        /// <summary>
        /// Removes the cached last run date from the property cache
        /// </summary>
        /// <param name="syncDatabase"></param>
        private void ClearPropertyCache(Database syncDatabase)
        {
            var cacheName = string.Format("SqlDataProvider - Property data({0})", syncDatabase.Name);
            //var propertyCache = Cache.GetNamedInstance(cacheName, Settings.Caching.DefaultPropertyCacheSize);

            var propertyCache = CacheManager.GetNamedInstance(cacheName, Settings.Caching.DefaultPropertyCacheSize, true);

            propertyCache.Remove(_lastRunDatePropertyName);
        }

        /// <summary>
        /// Returns the time of the last run of the job
        /// </summary>
        /// <param name="syncDatabase"></param>
        /// <returns></returns>
        private DateTime GetLastRunDate(Database syncDatabase)
        {
            var lastRunSignature = syncDatabase.Properties[_lastRunDatePropertyName];

            //The last run signature is a time and a unique string. We need just the time.
            if (!string.IsNullOrEmpty(lastRunSignature))
            {
                var barPos = lastRunSignature.IndexOf("|", StringComparison.Ordinal);

                if (barPos != -1)
                    lastRunSignature = lastRunSignature.Substring(0, barPos);
            }

            DateTime lastRunDate;
            if (!DateTime.TryParse(lastRunSignature, out lastRunDate))
                lastRunDate = DateTime.MinValue;

            return lastRunDate;
        }

        private DateTime GetNextRuntimeAddHours(Database syncDatabase, int tempHours)
        {
            var lastRunSignature = syncDatabase.Properties[_lastRunDatePropertyName];

            //The last run signature is a time and a unique string. We need just the time.
            if (!string.IsNullOrEmpty(lastRunSignature))
            {
                var barPos = lastRunSignature.IndexOf("|", StringComparison.Ordinal);

                if (barPos != -1)
                    lastRunSignature = lastRunSignature.Substring(0, barPos);
            }

            DateTime nextRunDate;
            if (!DateTime.TryParse(lastRunSignature, out nextRunDate))
                nextRunDate = DateTime.Now.AddHours(tempHours);
            else
                nextRunDate = nextRunDate.AddHours(tempHours);

            return nextRunDate;
        }

    }
}
