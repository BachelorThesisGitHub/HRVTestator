using System;
using System.Collections.Generic;
using Android.Content;
using System.Text;

namespace HRVTestator
{
    public static class Email
    {
        public static void Send(Context context, List<Tuple<DateTime, string>> mesures)
        {
            var email = new Intent(Intent.ActionSend);

            email.PutExtra(Intent.ExtraEmail, new string[] { "andreas_rieder@hotmail.com" });

            email.PutExtra(Intent.ExtraSubject, "Analyseresult");

            email.PutExtra(Intent.ExtraText, FormatData(mesures));

            email.SetType("message/rfc822");

            context.StartActivity(email);
        }

        private static string FormatData(List<Tuple<DateTime, string>> mesures)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Tuple<DateTime, string> item in mesures)
            {
                sb.AppendLine(string.Format("{0},{1};", item.Item1, item.Item2));
            }

            return sb.ToString();
        }
    }
}