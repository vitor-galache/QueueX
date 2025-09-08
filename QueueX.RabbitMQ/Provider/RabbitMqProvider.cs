using System.Text;
using System.Text.Json;
using QueueX.Configuration;
using QueueX.Contracts;
using QueueX.RabbitMQ.Publisher;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace QueueX.RabbitMQ.Provider;

public class RabbitMqProvider : IQueueProvider
{
    private readonly IConnection _connection;
    private readonly QueueOptions _options;

    public RabbitMqProvider(QueueOptions options)
    {
        _options = options;
        var factory = new ConnectionFactory
        {
            HostName = options.Host,
            Port = options.Port,
            UserName = options.User,
            Password = options.Password,
        };

        _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
    }

    public QueueProvider Provider => QueueProvider.RabbitMq;

    public IMessagePublisher CreatePublisher() => new RabbitMqPublisher(_connection);

    public void RegisterConsumer<T>(string queueOrTopic, IMessageConsumer<T> consumer)
    {
        _ = RegisterConsumerInternal(queueOrTopic, consumer);
    }

    private async Task RegisterConsumerInternal<T>(string queueOrTopic, IMessageConsumer<T> consumer)
    {
        var channel = await _connection.CreateChannelAsync();
        await channel.QueueDeclareAsync(
            queue: queueOrTopic,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var asyncConsumer = new AsyncEventingBasicConsumer(channel);
        asyncConsumer.ReceivedAsync += async (sender, ea) =>
        {
            try
            {
                var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                var message = JsonSerializer.Deserialize<T>(json);
                if (message is not null)
                {
                    await consumer.HandleAsync(message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao processar mensagem: {ex}");
            }
        };
        
        await channel.BasicConsumeAsync(
            queue: queueOrTopic,
            autoAck: true,
            consumer: asyncConsumer);
    }
}