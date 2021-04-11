using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json;
using Pastel;

namespace Rabbit.Common.Display
{
    public sealed class DisplayInfo<T>
    {
        private DisplayInfo(T message)
        {
            Message = message;
        }

        public string Exchange { get; private set; } = string.Empty;
        public IReadOnlyDictionary<string, object> Headers { get; private set; } = ImmutableDictionary<string, object>.Empty;
        public T Message { get; private set; }
        public string Queue { get; private set; } = string.Empty;
        public string RoutingKey { get; private set; } = string.Empty;
        public string Topic { get; private set; } = string.Empty;
        public string VirtualHost { get; private set; } = string.Empty;

        public void Display(Color displayColor)
        {
            Console.WriteLine("");
            Console.WriteLine(ToString().Pastel(displayColor));
        }

        public override string ToString()
        {
            var output = new StringBuilder();
            output.AppendLine($"VIRTUAL HOST: {VirtualHost}");
            output.AppendLine($"EXCHANGE: {Exchange}");
            output.AppendLine($"QUEUE: {Queue}");
            output.AppendLine($"ROUTING KEY: {RoutingKey}");
            output.AppendLine($"TOPIC: {Topic}");
            output.AppendLine($"HEADERS: {NormalizedHeaders}");
            output.AppendLine($"MESSAGE: ");
            output.AppendLine($"{JsonSerializer.Serialize(Message, new JsonSerializerOptions { WriteIndented = true })}");
            return output.ToString();
        }

        public DisplayInfo<T> SetExchange(string exchangeName)
        {
            Exchange = exchangeName;
            return this;
        }

        public DisplayInfo<T> SetHeaders(IReadOnlyDictionary<string, object> headers)
        {
            Headers = headers;
            return this;
        }

        public DisplayInfo<T> SetQueue(string queueName)
        {
            Queue = queueName;
            return this;
        }

        public DisplayInfo<T> SetRoutingKey(string routingKey)
        {
            RoutingKey = routingKey;
            return this;
        }

        public DisplayInfo<T> SetTopic(string topic)
        {
            Topic = topic;
            return this;
        }

        public DisplayInfo<T> SetVirtualHost(string virtualHost)
        {
            VirtualHost = virtualHost;
            return this;
        }

        public static DisplayInfo<T> For(T message) => new DisplayInfo<T>(message);

        private string NormalizedHeaders => string.Join(
            separator: ", ",
            values: Headers.Select(header => $"{{ {header.Key}={header.Value} }}"));
    }
}
