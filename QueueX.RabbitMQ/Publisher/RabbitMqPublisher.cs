using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace QueueX.RabbitMQ.Publisher;

internal sealed class RabbitMqPublisher : IMessagePublisher
{
    private readonly IConnection _connection;
    public RabbitMqPublisher(IConnection connection)
    {
        _connection = connection;
    }

    public async Task PublishAsync<T>(T message, string destination)
    {
        await using var channel = await _connection.CreateChannelAsync();

        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);
        
        var props = new BasicProperties
        {
            DeliveryMode = DeliveryModes.Persistent
        };

        await channel.BasicPublishAsync(
            exchange: "",
            routingKey: destination, 
            mandatory: false,
            basicProperties: props, 
            body: body);
    }
}