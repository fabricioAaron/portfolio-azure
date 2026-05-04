using RabbitMQ.Client;
using System;
using System.Text;

namespace MiWebAPP.Services
{
    public class RabbitMqService
    {
        private readonly ConnectionFactory _factory;

        public RabbitMqService()
        {
            _factory = new ConnectionFactory
            {
                HostName = "192.168.1.130",
                Port = 5672,
                UserName = "admin",
                Password = "admin"
            };
        }

        public void EnviarReserva(string mensaje)
        {
            Console.WriteLine(">>> EnviarReserva SE ESTÁ EJECUTANDO");

            using var connection = _factory.CreateConnection();
            using var channel = connection.CreateModel();

            // 1. Declarar EXCHANGE
            channel.ExchangeDeclare(
                exchange: "test_exchange",
                type: ExchangeType.Direct,
                durable: true
            );

            // 2. Declarar COLA
            channel.QueueDeclare(
                queue: "test_queue",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            // 3. Crear BINDING
            channel.QueueBind(
                queue: "test_queue",
                exchange: "test_exchange",
                routingKey: "test_rk"
            );

            // 4. Publicar mensaje
            var body = Encoding.UTF8.GetBytes(mensaje);

            channel.BasicPublish(
                exchange: "test_exchange",
                routingKey: "test_rk",
                basicProperties: null,
                body: body
            );

            Console.WriteLine(">>> Mensaje enviado correctamente a test_exchange → test_queue");
        }
    }
}
