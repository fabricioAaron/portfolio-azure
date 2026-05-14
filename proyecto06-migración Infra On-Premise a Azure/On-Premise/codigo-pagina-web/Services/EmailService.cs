using System.Net;
using System.Net.Mail;

namespace MiWebAPP.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration config, ILogger<EmailService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public void EnviarEmail(string para, string asunto, string mensaje)
        {
            var settings = _config.GetSection("EmailSettings");
            var host = settings["Host"] ?? string.Empty;
            var user = settings["User"] ?? string.Empty;
            var password = settings["Password"] ?? string.Empty;

            if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(user))
            {
                _logger.LogWarning("EmailSettings no está configurado. El envío de correo se omitió para {Destinatario}.", para);
                return;
            }

            var portValue = settings["Port"];
            var enableSslValue = settings["EnableSSL"];

            var port = 25;
            if (!string.IsNullOrWhiteSpace(portValue) && int.TryParse(portValue, out var parsedPort))
            {
                port = parsedPort;
            }

            var enableSsl = false;
            if (!string.IsNullOrWhiteSpace(enableSslValue) && bool.TryParse(enableSslValue, out var parsedEnableSsl))
            {
                enableSsl = parsedEnableSsl;
            }

            var smtp = new SmtpClient
            {
                Host = host,
                Port = port,
                EnableSsl = enableSsl,
                Credentials = new NetworkCredential(user, password)
            };

            var mail = new MailMessage
            {
                From = new MailAddress(user),
                Subject = asunto,
                Body = mensaje,
                IsBodyHtml = true
            };

            mail.To.Add(para);

            smtp.Send(mail);
        }
    }
}
