using FiveWords.CommonModels.SavingChallengeResults;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace FiveWords.ChallengeResultsSaverService
{
    internal class RabbitMqListener : BackgroundService
    {
        private readonly ILogger<RabbitMqListener> _logger;

        private readonly IConnection _connection;
        private readonly IModel _channel;
        //private readonly QueueDeclareOk _queue;
        private readonly string _queueName;
        private readonly EventingBasicConsumer _consumer;
        private string? _consumerTag;
        private IChallengeResultsSaver _challengeResultsSaver;

        public RabbitMqListener(ILogger<RabbitMqListener> logger, IOptions<RabbitQueuesOptions> rabbitQueuesOptions, IChallengeResultsSaver challengeResultsSaver)
        {
            _logger = logger;
            _connection = new ConnectionFactory().CreateConnection();
            _channel = _connection.CreateModel();
            /* This queue should be already created by the main FiveWords service and exist.
             * _queue = _channel.QueueDeclare(
                rabbitQueuesOptions.Value.ChallengeResultsSavingQueueName,
                autoDelete: false,
                exclusive: false
                );*/
            _queueName = rabbitQueuesOptions.Value.ChallengeResultsSavingQueueName;
            _consumer = new EventingBasicConsumer(_channel);
            _challengeResultsSaver = challengeResultsSaver;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (stoppingToken.IsCancellationRequested)
                return;
            await Task.Yield();

            _consumer.Received += async (sender, e) =>
            {
                var messageBody = e.Body.ToArray();
                var messageBodyString = Encoding.UTF8.GetString(messageBody);
                try
                {
                    var messageBodyData = JsonSerializer.Deserialize<ChoosingRightOptionChallengeCompletedByUser<string, string>>(messageBodyString);
                    Guid challengeGuid = messageBodyData!.Id;
                    _logger.LogInformation($"Saving {challengeGuid} started at {DateTime.Now:HH.mm.ss.fff}");
                    await _challengeResultsSaver.AppendChallengeResultsAsync(messageBodyData!);
                    _logger.LogInformation($"Saving {challengeGuid} completed at {DateTime.Now:HH.mm.ss.fff}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Deserialize failed. Body string: {messageBodyString}");
                }
            };

            _consumerTag = _channel.BasicConsume(_queueName, autoAck: true, _consumer);

            /*while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation($"Worker running at: {DateTimeOffset.Now:mm:ss.fff}");
                await Task.Delay(1000, stoppingToken);
            }*/
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await Task.Yield();
            _channel.BasicCancel(_consumerTag);
            await base.StopAsync(cancellationToken);
        }

        public override void Dispose()
        {
            try { Console.WriteLine("Dispose called (ChallengeResultsSaver)"); } catch { }
            _channel?.Dispose();
            _connection?.Dispose();
            base.Dispose();
        }
    }
}
