# Work Queues (Task Queue)

## Contents

- [Overview](#overview)
- [Characteristics](#characteristics)
- [Flow](#flow)
- [Example Solution](#example-solution)
- [Running the Example](#running-the-example)

---

## Overview

- A _Work Queue_ is used to spread tasks across multiple workers.
- Typically one would use a _Work Queue_ to offload a piece of work or task to get processed asynchronously.
- A _Producer_ sends a message to an _Exchange_ that routes the _Message_ to a named _Queue_. The _Queue_ can have multiple _Subscribers/Consumers_ known as _Workers_ that will each get a turn (using a _Round Robin_ strategy by default) to process a message.

---

## Characteristics

- An empty name is specified for the _Exchange_. An _Exchange_ with an empty name is called the default, or nameless, exchange.
  - _Exchange_ = "" (nameless or default exchange)
- A _Routing Key_ is specified that is identical to the name of the _Queue_ it is routing to
  - _Routing Key_ = _Queue Name_
- A _Transient_ or _Durable_ queue may be used
- Multiple _Consumers (Workers)_ listen for messages on the same queue
- Uses _Round Robin_ strategy by default for dispatching messages to _Workers_. A _Fair Dispatch_ can be configured

---

## Flow

![rmq-work-queue](https://user-images.githubusercontent.com/33935506/98647955-7cc2a980-239a-11eb-9c7c-3ae1c4bbd4fc.png)

- A _Producer_ sends a messages (containing routing key) to an _Exchange_.
- A _Binding_ is created between _Queue_ and _Exchange_ using a _Routing Key_.
- After messages are received by the exchange, the messages are routed to the message _Queue_
- _Consumers (Workers)_ listen to the _Queue_ for messages.
- Each message is dispatched to a _Consumer (Worker)_ using either _Round Robin_ dispatching (default), or _Fair_ dispatching (prefetch count = 1)
- When a _Worker_ completes processing the message, it sends an _Acknowledgement (ACK)_

---

## Example Solution

There are 2 parts to the solution. A _Producer_ and a _Consumer_. The _Producer_ is a .Net Core Console Application that sends _trades_ to a queue at a specific interval. The _Consumer_ is a .NET Core Console application that waits and consumes messages as messages arrive on the queue.

The RabbitMQ .NET client is the official client library for C#. The following links provide more information:

- [RabbitMQ Client NuGet](https://www.nuget.org/packages/RabbitMQ.Client)
- [RabbitMQ Client Source Repository](https://github.com/rabbitmq/rabbitmq-dotnet-client)
- [RabbitMQ Client Project Site](https://github.com/rabbitmq/rabbitmq-dotnet-client)

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

using var connection = connectionFactory.CreateConnection();
```

#### Step 2 - Create Channel

```csharp
using var channel = connection.CreateModel();

// Round Robin dispatching is used by default
// Uncomment the following code to enable Fair dispatch
// channel.BasicQos(
//     prefetchSize: 0,
//     prefetchCount: 1,
//     global: false);
```

#### Step 3 - Declare Queue

```csharp
var queue = channel.QueueDeclare(
    queue: "example2_signals_queue",
    durable: false,
    exclusive: false,
    autoDelete: false,
    arguments: ImmutableDictionary<string, object>.Empty);
```

#### Step 4 - Create Consumer

```csharp
var consumer = new EventingBasicConsumer(channel);

consumer.Received += (sender, eventArgs) =>
{
    var messageBody = eventArgs.Body.ToArray();
    var signal = Signal.FromBytes(messageBody);

    DisplayInfo<Signal>
        .For(signal)
        .SetExchange(eventArgs.Exchange)
        .SetQueue(queue)
        .SetRoutingKey(eventArgs.RoutingKey)
        .SetVirtualHost(connectionFactory.VirtualHost)
        .Display(Color.Yellow);

    DecodeSignal(signal);

    channel.BasicAck(eventArgs.DeliveryTag, multiple: false);
};
```

#### Step 5 - Consume Messages

```csharp
channel.BasicConsume(
    queue: queue.QueueName,
    autoAck: false,
    consumer: consumer);
```

#### Step 6 - Send Acknowledgements (ACKS)

```csharp
channel.BasicAck(eventArgs.DeliveryTag, multiple: false);
```

#### Full Listing

```csharp
internal class Program
{
    private static void Main()
    {
        Console.WriteLine($"\nEXAMPLE 2 : WORK QUEUE : CONSUMER");

        var connectionFactory = new ConnectionFactory
        {
            HostName = "localhost",
            UserName = "admin",
            Password = "password"
        };

        using var connection = connectionFactory.CreateConnection();

        using var channel = connection.CreateModel();

        // Round Robin dispatching is used by default
        // Uncomment the following code to enable Fair dispatch
        // channel.BasicQos(
        //     prefetchSize: 0,
        //     prefetchCount: 1,
        //     global: false);

        var queue = channel.QueueDeclare(
            queue: "example2_signals_queue",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: ImmutableDictionary<string, object>.Empty);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (sender, eventArgs) =>
        {
            var messageBody = eventArgs.Body.ToArray();
            var signal = Signal.FromBytes(messageBody);

            DisplayInfo<Signal>
                .For(signal)
                .SetExchange(eventArgs.Exchange)
                .SetQueue(queue)
                .SetRoutingKey(eventArgs.RoutingKey)
                .SetVirtualHost(connectionFactory.VirtualHost)
                .Display(Color.Yellow);

            DecodeSignal(signal);

            channel.BasicAck(eventArgs.DeliveryTag, multiple: false);
        };

        channel.BasicConsume(
            queue: queue.QueueName,
            autoAck: false,
            consumer: consumer);

        Console.ReadLine();
    }

    private static void DecodeSignal(Signal signal)
    {
        Console.WriteLine($"\nDECODE STARTED: [ TX: {signal.TransmitterName}, ENCODED DATA: {signal.Data} ]".Pastel(Color.Lime));

        var stopwatch = new Stopwatch();
        stopwatch.Start();

        var decodedData = Receiver.DecodeSignal(signal);

        stopwatch.Stop();

        Console.WriteLine($@"DECODE COMPLETE: [ TIME: {stopwatch.Elapsed.Seconds} sec, TX: {signal.TransmitterName}, DECODED DATA: {decodedData} ]".Pastel(Color.Lime));
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

using var connection = connectionFactory.CreateConnection();
```

#### Step 2 - Create Channel

```csharp
using var channel = connection.CreateModel();
```

#### Step 3 - Declare Queue

```csharp
var queue = channel.QueueDeclare(
    queue: QueueName,
    durable: false,
    exclusive: false,
    autoDelete: false,
    arguments: ImmutableDictionary<string, object>.Empty);
```

#### Step 4 - Create and Publish Message

```csharp
var signal = Transmitter.Fake().Transmit();

channel.BasicPublish(
    exchange: ExchangeName,
    routingKey: QueueName,
    body: Encoding.UTF8.GetBytes(signal.ToJson())
);
```

#### Full Listing

```csharp
namespace Rabbit.Example2.Producer
{
    internal class Program
    {
        internal static async Task Main()
        {
            Console.WriteLine("\nEXAMPLE 1 : WORK QUEUES : PRODUCER");

            const string ExchangeName = "";
            const string QueueName = "example2_signals_queue";

            var connectionFactory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "admin",
                Password = "password"
            };

            using var connection = connectionFactory.CreateConnection();

            using var channel = connection.CreateModel();

            var queue = channel.QueueDeclare(
                queue: QueueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: ImmutableDictionary<string, object>.Empty);

            while (true)
            {
                var signal = Transmitter.Fake().Transmit();

                channel.BasicPublish(
                    exchange: ExchangeName,
                    routingKey: QueueName,
                    body: Encoding.UTF8.GetBytes(signal.ToJson())
                );

                DisplayInfo<Signal>
                    .For(signal)
                    .SetExchange(ExchangeName)
                    .SetQueue(QueueName)
                    .SetRoutingKey(QueueName)
                    .SetVirtualHost(connectionFactory.VirtualHost)
                    .Display(Color.Cyan);

                await Task.Delay(millisecondsDelay: 5000);
            }
        }
    }
}
```

---

## Running the Example

### Source Code Repository

All the code required to run this example can be found on [Github](https://github.com/drminnaar/dotnet-rabbitmq)

```bash

git clone https://github.com/drminnaar/dotnet-rabbitmq.git

```

### Manage RabbitMQ Server

For the example, RabbitMQ is hosted within a _Docker_ container.

The example code repository includes a _'docker-compose'_ file that describes the RabbitMQ stack with a reasonable set of defaults. Use _docker-compose_ to start, stop and display information about the RabbitMQ stack as follows:

```bash
# Verify that 'docker-compose' is installed
docker-compose --version

# Start RabbitMQ stack in the background
docker-compose up --detach

# Verify that RabbitMQ container is running
docker-compose ps

# Display RabbitMQ logs
docker-compose logs

# Display and follow RabbitMQ logs
docker-compose logs --tail="all" --follow

# Tear down RabbitMQ stack
# Remove named volumes declared in the `volumes`
# section of the Compose file and anonymous volumes
# attached to container
docker-compose down --volumes
```

### Start Producer

```bash

# open new terminal and run the following command
dotnet run -p ./Example2/Rabbit.Example2.Producer/

```

### Start Worker 1

```bash

# open new terminal and run the following command
dotnet run -p ./Example2/Rabbit.Example2.Consumer/

```

### Start Worker 2

```bash

# open new terminal and run the following command
dotnet run -p ./Example2/Rabbit.Example2.Consumer/

```

### Start Worker 3

```bash

# open new terminal and run the following command
dotnet run -p ./Example2/Rabbit.Example2.Consumer/

```

### Display

![rmq-worker-output](https://user-images.githubusercontent.com/33935506/98651080-a2ea4880-239e-11eb-98a4-96faeea91534.png)