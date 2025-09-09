using Microsoft.Extensions.DependencyInjection;
using QueueX.Configuration;
using QueueX.Contracts;
using QueueX.RabbitMQ.Provider;

namespace QueueX.RabbitMQ.Extensions;

public static class RabbitMqServiceCollectionExtensions
{
    public static IServiceCollection UseRabbitMq(this IServiceCollection services)
    {
        services.AddSingleton<IQueueProvider>(sp =>
        {
            var options = sp.GetRequiredService<QueueOptions>();
            return new RabbitMqProvider(options);
        });
        
        services.AddTransient<IMessagePublisher>(sp =>
        {
            var provider = sp.GetRequiredService<IQueueProvider>();
            return provider.CreatePublisher();
        });
        
        return services;
    }
}