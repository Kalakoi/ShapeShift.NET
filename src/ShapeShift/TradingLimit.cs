using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Kalakoi.Crypto.ShapeShift
{
    /// <summary>
    /// Provides access to trade limits for specific coin pairs.
    /// </summary>
    public class TradingLimit
    {
        /*url: shapeshift.io/limit/[pair]
        method: GET
 
        [pair] is any valid coin pair such as btc_ltc or ltc_btc
 
        Success Output:
            {
                "pair" : "btc_ltc",
                "limit" : "1.2345"
            }
        */

        /// <summary>
        /// Currency pair.
        /// </summary>
        public string Pair { get; private set; }
        /// <summary>
        /// Trade limit.
        /// </summary>
        public double Limit { get; private set; }

        private TradingLimit() { }

        /// <summary>
        /// Gets trade limit for specified currency pair.
        /// </summary>
        /// <param name="Pair">Currency pair to exchange.</param>
        /// <returns>Trading limit information.</returns>
        internal static async Task<TradingLimit> GetLimitAsync(string Pair)
        {
            Uri uri = GetUri(Pair);
            string response = await RestServices.GetResponseAsync(uri).ConfigureAwait(false);
            return await ParseResponseAsync(response).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets trade limit for specified currency pair.
        /// </summary>
        /// <param name="Ticker1">Ticker for currency to exchange from.</param>
        /// <param name="Ticker2">Ticker for currency to exchange to.</param>
        /// <returns>Trading limit information.</returns>
        internal static async Task<TradingLimit> GetLimitAsync(string Ticker1, string Ticker2) =>
            await GetLimitAsync(string.Format("{0}_{1}", Ticker1, Ticker2)).ConfigureAwait(false);

        /// <summary>
        /// Gets list of all trade limits.
        /// </summary>
        /// <returns>List of all trade limits.</returns>
        internal static async Task<List<TradingLimit>> GetAllLimitsAsync()
        {
            List<TradingLimit> LimitList = new List<TradingLimit>();
            List<TradingPair> PairList = await TradingPair.GetAllPairsAsync().ConfigureAwait(false);
            foreach (TradingPair tp in PairList)
            {
                TradingLimit NewLimit = await GetLimitAsync(tp.Pair).ConfigureAwait(false);
                LimitList.Add(NewLimit);
            }
            return LimitList;
        }

        private static Uri GetUri(string Pair) =>
            new Uri(string.Format(@"https://shapeshift.io/limit/{0}", Pair));

        private static async Task<TradingLimit> ParseResponseAsync(string response)
        {
            TradingLimit limit = new TradingLimit();
            using (JsonTextReader jtr = new JsonTextReader(new StringReader(response)))
            {
                while (await jtr.ReadAsync().ConfigureAwait(false))
                {
                    if (jtr.Value != null)
                    {
                        if (jtr.Value.ToString() == "pair")
                        {
                            await jtr.ReadAsync().ConfigureAwait(false);
                            limit.Pair = jtr.Value.ToString();
                        }
                        else if (jtr.Value.ToString() == "limit")
                        {
                            await jtr.ReadAsync().ConfigureAwait(false);
                            limit.Limit = Convert.ToDouble(jtr.Value.ToString());
                        }
                        else continue;
                    }
                }
            }
            return limit;
        }
    }
}
