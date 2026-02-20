using System.Threading.Tasks;
using RabbitMQ.Client;
using System.Text;
using Rabbit.Common.Data.Signals;
using Rabbit.Common.Display;
using System;
using System.Drawing;
using System.Collections.Generic;

namespace Rabbit.Example2.Producer;

internal sealed class Program
{
    private static async Task Main()
    {
        Console.WriteLine("\nEXAMPLE 2 : WORK QUEUE : PRODUCER");

        var connectionFactory = new ConnectionFactory
        {
            HostName = "localhost",
            UserName = "admin",
            Password = "password"
        };

        using var connection = await connectionFactory.CreateConnectionAsync();

        using var channel = await connection.CreateChannelAsync();

        const string ExchangeName = "";

        const string QueueName = "example2_signals_queue";

        var queue = await channel.QueueDeclareAsync(
            queue: QueueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: new Dictionary<string, object?>());

        while (true)
        {
            var signal = Transmitter.Fake().Transmit();

            await channel.BasicPublishAsync(
                exchange: ExchangeName,
                routingKey: QueueName,
                body: Encoding.UTF8.GetBytes(signal.ToJson())
            );

            DisplayInfo<Signal>
                .For(signal)
                .SetExchange(ExchangeName)
                .SetQueue(QueueName)
                .SetRoutingKey(QueueName)
                .SetVirtualHost(connectionFactory.VirtualHost)
                .Display(Color.Cyan);

            await Task.Delay(millisecondsDelay: 3000);
        }
    }
}
