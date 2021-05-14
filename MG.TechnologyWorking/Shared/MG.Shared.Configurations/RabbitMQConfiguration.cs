namespace MG.Shared.Configurations
{
    public class RabbitMQConfiguration
    {
        public string RabbitMQUrl { get; set; }
        public string RabbitMQPort { get; set; }
        public string RabbitMQUserName { get; set; }
        public string RabbitMQPassword { get; set; }
        public bool DispatcAsync { get; set; }
    }
}