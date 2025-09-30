# QueueX

Pacote com intuito de simplificar a integração de serviços de mensageria (RabbitMQ, Kafka) com ecossistema .NET.


## Introdução

### Pré-requisitos
- .NET 9

### Instalação

1. Abra o Package Manager Console no Visual Studio e execute o comando `Install-Package QueueX`
2. Via CLI execute: `dotnet add package QueueX`

## Exemplo de Uso

### Configuração
Adicione a configuração Program.cs do seu serviço para registrar o QueueX e os seus consumidores personalizados:

```csharp
// Program.cs

builder.Services.AddQueueX(options =>
{
    options.Host = "localhost";
    options.User = "guest";
    options.Password = "guest";
    options.Port = 5672;
}).UseRabbitMq(); // Configure RabbitMQ ou Kafka aqui

builder.Services.AddQueueConsumer<MyConsumer, MyMessage>("queue-test");
```
Integração disponível apenas para RabbitMQ atualmente.

### Messages
As mensagens serão as classes que irão ser desserializadas ao consumir a mensagem no seu Background Service, cada mensagem deve ter apenas um consumidor registrado para ela.


```csharp
public class MyMessage
{
    public string Content = "Olá";
}
```


### Consumers
Os consumidores devem implementar a interface IMessageConsumer, que possuí um método HandleAsync que irá indicar como a mensagem será processada ao ser consumida.
```csharp
public class MyConsumer : IMessageConsumer<MyMessage>
{
    public async ValueTask HandleAsync(MyMessage message)
    {
        Console.WriteLine($"Received message: {message.Content}");
        Task.Delay(5000).Wait();
        await Task.CompletedTask;
    }
}
```


### Publishers
Para publicar mensagens, basta injetar a interface IMessagePublisher no seu serviço. 

Exemplo de um BackgroundService que publica mensagens periodicamente:

```csharp
public class Worker : BackgroundService
{
    private readonly IMessagePublisher _publisher;
    public Worker(IMessagePublisher publisher) => _publisher = publisher;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Publica uma mensagem do tipo MyMessage na fila "queue-test"
        await _publisher.PublishAsync(new MyMessage { Texto = "Olá!" }, "queue-test");
    }
}

```
