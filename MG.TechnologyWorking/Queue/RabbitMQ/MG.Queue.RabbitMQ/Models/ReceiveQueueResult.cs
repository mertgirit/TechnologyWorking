namespace MG.Queue.RabbitMQ.Models
{
    public class ReceiveQueueResult<T> : BaseQueueResult
    {
        public T model { get; set; }
    }
}