using Confluent.Kafka;

namespace Stok.API.Services
{
    public interface IBus
    {
        ConsumerConfig GetConsumerConfig(string groupId);
    }
}
