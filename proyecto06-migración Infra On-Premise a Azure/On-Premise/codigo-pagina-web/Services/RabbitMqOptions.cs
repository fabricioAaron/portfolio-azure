namespace MiWebAPP.Services
{
    public sealed class RabbitMqOptions
    {
        public const string SectionName = "RabbitMq";

        public bool Enabled { get; set; }

        public string HostName { get; set; } = "localhost";

        public int Port { get; set; } = 5672;

        public string UserName { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string VirtualHost { get; set; } = "/";

        public string Exchange { get; set; } = "reservas.exchange";

        public string Queue { get; set; } = "reservas.queue";

        public string RoutingKey { get; set; } = "reservas.created";
    }
}