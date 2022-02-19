using Confluent.Kafka;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Accounts.MinimalWebAPI.Services
{
    public class KafkaConsumerService: BackgroundService
    {
        private readonly string _topic;
        private readonly ConsumerConfig _config;
        private readonly ILogger<KafkaConsumerService> _logger;
        private readonly IServiceProvider _serviceProvider;            

        public KafkaConsumerService(ILogger<KafkaConsumerService> logger, IOptions<KafkaConsumerConfig> options, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _config = new ConsumerConfig
            {
                GroupId = !string.IsNullOrEmpty(options.Value.GroupId)
                    ? options.Value.GroupId
                    : Guid.NewGuid().ToString(),
                BootstrapServers = options.Value.BootstrapServers,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
            _topic = options.Value.Topic;
            _serviceProvider = serviceProvider;            
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                using var consumer = new ConsumerBuilder<Ignore, string>(_config).Build();
                consumer.Subscribe(_topic);

                while (!stoppingToken.IsCancellationRequested)
                {
                    var consume = consumer.Consume(TimeSpan.FromSeconds(5)); 
                    if (consume is not null)
                    {
                        var message = JsonSerializer.Deserialize<string>(consume.Message.Value);
                        using var scope = _serviceProvider.CreateScope();
                        var accountsService = scope.ServiceProvider.GetRequiredService<IAccountsService>();
                        await accountsService.Create(message, stoppingToken);
                        //consumer.Commit(consume);
                    }
                }
                
                consumer.Close();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Oops, something went wrong: {ex}");
            }
        }
    }
}
