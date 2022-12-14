using Microsoft.Extensions.Options;
using MimeKit;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using static WelcomeUser.Mail.InfTemplateMail;

namespace WelcomeUser.Mail
{
    public class MailService : IMailService
    {
        public readonly MailSettings _mailSettings;
        public MailService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        public async Task SendEmailAsync(MailRequest mailRequest)
        {
            var email = new SendGridMessage();
            email.SetFrom(_mailSettings.Mail);
            var recipients = new List<EmailAddress> { new EmailAddress(mailRequest.ToEmail, "Pevaar Client") };
            email.AddTos(recipients);
            email.SetSubject(mailRequest.Subject);
            //var builder = new BodyBuilder();
            //if (mailRequest.Attachments != null)
            //{
            //    byte[] fileBytes;
            //  foreach (var file in mailRequest.Attachments)
            //    {
            //        if (file.Length > 0)
            //        {
            //            using (var ms = new MemoryStream())
            //            {
            //                file.CopyTo(ms);
            //                fileBytes = ms.ToArray();
            //            }
            //            builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
            //        }
            //    }
            //}

            if (mailRequest.Subject == "Cotización" && mailRequest.BotAttachments != null) { mailRequest.BotAttachments.Clear(); }
            else if (mailRequest.BotAttachments != null && mailRequest.Subject != "Cotización")
            {
                foreach (var file in mailRequest.BotAttachments)
                {
                    using (var webClient = new WebClient())
                    {
                        byte[] fileBytes;
                        var filestream = webClient.OpenRead(file.ContentUrl);

                        using (var ms = new MemoryStream())
                        {
                            filestream.CopyTo(ms);
                            fileBytes = ms.ToArray();
                        }
                        //Byte[] bytes = File.ReadAllBytes(filestream.);
                        email.AddAttachment(file.Name, Convert.ToBase64String(fileBytes), file.ContentType, "inline", file.Name);
                    }

                }
            }

            //builder.HtmlBody = mailRequest.Body;
            //email.AddContent(MimeType.Html, mailRequest.Body);
            email.SetTemplateId("d-f39627877d8042f3b3ec9d14143b1bf9");
            email.SetTemplateData(new InfTemplateMail { subject=mailRequest.Subject, Name=mailRequest.Name, Phone=mailRequest.Phone, Description=mailRequest.Description, Email=mailRequest.Email});
                
           //using var smtp = new SmtpClient();
           //smtp.Connect(_mailSettings.Host, _mailSettings.Port);
           //smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
           //await smtp.SendAsync(email);
           //smtp.Disconnect(true);
           //var apiKey = System.Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
           var client = new SendGridClient("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");
           var response = await client.SendEmailAsync(email);
        }
    }
}
