using System.Threading.Tasks;
using RabbitMQ.Client;
using Rabbit.Common.Display;
using Rabbit.Common.Data.Trades;
using System;
using System.Drawing;

namespace Rabbit.Example5.Producer;

internal sealed class Program
{
    private static async Task Main()
    {
        Console.WriteLine("\nEXAMPLE 5 : TOPICS : PRODUCER");

        var connectionFactory = new ConnectionFactory
        {
            HostName = "localhost",
            UserName = "admin",
            Password = "password"
        };

        using var connection = await connectionFactory.CreateConnectionAsync();

        using var channel = await connection.CreateChannelAsync();

        const string ExchangeName = "example5_trades_exchange";

        await channel.ExchangeDeclareAsync(exchange: ExchangeName, type: ExchangeType.Topic);

        while (true)
        {
            var trade = TradeData.GetFakeTrade();

            var topic = $"{trade.NormalizedRegion}.{trade.NormalizedIndustry}.{trade.NormalizedAction}";

            await channel.BasicPublishAsync(
                exchange: ExchangeName,
                routingKey: topic,
                body: trade.ToBytes());

            DisplayInfo<Trade>
                .For(trade)
                .SetExchange(ExchangeName)
                .SetRoutingKey(topic)
                .SetTopic(topic)
                .SetVirtualHost(connectionFactory.VirtualHost)
                .Display(Color.Cyan);

            await Task.Delay(millisecondsDelay: 5000);
        }
    }
}
