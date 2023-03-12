using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace FiveWords.ChallengeResultsSaverService
{
    public class ChallengeResultsSaver : BackgroundService
    {
        private readonly ILogger<ChallengeResultsSaver> _logger;

        private readonly IConnection _connection;
        private readonly IModel _channel;
        //private readonly QueueDeclareOk _queue;
        private readonly string _queueName;
        private readonly EventingBasicConsumer _consumer;
        private string? _consumerTag;

        public ChallengeResultsSaver(ILogger<ChallengeResultsSaver> logger, IOptions<RabbitQueuesOptions> rabbitQueuesOptions)
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
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (stoppingToken.IsCancellationRequested)
                return;

            _consumer.Received += (sender, e) =>
            {
                Console.WriteLine($"{_consumer.IsRunning}, {_consumer.ConsumerTags.Length}, {_consumer.ConsumerTags.FirstOrDefault()}");
                Console.WriteLine($"Received message: {Encoding.UTF8.GetString(e.Body.ToArray())}");
            };

            _consumerTag = _channel.BasicConsume(_queueName, autoAck: true, _consumer);

            /*while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation($"Worker running at: {DateTimeOffset.Now:mm:ss.fff}");
                await Task.Delay(1000, stoppingToken);
            }*/
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _channel.BasicCancelNoWait(_consumerTag);
            return base.StopAsync(cancellationToken);
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
