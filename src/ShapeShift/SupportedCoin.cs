using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Kalakoi.Crypto.ShapeShift
{
    /// <summary>
    /// Exchange availability status of coin.
    /// </summary>
    public enum CoinStatuses { Available, Unavailable }

    /// <summary>
    /// Provides information on specific currencies.
    /// </summary>
    public class SupportedCoin
    {
        /*url: shapeshift.io/getcoins
        method: GET
 
        Success Output:
 
            {
                "SYMBOL1" :
                    {
                        name: ["Currency Formal Name"],
                        symbol: <"SYMBOL1">,
                        image: ["https://shapeshift.io/images/coins/coinName.png"],
                        status: [available / unavailable]
                    }
                (one listing per supported currency)
            }
 
        The status can be either "available" or "unavailable". Sometimes coins become temporarily unavailable during updates or
        unexpected service issues.
        */

        /// <summary>
        /// Name of currency.
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Ticker symbol for currency.
        /// </summary>
        public string Symbol { get; private set; }
        /// <summary>
        /// Link to currency icon.
        /// </summary>
        public string ImageLink { get; private set; }
        /// <summary>
        /// Currency exchange availability.
        /// </summary>
        public CoinStatuses Status { get; private set; }

        private SupportedCoin() { }

        /// <summary>
        /// Provides information on all currencies supported by ShapeShift.
        /// </summary>
        /// <returns>List of all supported currencies.</returns>
        internal static async Task<List<SupportedCoin>> GetCoinsAsync()
        {
            Uri uri = GetUri();
            string response = await RestServices.GetResponseAsync(uri).ConfigureAwait(false);
            return await ParseResponseAsync(response).ConfigureAwait(false);
        }

        /// <summary>
        /// Provides information on a specific currency supported by ShapeShift.
        /// </summary>
        /// <param name="Symbol">Ticker symbol of currency.</param>
        /// <returns>Information on specific supported currency.</returns>
        internal static async Task<SupportedCoin> GetCoinAsync(string Symbol) =>
            (await GetCoinsAsync().ConfigureAwait(false)).First(c => c.Symbol == Symbol);

        private static Uri GetUri() => new Uri(@"https://shapeshift.io/getcoins");

        private static async Task<List<SupportedCoin>> ParseResponseAsync(string response)
        {
            List<SupportedCoin> CoinList = new List<SupportedCoin>();
            SupportedCoin ToAdd = new SupportedCoin();
            using (JsonTextReader jtr = new JsonTextReader(new StringReader(response)))
            {
                while (await jtr.ReadAsync().ConfigureAwait(false))
                {
                    if (jtr.TokenType.ToString() == "StartObject")
                    {
                        if (!string.IsNullOrEmpty(ToAdd.Name))
                            CoinList.Add(ToAdd);
                        ToAdd = new SupportedCoin();
                        continue;
                    }
                    if (jtr.Value != null)
                    {
                        if (jtr.Value.ToString() == "name")
                        {
                            await jtr.ReadAsync().ConfigureAwait(false);
                            ToAdd.Name = jtr.Value.ToString();
                        }
                        else if (jtr.Value.ToString() == "symbol")
                        {
                            await jtr.ReadAsync().ConfigureAwait(false);
                            ToAdd.Symbol = jtr.Value.ToString();
                        }
                        else if (jtr.Value.ToString() == "image")
                        {
                            await jtr.ReadAsync().ConfigureAwait(false);
                            ToAdd.ImageLink = jtr.Value.ToString();
                        }
                        else if (jtr.Value.ToString() == "status")
                        {
                            await jtr.ReadAsync().ConfigureAwait(false);
                            ToAdd.Status = jtr.Value.ToString() == "available" ? CoinStatuses.Available : CoinStatuses.Unavailable;
                        }
                        else continue;
                    }
                }
            }
            if (!string.IsNullOrEmpty(ToAdd.Name))
                CoinList.Add(ToAdd);

            return CoinList;
        }
    }
}
