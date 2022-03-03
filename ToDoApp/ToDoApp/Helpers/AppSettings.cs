using System;
using Xamarin.Essentials;

namespace ToDoApp.Helpers
{
    public static class AppSettings
    {
        public static string ApplicationTheme
        {
            get
            {
                if (Preferences.ContainsKey("ApplicationTheme"))
                {
                    return Preferences.Get("ApplicationTheme", String.Empty);
                }

                return string.Empty;
            }
            set => Preferences.Set("ApplicationTheme", value);
        }

        public static bool IsDarkModeEnabled
        {
            get
            {
                if (Preferences.ContainsKey("IsDarkModeEnabled"))
                {
                    return Preferences.Get("IsDarkModeEnabled", false);
                }

                return false;
            }
            set => Preferences.Set("IsDarkModeEnabled", value);
        }

        public static bool IsHidingDoneTasksEnabled
        {
            get
            {
                if (Preferences.ContainsKey("IsHidingDoneTasksEnabled"))
                {
                    return Preferences.Get("IsHidingDoneTasksEnabled", false);
                }

                return false;
            }
            set => Preferences.Set("IsHidingDoneTasksEnabled", value);
        }
    }
}