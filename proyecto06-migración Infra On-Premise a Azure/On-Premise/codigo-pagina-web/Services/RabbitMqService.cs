using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System;
using System.Text;

namespace MiWebAPP.Services
{
    public class RabbitMqService
    {
        private readonly RabbitMqOptions _options;
        private readonly ILogger<RabbitMqService> _logger;

        public RabbitMqService(IOptions<RabbitMqOptions> options, ILogger<RabbitMqService> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        public void EnviarReserva(string mensaje)
        {
            if (!_options.Enabled)
            {
                _logger.LogInformation("RabbitMQ está deshabilitado por configuración. Mensaje omitido: {Mensaje}", mensaje);
                return;
            }

            if (string.IsNullOrWhiteSpace(_options.HostName) || string.IsNullOrWhiteSpace(_options.UserName) || string.IsNullOrWhiteSpace(_options.Password))
            {
                _logger.LogWarning("RabbitMQ no está configurado completamente. Mensaje omitido: {Mensaje}", mensaje);
                return;
            }

            var factory = new ConnectionFactory
            {
                HostName = _options.HostName,
                Port = _options.Port,
                UserName = _options.UserName,
                Password = _options.Password,
                VirtualHost = _options.VirtualHost,
                DispatchConsumersAsync = true
            };

            try
            {
                using var connection = factory.CreateConnection();
                using var channel = connection.CreateModel();

                channel.ExchangeDeclare(
                    exchange: _options.Exchange,
                    type: ExchangeType.Direct,
                    durable: true
                );

                channel.QueueDeclare(
                    queue: _options.Queue,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );

                channel.QueueBind(
                    queue: _options.Queue,
                    exchange: _options.Exchange,
                    routingKey: _options.RoutingKey
                );

                var body = Encoding.UTF8.GetBytes(mensaje);

                channel.BasicPublish(
                    exchange: _options.Exchange,
                    routingKey: _options.RoutingKey,
                    basicProperties: null,
                    body: body
                );

                _logger.LogInformation("Mensaje enviado correctamente a RabbitMQ: {Exchange} -> {Queue}", _options.Exchange, _options.Queue);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "No se pudo enviar el mensaje a RabbitMQ.");
            }
        }
    }
}
