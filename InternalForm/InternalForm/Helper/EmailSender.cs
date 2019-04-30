using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;

namespace InternalForm.Helper
{
    public static class EmailSender
    {

        #region Static Members
        public static void SendMailMessage(string toEmail, string fromEmail, string subject, string body, List<string> attachmentFullPath)
        {
            //create the MailMessage object
            using (MailMessage mMailMessage = new MailMessage())
            {

                //set the sender address of the mail message
                if (!string.IsNullOrEmpty(fromEmail))
                {
                    mMailMessage.From = new MailAddress(fromEmail);
                }

                //set the recipient address of the mail message
                mMailMessage.To.Add(new MailAddress(toEmail));

                //set the subject of the mail message
                if (!string.IsNullOrEmpty(subject))
                {
                    mMailMessage.Subject = "Email About Training Class";
                }
                else
                {
                    mMailMessage.Subject = subject;
                }

                //set the body of the mail message
                mMailMessage.Body = body;

                //set the format of the mail message body
                mMailMessage.IsBodyHtml = true;

                //set the priority
                mMailMessage.Priority = MailPriority.Normal;

                //add any attachments from the filesystem
                foreach (var attachmentPath in attachmentFullPath)
                {
                    Attachment mailAttachment = new Attachment(attachmentPath);
                    mMailMessage.Attachments.Add(mailAttachment);
                }

                //create the SmtpClient instance
                using (var mSmtpClient = new SmtpClient("smtp.gmail.com"))
                {
                    mSmtpClient.Port = 587;
                    mSmtpClient.Credentials = new System.Net.NetworkCredential("your Email", "password");
                    mSmtpClient.EnableSsl = true;
                    //send the mail message
                    mSmtpClient.Send(mMailMessage);
                }
            }
        }

        /// <summary>
        /// Determines whether an email address is valid.
        /// </summary>
        /// <param name="emailAddress">The email address to validate.</param>
        /// <returns>
        /// 	<c>true</c> if the email address is valid; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsValidEmailAddress(string emailAddress)
        {
            // An empty or null string is not valid
            if (string.IsNullOrEmpty(emailAddress))
            {
                return (false);
            }

            // Regular expression to match valid email address
            string emailRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                                @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                                @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";

            // Match the email address using a regular expression
            Regex re = new Regex(emailRegex);
            if (re.IsMatch(emailAddress))
                return (true);
            else
                return (false);
        }


        //public static string GetBody(string className)
        //{

        //    string bodyHTML = "gjhjjgj gg" + className;

        //    return bodyHTML;
        //}
        #endregion
    }
}