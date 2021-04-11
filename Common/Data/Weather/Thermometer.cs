using System;
using Bogus;

namespace Rabbit.Common.Data.Weather
{
    public sealed class Thermometer
    {
        public Thermometer(string thermometerId, string thermometerName, string thermometerRegion)
        {
            ThermometerId = thermometerId;
            ThermometerName = thermometerName;
            ThermometerRegion = thermometerRegion;
        }

        public string ThermometerId { get; } = string.Empty;
        public string ThermometerName { get; } = string.Empty;
        public string ThermometerRegion { get; } = string.Empty;

        public Forecast Report() => CreateFakeForecast();

        public static Thermometer Fake()
        {
            var faker = new Faker();
            return new Thermometer(
                thermometerId: Guid.NewGuid().ToString(),
                thermometerName: $"{faker.Hacker.Verb()}-{faker.Hacker.Noun()}",
                thermometerRegion: faker.Address.City());
        }

        private Forecast CreateFakeForecast()
        {
            var faker = new Faker();
            return new Forecast
            {
                ThermometerId = this.ThermometerId,
                ThermometerName = this.ThermometerName,
                Region = this.ThermometerRegion,
                Conditions = faker.PickRandom(new string[3] { "Sunny", "Cloudy", "Partly Cloudy" }),
                Date = faker.Date.RecentOffset(days: 7).ToString("yyyy-MMM-dd"),
                High = faker.Random.Double(min: 15, max: 30),
                Low = faker.Random.Double(min: 5, max: 15),
                Time = faker.Date.Recent(days: 7).ToString("HH:mm:ss")
            };
        }
    }
}
