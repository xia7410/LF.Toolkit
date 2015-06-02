using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace LF.Toolkit.Misc
{
    public class SmtpProvider
    {
        /*
         * System.Net.Mail only supports “Explicit SSL”.  Explicit SSL starts as unencrypted on port 25, then issues a STARTTLS and switches to an Encrypted connection.  See RFC 2228.
         * Explicit  SLL would go something like: Connect on 25 -> StartTLS (starts to encrypt) -> authenticate -> send data
         *
         */

        SmtpClient client;
        MailAddress sender;

        /// <summary>
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="mailAddress"></param>
        /// <param name="mailPassword"></param>
        public SmtpProvider(string host, int port, string mailAddress, string mailPassword)
            : this(host, port, true, 30000, mailAddress, mailPassword)
        {

        }

        public SmtpProvider(string host, int port, bool ssl, int timeout, string mailAddress, string mailPassword)
        {
            if (string.IsNullOrEmpty(host)) throw new ArgumentNullException("host");
            if (string.IsNullOrEmpty(mailAddress)) throw new ArgumentNullException("mailAddress");
            if (string.IsNullOrEmpty(mailPassword)) throw new ArgumentNullException("mailPassword");

            this.client = new SmtpClient(host, port);
            this.client.EnableSsl = ssl;
            this.client.Timeout = timeout;
            this.client.DeliveryMethod = SmtpDeliveryMethod.Network;
            this.client.UseDefaultCredentials = true;        
            this.client.Credentials = new NetworkCredential(mailAddress, mailPassword);
            this.sender = new MailAddress(mailAddress);
        }

        public Task SendAsync(string subject, string body, bool isBodyHtml, params MailAddress[] recipients)
        {
            if (string.IsNullOrEmpty(subject)) throw new ArgumentNullException("subject");
            if (string.IsNullOrEmpty(body)) throw new ArgumentNullException("body");
            if (recipients == null || recipients.Length <= 0) throw new ArgumentNullException("recipients");

            MailMessage mm = new MailMessage();
            mm.IsBodyHtml = isBodyHtml;
            mm.BodyEncoding = UTF8Encoding.UTF8;
            mm.HeadersEncoding = UTF8Encoding.UTF8;
            mm.Subject = subject;
            mm.Body = body;
            mm.Priority = MailPriority.High;
            mm.DeliveryNotificationOptions = DeliveryNotificationOptions.OnSuccess;
            mm.From = this.sender;

            foreach (var addr in recipients)
            {
                mm.To.Add(addr);
            }

            return this.client.SendMailAsync(mm);
        }
    }
}
