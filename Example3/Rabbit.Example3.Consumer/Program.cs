using System;
using System.Drawing;
using Rabbit.Common.Data.Weather;
using Rabbit.Common.Display;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Rabbit.Example3.Consumer
{
    internal sealed class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("\nEXAMPLE 3 : PUB/SUB : CONSUMER");

            var connectionFactory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "admin",
                Password = "password"
            };

            var connection = connectionFactory.CreateConnection();

            var channel = connection.CreateModel();

            var queueName = channel.QueueDeclare().QueueName;

            const string ExchangeName = "example3_forecasts_exchange";

            channel.ExchangeDeclare(exchange: ExchangeName, type: ExchangeType.Fanout);

            channel.QueueBind(
                queue: queueName,
                exchange: ExchangeName,
                routingKey: string.Empty);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (sender, eventArgs) =>
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

                channel.BasicAck(eventArgs.DeliveryTag, multiple: false);
            };

            channel.BasicConsume(
                queue: queueName,
                autoAck: false,
                consumer: consumer);

            Console.ReadLine();
        }
    }
}
