using Android.Content;

namespace HRVTestator
{
    /// <summary>
    /// Die Klasse <see cref="Email"/> ist verantwortlich für zum Senden des EMails der Resultate.
    /// Hinweis: Der Empfänger ist Hardcodiert. Für die weitere Verwendung des Tool ist ein Outsourcing in ein Konfig File zu bevorzugen.
    /// </summary>
    public static class Email
    {
        /// <summary>
        /// Sends the specified context.
        /// </summary>
        /// <param name="context">Der Kontext</param>
        /// <param name="emailText">Der Text des Emails.</param>
        public static void Send(Context context, string emailText)
        {
            var email = new Intent(Intent.ActionSend);

            email.PutExtra(Intent.ExtraEmail, new string[] { "andreas_rieder@hotmail.com" }); // TODO: But to a Configfile.

            email.PutExtra(Intent.ExtraSubject, "Analyseresult");

            email.PutExtra(Intent.ExtraText, emailText);

            email.SetType("message/rfc822");

            context.StartActivity(email);
        }
    }
}