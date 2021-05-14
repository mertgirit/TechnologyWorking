namespace MG.Queue.RabbitMQ.Models
{
    public class BaseQueueResult
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
    }
}