using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.Extensions.Configuration;
using Shared.Events.Constants;
using Shared.Events.Serializer;

namespace Stok.API.Services
{
    public class Bus: IBus
    {
        private readonly ConsumerConfig config;
        private readonly IConfiguration _configuration;
        public Bus(IConfiguration configuration)
        {
            _configuration= configuration;
          

        }       
        public ConsumerConfig GetConsumerConfig(string groupId)
        {
            var config = new ConsumerConfig()
            {
                BootstrapServers = _configuration.GetSection("BusSettings").GetSection("Kafka")["BootstrapServers"],
                GroupId = groupId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false
            };
            return config;
        }
    }
}
