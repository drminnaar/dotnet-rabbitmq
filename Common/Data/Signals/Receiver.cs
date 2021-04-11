using System;
using System.Text;
using System.Threading;

namespace Rabbit.Common.Data.Signals
{
    public static class Receiver
    {
        private static readonly Random _random = new Random();

        public static string DecodeSignal(Signal signal)
        {
            var encodedData = Convert.FromBase64String(signal.Data);
            var decodedData = Encoding.UTF8.GetString(encodedData);

            // simulate processing time
            Thread.Sleep(_random.Next(1, 10) * 1000);

            return decodedData;
        }
    }
}
