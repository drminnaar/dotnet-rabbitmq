using System.Threading.Tasks;
using RabbitMQ.Client;
using Rabbit.Common.Display;
using Rabbit.Common.Data.Trades;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Drawing;

namespace Rabbit.Example4.Producer;

internal sealed class Program
{
    private static async Task Main()
    {
        Console.WriteLine("EXAMPLE 4 : ROUTING : PRODUCER");

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

        var QueueNames = TradeData
            .Regions
            .Select(region =>
            {
                var normalizedRegion = region.ToLower().Trim().Replace(" ", string.Empty);
                var queueName = $"example4_trades_{normalizedRegion}_queue";
                return new KeyValuePair<string, string>(region, queueName);
            })
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        foreach (var region in TradeData.Regions)
        {
            var queue = await channel.QueueDeclareAsync(
                queue: QueueNames[region],
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: new Dictionary<string, object?>());

            await channel.QueueBindAsync(
                queue: queue.QueueName,
                exchange: ExchangeName,
                routingKey: region,
                arguments: new Dictionary<string, object?>());
        }

        while (true)
        {
            var trade = TradeData.GetFakeTrade();

            string routingKey = trade.Region;

            await channel.BasicPublishAsync(
                exchange: ExchangeName,
                routingKey: routingKey,
                body: trade.ToBytes()
            );

            DisplayInfo<Trade>
                .For(trade)
                .SetExchange(ExchangeName)
                .SetRoutingKey(routingKey)
                .SetVirtualHost(connectionFactory.VirtualHost)
                .Display(Color.Cyan);

            await Task.Delay(millisecondsDelay: 3000);
        }
    }
}
