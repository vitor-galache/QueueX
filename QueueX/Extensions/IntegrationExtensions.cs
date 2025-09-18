using Microsoft.Extensions.DependencyInjection;
using QueueX.RabbitMQ.Extensions;

namespace QueueX;

public static class IntegrationExtensions
{
    public static IServiceCollection UseRabbitMq(this IServiceCollection services)
    {
        return RabbitMqServiceCollectionExtensions.UseRabbitMq(services);
    }

}