
using Confluent.Kafka;
using Shared.Events.Constants;
using Shared.Events.Events;
using Shared.Events.Serializer;
using Stok.API.Services;

namespace Stok.API.BackgroundServices
{
    public class OrderCretedEventConsumerBackgroundService(IBus bus,ILogger<OrderCretedEventConsumerBackgroundService> _logger) : BackgroundService
    {
        private IConsumer<string,OrderCreatedEvent>? _consumer;
        public override Task StartAsync(CancellationToken cancellationToken)// When the Background service first started, this code works.
        {
            var config = bus.GetConsumerConfig(BusConstants.OrderCreatedEventTopicGroupId);
            _consumer = new ConsumerBuilder<string, OrderCreatedEvent>(config)
                .SetValueDeserializer(new CustomValueDeserializer<OrderCreatedEvent>())
                .Build();
            _consumer.Subscribe(BusConstants.OrderCreatedEventTopicName);

            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {
                var consumeResult = _consumer!.Consume(5000);
                if (consumeResult != null)
                {
                    try
                    {
                        var orderCreatedEvent = consumeResult.Message.Value;
                        // descrease from stock.
                        _logger.LogInformation($"Consumer Message Detail: " +
                            $"OrderCode: {orderCreatedEvent.OrderCode}, " +
                            $"TotalPrice: {orderCreatedEvent.TotalPrice} " +
                            $"UserId: {orderCreatedEvent.UserId}");

                        _consumer.Commit(consumeResult); // send a success message to Kafka.
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Ex: {ex.Message}");                       
                    }
                }
                await Task.Delay(10,stoppingToken);

            }
        }
    }
}
