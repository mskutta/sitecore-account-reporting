using System;
using Sitecore.Data;
using Sitecore.Globalization;
using Sitecore.Security.Accounts;
using Sitecore.SecurityModel;
using Sitecore.Sites;

namespace Skutta.AccountReporting.Reports
{
    public class ReportContext : IDisposable
    {
        private UserSwitcher _userSwitcher;
        private DatabaseSwitcher _databaseSwitcher;
        private LanguageSwitcher _languageSwitcher;
        private SiteContextSwitcher _siteContextSwitcher;
        private SecurityDisabler _securityDisabler;


        public ReportContext(string userName = "sitecore/admin", string siteName = "shell", string databaseName = "master", string language = null)
        {
            var user = User.FromName(userName, false);

            _userSwitcher = new UserSwitcher(user);
            _databaseSwitcher = new DatabaseSwitcher(Sitecore.Configuration.Factory.GetDatabase(databaseName));

            if (!string.IsNullOrEmpty(language))
                _languageSwitcher = new LanguageSwitcher(language);

            _siteContextSwitcher = new SiteContextSwitcher(SiteContextFactory.GetSiteContext(siteName));
            _securityDisabler = new SecurityDisabler();
        }

        public void Dispose()
        {
            if (_securityDisabler != null)
            {
                _securityDisabler.Dispose();
                _securityDisabler = null;
            }

            if (_siteContextSwitcher != null)
            {
                _siteContextSwitcher.Dispose();
                _siteContextSwitcher = null;
            }

            if (_languageSwitcher != null)
            {
                _languageSwitcher.Dispose();
                _languageSwitcher = null;
            }

            if (_databaseSwitcher != null)
            {
                _databaseSwitcher.Dispose();
                _databaseSwitcher = null;
            }

            if (_userSwitcher != null)
            {
                _userSwitcher.Dispose();
                _userSwitcher = null;
            }
        }
    }
}
