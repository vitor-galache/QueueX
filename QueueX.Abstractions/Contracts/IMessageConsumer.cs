namespace QueueX;

/// <summary>
/// Define um consumidor de mensagens tipadas.
/// </summary>
/// <typeparam name="T">Tipo da mensagem que será consumida.</typeparam>
public interface IMessageConsumer<T>
{
    /// <summary>
    /// Manipula a mensagem recebida da fila ou tópico.
    /// </summary>
    /// <param name="message">Mensagem recebida.</param>
    ValueTask HandleAsync(T message);    
}