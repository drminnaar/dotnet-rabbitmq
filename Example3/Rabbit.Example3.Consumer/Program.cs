using System;
using System.Drawing;
using System.Threading.Tasks;
using Rabbit.Common.Data.Weather;
using Rabbit.Common.Display;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Rabbit.Example3.Consumer;

internal sealed class Program
{
    private static async Task Main(string[] args)
    {
        Console.WriteLine("\nEXAMPLE 3 : PUB/SUB : CONSUMER");

        var connectionFactory = new ConnectionFactory
        {
            HostName = "localhost",
            UserName = "admin",
            Password = "password"
        };

        var connection = await connectionFactory.CreateConnectionAsync();

        var channel = await connection.CreateChannelAsync();

        var queueName = (await channel.QueueDeclareAsync()).QueueName;

        const string ExchangeName = "example3_forecasts_exchange";

        await channel.ExchangeDeclareAsync(exchange: ExchangeName, type: ExchangeType.Fanout);

        await channel.QueueBindAsync(
            queue: queueName,
            exchange: ExchangeName,
            routingKey: string.Empty);

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (sender, eventArgs) =>
        {
            var body = eventArgs.Body.ToArray();
            var forecast = Forecast.FromBytes(body);

            DisplayInfo<Forecast>
                .For(forecast)
                .SetExchange(eventArgs.Exchange)
                .SetQueue(queueName)
                .SetRoutingKey(eventArgs.RoutingKey)
                .SetVirtualHost(connectionFactory.VirtualHost)
                .Display(Color.Yellow);

            await channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
        };

        await channel.BasicConsumeAsync(
            queue: queueName,
            autoAck: false,
            consumer: consumer);

        Console.ReadLine();
    }
}
