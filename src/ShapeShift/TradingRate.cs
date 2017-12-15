using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Kalakoi.Crypto.ShapeShift
{
    /// <summary>
    /// Provides ability to find exchange rates for coin pairs.
    /// </summary>
    public class TradingRate
    {
        /*url: shapeshift.io/rate/[pair]
        method: GET
 
        [pair] is any valid coin pair such as btc_ltc or ltc_btc
 
        Success Output:
   
            {
                "pair" : "btc_ltc",
                "rate" : "70.1234"
            }
        */

        /// <summary>
        /// Coin pair.
        /// </summary>
        public string Pair { get; private set; }
        /// <summary>
        /// Exchange rate.
        /// </summary>
        public double Rate { get; private set; }

        private TradingRate() { }

        /// <summary>
        /// Finds exchange rate for specified coin pair.
        /// </summary>
        /// <param name="Pair">Coin pair to find rate for.</param>
        /// <returns>Exchange rate.</returns>
        internal static async Task<TradingRate> GetRateAsync(string Pair)
        {
            Uri uri = GetUri(Pair);
            string response = await RestServices.GetResponseAsync(uri).ConfigureAwait(false);
            return await ParseResponseAsync(response).ConfigureAwait(false);
        }

        /// <summary>
        /// Finds exchange rate for specified coin pair.
        /// </summary>
        /// <param name="Ticker1">Ticker symbol for coin to exchange.</param>
        /// <param name="Ticker2">Ticker symbol for resulting coin.</param>
        /// <returns>Exchange rate.</returns>
        internal static async Task<TradingRate> GetRateAsync(string Ticker1, string Ticker2) =>
            await GetRateAsync(string.Format("{0}_{1}", Ticker1, Ticker2)).ConfigureAwait(false);

        /// <summary>
        /// Finds exchange rates for all valid coin pairs.
        /// </summary>
        /// <returns>List of exchange rates.</returns>
        internal static async Task<List<TradingRate>> GetAllRatesAsync()
        {
            List<TradingRate> RateList = new List<TradingRate>();
            List<TradingPair> PairList = await TradingPair.GetAllPairsAsync().ConfigureAwait(false);
            foreach (TradingPair tp in PairList)
            {
                TradingRate NewRate = await GetRateAsync(tp.Pair).ConfigureAwait(false);
                RateList.Add(NewRate);
            }
            return RateList;
        }

        private static Uri GetUri(string Pair) =>
            new Uri(string.Format(@"https://shapeshift.io/rate/{0}", Pair));

        private static async Task<TradingRate> ParseResponseAsync(string response)
        {
            TradingRate rate = new TradingRate();
            using (JsonTextReader jtr = new JsonTextReader(new StringReader(response)))
            {
                while (await jtr.ReadAsync().ConfigureAwait(false))
                {
                    if (jtr.Value != null)
                    {
                        if (jtr.Value.ToString() == "pair")
                        {
                            await jtr.ReadAsync().ConfigureAwait(false);
                            rate.Pair = jtr.Value.ToString();
                        }
                        else if (jtr.Value.ToString() == "rate")
                        {
                            await jtr.ReadAsync().ConfigureAwait(false);
                            rate.Rate = Convert.ToDouble(jtr.Value.ToString());
                        }
                        else continue;
                    }
                }
            }
            return rate;
        }
    }
}
