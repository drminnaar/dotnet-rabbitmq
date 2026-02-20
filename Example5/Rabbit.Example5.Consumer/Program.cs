using Pastel;
using Rabbit.Common.Data.Trades;
using Rabbit.Common.Display;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Drawing;
using System.Threading.Tasks;

namespace Rabbit.Example5.Consumer;

internal sealed class Program
{
    private static async Task Main(string[] topics)
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

        using var connection = await connectionFactory.CreateConnectionAsync();

        using var channel = await connection.CreateChannelAsync();

        const string ExchangeName = "example5_trades_exchange";

        await channel.ExchangeDeclareAsync(exchange: ExchangeName, type: ExchangeType.Topic);

        var queue = await channel.QueueDeclareAsync();

        foreach (var topic in topics)
        {
            Console.WriteLine(topic);
            await channel.QueueBindAsync(
                queue: queue.QueueName,
                exchange: ExchangeName,
                routingKey: topic);
        }

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (sender, eventArgs) =>
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

            await channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
        };

        await channel.BasicConsumeAsync(
            queue: queue.QueueName,
            autoAck: false,
            consumer: consumer);

        Console.ReadLine();
    }
}
