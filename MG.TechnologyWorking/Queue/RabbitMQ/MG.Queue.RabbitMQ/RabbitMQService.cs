using System;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace MG.Queue.RabbitMQ
{
    using MG.Shared.Enums;
    using MG.Queue.RabbitMQ.Models;
    using MG.Shared.Configurations;
    using MG.Queue.RabbitMQ.Interfaces;

    public class RabbitMQService : IRabbitMQService
    {
        private static IConnection Connection;
        private RabbitMQConfiguration RabbitMQConfiguration;
        private readonly ILogger<RabbitMQService> logger;

        public static bool connectionIsBlocked = false;
        public static string connectionBlockedReason = string.Empty;

        public RabbitMQService(IConfiguration configuration,
                               ILogger<RabbitMQService> logger)
        {
            var RabbitMQConfiguration = configuration.GetSection("RabbitMQConfiguration").Get<RabbitMQConfiguration>();
            this.RabbitMQConfiguration = RabbitMQConfiguration;
            this.logger = logger;
        }

        #region PrivateMethods

        private void Connection_ConnectionUnblocked(object sender, EventArgs e)
        {
            logger.LogWarning(new EventId((int)EventName.RabbitMQService, $"{nameof(EventName.RabbitMQService)}/{nameof(Connection_ConnectionUnblocked)}"), $"RabbitMQ Connection is online");
            connectionIsBlocked = false;
            connectionBlockedReason = string.Empty;
        }

        private void Connection_ConnectionBlocked(object sender, ConnectionBlockedEventArgs e)
        {
            logger.LogError(new EventId((int)EventName.RabbitMQService, $"{nameof(EventName.RabbitMQService)}/{nameof(Connection_ConnectionBlocked)}"), $"RabbitMQ Connection is blocked. Reason: {e.Reason}");

            connectionIsBlocked = true;
            connectionBlockedReason = e.Reason;
        }

        private bool IsConnectionIsBlocked()
        {
            return connectionIsBlocked;
        }

        private ConnectionFactory GetConnectionFactory()
        {
            try
            {
                return new ConnectionFactory
                {
                    HostName = RabbitMQConfiguration.RabbitMQUrl,
                    Port = Convert.ToInt32(RabbitMQConfiguration.RabbitMQPort),
                    UserName = RabbitMQConfiguration.RabbitMQUserName,
                    Password = RabbitMQConfiguration.RabbitMQPassword,
                    DispatchConsumersAsync = true
                };
            }
            catch (Exception ex)
            {
                logger.LogError(new EventId((int)EventName.RabbitMQService, $"{nameof(EventName.RabbitMQService)}/{nameof(GetConnectionFactory)}"), ex, $"GetConnectionFactory Error.");
                throw;
            }
        }

        #endregion

        public async Task<IConnection> GetConnection()
        {
            try
            {
                IConnection connection = null;
                await Task.Run(() =>
                {
                    if (Connection != null && Connection.IsOpen)
                    {
                        connection = Connection;
                    }
                    else
                    {
                        Connection = GetConnectionFactory().CreateConnection();
                        connection = Connection;
                    }

                    Connection.ConnectionBlocked += Connection_ConnectionBlocked;
                    Connection.ConnectionUnblocked += Connection_ConnectionUnblocked;
                });

                return connection;
            }
            catch (Exception ex)
            {
                throw new Exception("Could not Create Connection ", ex);
            }
        }

        public async Task<uint> GetQueueCount(QueueName queueName)
        {
            try
            {
                if (IsConnectionIsBlocked())
                {
                    logger.LogError(new EventId((int)EventName.RabbitMQService, $"{nameof(EventName.RabbitMQService)}/{nameof(GetQueueCount)}"), $"RabbitMQ Connection is blocked. Reason: {connectionBlockedReason}");
                    return 0;
                }

                var connection = await GetConnection();
                using (var rabbitMQChannel = connection.CreateModel())
                {
                    var queueResult = rabbitMQChannel.QueueDeclare(queue: queueName.ToString(), durable: true, exclusive: false, autoDelete: false, arguments: null);
                    return queueResult.MessageCount;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(new EventId((int)EventName.RabbitMQService, $"{nameof(EventName.RabbitMQService)}/{nameof(GetQueueCount)}"), ex, $"GetQueueCount Error: RabbitMQ Connection is blocked. Reason: {connectionBlockedReason}");
                throw;
            }
        }

        public async Task<SendQueueResult> SendToQueue(QueueName queueType, string data)
        {
            //TODO MG: messageValidate
            if (string.IsNullOrEmpty(data))
            {
                throw new Exception("message is empty");
            }

            try
            {
                ReadOnlyMemory<byte> body = Encoding.UTF8.GetBytes(data);
                return await SendToQueue(queueType, body);
            }
            catch (Exception ex)
            {
                return new SendQueueResult { IsSuccess = false, ErrorMessage = ex.Message };
            }
        }

        public async Task<SendQueueResult> SendToQueue<T>(QueueName queueName, T data) where T : class
        {
            string json = JsonConvert.SerializeObject(data);
            return await SendToQueue(queueName, json);
        }

        public async Task<SendQueueResult> SendToQueue(QueueName queueName, ReadOnlyMemory<byte> data)
        {
            try
            {
                var connection = await GetConnection();
                using (var rabbitMQChannel = connection.CreateModel())
                {
                    var queueResult = rabbitMQChannel.QueueDeclare(queue: queueName.ToString(), durable: true, exclusive: false, autoDelete: false, arguments: null);

                    rabbitMQChannel.BasicPublish(exchange: "", routingKey: queueName.ToString(), basicProperties: null, body: data);
                }

                return new SendQueueResult { IsSuccess = true, ErrorMessage = "" };
            }
            catch (Exception ex)
            {
                return new SendQueueResult { IsSuccess = false, ErrorMessage = ex.Message };
            }
        }

        public async Task<SendQueueResult> SendToQueueBatch<T>(QueueName queueName, List<T> dataList) where T : class
        {
            if (IsConnectionIsBlocked())
            {
                return new SendQueueResult { IsSuccess = false, ErrorMessage = $"Connection is blocked. Reason: {connectionBlockedReason}" };
            }

            List<string> jsonList = new List<string>();

            foreach (var data in dataList)
            {
                string json = JsonConvert.SerializeObject(data);
                jsonList.Add(json);
            }

            return await SendToQueueBatch(queueName, jsonList);
        }

        public async Task<SendQueueResult> SendToQueueBatch(QueueName queueName, List<string> dataList)
        {
            if (IsConnectionIsBlocked())
            {
                return new SendQueueResult { IsSuccess = false, ErrorMessage = $"Connection is blocked. Reason: {connectionBlockedReason}" };
            }

            //TODO MG: messageValidate
            List<ReadOnlyMemory<byte>> dataListBytes = new List<ReadOnlyMemory<byte>>();

            try
            {
                foreach (var data in dataList)
                {
                    var body = Encoding.UTF8.GetBytes(data);
                    dataListBytes.Add(body);
                }
                return await SendToQueueBatch(queueName, dataListBytes);
            }
            catch (Exception ex)
            {
                return new SendQueueResult { IsSuccess = false, ErrorMessage = ex.Message };
            }
        }

        public async Task<SendQueueResult> SendToQueueBatch(QueueName queueName, List<ReadOnlyMemory<byte>> dataList)
        {
            try
            {
                var connection = await GetConnection();
                using (var rabbitMQChannel = connection.CreateModel())
                {
                    var queueResult = rabbitMQChannel.QueueDeclare(queue: queueName.ToString(), durable: true, exclusive: false, autoDelete: false, arguments: null);

                    IBasicPublishBatch publishBatch = rabbitMQChannel.CreateBasicPublishBatch();

                    foreach (var data in dataList)
                    {
                        publishBatch.Add(exchange: "", routingKey: queueName.ToString(), mandatory: true, null, body: data);
                    }

                    publishBatch.Publish();
                }

                return new SendQueueResult { IsSuccess = true, ErrorMessage = "" };
            }
            catch (Exception ex)
            {
                return new SendQueueResult { IsSuccess = false, ErrorMessage = ex.Message };
            }
        }

        /// <summary>
        /// Don't Use.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queueName"></param>
        /// <returns></returns>
        public async Task<List<ReceiveQueueResult<T>>> ReceiveBatchFromQueue<T>(QueueName queueName)
        {
            List<ReceiveQueueResult<T>> receiveQueueResults = new List<ReceiveQueueResult<T>>();

            var connection = await GetConnection();
            using (var rabbitMQChannel = connection.CreateModel())
            {
                string consumedMessage;

                var consumer = new EventingBasicConsumer(rabbitMQChannel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    consumedMessage = Encoding.UTF8.GetString(body.ToArray());
                    receiveQueueResults.Add(new ReceiveQueueResult<T>
                    {
                        ErrorMessage = string.Empty,
                        IsSuccess = true,
                        model = JsonConvert.DeserializeObject<T>(consumedMessage)
                    });
                };

                consumedMessage = rabbitMQChannel.BasicConsume(queue: queueName.ToString(),
                                 autoAck: true,
                                 consumer: consumer);
            }

            return receiveQueueResults;
        }

        /// <summary>
        /// Don't Use.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queueName"></param>
        /// <returns></returns>
        public async Task<ReceiveQueueResult<T>> ReceiveFromQueue<T>(QueueName queueName)
        {
            if (IsConnectionIsBlocked())
            {
                return new ReceiveQueueResult<T>
                {
                    model = default(T),
                    IsSuccess = false,
                    ErrorMessage = $"Connection is blocked. Reason: {connectionBlockedReason}"
                };
            }

            var connection = await GetConnection();
            using (var rabbitMQChannel = connection.CreateModel())
            {
                string consumedMessage = string.Empty;
                var queueResult = rabbitMQChannel.QueueDeclare(queue: queueName.ToString(), durable: true, exclusive: false, autoDelete: false, arguments: null);

                BasicGetResult result = null;
                try
                {
                    result = rabbitMQChannel.BasicGet(queue: queueName.ToString(), true);

                    if (result != null && result.Body.Length > 0)
                    {
                        T model = JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(result.Body.ToArray()));
                        return new ReceiveQueueResult<T>
                        {
                            ErrorMessage = string.Empty,
                            IsSuccess = true,
                            model = model
                        };
                    }
                    else
                    {
                        return new ReceiveQueueResult<T> { IsSuccess = false, ErrorMessage = $"Result is null. QueueName:{queueName}", model = default(T) };
                    }
                }
                catch (Exception ex)
                {
                    return new ReceiveQueueResult<T> { IsSuccess = false, ErrorMessage = ex.Message, model = default(T) };
                }
            }
        }
    }
}
