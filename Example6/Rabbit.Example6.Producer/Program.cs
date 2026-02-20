using System.Threading.Tasks;
using RabbitMQ.Client;
using Rabbit.Common.Display;
using Rabbit.Common.Data.Trades;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Drawing;

namespace Rabbit.Example6.Producer;

internal sealed class Program
{
    private static async Task Main()
    {
        Console.WriteLine("EXAMPLE 6 : HEADERS : PRODUCER");

        var connectionFactory = new ConnectionFactory
        {
            HostName = "localhost",
            UserName = "admin",
            Password = "password"
        };

        using var connection = await connectionFactory.CreateConnectionAsync();

        using var channel = await connection.CreateChannelAsync();

        const string ExchangeName = "example6_trades_exchange";

        await channel.ExchangeDeclareAsync(exchange: ExchangeName, type: ExchangeType.Headers);

        while (true)
        {
            var trade = TradeData.GetFakeTrade();

            var properties = new BasicProperties
            {
                Persistent = true,
                Headers = new Dictionary<string, object?>
                {
                    { "region", trade.NormalizedRegion },
                    { "industry", trade.NormalizedIndustry }
                }
            };

            await channel.BasicPublishAsync(
                exchange: ExchangeName,
                routingKey: string.Empty,
                mandatory: false,
                basicProperties: properties,
                body: trade.ToBytes());

            DisplayInfo<Trade>
                .For(trade)
                .SetExchange(ExchangeName)
                .SetHeaders(properties.Headers.ToDictionary(header => header.Key, header => header.Value!))
                .SetVirtualHost(connectionFactory.VirtualHost)
                .Display(Color.Cyan);

            await Task.Delay(millisecondsDelay: 5000);
        }
    }
}
