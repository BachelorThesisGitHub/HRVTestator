using System;
using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using static Android.App.ActivityManager;

namespace HRVTestator
{
    /// <summary>
    /// http://stackoverflow.com/questions/600207/how-to-check-if-a-service-is-running-on-android
    /// </summary>
    public class ServiceTools
    {
        public static bool IsServiceRunning(string serviceClassName)
        {
            ActivityManager activityManager = (ActivityManager)Application.Context.GetSystemService(Context.ActivityService);
            IList<RunningServiceInfo> services = activityManager.GetRunningServices(int.MaxValue);

            foreach (RunningServiceInfo runningServiceInfo in services)
            {
                if (runningServiceInfo.Service.ClassName.Equals(serviceClassName))
                {
                    return true;
                }
            }
            return false;
        }
    }
}