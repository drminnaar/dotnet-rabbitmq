using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Xml;
using System.Xml.Serialization;

namespace Rabbit.Common.Data.Trades
{
    public record Trade
    {
        public string Action { get; init; } = string.Empty;
        public string NormalizedAction => Action.ToLower().Trim().Replace(" ", string.Empty);
        public decimal Amount { get; init; }
        public string Region { get; init; } = string.Empty;
        public string NormalizedRegion => Region.ToLower().Trim().Replace(" ", string.Empty);
        public string Industry { get; init; } = string.Empty;
        public string NormalizedIndustry => Industry.ToLower().Trim().Replace(" ", string.Empty);
        public string Ticker { get; init; } = string.Empty;

        public byte[] ToBytes() => Encoding.UTF8.GetBytes(this.ToJson());

        public string ToJson()
        {
            return JsonSerializer.Serialize(
                this,
                new JsonSerializerOptions { WriteIndented = true });
        }

        public string ToXml()
        {
            var namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            var settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.OmitXmlDeclaration = true;
            settings.CheckCharacters = false;

            var xml = new StringBuilder();
            using var stream = new StringWriter(xml);
            using var writer = XmlWriter.Create(stream, settings);
            var serializer = new XmlSerializer(this.GetType());
            serializer.Serialize(writer, this, namespaces);
            return xml.ToString();
        }

        public static Trade FromBytes(byte[] tradeAsBytes)
        {
            var trade = Encoding.UTF8.GetString(tradeAsBytes) ?? string.Empty;
            return JsonSerializer.Deserialize<Trade>(trade) ??
                throw NewDeserializationException(
                    from: $"{nameof(tradeAsBytes)} {tradeAsBytes.GetType().Name}",
                    to: $"{typeof(Trade).Name}");
        }

        private static SerializationException NewDeserializationException(string from, string to) =>
            new SerializationException($"Deserialization from '{from}' to '{to}' failed.");
    }
}