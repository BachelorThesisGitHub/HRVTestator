using Android.Content;

namespace HRVTestator
{
    public static class Email
    {
        public static void Send(Context context, string emailText)
        {
            var email = new Intent(Intent.ActionSend);

            email.PutExtra(Intent.ExtraEmail, new string[] { "andreas_rieder@hotmail.com" });

            email.PutExtra(Intent.ExtraSubject, "Analyseresult");

            email.PutExtra(Intent.ExtraText, emailText);

            email.SetType("message/rfc822");

            context.StartActivity(email);
        }
    }
}