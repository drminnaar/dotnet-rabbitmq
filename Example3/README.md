# Publish / Subscribe

## Contents

- [Overview](#overview)
- [Characteristics](#characteristics)
- [Example Solution](#example-solution)
- [Running the Example](#running-the-example)

---

## Overview

![rmq-pubsub](https://user-images.githubusercontent.com/33935506/98722034-e02df500-23f5-11eb-88f4-982b2b3621ad.png)

---

## Characteristics

- TODO

---

## Example Solution

There are 2 parts to the solution. A _Producer_ and a _Consumer_. The _Producer_ is a .Net Core Console Application that sends _forecasts_ to a queue at a specific interval. The _Consumer_ is a .NET Core Console application that waits and consumes messages as messages arrive on the queue.

The following 2 sections, _Producer_ and _Consumer_, highlight the code required to interact with _RabbitMQ_

### Consumer

#### Step 1 - Create Connection

```csharp
var connectionFactory = new ConnectionFactory
{
    HostName = "localhost",
    UserName = "admin",
    Password = "password"
};

using var connection = await connectionFactory.CreateConnectionAsync();
```

#### Step 2 - Create Channel

```csharp
using var channel = await connection.CreateChannelAsync();
```

#### Step 3 - Declare Queue

```csharp
var queueName = channel.QueueDeclare().QueueName;
```

#### Step 4 - Declare Exchange

```csharp
const string ExchangeName = "example3_forecasts_exchange";

await channel.ExchangeDeclareAsync(exchange: ExchangeName, type: ExchangeType.Fanout);
```

#### Step 5 - Create Binding

```csharp
channel.QueueBind(
    queue: queueName,
    exchange: ExchangeName,
    routingKey: string.Empty);
```

#### Step 6 - Create Consumer

```csharp
var consumer = new AsyncEventingBasicConsumer(channel);

consumer.ReceivedAsync += async (sender, eventArgs) =>
{
    var body = eventArgs.Body.ToArray();
    var forecast = Forecast.FromBytes(body);

    DisplayInfo<Forecast>
        .For(forecast)
        .SetExchange(eventArgs.Exchange)
        .SetQueue(queueName)
        .SetRoutingKey(eventArgs.RoutingKey)
        .SetVirtualHost(connectionFactory.VirtualHost)
        .Display(Color.Yellow);

    await channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
};
```

#### Step 7 - Consume Messages

```csharp
await channel.BasicConsumeAsync(
    queue: queueName,
    autoAck: false,
    consumer: consumer);
```

#### Step 8 - Send Acknowledgements (ACKS)

```csharp
await channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
```

#### Full Listing

```csharp
namespace Rabbit.Example3.Consumer
{
    internal sealed class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine($"EXAMPLE 3 : PUB/SUB : CONSUMER");

            var connectionFactory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "admin",
                Password = "password"
            };

            var connection = connectionFactory.CreateConnection();

            var channel = connection.CreateModel();

            var queueName = channel.QueueDeclare().QueueName;

            const string ExchangeName = "example3_forecasts_exchange";

            await channel.ExchangeDeclareAsync(exchange: ExchangeName, type: ExchangeType.Fanout);

            channel.QueueBind(
                queue: queueName,
                exchange: ExchangeName,
                routingKey: string.Empty);

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (sender, eventArgs) =>
            {
                var body = eventArgs.Body.ToArray();
                var forecast = Forecast.FromBytes(body);

                DisplayInfo<Forecast>
                    .For(forecast)
                    .SetExchange(eventArgs.Exchange)
                    .SetQueue(queueName)
                    .SetRoutingKey(eventArgs.RoutingKey)
                    .SetVirtualHost(connectionFactory.VirtualHost)
                    .Display(Color.Yellow);

                await channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
            };

            await channel.BasicConsumeAsync(
                queue: queueName,
                autoAck: false,
                consumer: consumer);

            Console.ReadLine();
        }
    }
}
```

### Producer

#### Step 1 - Create Connection

```csharp
var connectionFactory = new ConnectionFactory
{
    HostName = "localhost",
    UserName = "admin",
    Password = "password"
};

using var connection = await connectionFactory.CreateConnectionAsync();
```

#### Step 2 - Create Channel

```csharp
using var channel = await connection.CreateChannelAsync();
```

#### Step 3 - Declare Exchange

```csharp
await channel.ExchangeDeclareAsync(exchange: ExchangeName, type: ExchangeType.Fanout);
```

#### Step 4 - Create and Publish Message

```csharp
var forecast = Thermometer.Fake().Report();

await channel.BasicPublishAsync(
    exchange: ExchangeName,
    routingKey: QueueName,
    body: Encoding.UTF8.GetBytes(forecast.ToJson())
);
```

#### Full Listing

```csharp
namespace Rabbit.Example3.Producer
{
    internal sealed class Program
    {
        internal static async Task Main()
        {
            Console.WriteLine($"EXAMPLE 3 : PUB/SUB : PRODUCER)");

            var connectionFactory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "admin",
                Password = "password"
            };

            using var connection = await connectionFactory.CreateConnectionAsync();

            using var channel = await connection.CreateChannelAsync();

            const string QueueName = "";
            const string ExchangeName = "example3_forecasts_exchange";

            await channel.ExchangeDeclareAsync(exchange: ExchangeName, type: ExchangeType.Fanout);

            while (true)
            {
                var forecast = Thermometer.Fake().Report();

                await channel.BasicPublishAsync(
                    exchange: ExchangeName,
                    routingKey: QueueName,
                    body: Encoding.UTF8.GetBytes(forecast.ToJson())
                );

                DisplayInfo<Forecast>
                    .For(forecast)
                    .SetExchange(ExchangeName)
                    .SetQueue(QueueName)
                    .SetRoutingKey(QueueName)
                    .SetVirtualHost(connectionFactory.VirtualHost)
                    .Display(Color.Yellow);

                await Task.Delay(millisecondsDelay: 3000);
            }
        }
    }
}
```

---

## Running the Example

> [!NOTE]
> &nbsp;  
> See [RabbitMQ Quickstart](/quickstart.md).  
> &nbsp;  

### Start Producer

```bash

# open new terminal and run the following command
dotnet run -p ./Example3/Rabbit.Example3.Producer/

```

### Start Worker 1

```bash

# open new terminal and run the following command
dotnet run -p ./Example3/Rabbit.Example3.Consumer/

```

### Start Worker 2

```bash

# open new terminal and run the following command
dotnet run -p ./Example3/Rabbit.Example3.Consumer/

```

### Display

![example-pubsub-1](https://user-images.githubusercontent.com/33935506/115130034-12e0b700-a040-11eb-90aa-b6b2126fadbc.png)