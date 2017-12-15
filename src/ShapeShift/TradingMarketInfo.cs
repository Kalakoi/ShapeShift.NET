using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Kalakoi.Crypto.ShapeShift
{
    /// <summary>
    /// Provides access to exchange market info for currency pairs.
    /// </summary>
    public class TradingMarketInfo
    {
        /*url: shapeshift.io/marketinfo/[pair]
        method: GET
 
        [pair] (OPTIONAL) is any valid coin pair such as btc_ltc or ltc_btc.
        The pair is not required and if not specified will return an array of all market infos.
 
        Success Output:
            {
                "pair"     : "btc_ltc",
                "rate"     : 130.12345678,
                "limit"    : 1.2345,
                "min"      : 0.02621232,
                "minerFee" : 0.0001
            }
        */

        /// <summary>
        /// Currency pair.
        /// </summary>
        public string Pair { get; private set; }
        /// <summary>
        /// Exchange rate.
        /// </summary>
        public double Rate { get; private set; }
        /// <summary>
        /// Exchange limit.
        /// </summary>
        public double Limit { get; private set; }
        /// <summary>
        /// Minimum exchange amount.
        /// </summary>
        public double Min { get; private set; }
        /// <summary>
        /// Fee to be sent to miners for exchange.
        /// </summary>
        public double MinerFee { get; private set; }

        private TradingMarketInfo() { }

        /// <summary>
        /// Gets market info for specific currency pair.
        /// </summary>
        /// <param name="Pair">Pair to get information for.</param>
        /// <returns>Market Information.</returns>
        internal static async Task<TradingMarketInfo> GetMarketInfoAsync(string Pair)
        {
            Uri uri = GetUri(Pair);
            string response = await RestServices.GetResponseAsync(uri).ConfigureAwait(false);
            return await ParseResponseAsync(response).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets market info for specific currency pair.
        /// </summary>
        /// <param name="Ticker1">Ticker for currency to exchange from.</param>
        /// <param name="Ticker2">Ticker for currency to exchange to.</param>
        /// <returns>Market Information.</returns>
        internal static async Task<TradingMarketInfo> GetMarketInfoAsync(string Ticker1, string Ticker2) =>
            await GetMarketInfoAsync(string.Format("{0}_{1}", Ticker1, Ticker2)).ConfigureAwait(false);

        /// <summary>
        /// Gets market info for all currency pairs.
        /// </summary>
        /// <returns>List of Market Information.</returns>
        internal static async Task<List<TradingMarketInfo>> GetAllMarketsAsync()
        {
            List<TradingMarketInfo> MarketList = new List<TradingMarketInfo>();
            List<TradingPair> PairList = await TradingPair.GetAllPairsAsync().ConfigureAwait(false);
            foreach (TradingPair tp in PairList)
            {
                TradingMarketInfo NewMarket = await GetMarketInfoAsync(tp.Pair).ConfigureAwait(false);
                MarketList.Add(NewMarket);
            }
            return MarketList;
        }

        private static Uri GetUri(string Pair) =>
            new Uri(string.Format(@"https://shapeshift.io/marketinfo/{0}", Pair));

        private static async Task<TradingMarketInfo> ParseResponseAsync(string response)
        {
            TradingMarketInfo MarketInfo = new TradingMarketInfo();
            using (JsonTextReader jtr = new JsonTextReader(new StringReader(response)))
            {
                while (await jtr.ReadAsync().ConfigureAwait(false))
                {
                    if (jtr.Value != null)
                    {
                        if (jtr.Value.ToString() == "pair")
                        {
                            await jtr.ReadAsync().ConfigureAwait(false);
                            MarketInfo.Pair = jtr.Value.ToString();
                        }
                        else if (jtr.Value.ToString() == "rate")
                        {
                            await jtr.ReadAsync().ConfigureAwait(false);
                            MarketInfo.Rate = Convert.ToDouble(jtr.Value.ToString());
                        }
                        else if (jtr.Value.ToString() == "limit")
                        {
                            await jtr.ReadAsync().ConfigureAwait(false);
                            MarketInfo.Limit = Convert.ToDouble(jtr.Value.ToString());
                        }
                        else if (jtr.Value.ToString() == "min")
                        {
                            await jtr.ReadAsync().ConfigureAwait(false);
                            MarketInfo.Min = Convert.ToDouble(jtr.Value.ToString());
                        }
                        else if (jtr.Value.ToString() == "minerFee")
                        {
                            await jtr.ReadAsync().ConfigureAwait(false);
                            MarketInfo.MinerFee = Convert.ToDouble(jtr.Value.ToString());
                        }
                        else continue;
                    }
                    else continue;
                }
            }
            return MarketInfo;
        }
    }
}
