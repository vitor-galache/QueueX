namespace QueueX.Contracts;

public interface IMessagePublisher
{
    // <summary>
    /// Publica uma mensagem tipada em uma fila ou tópico.
    /// </summary>
    /// <typeparam name="T">Tipo da mensagem.</typeparam>
    /// <param name="message">Objeto da mensagem.</param>
    /// <param name="destination">Nome da fila ou tópico.</param>
    Task PublishAsync<T>(T message, string destination);
}