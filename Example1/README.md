# One-Way Messaging

## Contents

- [Overview](#overview)
- [Characteristics](#characteristics)
- [Flow](#flow)
- [Example Solution](#example-solution)
- [Running the Example](#running-the-example)

---

## Overview

The easiest way to use _RabbitMQ_ is to implement the _One-Way Messaging_ pattern. In this scenario, a _Producer_ sends a _Message_ to a _Queue_. A _Consumer_ of the _Queue_ recieves the _Message_ and processes it.

---

## Characteristics

- An empty name is specified for the _Exchange_. An _Exchange_ with an empty name is called the _default (or nameless) exchange_.
  - _Exchange_ = "" (nameless or default exchange)
- A _Routing Key_ is specified that is identical to the name of the _Queue_ it is routing to
  - _Routing Key_ = _Queue Name_

---

## Flow

![rmq-one-way-messaging](https://user-images.githubusercontent.com/33935506/98647951-7b917c80-239a-11eb-8ad7-371ff318adb0.png)

- _Producer_ sends _Message_ to _Queue_ using the _Default Exchange_
- The _Message_ is routed from the _Default Exchange_ to the _Queue_ using a _routing key_
- _Consumer_ subscribes to _Queue_
- _Consumer_ receives and processes _Message_
- _Consumer_ sends an acknowledgement (ACK). An _ACK_ can be sent automatically or manually.
- When an _ACK_ is received, the message is permanently removed from _Queue_

---

## Example Solution

There are 2 parts to the solution. A _Producer_ and a _Consumer_. The _Producer_ is a .NET Core Console Application that sends _trades_ to a queue at a specific interval. The _Consumer_ is a .NET Core Console application that waits and consumes messages from the queue.

The following 2 sections, _Producer_ and _Consumer_, highlight the code required to interact with _RabbitMQ_

### Consumer

#### Step 1 - Create Connection

Create a _Connection_ using a _ConnectionFactory_. The _ConnectionFactory_ specifies a number of default properties, for example:

```csharp
public const string DefaultPass = "guest";
public const string DefaultUser = "guest";
public const string DefaultVHost = "/";
```

Because we are using _Docker_ to host our _RabbitMQ_ instance, we need to use the values defined by our _RabbitMQ_ stack. Take note that the username and password can be found in the _`docker-compose`_ file:

```yaml
environment:
    ...
    RABBITMQ_DEFAULT_USER: admin
    RABBITMQ_DEFAULT_PASS: password
```

Create a _ConnectionFactory_ and then use the _ConnectionFactory_ to create a _Connection_ as follows:

```csharp
var connectionFactory = new ConnectionFactory
{
    HostName = "localhost",
    UserName = "admin",
    Password = "password"
};

// a connection is of type IConnection and implements IDisposable
using var connection = connectionFactory.CreateConnection();
```

A _Connection_ represents a TCP connection between the _Client_ and the _Broker_. It's responsible for all the underlying networking and authentication tasks.

#### Step 2 - Create Channel

_Channels_ enable efficient communication and are "lightweight connections" that share the primary TCP connection. All communication happens over a _Channel_. _Channels_ are also completely isolation from one another. 

```csharp
// a channel is of type IModel and implements IDisposable
using var channel = connection.CreateModel();
```

#### Step 3 - Declare Queue

A _Queue_ is a FIFO (First-In-First-Out) ordered collection of messages. When declaring a _Queue_, a _Binding_ between the _Queue_ and the _Default Exchange_ is automatically created. The _Binding_ will have a _Routing Key_ that is identical to the name of the _Queue_. For example, for the following _Queue_, the _Binding_ created will have a _Routing Key_ of _"example1_trades_queue"_ as well.

Routing Key = Queue Name = example1_trades_queue

```csharp
var queue = channel.QueueDeclare(
    queue: "example1_trades_queue",
    durable: false,
    exclusive: false,
    autoDelete: false,
    arguments: ImmutableDictionary<string, object>.Empty);
```

#### Step 4 - Create Consumer

Create a _Consumer_ and subscribe to _Received_ events. In the _callback_ of the event, one is able to extract and process message that was sent to _Queue_.

```csharp
var consumer = new EventingBasicConsumer(channel);

// subscribe to 'Received' event      
consumer.Received += (sender, eventArgs) =>
{
    var messageBody = eventArgs.Body.ToArray();

    var trade = Trade.FromBytes(messageBody);
    
    ...
    ...
    ...
};
```

#### Step 5 - Consume Messages

Start consuming messages from the _Queue_ using the _Consumer_ that was created. For this example we also disable automatic _Acknowledgements (ACKS)_.

```csharp
channel.BasicConsume(
    queue: queue.QueueName,
    autoAck: false,
    consumer: consumer);
```

#### Step 6 - Send Acknowledgements (ACKS)

Because automatic _Acknowledgements_ were disabled in the previous step, an _ACK_ must be sent manually in the _callback_ of the handled _Received Event_.

```csharp
// subscribe to 'Received' event      
consumer.Received += (sender, eventArgs) =>
{
    ...
    ...
    ...    
    channel.BasicAck(eventArgs.DeliveryTag, multiple: false);
};
```

#### Full Listing

```csharp
namespace Rabbit.Example1.Consumer
{
    internal sealed class Program
    {
        private static void Main()
        {
            Console.WriteLine("\nEXAMPLE 1 : ONE-WAY MESSAGING : CONSUMER");

            var connectionFactory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "admin",
                Password = "password"
            };

            using var connection = connectionFactory.CreateConnection();

            using var channel = connection.CreateModel();

            var queue = channel.QueueDeclare(
                queue: "example1_trades_queue",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: ImmutableDictionary<string, object>.Empty);

            var consumer = new EventingBasicConsumer(channel);
            
            consumer.Received += (sender, eventArgs) =>
            {
                var messageBody = eventArgs.Body.ToArray();
                var trade = Trade.FromBytes(messageBody);

                // helper to display broker information to the console
                DisplayInfo<Trade>
                    .For(trade)
                    .SetExchange(eventArgs.Exchange)
                    .SetQueue(queue)
                    .SetRoutingKey(eventArgs.RoutingKey)
                    .SetVirtualHost(connectionFactory.VirtualHost)
                    .Display(Color.Yellow);
                
                channel.BasicAck(eventArgs.DeliveryTag, multiple: false);
            };

            channel.BasicConsume(
                queue: queue.QueueName,
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
var trade = TradeData.GetFakeTrade();

channel.BasicPublish(
    exchange: ExchangeName,
    routingKey: QueueName,
    mandatory: false,
    basicProperties: null,
    body: trade.ToBytes()
);
```

#### Full Listing

```csharp
namespace Rabbit.Example1.Producer
{
    internal class Program
    {
        internal static async Task Main()
        {
            Console.WriteLine("\nEXAMPLE 1 : ONE-WAY MESSAGING : PRODUCER");

            const string ExchangeName = "";
            const string QueueName = "example1_trades_queue";

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
                var trade = TradeData.GetFakeTrade();

                channel.BasicPublish(
                    exchange: ExchangeName,
                    routingKey: QueueName,
                    mandatory: false,
                    basicProperties: null,
                    body: trade.ToBytes()
                );

                DisplayInfo<Trade>
                    .For(trade)
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

dotnet run -p ./Example1/Rabbit.Example1.Producer/

```

### Start Consumer

```bash

dotnet run -p ./Example1/Rabbit.Example1.Consumer/

```

### Display

![rmq-example1](https://user-images.githubusercontent.com/33935506/98496838-6851b480-22a7-11eb-8cde-04da3a82e598.png)