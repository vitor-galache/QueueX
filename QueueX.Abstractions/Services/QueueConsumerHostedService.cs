using Microsoft.Extensions.Hosting;
using QueueX.Contracts;

namespace QueueX.Services;

internal sealed class QueueConsumerHostedService<TMessage> : IHostedService
{
	private readonly IQueueProvider _provider;
	private readonly IMessageConsumer<TMessage> _consumer;
	private readonly string _queueOrTopic;

	public QueueConsumerHostedService(
		IQueueProvider provider,
		IMessageConsumer<TMessage> consumer,
		string queueOrTopic)
	{
		_provider = provider;
		_consumer = consumer;
		_queueOrTopic = queueOrTopic;
	}

	public Task StartAsync(CancellationToken cancellationToken)
	{
		return _provider.RegisterConsumer(_queueOrTopic, _consumer);
	}

	public async Task StopAsync(CancellationToken cancellationToken)
	{
		await Task.CompletedTask;
	}
}