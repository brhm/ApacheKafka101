using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.Extensions.Configuration;
using Shared.Events.Serializer;

namespace Order.API.Services
{
    public class Bus: IBus
    {
        private readonly ProducerConfig config;
        private readonly IConfiguration _configuration;
        private readonly ILogger<Bus> _logger;
        public Bus(IConfiguration configuration,ILogger<Bus> logger)
        {
            _configuration= configuration;
            _logger= logger;
            config = new ProducerConfig()
            {
                BootstrapServers = _configuration.GetSection("BusSettings").GetSection("Kafka")["BootstrapServers"],
                Acks = Acks.All,
                //MessageSendMaxRetries=5 // retry count.
                MessageTimeoutMs = 10000, // retry timeout, then throw exception.
                //AllowAutoCreateTopics=true  // we created topic automatic if hier is true.
                //but then we change the code and we create topic with CreateTopicOrQueueAsync. and we call this method in programs.cs when set singleton.
            }; 

        }
        public async Task<bool> Publish<T1,T2>(T1 key, T2 value,string topicOrQueueName)
        {
            using var producer= new ProducerBuilder<T1,T2>(config)
                .SetKeySerializer(new CustomKeySerializer<T1>())
                .SetValueSerializer(new CustomValueSerializer<T2>())
                .Build();

            var message = new Message<T1, T2>()
            {
                Key = key,
                Value = value,
            };
            var result = await producer.ProduceAsync(topicOrQueueName, message);
            return result.Status == PersistenceStatus.Persisted;
        }

        public async Task CreateTopicOrQueueAsync(List<string> topicOrQueueNameList)
        {
            using var adminClient = new AdminClientBuilder(new AdminClientConfig()
            {
                BootstrapServers = _configuration.GetSection("BusSettings").GetSection("Kafka")["BootstrapServers"] 
                // Port in Docker Container.

            }).Build();

            try
            {
                // https://docs.confluent.io/platform/current/installation/configuration/topic-configs.html
                var config = new Dictionary<string, string>()
                {
                    {"message.timestamp.type","LogAppendTime" }
                };
                foreach (var topicOrQueue in topicOrQueueNameList)
                {
                    await adminClient.CreateTopicsAsync(new[]
                    {
                        new TopicSpecification()
                        {
                            Name=topicOrQueue, NumPartitions=6, ReplicationFactor=1, Configs = config
                        }
                    });
                    _logger.LogInformation($"Topic {topicOrQueue} oluştu.");
                    Console.WriteLine($"Topic {topicOrQueue} oluştu.");

                }

            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Topic : {ex.Message}");
                Console.WriteLine(ex.ToString());
            }

        }
    }
}
