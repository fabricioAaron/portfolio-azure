using System.Net;
using System.Net.Mail;

namespace MiWebAPP.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public void EnviarEmail(string para, string asunto, string mensaje)
        {
            var settings = _config.GetSection("EmailSettings");

            var smtp = new SmtpClient
            {
                Host = settings["Host"],
                Port = int.Parse(settings["Port"]),
                EnableSsl = bool.Parse(settings["EnableSSL"]),
                Credentials = new NetworkCredential(settings["User"], settings["Password"])
            };

            var mail = new MailMessage
            {
                From = new MailAddress(settings["User"]),
                Subject = asunto,
                Body = mensaje,
                IsBodyHtml = true
            };

            mail.To.Add(para);

            smtp.Send(mail);
        }
    }
}
