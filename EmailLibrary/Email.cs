using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace EmailLibrary
{
    public class Email
    {
        private static string SMTPServer = ConfigurationManager.AppSettings["SMTP Server"];
        private static string SenderEmail = ConfigurationManager.AppSettings["SenderEmail"];
        private static string Password = ConfigurationManager.AppSettings["Password"];
        private static int RetryCount = int.Parse(ConfigurationManager.AppSettings["RetryCount"]);
        public string Recipient { get; set; }
        public string Sender { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public DateTime Date { get; set; }
        public bool Delivered { get; set; }
        public Email(string r, string s, string b, DateTime d, bool del)
        {
            Recipient = r;
            Subject = s;
            Body = b;
            Date = d;
            Delivered = del;
            Sender = SenderEmail;
        }
        public bool Send()
        {
            bool sent = false;
            int attemptCount = 0;

            //construct email
            MailMessage message = new MailMessage();
            SmtpClient smtp = new SmtpClient();
            message.From = new MailAddress(SenderEmail);
            message.To.Add(new MailAddress(this.Recipient));
            message.Subject = this.Subject;
            message.IsBodyHtml = false; //could change this in later version to true and add HTML formatting
            message.Body = this.Body;
            smtp.Port = 587;
            smtp.Host = SMTPServer;
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(SenderEmail, Password);
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

            while (attemptCount < RetryCount)
            {
                try
                {
                    smtp.Send(message);
                    sent = true;
                }
                catch (Exception e)
                {
                    attemptCount++;
                    if (attemptCount == RetryCount)
                    {
                        return sent;
                    }
                }
            }
            this.Delivered = sent;
            return sent;

        }

    }

}
