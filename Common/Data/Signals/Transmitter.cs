using System;
using System.Text;
using Bogus;

namespace Rabbit.Common.Data.Signals
{
    public sealed class Transmitter
    {
        public Transmitter(string transmitterId, string transmitterName, string transmitterRegion)
        {
            TransmitterId = transmitterId;
            TransmitterName = transmitterName;
            TransmitterRegion = transmitterRegion;
        }

        public string TransmitterId { get; } = string.Empty;
        public string TransmitterName { get; } = string.Empty;
        public string TransmitterRegion { get; } = string.Empty;

        public Signal Transmit() => new Signal
        {
            TransmitterId = this.TransmitterId,
            TransmitterName = this.TransmitterName,
            TransmitterRegion = this.TransmitterRegion,
            Data = GetData()
        };

        public static Transmitter Fake()
        {
            var faker = new Faker();
            return new Transmitter(
                transmitterId: Guid.NewGuid().ToString(),
                transmitterName: $"{faker.Hacker.Verb()}-{faker.Hacker.Noun()}",
                transmitterRegion: faker.Address.Country());
        } 

        private static string GetData()
        {
            var data = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString().ToUpper());

            return Convert.ToBase64String(data);
        }
    }
}
