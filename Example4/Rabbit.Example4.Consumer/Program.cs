using Pastel;
using Rabbit.Common.Data.Trades;
using Rabbit.Common.Display;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace Rabbit.Example4.Consumer;

internal sealed class Program
{
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
}
