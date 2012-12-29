using System;
using System.Collections.Generic;
using System.Text;
using GetTogether.Studio.Logging.Files;

namespace GetTogether.Studio.Functions
{
    public class EmailHelper
    {
        public static bool SendEmailByCredential(string[] mailto, string[] cc, string[] bcc, string subject, string body, bool is_body_html, string[] attachments)
        {
            return GetTogether.Utility.EmailHelper.SendEmailByCredential(
                Config.Original.CredentialHost,
                System.Net.Mail.MailPriority.Normal,
                Config.Original.CredentialUserName,
                Config.Original.CredentialPassword,
                mailto,
                cc,
                bcc,
                subject,
                body,
                is_body_html,
                attachments);
        }
        public static bool SendEmail(string from, string to, string subject, string body, bool is_body_html)
        {
            try
            {
                Log.Info(string.Format("From:{0},To:{1},Subject:{2},Body:{3},Is Body Html:{4},Time:{5}", from, to, subject, "...", is_body_html, DateTime.Now.ToString()));
                if (!Config.Original.EnableCredential)
                {
                    return GetTogether.Utility.EmailHelper.SendMail(
                        Config.Original.EmailServer,
                        System.Net.Mail.MailPriority.Normal,
                        from,
                        new string[] { to },
                        null,
                        null,
                        subject,
                        body,
                        is_body_html,
                        null);
                }
                else
                {
                    return SendEmailByCredential(new string[] { to }, null, null, subject, body, is_body_html, null);
                }
            }
            catch (Exception ex)
            {
                GetTogether.Utility.LogHelper.WriteError(Enums.Loggers.Default, ex.ToString());
                return false;
            }
        }

        public static bool SendEmail(string from, string[] to, string subject, string body, bool is_body_html)
        {
            try
            {
                Log.Info(string.Format(
                    "From:{0},To:{1},Subject:{2},Body:{3},Is Body Html:{4},Time:{5}",
                    from, GetTogether.Utility.StringHelper.ArrayToString(to, ","), subject, "...", is_body_html, DateTime.Now.ToString()));
                if (!Config.Original.EnableCredential)
                {
                    return GetTogether.Utility.EmailHelper.SendMail(
                        Config.Original.EmailServer,
                        System.Net.Mail.MailPriority.Normal,
                        from,
                        to,
                        null,
                        null,
                        subject,
                        body,
                        is_body_html,
                        null);
                }
                else
                {
                    return SendEmailByCredential(to, null, null, subject, body, is_body_html, null);
                }
            }
            catch (Exception ex)
            {
                GetTogether.Utility.LogHelper.WriteError(Enums.Loggers.Default, ex.ToString());
                return false;
            }
        }

        public static bool SendEmail(string from, string[] to, string[] cc, string[] bcc, string subject, string body, bool is_body_html, params string[] attachments)
        {
            try
            {
                Log.Info(string.Format(
                    "From:{0},To:{1},CC:{2},BCC:{3},Subject:{4},Body:{5},Is Body Html:{6},Attachments:{7},Time:{8}",
                    from,
                    GetTogether.Utility.StringHelper.ArrayToString(to, ","),
                    GetTogether.Utility.StringHelper.ArrayToString(cc, ","),
                    GetTogether.Utility.StringHelper.ArrayToString(bcc, ","),
                    subject,
                    "...",
                    is_body_html,
                    GetTogether.Utility.StringHelper.ArrayToString(attachments, ","),
                    DateTime.Now.ToString()));
                if (!Config.Original.EnableCredential)
                {
                    return GetTogether.Utility.EmailHelper.SendMail(
                        Config.Original.EmailServer,
                        System.Net.Mail.MailPriority.Normal,
                        from,
                        to,
                        cc,
                        bcc,
                        subject,
                        body,
                        is_body_html,
                        attachments);
                }
                else
                {
                    return SendEmailByCredential(to, cc, bcc, subject, body, is_body_html, attachments);
                }
            }
            catch (Exception ex)
            {
                GetTogether.Utility.LogHelper.WriteError(Enums.Loggers.Default, ex.ToString());
                return false;
            }
        }
    }
}
