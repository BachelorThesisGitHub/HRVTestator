using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;


namespace HRVTestator
{

    [BroadcastReceiver(Enabled = true)] //[IntentFilter(new[] { "HEARD_RATE_UPDATE" })]
    public class PhonecallReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            var manager = (Android.Telephony.TelephonyManager)context.GetSystemService(Context.TelephonyService);
            IntPtr TelephonyManager_getITelephony = JNIEnv.GetMethodID(
                    manager.Class.Handle,
                    "getITelephony",
                    "()Lcom/android/internal/telephony/ITelephony;");

            IntPtr telephony = JNIEnv.CallObjectMethod(manager.Handle, TelephonyManager_getITelephony);
            IntPtr ITelephony_class = JNIEnv.GetObjectClass(telephony);
            IntPtr ITelephony_endCall = JNIEnv.GetMethodID(
                    ITelephony_class,
                    "endCall",
                    "()Z");
            JNIEnv.CallBooleanMethod(telephony, ITelephony_endCall);
            JNIEnv.DeleteLocalRef(telephony);
            JNIEnv.DeleteLocalRef(ITelephony_class);
        }
    }
}