using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace PuckatorService
{
    public class EmailFeedService
    {
        private bool _testMode;

        public EmailFeedService(bool TestMode)
        {
            _testMode = TestMode;
        }
        public class EmailResponse
        {
            public bool HasError { get; set; }
            public string ErrorMessage { get; set; }
        }

        private string GetHtml(string templatePath, Dictionary<string, string> Parameters)
        {
            var rawString = System.IO.File.ReadAllText(templatePath);
            foreach (var parameter in Parameters)
            {
                if (rawString.Contains("{#" + parameter.Key.ToUpper() + "#}"))
                {
                    rawString = rawString.Replace("{#" + parameter.Key.ToUpper() + "#}", parameter.Value);
                }
            }

            return rawString;
        }

        private EmailResponse EmailBySMTP(string toEmailAddress, string fromAddress, string body, string subject)
        {
            EmailResponse res = new EmailResponse() { HasError = false, ErrorMessage = string.Empty };

            MailMessage m = new MailMessage();
            SmtpClient sc = new SmtpClient();
            m.From = new MailAddress(fromAddress);
            m.To.Add(_testMode ? ConfigurationManager.AppSettings["SMTP_TO"] : toEmailAddress);
            m.Subject = subject;
            m.Body = body;
            sc.Host = ConfigurationManager.AppSettings["SMTP_HOST"];
            string str1 = "gmail.com";
            if (fromAddress.ToLower().Contains(str1))
            {
                try
                {
                    sc.Port = 587;
                    sc.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["SMTP_FROM"], ConfigurationManager.AppSettings["SMTP_Password"]);
                    sc.EnableSsl = true;
                    m.IsBodyHtml = true;

                    if (!_testMode)
                        sc.Send(m);
                }
                catch (Exception ex)
                {
                    res.HasError = true;
                    res.ErrorMessage = ex.InnerException.Message;
                }
            }
            else
            {
                try
                {
                    sc.Port = 25;
                    sc.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["SMTP_FROM"], ConfigurationManager.AppSettings["SMTP_Password"]);
                    sc.EnableSsl = false;
                    m.IsBodyHtml = true;
                    if (!_testMode)
                        sc.Send(m);

                }
                catch (Exception ex)
                {
                    res.HasError = true;
                    res.ErrorMessage = ex.InnerException.Message;
                }
            }

            return res;
        }

        public void NotifyFileCreation(List<string> messageLine, string subject)
        {
            StringBuilder sb = new StringBuilder().AppendLine($"DateTime: {DateTime.Now}");
            foreach (var item in messageLine)
            {
                sb.AppendLine(item);
            }

            Dictionary<string, string> param = new Dictionary<string, string>
            {
                { "NAME", System.Configuration.ConfigurationManager.AppSettings["SMTP_TO_NAME"] },
                { "MESSAGE", sb.ToString()}
            };
            string html = GetHtml(Path.Combine(Common.GetBaseDirectory(), System.Configuration.ConfigurationManager.AppSettings["mxTemplatePath"], "tmpDefault.html"), param);
            EmailBySMTP(System.Configuration.ConfigurationManager.AppSettings["SMTP_TO"], System.Configuration.ConfigurationManager.AppSettings["SMTP_FROM"], html, subject);
        }

        public void NotifyException(string data, string subject)
        {
            var message = $"DateTime: {DateTime.Now} {Environment.NewLine} {data.ToUpper()}";
            Dictionary<string, string> param = new Dictionary<string, string>
            {
                { "NAME", System.Configuration.ConfigurationManager.AppSettings["SMTP_TO_NAME"] },
                { "MESSAGE", message }
            };
            string html = GetHtml(Path.Combine(Common.GetBaseDirectory(), System.Configuration.ConfigurationManager.AppSettings["mxTemplatePath"], "tmpDefault.html"), param);
            EmailBySMTP(System.Configuration.ConfigurationManager.AppSettings["SMTP_TO"], System.Configuration.ConfigurationManager.AppSettings["SMTP_FROM"], html, subject);
        }
    }
}