using RabbitMQ.Client;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MG.Queue.RabbitMQ.Interfaces
{
    using MG.Shared.Enums;
    using MG.Queue.RabbitMQ.Models;
    using System;

    public interface IRabbitMQService
    {
        Task<IConnection> GetConnection();

        Task<uint> GetQueueCount(QueueName queueType);

        Task<SendQueueResult> SendToQueue(QueueName queueType, string data);

        Task<SendQueueResult> SendToQueue(QueueName queueType, ReadOnlyMemory<byte> data);

        Task<SendQueueResult> SendToQueue<T>(QueueName queueType, T data) where T : class;

        Task<SendQueueResult> SendToQueueBatch(QueueName queueType, List<ReadOnlyMemory<byte>> dataList);

        Task<SendQueueResult> SendToQueueBatch(QueueName queueType, List<string> dataList);

        Task<SendQueueResult> SendToQueueBatch<T>(QueueName queueType, List<T> dataList) where T : class;

        Task<ReceiveQueueResult<T>> ReceiveFromQueue<T>(QueueName queueType);

        Task<List<ReceiveQueueResult<T>>> ReceiveBatchFromQueue<T>(QueueName queueType);
    }
}