# Routing

## Contents

- [Overview](#overview)
- [Characteristics](#characteristics)
- [Example Solution](#example-solution)
- [Running the Example](#running-the-example)

---

## Overview

![example-routing-overview](https://user-images.githubusercontent.com/33935506/115131049-e715ff00-a048-11eb-8d05-82c30d665072.png)

---

## Characteristics

The _Routing Message Pattern_ uses a _Direct Exchange_, and captures the idea of allowing _Consumers_ to receive messages selectively. The _Producer_ publishes messages using various _Routing Keys_ to a _Direct Exchange_. _Consumers_ selectively subscribe to messages of interest using specific _Routing Keys_.

---

## Example Solution

There are 2 parts to the solution. A _Producer_ and a _Consumer_. The _Producer_ is a .Net Core Console Application that sends _trades_ to a queue at a specific interval. The _Consumer_ is a .NET Core Console application that waits and consumes messages as messages arrive on the queue.

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
const string ExchangeName = "example4_trades_exchange";

channel.ExchangeDeclare(
    exchange: ExchangeName,
    type: ExchangeType.Direct,
    durable: false,
    autoDelete: false,
    arguments: ImmutableDictionary<string, object>.Empty);
```

#### Step 5 - Create Binding

```csharp
var queue = await channel.QueueDeclareAsync(
    queue: QueueNames[region],
    durable: false,
    exclusive: false,
    autoDelete: false,
    arguments: ImmutableDictionary<string, object>.Empty);

channel.QueueBind(
    queue: queue.QueueName,
    exchange: ExchangeName,
    routingKey: region);
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
        .SetVirtualHost(connectionFactory.VirtualHost)
        .Display(Color.Yellow);

    await channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
};
```

#### Step 7 - Consume Messages

```csharp
await channel.BasicConsumeAsync(
    queue: queue.QueueName,
    autoAck: false,
    consumer: consumer);
```

#### Step 8 - Send Acknowledgements (ACKS)

```csharp
await channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
```

#### Full Listing

```csharp
private static async Task Main(string[] regions)
{
    Console.WriteLine("\nEXAMPLE 4 : ROUTING : CONSUMER");

    var region = regions.FirstOrDefault() ?? string.Empty;

    var QueueNames = TradeData
        .Regions
        .Select(region =>
        {
            var normalizedRegion = region.ToLower().Trim().Replace(" ", string.Empty);
            var queueName = $"example4_trades_{normalizedRegion}_queue";
            return new KeyValuePair<string, string>(region, queueName);
        })
        .ToImmutableDictionary();

    if (!QueueNames.ContainsKey(region))
    {
        Console.WriteLine($"\nInvalid region '{region}'.".Pastel(Color.Tomato));
        Console.WriteLine($"Enter valid region name to start ({string.Join(", ", QueueNames.Keys)})".Pastel(Color.Tomato));
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

    const string ExchangeName = "example4_trades_exchange";

    await channel.ExchangeDeclareAsync(
        exchange: ExchangeName,
        type: ExchangeType.Direct,
        durable: false,
        autoDelete: false,
        arguments: new Dictionary<string, object?>());

    var queue = await channel.QueueDeclareAsync(
        queue: QueueNames[region],
        durable: false,
        exclusive: false,
        autoDelete: false,
        arguments: new Dictionary<string, object?>());

    await channel.QueueBindAsync(
        queue: queue.QueueName,
        exchange: ExchangeName,
        routingKey: region);

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
const string ExchangeName = "example4_trades_exchange";

channel.ExchangeDeclare(
    exchange: ExchangeName,
    type: ExchangeType.Direct,
    durable: false,
    autoDelete: false,
    arguments: ImmutableDictionary<string, object>.Empty);
```

#### Step 4 - Create Bindings

```csharp
var QueueNames = TradeData
    .Regions
    .Select(region =>
    {
        var normalizedRegion = region.ToLower().Trim().Replace(" ", string.Empty);
        var queueName = $"example4_trades_{normalizedRegion}_queue";
        return new KeyValuePair<string, string>(region, queueName);
    })
    .ToImmutableDictionary();

foreach (var region in TradeData.Regions)
{
    var queue = await channel.QueueDeclareAsync(
        queue: QueueNames[region],
        durable: false,
        exclusive: false,
        autoDelete: false,
        arguments: ImmutableDictionary<string, object>.Empty);

    channel.QueueBind(
        queue: queue.QueueName,
        exchange: ExchangeName,
        routingKey: region,
        arguments: ImmutableDictionary<string, object>.Empty);
}
```

#### Step 5 - Create and Publish Message

```csharp
var trade = TradeData.GetFakeTrade();

string routingKey = trade.Region;

await channel.BasicPublishAsync(
    exchange: ExchangeName,
    routingKey: routingKey,
    body: trade.ToBytes()
);
```

#### Full Listing

```csharp
private static async Task Main()
{
    Console.WriteLine("EXAMPLE 4 : ROUTING : PRODUCER");

    var connectionFactory = new ConnectionFactory
    {
        HostName = "localhost",
        UserName = "admin",
        Password = "password"
    };

    using var connection = await connectionFactory.CreateConnectionAsync();

    using var channel = await connection.CreateChannelAsync();

    const string ExchangeName = "example4_trades_exchange";

    await channel.ExchangeDeclareAsync(
        exchange: ExchangeName,
        type: ExchangeType.Direct,
        durable: false,
        autoDelete: false,
        arguments: new Dictionary<string, object?>());

    var QueueNames = TradeData
        .Regions
        .Select(region =>
        {
            var normalizedRegion = region.ToLower().Trim().Replace(" ", string.Empty);
            var queueName = $"example4_trades_{normalizedRegion}_queue";
            return new KeyValuePair<string, string>(region, queueName);
        })
        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

    foreach (var region in TradeData.Regions)
    {
        var queue = await channel.QueueDeclareAsync(
            queue: QueueNames[region],
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: new Dictionary<string, object?>());

        await channel.QueueBindAsync(
            queue: queue.QueueName,
            exchange: ExchangeName,
            routingKey: region,
            arguments: new Dictionary<string, object?>());
    }

    while (true)
    {
        var trade = TradeData.GetFakeTrade();

        string routingKey = trade.Region;

        await channel.BasicPublishAsync(
            exchange: ExchangeName,
            routingKey: routingKey,
            body: trade.ToBytes()
        );

        DisplayInfo<Trade>
            .For(trade)
            .SetExchange(ExchangeName)
            .SetRoutingKey(routingKey)
            .SetVirtualHost(connectionFactory.VirtualHost)
            .Display(Color.Cyan);

        await Task.Delay(millisecondsDelay: 3000);
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
dotnet run -p ./Example4/Rabbit.Example4.Producer/

```

### Start Worker 1

```bash

# open new terminal and run the following command
dotnet run "Australia" -p ./Example4/Rabbit.Example4.Consumer/

```

### Start Worker 2

```bash

# open new terminal and run the following command
dotnet run "Great Britain" -p ./Example4/Rabbit.Example4.Consumer/

```

### Start Worker 3

```bash

# open new terminal and run the following command
dotnet run "USA" -p ./Example4/Rabbit.Example4.Consumer/

```

### Display

![example-routing-1](https://user-images.githubusercontent.com/33935506/115109909-ac6e8100-9fcc-11eb-9fb2-b0a7337f274a.png)

![example-routing-2](https://user-images.githubusercontent.com/33935506/115109911-ae384480-9fcc-11eb-99fc-628c901fefef.png)