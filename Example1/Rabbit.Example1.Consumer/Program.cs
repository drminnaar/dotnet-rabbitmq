using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using Rabbit.Common.Data.Trades;
using Rabbit.Common.Display;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Rabbit.Example1.Consumer;

internal sealed class Program
{
    private static async Task Main()
    {
        Console.WriteLine("\nEXAMPLE 1 : ONE-WAY MESSAGING : CONSUMER");

        var connectionFactory = new ConnectionFactory
        {
            HostName = "localhost",
            UserName = "admin",
            Password = "password"
        };

        using var connection = await connectionFactory.CreateConnectionAsync();

        using var channel = await connection.CreateChannelAsync();

        var queue = await channel.QueueDeclareAsync(
            queue: "example1_trades_queue",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: new Dictionary<string, object?>());

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (sender, eventArgs) =>
        {
            var messageBody = eventArgs.Body.ToArray();
            var trade = Trade.FromBytes(messageBody);

            DisplayInfo<Trade>
                .For(trade)
                .SetExchange(eventArgs.Exchange)
                .SetQueue(queue)
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
}
