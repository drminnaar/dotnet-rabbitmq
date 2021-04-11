using System.Threading.Tasks;
using RabbitMQ.Client;
using Rabbit.Common.Display;
using Rabbit.Common.Data.Trades;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Drawing;

namespace Rabbit.Example6.Producer
{
    internal sealed class Program
    {
        private static async Task Main()
        {
            Console.WriteLine($"EXAMPLE 6 : HEADERS : PRODUCER");

            var connectionFactory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "admin",
                Password = "password"
            };

            using var connection = connectionFactory.CreateConnection();

            using var channel = connection.CreateModel();

            const string ExchangeName = "example6_trades_exchange";

            channel.ExchangeDeclare(exchange: ExchangeName, type: ExchangeType.Headers);

            while (true)
            {
                var trade = TradeData.GetFakeTrade();

                var properties = channel.CreateBasicProperties();
                properties.Headers = new Dictionary<string, object>();
                properties.Headers.Add("region", trade.NormalizedRegion);
                properties.Headers.Add("industry", trade.NormalizedIndustry);

                channel.BasicPublish(
                    exchange: ExchangeName,
                    routingKey: string.Empty,
                    basicProperties: properties,
                    body: trade.ToBytes());
                
                DisplayInfo<Trade>
                    .For(trade)
                    .SetExchange(ExchangeName)
                    .SetHeaders(properties.Headers.ToDictionary(header => header.Key, header => header.Value))
                    .SetVirtualHost(connectionFactory.VirtualHost)
                    .Display(Color.Cyan);

                await Task.Delay(millisecondsDelay: 5000);
            }
        }
    }
}
