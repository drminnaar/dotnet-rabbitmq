# Headers

## Contents

- [Overview](#overview)
- [Characteristics](#characteristics)
- [Example Solution](#example-solution)
- [Running the Example](#running-the-example)

---

## Overview

![example-headers-overview](https://user-images.githubusercontent.com/33935506/115129880-3276e000-a03e-11eb-96d3-ebd0a2e06ff1.png)

A headers exchange is like a direct exchange that provisions routing based on multiple attributes expressed as headers. Header attributes are used for routing as opposed to using the routing key attribute (routing key ttribute is ignored), and it's possible to bind a queue to an exchange using multiple headers.

---

## Characteristics

- Messages are sent to an exchange of type _Headers_
- The messages sent to exchange are sent with 1 or more headers eg { "region", "australia" }
- Consumers specify binding argument "x-match" to indicate whether all or any headers should match. For example:
  
  ```text
  Producer:
    Headers = { "region": "australia"}, { "industry": "software" }, { "action": "buy" }
    Routing Key = ""
  
  Consumer1
    Headers = { "x-match": "all" }, { "industry": "software" }, { "action": "buy" }
    Result: Matches message that has ALL 3 headers specified

  Consumer2
    Headers = { "x-match": "any" }, { "industry": "software" }, { "region": "usa" }
    Result: Matches message that has ANY of the 2 headers specified
  ```

- The routing key attribute is ignored
- Headers beginning with `x-` are ignored and not used to evaluate matches

---

## Example Solution

There are 2 parts to the solution. A _Producer_ and a _Consumer_. The _Producer_ is a .Net Core Console Application that sends _trades_ to a _Headers Exchange_ at a specific interval. The _Consumer_ is a .NET Core Console application that waits and consumes messages as messages arrive on the queue.

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
const string ExchangeName = "example6_trades_exchange";

await channel.ExchangeDeclareAsync(exchange: ExchangeName, type: ExchangeType.Headers);
```

#### Step 5 - Create Binding

```csharp
channel.QueueBind(
    queue: queue.QueueName,
    exchange: ExchangeName,
    routingKey: string.Empty,
    arguments: headers);
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
        .SetExchange(ExchangeName)
        .SetHeaders(eventArgs.BasicProperties.Headers.ToDictionary(header => header.Key, header => header.Value))
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
    queue: queueName,
    autoAck: false,
    consumer: consumer);
```

#### Step 8 - Send Acknowledgements (ACKS)

```csharp
await channel.BasicConsumeAsync(
    queue: queue.QueueName,
    autoAck: false,
    consumer: consumer);
```

#### Full Listing

```csharp
private static readonly IReadOnlyList<string> MatchExpressions = ["all", "any"];

private static async Task Main()
{
    Console.WriteLine("\nEXAMPLE 6 : HEADERS : CONSUMER");

    var headers = GetHeadersFromInput();

    var connectionFactory = new ConnectionFactory
    {
        HostName = "localhost",
        UserName = "admin",
        Password = "password"
    };

    using var connection = await connectionFactory.CreateConnectionAsync();

    using var channel = await connection.CreateChannelAsync();

    var queue = await channel.QueueDeclareAsync();

    const string ExchangeName = "example6_trades_exchange";

    await channel.ExchangeDeclareAsync(exchange: ExchangeName, type: ExchangeType.Headers);

    await channel.QueueBindAsync(
        queue: queue.QueueName,
        exchange: ExchangeName,
        routingKey: string.Empty,
        arguments: headers);

    var consumer = new AsyncEventingBasicConsumer(channel);

    consumer.ReceivedAsync += async (sender, eventArgs) =>
    {
        var messageBody = eventArgs.Body.ToArray();

        var trade = Trade.FromBytes(messageBody);

        DisplayInfo<Trade>
            .For(trade)
            .SetExchange(ExchangeName)
            .SetHeaders(eventArgs.BasicProperties?.Headers?.ToDictionary(
                header => header.Key,
                header => (object)Encoding.UTF8.GetString((byte[])header.Value!)))
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

private static Dictionary<string, object?> GetHeadersFromInput()
{
    var headers = new Dictionary<string, object?>();

    while (true)
    {
        Console.Write("\nCreate subscription for 'all' or 'any' headers: ");
        var matchExpression = Console.ReadLine()?.ToLower() ?? string.Empty;
        if (!MatchExpressions.Contains(matchExpression))
            continue;

        headers.Add("x-match", matchExpression);

        Console.Write("\nEnter region (Australia, Great Britain, USA): ");
        var region = Console.ReadLine()?.ToLower() ?? "";
        if (TradeData.ContainsRegion(region))
            headers.Add("region", region);

        Console.Write("Enter industry (Banking, Financial Services, Software): ");
        var industry = Console.ReadLine()?.ToLower() ?? "";
        if (TradeData.ContainsIndustry(industry))
            headers.Add("industry", industry);

        if (headers.Count > 1)
            return headers;
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
const string ExchangeName = "example6_trades_exchange";

await channel.ExchangeDeclareAsync(exchange: ExchangeName, type: ExchangeType.Headers);
```

#### Step 4 - Create and Publish Message

```csharp
var trade = TradeData.GetFakeTrade();

var properties = new BasicProperties
{
    Persistent = true,
    Headers = new Dictionary<string, object?>
    {
        { "region", trade.NormalizedRegion },
        { "industry", trade.NormalizedIndustry }
    }
};

await channel.BasicPublishAsync(
    exchange: ExchangeName,
    routingKey: string.Empty,
    mandatory: false,
    basicProperties: properties,
    body: trade.ToBytes());
```

#### Full Listing

```csharp
private static async Task Main()
{
    Console.WriteLine("EXAMPLE 6 : HEADERS : PRODUCER");

    var connectionFactory = new ConnectionFactory
    {
        HostName = "localhost",
        UserName = "admin",
        Password = "password"
    };

    using var connection = await connectionFactory.CreateConnectionAsync();

    using var channel = await connection.CreateChannelAsync();

    const string ExchangeName = "example6_trades_exchange";

    await channel.ExchangeDeclareAsync(exchange: ExchangeName, type: ExchangeType.Headers);

    while (true)
    {
        var trade = TradeData.GetFakeTrade();

        var properties = new BasicProperties
        {
            Persistent = true,
            Headers = new Dictionary<string, object?>
            {
                { "region", trade.NormalizedRegion },
                { "industry", trade.NormalizedIndustry }
            }
        };

        await channel.BasicPublishAsync(
            exchange: ExchangeName,
            routingKey: string.Empty,
            mandatory: false,
            basicProperties: properties,
            body: trade.ToBytes());

        DisplayInfo<Trade>
            .For(trade)
            .SetExchange(ExchangeName)
            .SetHeaders(properties.Headers.ToDictionary(header => header.Key, header => header.Value!))
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
# change directory to Example6 Producer
cd ./Example6/Rabbit.Example6.Producer

# open new terminal and run the following command
dotnet run

```

### Start Consumer 1 (Match All)

```bash
# change directory to Example6 Consumer
cd ./Example6/Rabbit.Example6.Consumer

# start consumer
dotnet run

###########
## output
###########
EXAMPLE 6 : HEADERS : CONSUMER

# choose to match ALL headers: { "x-match": "all" }
Create subscription for 'all' or 'any' headers: all

# creates header { "region": "australia" }
Enter region (Australia, Great Britain, USA): australia

# creates header { "industry": "software" }
Enter industry (Banking, Financial Services, Software): software

```

### Start Consumer 2 (Match Any)

```bash
# change directory to Example6 Consumer
cd ./Example6/Rabbit.Example6.Consumer

# start consumer
dotnet run

###########
## output
###########
EXAMPLE 6 : HEADERS : CONSUMER

# choose to match ANY headers: { "x-match": "any" }
Create subscription for 'all' or 'any' headers: any

# creates header { "region": "usa" }
Enter region (Australia, Great Britain, USA): usa

# creates header { "industry": "banking" }
Enter industry (Banking, Financial Services, Software): banking

```

### Display

![example-headers-1](https://user-images.githubusercontent.com/33935506/115129497-a8794800-a03a-11eb-9491-b94323fb026d.png)

![example-headers-2](https://user-images.githubusercontent.com/33935506/115111801-38d17180-9fd6-11eb-8d94-329b61672909.png)
