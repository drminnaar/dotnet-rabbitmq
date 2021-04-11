using Pastel;
using Rabbit.Common.Data.Signals;
using Rabbit.Common.Display;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Drawing;

namespace Rabbit.Example2.Consumer
{
    internal sealed class Program
    {
        private static void Main()
        {
            Console.WriteLine("\nEXAMPLE 2 : WORK QUEUE : CONSUMER");

            var connectionFactory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "admin",
                Password = "password"
            };

            using var connection = connectionFactory.CreateConnection();

            using var channel = connection.CreateModel();

            // Round Robin dispatching is used by default
            // Uncomment the following code to enable Fair dispatch
            // channel.BasicQos(
            //     prefetchSize: 0,
            //     prefetchCount: 1,
            //     global: false);

            var queue = channel.QueueDeclare(
                queue: "example2_signals_queue",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: ImmutableDictionary<string, object>.Empty);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, eventArgs) =>
            {
                var messageBody = eventArgs.Body.ToArray();
                var signal = Signal.FromBytes(messageBody);

                DisplayInfo<Signal>
                    .For(signal)
                    .SetExchange(eventArgs.Exchange)
                    .SetQueue(queue)
                    .SetRoutingKey(eventArgs.RoutingKey)
                    .SetVirtualHost(connectionFactory.VirtualHost)
                    .Display(Color.Yellow);

                DecodeSignal(signal);

                channel.BasicAck(eventArgs.DeliveryTag, multiple: false);
            };

            channel.BasicConsume(
                queue: queue.QueueName,
                autoAck: false,
                consumer: consumer);

            Console.ReadLine();
        }

        private static void DecodeSignal(Signal signal)
        {
            Console.WriteLine($"\nDECODE STARTED: [ TX: {signal.TransmitterName}, ENCODED DATA: {signal.Data} ]".Pastel(Color.Lime));

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var decodedData = Receiver.DecodeSignal(signal);

            stopwatch.Stop();

            Console.WriteLine($@"DECODE COMPLETE: [ TIME: {stopwatch.Elapsed.Seconds} sec, TX: {signal.TransmitterName}, DECODED DATA: {decodedData} ]".Pastel(Color.Lime));
        }
    }
}
