# Topics

## Contents

- [Overview](#overview)
- [Characteristics](#characteristics)
- [Example Solution](#example-solution)
- [Running the Example](#running-the-example)

---

## Overview

![example-topics-overview](https://user-images.githubusercontent.com/33935506/115108890-efc5f100-9fc6-11eb-9f00-dd1427755383.png)

---

## Characteristics

- Messages are sent to an exchange of type _Topic_
- The messages sent to exchange must use a "dot delimited list" format eg usa.banking.sell, australia.software.buy
- Consumers define the topics that they're interested in. For example:
  
  ```text
  Producer:
    Routing Key = australia.software.buy
    Routing Key = usa.banking.sell
  
  Consumer1
    Topic = #
    Result: Get's all messages for above routing keys

  Consumer2
    Topic = usa.banking.*
    Result: Get's all messages starting with "usa.banking"
  ```

---

## Example Solution

There are 2 parts to the solution. A _Producer_ and a _Consumer_. The _Producer_ is a .Net Core Console Application that sends _trades_ to a _Topic Exchange_ at a specific interval. The _Consumer_ is a .NET Core Console application that waits and consumes messages as messages arrive on the queue.

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
const string ExchangeName = "example5_trades_exchange";

await channel.ExchangeDeclareAsync(exchange: ExchangeName, type: ExchangeType.Topic);
```

#### Step 5 - Create Binding

```csharp
channel.QueueBind(
    queue: queueName,
    exchange: ExchangeName,
    routingKey: topic);
```

#### Step 6 - Create Consumer

```csharp
var consumer = new AsyncEventingBasicConsumer(channel);

consumer.ReceivedAsync += async (sender, eventArgs) =>
{
    var messageBody = eventArgs.Body.ToArray();
    var trade = Trade.FromBytes(messageBody);

    DisplayInfo<Trade>
        .For(trade)
        .SetExchange(eventArgs.Exchange)
        .SetQueue(queue.QueueName)
        .SetRoutingKey(eventArgs.RoutingKey)
        .SetTopic(eventArgs.RoutingKey)
        .SetVirtualHost(connectionFactory.VirtualHost)
        .Display(Color.Yellow);
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
private static async Task Main(string[] topics)
{
    Console.WriteLine("\nEXAMPLE 5 : TOPICS : CONSUMER");

    if (topics == null || topics.Length < 1)
    {
        Console.WriteLine("\nMessage type not specified. Try the following:".Pastel(Color.Tomato));
        Console.WriteLine("  - dotnet run # (match all)".Pastel(Color.Tomato));
        Console.WriteLine("  - dotnet run australia.*.buy (match australia.software.buy, australia.banking.buy etc)".Pastel(Color.Tomato));
        Console.WriteLine("  - dotnet run *.software.sell (match usa.software.sell, greatbritain.software.sell etc)".Pastel(Color.Tomato));
        Console.WriteLine();
        Environment.ExitCode = 1;
        return;
    }

    var connectionFactory = new ConnectionFactory
    {
        HostName = "localhost",
        UserName = "admin",
        Password = "password"
    };

    using var connection = await connectionFactory.CreateConnectionAsync();

    using var channel = await connection.CreateChannelAsync();

    const string ExchangeName = "example5_trades_exchange";

    await channel.ExchangeDeclareAsync(exchange: ExchangeName, type: ExchangeType.Topic);

    var queue = await channel.QueueDeclareAsync();

    foreach (var topic in topics)
    {
        Console.WriteLine(topic);
        await channel.QueueBindAsync(
            queue: queue.QueueName,
            exchange: ExchangeName,
            routingKey: topic);
    }

    var consumer = new AsyncEventingBasicConsumer(channel);

    consumer.ReceivedAsync += async (sender, eventArgs) =>
    {
        var messageBody = eventArgs.Body.ToArray();
        var trade = Trade.FromBytes(messageBody);

        DisplayInfo<Trade>
            .For(trade)
            .SetExchange(eventArgs.Exchange)
            .SetQueue(queue.QueueName)
            .SetRoutingKey(eventArgs.RoutingKey)
            .SetTopic(eventArgs.RoutingKey)
            .SetVirtualHost(connectionFactory.VirtualHost)
            .Display(Color.Yellow);

        await channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
    };

    await channel.BasicConsumeAsync(
        queue: queue.QueueName,
        autoAck: false,
        consumer: consumer);

    Console.ReadLine();
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
await channel.ExchangeDeclareAsync(exchange: ExchangeName, type: ExchangeType.Topic);
```

#### Step 4 - Create and Publish Message

```csharp
var trade = TradeData.GetFakeTrade();

var topic = $"{trade.NormalizedRegion}.{trade.NormalizedIndustry}.{trade.NormalizedAction}";

await channel.BasicPublishAsync(
    exchange: ExchangeName,
    routingKey: topic,
    body: trade.ToBytes());
```

#### Full Listing

```csharp
private static async Task Main()
{
    Console.WriteLine("\nEXAMPLE 5 : TOPICS : PRODUCER");

    var connectionFactory = new ConnectionFactory
    {
        HostName = "localhost",
        UserName = "admin",
        Password = "password"
    };

    using var connection = await connectionFactory.CreateConnectionAsync();

    using var channel = await connection.CreateChannelAsync();

    const string ExchangeName = "example5_trades_exchange";

    await channel.ExchangeDeclareAsync(exchange: ExchangeName, type: ExchangeType.Topic);

    while (true)
    {
        var trade = TradeData.GetFakeTrade();

        var topic = $"{trade.NormalizedRegion}.{trade.NormalizedIndustry}.{trade.NormalizedAction}";

        await channel.BasicPublishAsync(
            exchange: ExchangeName,
            routingKey: topic,
            body: trade.ToBytes());

        DisplayInfo<Trade>
            .For(trade)
            .SetExchange(ExchangeName)
            .SetRoutingKey(topic)
            .SetTopic(topic)
            .SetVirtualHost(connectionFactory.VirtualHost)
            .Display(Color.Cyan);

        await Task.Delay(millisecondsDelay: 5000);
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
# change directory to Example5 Producer
cd ./Example5/Rabbit.Example5.Producer

# open new terminal and run the following command
dotnet run

```

### Start Consumer 1

```bash
# change directory to Example5 Consumer
cd ./Example5/Rabbit.Example5.Consumer

# open new terminal and run the following command
# to start consumer that will consume ALL messages
# being sent by the producer
# usage: dotnet run topic_name
dotnet run "#"

```

### Start Consumer 2

```bash
# change directory to Example5 Consumer
cd ./Example5/Rabbit.Example5.Consumer

# open new terminal and run the following command
# to start consumer that will consumer all messages
# having a routing key ending in "buy"
# usage: dotnet run topic_name
dotnet run "#.buy"

```

### Start Consumer 3

```bash
# change directory to Example5 Consumer
cd ./Example5/Rabbit.Example5.Consumer

# to start consumer that will consumer all messages
# having a routing key starting with "usa" and ending in "sell"
# usage: dotnet run topic_name
dotnet run "usa.*.sell"

```

### Start Consumer 4

```bash
# change directory to Example5 Consumer
cd ./Example5/Rabbit.Example5.Consumer

# to start consumer that will consumer all messages
# having a routing key containing "banking"
# usage: dotnet run topic_name
dotnet run "*.banking.*"

```

### Display

![example-topics-1](https://user-images.githubusercontent.com/33935506/115107917-be96f200-9fc1-11eb-9a38-c06a30fa6129.png)

![example-topics-2](https://user-images.githubusercontent.com/33935506/115107920-bfc81f00-9fc1-11eb-8733-d60f3a188788.png)