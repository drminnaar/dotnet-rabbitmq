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

namespace Rabbit.Example4.Consumer
{
    internal sealed class Program
    {
        private static void Main(string[] regions)
        {
            Console.WriteLine("\nEXAMPLE 4 : ROUTING : CONSUMER");

            var region = regions.FirstOrDefault() ?? string.Empty;

            var QueueNames = TradeData
                .Regions
                .Select(region =>
                {
                    var normalizedRegion = region.Normalize().ToLower().Trim().Replace(" ", string.Empty);
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

            using var connection = connectionFactory.CreateConnection();

            using var channel = connection.CreateModel();

            const string ExchangeName = "example4_trades_exchange";

            channel.ExchangeDeclare(
                exchange: ExchangeName,
                type: ExchangeType.Direct,
                durable: false,
                autoDelete: false,
                arguments: ImmutableDictionary<string, object>.Empty);

            var queue = channel.QueueDeclare(
                queue: QueueNames[region],
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: ImmutableDictionary<string, object>.Empty);

            channel.QueueBind(
                queue: queue.QueueName,
                exchange: ExchangeName,
                routingKey: region);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, eventArgs) =>
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
