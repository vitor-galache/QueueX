using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QueueX.Configuration;
using QueueX.Contracts;
using QueueX.Services;

namespace QueueX.Extensions;

public static class ServiceCollectionExtensions
{
    
    /// <summary>
    /// Adiciona o QueueX ao container de serviços, com configuração via delegate.
    /// </summary>
    /// <param name="services">Coleção de serviços do container.</param>
    /// <param name="configure">Ação para configurar o QueueOptions.</param>
    public static IServiceCollection AddQueueX(this IServiceCollection services, Action<QueueOptions> configure)
    {
        var options = new QueueOptions();
        configure(options);
        services.AddSingleton(options);

        return services;
    }
    
    /// <summary>
    /// Registra um consumidor tipado para uma fila ou tópico específico.
    /// </summary>
    /// <typeparam name="TConsumer">Tipo do consumidor.</typeparam>
    /// <typeparam name="TMessage">Tipo da mensagem consumida.</typeparam>
    /// <param name="services">Coleção de serviços do container.</param>
    /// <param name="queueOrTopic">Nome da fila ou tópico.</param>
    public static IServiceCollection AddQueueConsumer<TConsumer, TMessage>(
        this IServiceCollection services,
        string queueOrTopic)
        where TConsumer : class, IMessageConsumer<TMessage>
    {
        services.AddTransient<IMessageConsumer<TMessage>, TConsumer>();

        services.AddHostedService(sp =>
            new QueueConsumerHostedService<TMessage>(
                sp.GetRequiredService<IQueueProvider>(),
                sp.GetRequiredService<IMessageConsumer<TMessage>>(),
                queueOrTopic));

        return services;
    }
}