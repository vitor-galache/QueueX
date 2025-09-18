using Moq;
using QueueX.RabbitMQ.Provider;
using QueueX.Tests.RabbitMq.Mocks;
using RabbitMQ.Client;

namespace QueueX.Tests.RabbitMq;

public class RabbitMqProviderTests
{
    [Fact]
    public void Constructor_Should_Initialize_With_Queue_Options_And_ProviderType_Correct()
    {
        var options = new QueueOptions
        {
            Host = "localhost",
            Port = 5672,
            User = "guest",
            Password = "guest"
        };
        var mockConnection = new Mock<IConnection>();


        var provider = new RabbitMqProvider(options, mockConnection.Object);


        Assert.NotNull(provider);
        Assert.Equal(QueueProvider.RabbitMq, provider.Provider);
    }

    [Fact]
    public void CreatePublisher_Should_Return_Instance_Of_IMessagePublisher()
    {
        var options = new QueueOptions
        {
            Host = "localhost",
            Port = 5672,
            User = "guest",
            Password = "guest"
        };
        var mockConnection = new Mock<IConnection>();
        var provider = new RabbitMqProvider(options, mockConnection.Object);

        var publisher = provider.CreatePublisher();

        Assert.NotNull(publisher);
        Assert.IsAssignableFrom<IMessagePublisher>(publisher);
    }

    [Fact]
    public async Task RegisterConsumer_Should_DeclareQueue()
    {
        var options = new QueueOptions
        {
            Host = "localhost",
            Port = 5672,
            User = "guest",
            Password = "guest"
        };

        var mockConnection = new Mock<IConnection>();
        var mockChannel = new Mock<IChannel>();

        mockConnection.Setup(c => c.CreateChannelAsync(It.IsAny<CreateChannelOptions>(),It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockChannel.Object);


        var queueDeclareOk = new QueueDeclareOk("test-queue", 0, 0);

        mockChannel.Setup(c => c.QueueDeclareAsync(
               It.IsAny<string>(),
               It.IsAny<bool>(),
               It.IsAny<bool>(),
               It.IsAny<bool>(),
               It.IsAny<IDictionary<string, object?>?>(),
               It.IsAny<bool>(),
               It.IsAny<bool>(),
               It.IsAny<CancellationToken>()))
               .ReturnsAsync(queueDeclareOk);


        var provider = new RabbitMqProvider(options, mockConnection.Object);

        var mockConsumer = new Mock<IMessageConsumer<TestMessage>>();

        await provider.RegisterConsumer("test-queue", mockConsumer.Object);

        mockConnection.Verify(c => c.CreateChannelAsync(It.IsAny<CreateChannelOptions>(), It.IsAny<CancellationToken>()), Times.Once);

        mockChannel.Verify(c => c.QueueDeclareAsync(
        "test-queue",
        true,
        false,
        false,
        null,
        false,
        false,
        It.IsAny<CancellationToken>()), Times.Once);
    }
}
