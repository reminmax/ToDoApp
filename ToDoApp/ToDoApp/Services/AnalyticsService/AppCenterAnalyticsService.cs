using System;
using System.Collections.Generic;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

namespace ToDoApp.Services.AnalyticsService
{
    public class AppCenterAnalyticsService : IAnalyticsService
    {
        public void TrackError(Exception exception)
        {
            Crashes.TrackError(exception);
        }

        public void TrackError(Exception exception, IDictionary<string, string> data)
        {
            Crashes.TrackError(exception, data);
        }

        public void TrackEvent(string eventKey)
        {
            Analytics.TrackEvent(eventKey);
        }

        public void TrackEvent(string eventKey, IDictionary<string, string> data)
        {
            Analytics.TrackEvent(eventKey, data);
        }
    }
}
