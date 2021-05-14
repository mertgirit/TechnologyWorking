using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MG.Applications.WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> logger;

        private Task _executingTask;
        private CancellationTokenSource _cts;

        public Worker(ILogger<Worker> logger)
        {
            this.logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Worker service start");

            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _executingTask = ExecuteAsync(_cts.Token);

            return _executingTask.IsCompleted ? _executingTask : Task.CompletedTask;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            if (_executingTask == null)
            {
                return Task.CompletedTask;
            }

            logger.LogInformation("Worker service stopping");

            _cts.Cancel();

            Task.WhenAny(_executingTask, Task.Delay(-1, cancellationToken)).ConfigureAwait(true);

            cancellationToken.ThrowIfCancellationRequested();

            logger.LogInformation("Worker service stopped.");

            return Task.CompletedTask;
        }
    }
}