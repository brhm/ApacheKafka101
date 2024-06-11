using Order.API.Services;
using Shared.Events.Constants;

namespace Order.API.Extensions
{
    public static class BusExtension
    {
        public static async Task CreateTopicOrQueuesAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var bus = scope.ServiceProvider.GetRequiredService<IBus>();

            await bus.CreateTopicOrQueueAsync([BusConstants.OrderCreatedEventTopicName]);
        }
    }
}
