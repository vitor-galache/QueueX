using QueueX.Contracts;
using QueueX.Examples.Messages;

namespace QueueX.Examples;

public class Worker : BackgroundService
{
    private readonly IMessagePublisher _publisher;
    public Worker(IMessagePublisher publisher) => _publisher = publisher;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _publisher.PublishAsync(new MyMessage { Texto = "Olá!" }, "queue-test");
    }
}