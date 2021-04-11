using System.Threading.Tasks;
using RabbitMQ.Client;
using Rabbit.Common.Data.Trades;
using Rabbit.Common.Display;
using System;
using System.Drawing;
using System.Collections.Immutable;

namespace Rabbit.Example1.Producer
{
    internal sealed class Program
    {
        private static async Task Main()
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
