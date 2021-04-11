using System;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using Rabbit.Common.Data.Weather;
using Rabbit.Common.Display;
using RabbitMQ.Client;

namespace Rabbit.Example3.Producer
{
    internal sealed class Program
    {
        private static async Task Main()
        {
            Console.WriteLine($"\nEXAMPLE 3 : PUB/SUB : PRODUCER)");

            var connectionFactory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "admin",
                Password = "password"
            };

            using var connection = connectionFactory.CreateConnection();

            using var channel = connection.CreateModel();

            const string QueueName = "";

            const string ExchangeName = "example3_forecasts_exchange";

            channel.ExchangeDeclare(exchange: ExchangeName, type: ExchangeType.Fanout);

            while (true)
            {
                var forecast = Thermometer.Fake().Report();

                channel.BasicPublish(
                    exchange: ExchangeName,
                    routingKey: QueueName,
                    body: Encoding.UTF8.GetBytes(forecast.ToJson())
                );

                DisplayInfo<Forecast>
                    .For(forecast)
                    .SetExchange(ExchangeName)
                    .SetQueue(QueueName)
                    .SetRoutingKey(QueueName)
                    .SetVirtualHost(connectionFactory.VirtualHost)
                    .Display(Color.Yellow);

                await Task.Delay(millisecondsDelay: 3000);
            }
        }
    }
}
