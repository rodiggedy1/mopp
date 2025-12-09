namespace Infrastructure.MessageBroker.Config
{
    public class RabbitMqConfiguration
    {
        public const string SectionName = "RabbitMqConnection";

        public string HostName { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string VirtualHost { get; set; } = null!;
        public bool Enabled { get; set; }
        public ushort Port { get; set; }
    }
}
