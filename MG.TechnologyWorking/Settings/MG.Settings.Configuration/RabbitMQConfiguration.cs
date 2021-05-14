namespace MG.Settings.Configuration
{
    public class RabbitMQConfiguration
    {
        public RabbitMQConfiguration()
        {
        }
        public string RabbitMQUrl { get; set; }
        public string RabbitMQPort { get; set; }
        public string RabbitMQUserName { get; set; }
        public string RabbitMQPassword { get; set; }
    }
}