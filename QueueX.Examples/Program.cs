using QueueX.Examples;
using QueueX.Examples.Consumers;
using QueueX.Examples.Messages;
using QueueX;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();


builder.Services.AddQueueX(options =>
{
    options.Host = "localhost";
    options.User = "guest";
    options.Password = "guest";
    options.Port = 5672;
}).UseRabbitMq();

builder.Services.AddQueueConsumer<MyConsumer, MyMessage>("queue-test");

var host = builder.Build();
host.Run();