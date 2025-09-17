using Moq;
using QueueX.RabbitMQ.Publisher;
using QueueX.Tests.RabbitMq.Mocks;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace QueueX.Tests.RabbitMq;

public class RabbitMqPublisherTests
{
    [Fact]
    public async Task PublishAsync_Should_CreateChannel_And_PublishMessage()
    {
        // ARRANGE
        var mockConnection = new Mock<IConnection>();
        var mockChannel = new Mock<IChannel>();

        mockConnection.Setup(c => c.CreateChannelAsync(It.IsAny<CreateChannelOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockChannel.Object);

        mockChannel.Setup(c => c.BasicPublishAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<bool>(),
            It.IsAny<BasicProperties>(),
            It.IsAny<ReadOnlyMemory<byte>>(),
            It.IsAny<CancellationToken>()))
            .Returns(ValueTask.CompletedTask)
            .Verifiable();

        var publisher = new RabbitMqPublisher(mockConnection.Object);

        var message = new TestMessage { Content = "Hello, RabbitMQ!" };
        var destination = "test-queue";

        // ACT

        await publisher.PublishAsync(message, destination);


        // ASSERT
        
        // Verifica se CreateChannelAsync foi chamado uma vez
        mockConnection.Verify(c => c.CreateChannelAsync(It.IsAny<CreateChannelOptions>(), It.IsAny<CancellationToken>()), Times.Once);

        // Verifica se BasicPublishAsync foi chamado
        mockChannel.Verify(c => c.BasicPublishAsync<BasicProperties>(
       "",
       destination,
       false,
       It.Is<BasicProperties>(p => p.DeliveryMode == DeliveryModes.Persistent),
       It.Is<ReadOnlyMemory<byte>>(body =>
           Encoding.UTF8.GetString(body.ToArray(), 0, body.Length) == JsonSerializer.Serialize(message, (JsonSerializerOptions?)null)),
       It.IsAny<CancellationToken>()), Times.Once);

    }
}