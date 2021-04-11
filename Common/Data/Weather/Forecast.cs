using System;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;

namespace Rabbit.Common.Data.Weather
{
    public record Forecast
    {
        public string ThermometerId { get; init; } = string.Empty;
        public string ThermometerName { get; init; } = string.Empty;
        public string Date { get; init; } = string.Empty;
        public string Time { get; init; } = string.Empty;
        public double Low { get; init; }
        public double High { get; init; }
        public string Conditions { get; init; } = string.Empty;
        public string Region { get; init; } = string.Empty;

        public string ToJson() => JsonSerializer.Serialize(
            value: this,
            options: new JsonSerializerOptions { WriteIndented = true });
        
        public static Forecast FromJson(string signalAsJson)
        {
            if (string.IsNullOrWhiteSpace(signalAsJson))
            {
                throw new ArgumentException(
                    message: $"'{nameof(signalAsJson)}' cannot be null or whitespace",
                    paramName: nameof(signalAsJson));
            }

            return JsonSerializer.Deserialize<Forecast>(signalAsJson) ??
                throw NewDeserializationException(
                    from: $"{nameof(signalAsJson)} {signalAsJson.GetType().Name}",
                    to: $"{typeof(Forecast).Name}");
        }
        
        public static Forecast FromBytes(byte[] bytes)
        {
            if (bytes is null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            return JsonSerializer.Deserialize<Forecast>(Encoding.UTF8.GetString(bytes)) ??
                throw NewDeserializationException(
                    from: $"{nameof(bytes)} {bytes.GetType().Name}",
                    to: $"{typeof(Forecast).Name}");
        }

        private static SerializationException NewDeserializationException(string from, string to) =>
            new SerializationException($"Deserialization from '{from}' to '{to}' failed.");
    }
}
