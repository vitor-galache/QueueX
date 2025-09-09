using QueueX.Contracts;
using QueueX.Examples.Messages;

namespace QueueX.Examples.Consumers;

public class MyConsumer : IMessageConsumer<MyMessage>
{
    public async ValueTask HandleAsync(MyMessage message)
    {
        Console.WriteLine($"Received message: {message.Texto}");
        Task.Delay(5000).Wait();
        await Task.CompletedTask;
    }
}