using Rabbit.Common.Data.Trades;
using Rabbit.Common.Display;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Rabbit.Example6.Consumer
{
    internal sealed class Program
    {
        private static readonly IReadOnlyList<string> MatchExpressions = new string[2] { "all", "any" };

        private static void Main()
        {
            Console.WriteLine($"\nEXAMPLE 6 : HEADERS : CONSUMER");

            var headers = GetHeadersFromInput();

            var connectionFactory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "admin",
                Password = "password"
            };

            using var connection = connectionFactory.CreateConnection();

            using var channel = connection.CreateModel();

            var queue = channel.QueueDeclare();

            const string ExchangeName = "example6_trades_exchange";

            channel.ExchangeDeclare(exchange: ExchangeName, type: ExchangeType.Headers);

            channel.QueueBind(queue: queue.QueueName, exchange: ExchangeName, routingKey: string.Empty, arguments: headers);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (sender, eventArgs) =>
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

                channel.BasicAck(eventArgs.DeliveryTag, multiple: false);
            };

            channel.BasicConsume(
                queue: queue.QueueName,
                autoAck: false,
                consumer: consumer);

            Console.ReadLine();
        }

        private static Dictionary<string, object> GetHeadersFromInput()
        {
            var headers = new Dictionary<string, object>();

            while (true)
            {
                Console.Write("\nCreate subscription for 'all' or 'any' headers: ");
                var matchExpression = Console.ReadLine()?.ToLower() ?? string.Empty;
                if (!MatchExpressions.Contains(matchExpression))
                    continue;

                headers.Add("x-match", matchExpression);

                Console.Write("\nEnter region (Australia, Great Britain, USA): ");
                var region = Console.ReadLine() ?? "";
                if (TradeData.ContainsRegion(region))
                    headers.Add("region", region);

                Console.Write("Enter industry (Banking, Financial Services, Software): ");
                var industry = Console.ReadLine() ?? "";
                if (TradeData.ContainsIndustry(industry))
                    headers.Add("industry", industry);

                if (headers.Count > 1)
                    return headers;
            }
        }
    }
}
