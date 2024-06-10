using Confluent.Kafka;
using Shared.Events.Serializer;

namespace Order.API.Services
{
    public class Bus: IBus
    {
        private readonly ProducerConfig config;
        public Bus(IConfiguration configuration)
        {

            config = new ProducerConfig()
            {
                BootstrapServers = configuration.GetSection("BusSettings").GetSection("Kafka")["BootstrapServers"],
                Acks = Acks.All,
                //MessageSendMaxRetries=5 // retry count.
                MessageTimeoutMs = 10000, // retry timeout, then throw exception.
                AllowAutoCreateTopics=true
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
    }
}
