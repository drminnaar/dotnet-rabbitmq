using System;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;

namespace Rabbit.Common.Data.Signals
{
    public record Signal
    {
        public string TransmitterId { get; init; } = string.Empty;
        public string TransmitterName { get; init; } = string.Empty;
        public string TransmitterRegion { get; init; } = string.Empty;
        public string Data { get; init; } = string.Empty;

        public string ToJson() => JsonSerializer.Serialize(
            value: this,
            options: new JsonSerializerOptions { WriteIndented = true });
        
        public static Signal FromJson(string signalAsJson)
        {
            if (string.IsNullOrWhiteSpace(signalAsJson))
            {
                throw new ArgumentException(
                    $"'{nameof(signalAsJson)}' cannot be null or whitespace",
                    nameof(signalAsJson));
            }

            return JsonSerializer.Deserialize<Signal>(signalAsJson) ??
                throw NewDeserializationException(
                    from: $"{nameof(signalAsJson)} {signalAsJson.GetType().Name}",
                    to: $"{typeof(Signal).Name}");
        }
        
        public static Signal FromBytes(byte[] signalAsBytes)
        {
            if (signalAsBytes is null)
                throw new ArgumentNullException(nameof(signalAsBytes));

            return JsonSerializer.Deserialize<Signal>(Encoding.UTF8.GetString(signalAsBytes)) ??
                throw NewDeserializationException(
                    from: $"{nameof(signalAsBytes)} {signalAsBytes.GetType().Name}",
                    to: $"{typeof(Signal).Name}");
        }

        private static SerializationException NewDeserializationException(string from, string to) =>
            new SerializationException($"Deserialization from '{from}' to '{to}' failed.");
    }
}
