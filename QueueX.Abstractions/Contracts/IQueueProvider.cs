
namespace QueueX;

public interface IQueueProvider
{
    /// <summary>
    /// Provedor atual (RabbitMQ, Kafka, etc.).
    /// </summary>
    QueueProvider Provider { get; }

    /// <summary>
    /// Cria um publisher de mensagens.
    /// </summary>
    IMessagePublisher CreatePublisher();

    /// <summary>
    /// Registra um consumidor tipado para determinada fila/tópico.
    /// </summary>
    /// <typeparam name="T">Tipo da mensagem consumida.</typeparam>
    /// <param name="queueOrTopic">Nome da fila ou tópico.</param>
    /// <param name="consumer">Instância do consumidor.</param>
    Task RegisterConsumer<T>(string queueOrTopic, IMessageConsumer<T> consumer);
}