using System;
using System.Collections.Generic;
using System.Linq;
using Bogus;

namespace Rabbit.Common.Data.Trades
{
    public static class TradeData
    {
        public static readonly IReadOnlyCollection<string> Actions = new List<string>
        {
            "buy",
            "sell"
        };

        public static readonly IReadOnlyCollection<string> Tickers = new List<string>
        {
            "AAPL",
            "GOOG",
            "IBM",
            "MSFT",
            "AMZN"
        };

        public static readonly IReadOnlyCollection<string> Industries = new List<string>
        {
            "Software",
            "Financial Services",
            "Banking"
        };

        public static readonly IReadOnlyCollection<string> Regions = new List<string>
        {
            "Australia",
            "Great Britain",
            "USA",
        };

        static TradeData()
        {
            Randomizer.Seed = new Random(8675309);
        }

        public static bool ContainsRegion(string region) => Regions.Any(
            r => r.ToLower().Trim().Replace(" ", "") == region.ToLower().Trim().Replace(" ", ""));

        public static bool ContainsIndustry(string industry) => Industries.Any(
            i => i.ToLower().Trim().Replace(" ", "") == industry.ToLower().Trim().Replace(" ", ""));

        public static Trade GetFakeTrade()
        {
            return new Faker<Trade>()
                .RuleFor(trade => trade.Action, f => f.PickRandom<string>(Actions))
                .RuleFor(trade => trade.Amount, f => f.Finance.Amount())
                .RuleFor(trade => trade.Region, f => f.PickRandom<string>(Regions))
                .RuleFor(trade => trade.Industry, f => f.PickRandom<string>(Industries))
                .RuleFor(trade => trade.Ticker, f => f.PickRandom<string>(Tickers))
                .Generate();
        }
    }
}
