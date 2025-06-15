using DotNetEnv;
using SendGrid;
using SendGrid.Helpers.Mail;
using ZEN.Domain.Interfaces;

namespace ZEN.Infrastructure.Integrations.SendMail
{
    public class SendMail : ISendMail
    {
        private readonly string _apiKeySendMail;
        private readonly string _template;
        private readonly string _verifiedEmail;
        public SendMail()
        {
            Env.Load();
            _apiKeySendMail = Env.GetString("SENDGRIDAPIKEY");
            _template = Env.GetString("SENDGRIDTEMPLATE");
            _verifiedEmail = Env.GetString("VERIFIEDEMAIL");
        }
        public async Task<bool> SendHrContactEmailAsync(HrContactDto hrContact)
        {
            var client = new SendGridClient(_apiKeySendMail);
            // var from = new EmailAddress(hrContact.HrEmail, hrContact.HrName);
            var from = new EmailAddress(_verifiedEmail, "Portfolio Contact");
            var to = new EmailAddress(hrContact.user_email, hrContact.user_name);

            var msg = new SendGridMessage();
            //them de tranh vao thu rac
            msg.AddHeader("X-Mailer", "PortfolioMailer 1.0");
            msg.AddHeader("Precedence", "bulk");
            msg.AddHeader("X-Priority", "3");
            msg.AddHeader("X-MSMail-Priority", "Normal");
            msg.AddHeader("Importance", "Normal");
            msg.SetClickTracking(false, false);
            msg.SetOpenTracking(false);
            msg.SetGoogleAnalytics(false);
            msg.SetSubscriptionTracking(false);


            msg.SetFrom(from);
            msg.AddTo(to);
            msg.SetReplyTo(new EmailAddress(hrContact.HrEmail, hrContact.HrName));
            msg.SetTemplateId(_template);
            msg.SetSubject("Thông tin liên hệ từ bộ phận tuyển dụng");
            msg.SetTemplateData(new
            {
                hrName = hrContact.HrName,
                hrEmail = hrContact.HrEmail,
                hrPhone = hrContact.HrPhone,
                hrCompany = hrContact.HrCompany,
                hrNotes = hrContact.HrNotes,
                user_name = hrContact.user_name
            });

            //them de tranh vao thu rac
            msg.Categories = new List<string> { "hr-contact" };
            msg.CustomArgs = new Dictionary<string, string> { { "source", "portfolio" } };
            msg.MailSettings = new MailSettings
            {
                SandboxMode = new SandboxMode { Enable = false }
            };

            var response = await client.SendEmailAsync(msg);
            // System.Console.WriteLine($"Gmail o Sendmail tinh trang: {response.IsSuccessStatusCode}");
            return response.IsSuccessStatusCode;
        }
    }

}
