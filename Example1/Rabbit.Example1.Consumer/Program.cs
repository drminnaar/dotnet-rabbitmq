using System;
using System.Collections.Immutable;
using System.Drawing;
using Rabbit.Common.Data.Trades;
using Rabbit.Common.Display;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

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
