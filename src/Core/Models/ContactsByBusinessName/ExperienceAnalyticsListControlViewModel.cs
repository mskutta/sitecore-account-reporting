using Sitecore.ExperienceAnalytics.Client.Mvc.Presentation;
using Sitecore.Mvc.Presentation;
using System;
using System.Reflection;

namespace Skutta.AccountReporting.Models.ContactsByBusinessName
{
    public class ExperienceAnalyticsListControlViewModel : Sitecore.ExperienceAnalytics.Client.Mvc.Presentation.ExperienceAnalyticsListControlViewModel
    {
        public override void Initialize(Rendering rendering)
        {
            base.Initialize(rendering);
            SetPrivatePropertyValue(typeof(DvcRenderingModel), this, "DataUrl", "/sitecore/api/ao/aggregates/contactsbybusinessname/{KEYS}");
        }

        private static void SetPrivatePropertyValue<T>(Type type, object obj, string propName, T val)
        {
            if (type.GetProperty(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance) == null)
                throw new ArgumentOutOfRangeException("propName", string.Format("Property {0} was not found in Type {1}", propName, obj.GetType().FullName));
            type.InvokeMember(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.SetProperty | BindingFlags.Instance, null, obj, new object[] { val });
        }
    }
}
