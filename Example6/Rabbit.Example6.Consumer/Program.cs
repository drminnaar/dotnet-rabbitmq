using Rabbit.Common.Data.Trades;
using Rabbit.Common.Display;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rabbit.Example6.Consumer;

internal sealed class Program
{
    private static readonly IReadOnlyList<string> MatchExpressions = ["all", "any"];

    private static async Task Main()
    {
        Console.WriteLine("\nEXAMPLE 6 : HEADERS : CONSUMER");

        var headers = GetHeadersFromInput();

        var connectionFactory = new ConnectionFactory
        {
            HostName = "localhost",
            UserName = "admin",
            Password = "password"
        };

        using var connection = await connectionFactory.CreateConnectionAsync();

        using var channel = await connection.CreateChannelAsync();

        var queue = await channel.QueueDeclareAsync();

        const string ExchangeName = "example6_trades_exchange";

        await channel.ExchangeDeclareAsync(exchange: ExchangeName, type: ExchangeType.Headers);

        await channel.QueueBindAsync(
            queue: queue.QueueName,
            exchange: ExchangeName,
            routingKey: string.Empty,
            arguments: headers);

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (sender, eventArgs) =>
        {
            var messageBody = eventArgs.Body.ToArray();

            var trade = Trade.FromBytes(messageBody);

            DisplayInfo<Trade>
                .For(trade)
                .SetExchange(ExchangeName)
                .SetHeaders(eventArgs.BasicProperties?.Headers?.ToDictionary(
                    header => header.Key,
                    header => (object)Encoding.UTF8.GetString((byte[])header.Value!)))
                .SetQueue(queue.QueueName)
                .SetRoutingKey(eventArgs.RoutingKey)
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

    private static Dictionary<string, object?> GetHeadersFromInput()
    {
        var headers = new Dictionary<string, object?>();

        while (true)
        {
            Console.Write("\nCreate subscription for 'all' or 'any' headers: ");
            var matchExpression = Console.ReadLine()?.ToLower() ?? string.Empty;
            if (!MatchExpressions.Contains(matchExpression))
                continue;

            headers.Add("x-match", matchExpression);

            Console.Write("\nEnter region (Australia, Great Britain, USA): ");
            var region = Console.ReadLine()?.ToLower() ?? "";
            if (TradeData.ContainsRegion(region))
                headers.Add("region", region);

            Console.Write("Enter industry (Banking, Financial Services, Software): ");
            var industry = Console.ReadLine()?.ToLower() ?? "";
            if (TradeData.ContainsIndustry(industry))
                headers.Add("industry", industry);

            if (headers.Count > 1)
                return headers;
        }
    }
}
