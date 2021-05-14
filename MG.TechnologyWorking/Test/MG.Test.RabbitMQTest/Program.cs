using System;
using System.Text;
using RabbitMQ.Client;
using System.Threading.Tasks;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MG.Test.RabbitMQTest
{
    using MG.Shared.Enums;
    using MG.Queue.RabbitMQ;
    using MG.Queue.RabbitMQ.Interfaces;

    class Program
    {
        static IRabbitMQService RabbitMQService;
        static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            var serviceProvider = new ServiceCollection()
                .AddSingleton<IConfiguration>(configuration)
                .AddSingleton<IRabbitMQService, RabbitMQService>()
                .AddLogging()
                .BuildServiceProvider();

            RabbitMQService = serviceProvider.GetService<IRabbitMQService>();

            Parallel.Invoke(() =>
            {
                Task.Run(() => ReceiveMessages());
                Task.Run(() => ProduceMessages());
            });

            Console.WriteLine("Test is finished");
            Console.ReadLine();
        }

        private static async Task ProduceMessages()
        {
            for (int i = 1; i <= 100; i++)
            {
                await RabbitMQService.SendToQueue(QueueName.Name1, $"message{i}");
                Console.WriteLine($"message{i} sent to rabbitmq");
            }
        }

        private static async Task ReceiveMessages()
        {
            var connection = await RabbitMQService.GetConnection();
            var channel = connection.CreateModel();

            var queue = channel.QueueDeclare(QueueName.Name1.ToString(), true, false, false, null);

            string consumedMessage;
            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
             {
                 try
                 {
                     var body = ea.Body;
                     consumedMessage = Encoding.UTF8.GetString(body.ToArray());

                     var result = await ProcessMessage(consumedMessage);

                     if (result.IsSuccess)
                     {
                         channel.BasicAck(ea.DeliveryTag, false);
                     }
                     else if (ea.Redelivered)
                     {
                         channel.BasicAck(ea.DeliveryTag, false);
                         //Tekrar mesaj receive edilmesine rağmen işlenemedi. Kuyruğu tıkamaması için ack edildi.
                     }
                     else
                     {
                         channel.BasicNack(ea.DeliveryTag, false, true);// mesaj işlenirken hata oluştu, tekrar queueya alınsın.
                     }
                 }
                 catch (Exception ex)
                 {
                     try
                     {
                         if (ea.Redelivered)
                         {
                             channel.BasicAck(ea.DeliveryTag, false);
                         }
                         else
                         {
                             channel.BasicNack(ea.DeliveryTag, false, true);
                         }
                     }
                     catch (Exception ex2)
                     {
                         throw;
                     }
                 }
             };

            channel.BasicQos(0, 1, false);
            consumedMessage = channel.BasicConsume(queue: QueueName.Name1.ToString(), autoAck: false, consumer: consumer);
        }

        private static async Task<ResponseModel> ProcessMessage(string message)
        {
            await Task.Run(() =>
            {
                Console.WriteLine($"message receive from rabbitmq. Message: {message}");
            });

            return new ResponseModel { IsSuccess = true, ErrorMessage = string.Empty };
        }
    }

    class ResponseModel
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
    }
}