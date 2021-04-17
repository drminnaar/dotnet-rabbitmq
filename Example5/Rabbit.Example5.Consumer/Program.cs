using Pastel;
using Rabbit.Common.Data.Trades;
using Rabbit.Common.Display;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Drawing;

namespace Rabbit.Example5.Consumer
{
    internal sealed class Program
    {
        private static void Main(string[] topics)
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

            using var connection = connectionFactory.CreateConnection();

            using var channel = connection.CreateModel();

            const string ExchangeName = "example5_trades_exchange";

            channel.ExchangeDeclare(exchange: ExchangeName, type: ExchangeType.Topic);

            var queue = channel.QueueDeclare();

            foreach (var topic in topics)
            {
                Console.WriteLine(topic);
                channel.QueueBind(
                    queue: queue.QueueName,
                    exchange: ExchangeName,
                    routingKey: topic);
            }

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
                    .SetTopic(eventArgs.RoutingKey)
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
