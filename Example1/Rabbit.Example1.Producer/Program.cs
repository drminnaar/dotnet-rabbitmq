using System.Threading.Tasks;
using RabbitMQ.Client;
using Rabbit.Common.Data.Trades;
using Rabbit.Common.Display;
using System;
using System.Drawing;
using System.Collections.Generic;

namespace Rabbit.Example1.Producer;

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

        using var connection = await connectionFactory.CreateConnectionAsync();

        using var channel = await connection.CreateChannelAsync();

        var queue = await channel.QueueDeclareAsync(
            queue: QueueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: new Dictionary<string, object?>());

        while (true)
        {
            var trade = TradeData.GetFakeTrade();

            await channel.BasicPublishAsync(
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
