using Mailjet.Client;
using Mailjet.Client.Resources;
using Mailjet.Client.TransactionalEmails;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rocky_Utility
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        public MailJetSettings _mailJetSettings { get; set; }
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            return Execute(email, subject, htmlMessage);
        }
        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task Execute(string receiver, string subject, string body)
        {
            _mailJetSettings = _configuration.GetSection("MailJet").Get<MailJetSettings>();
            MailjetClient client = new MailjetClient(_mailJetSettings.ApiKey, _mailJetSettings.SecretKey);
            //MailjetRequest request = new MailjetRequest
            //{
            //    Resource = Send.Resource,
            //};

            var email = new TransactionalEmailBuilder()
                .WithFrom(new SendContact("rahim.khajei@northernhealth.ca","Rahim Pasha"))
                .WithSubject(subject)
                .WithHtmlPart(body)
                .WithTo(new SendContact(receiver,"RPK"))
                .Build();

            var response = await client.SendTransactionalEmailAsync(email);
            Console.WriteLine(response.Messages[0].ToString());
            Assert.AreEqual(1, response.Messages.Length);


            //      MailjetRequest request = new MailjetRequest
            //      {
            //          Resource = Send.Resource,
            //      }
            //       .Property(Send.Messages, new JArray {
            //      new JObject {
            //       {
            //        "From",
            //        new JObject {
            //         {"Email", "rahim.khajei@northernhealth.ca"},
            //         {"Name", "Rahim Khajei"}
            //        }
            //       }, {
            //        "To",
            //        new JArray {
            //         new JObject {
            //          {
            //           "Email",
            //           receiver
            //          }, {
            //           "Name",
            //           "DotNetMastery"
            //          }
            //         }
            //        }
            //       }, {
            //        "Subject",
            //        subject
            //       }, {
            //        "HTMLPart",
            //        body
            //       }
            //      }
            //});

            //      MailjetResponse response = await client.PostAsync(request);

            //      if (response.IsSuccessStatusCode)
            //      {
            //          Console.WriteLine(string.Format("Total: {0}, Count: {1}\n", response.GetTotal(), response.GetCount()));
            //          Console.WriteLine(response.GetData());
            //      }
            //      else
            //      {
            //          Console.WriteLine(string.Format("StatusCode: {0}\n", response.StatusCode));
            //          Console.WriteLine(string.Format("ErrorInfo: {0}\n", response.GetErrorInfo()));
            //          Console.WriteLine(response.GetData());
            //          Console.WriteLine(string.Format("ErrorMessage: {0}\n", response.GetErrorMessage()));
            //      }



            //MailjetRequest request = new MailjetRequest
            //{
            //    Resource = Send.Resource,
            //};
            //var email = new TransactionalEmailBuilder()
            //    .WithFrom(new SendContact("rahim.khajei@northernhealth.ca"))
            //    .WithSubject(subject)
            //    .WithHtmlPart(body)
            //    .WithTo(new SendContact(receiver))
            //    .Build();
            //
            //request.Property()
            //var response = await client.SendTransactionalEmailAsync(email);
            //Assert.AreEqual(1, response.Messages.Length);
        }
    }
}
