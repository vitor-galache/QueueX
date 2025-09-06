using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using QueueX.Configuration;
using QueueX.Contracts;

namespace QueueX.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddQueueX(
        this IServiceCollection services,
        Action<QueueOptions> configureOptions,
        params Assembly[] consumerAssemblies)
    {
        var options = new QueueOptions();
        configureOptions(options);
        services.AddSingleton(options);

        if (string.IsNullOrWhiteSpace(options.Host))
        {
            throw new InvalidOperationException("O Host do provedor de mensageria precisa ser configurado.");
        }

        RegisterConsumers(services, consumerAssemblies);

        switch (options.Provider)
        {
            case QueueProvider.RabbitMq:
                throw new NotImplementedException("O provedor RabbitMQ ainda não foi implementado.");
            
            case QueueProvider.Kafka:
                throw new NotImplementedException("O provedor Kafka ainda não foi implementado.");

            default:
                throw new ArgumentOutOfRangeException(nameof(options.Provider), "Provedor de mensageria não suportado ou inválido.");
        }

        return services;
    }

    private static void RegisterConsumers(IServiceCollection services, Assembly[] assembliesToScan)
    {
        if (assembliesToScan.Length == 0)
        {
            assembliesToScan = new[] { Assembly.GetEntryAssembly()! };
        }

        var consumerTypes = assembliesToScan
            .SelectMany(a => a.GetTypes())
            .Where(t => t.IsClass && !t.IsAbstract && t.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMessageConsumer<>)))
            .ToList();

        foreach (var type in consumerTypes)
        {
            services.AddScoped(type);
        }
    }
}