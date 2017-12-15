using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Kalakoi.Crypto.ShapeShift
{
    /// <summary>
    /// Provides access to information on recent exchanges completed by ShapeShift.
    /// </summary>
    public class RecentTx
    {
        /*url: shapeshift.io/recenttx/[max]
        method: GET
 
        [max] is an optional maximum number of transactions to return.
        If [max] is not specified this will return 5 transactions.
        Also, [max] must be a number between 1 and 50 (inclusive).
 
        Success Output:
            [
                {
                curIn : [currency input],
                curOut: [currency output],
                amount: [amount],
                timestamp: [time stamp]     //in seconds
                },
                ...
            ]
        */

        /// <summary>
        /// Currency user sent to exchange.
        /// </summary>
        public string CurrencyInput { get; private set; }
        /// <summary>
        /// Currency user requested from exchange.
        /// </summary>
        public string CurrencyOutput { get; private set; }
        /// <summary>
        /// Amount sent to user's withdrawal address.
        /// </summary>
        public double Amount { get; private set; }
        /// <summary>
        /// Timestamp of exchange.
        /// </summary>
        public double TimeStamp { get; private set; }

        /// <summary>
        /// Gets information on recent transactions completed by ShapeShift.
        /// </summary>
        /// <param name="Max">Maximum number of transactions to return. Must be betweeen 1 and 50, inclusive.</param>
        /// <returns>List of recent transactions.</returns>
        internal static async Task<List<RecentTx>> GetRecentTransactionsAsync(int Max)
        {
            if (Max < 1 || Max > 50) throw new InvalidOperationException();
            Uri uri = GetUri(Max);
            string response = await RestServices.GetResponseAsync(uri).ConfigureAwait(false);
            return await ParseResponseAsync(response).ConfigureAwait(false);
        }
        /// <summary>
        /// Gets information on recent transactions completed by ShapeShift.
        /// </summary>
        /// <returns>List of last 5 transactions.</returns>
        internal static async Task<List<RecentTx>> GetRecentTransactionsAsync() =>
            await GetRecentTransactionsAsync(5).ConfigureAwait(false);

        private static Uri GetUri(int max = 5) =>
            new Uri(string.Format(@"https://shapeshift.io/recenttx/{0}", max.ToString()));

        private static async Task<List<RecentTx>> ParseResponseAsync(string response)
        {
            List<RecentTx> TxList = new List<RecentTx>();
            RecentTx NewTx = new RecentTx();
            using (JsonTextReader jtr = new JsonTextReader(new StringReader(response)))
            {
                while (await jtr.ReadAsync().ConfigureAwait(false))
                {
                    if (jtr.TokenType.ToString() == "StartObject")
                    {
                        if (!string.IsNullOrEmpty(NewTx.CurrencyInput))
                            TxList.Add(NewTx);
                        NewTx = new RecentTx();
                    }
                    else if (jtr.Value == null) continue;
                    else if (jtr.Value.ToString() == "curIn")
                    {
                        await jtr.ReadAsync().ConfigureAwait(false);
                        NewTx.CurrencyInput = jtr.Value.ToString();
                    }
                    else if (jtr.Value.ToString() == "curOut")
                    {
                        await jtr.ReadAsync().ConfigureAwait(false);
                        NewTx.CurrencyOutput = jtr.Value.ToString();
                    }
                    else if (jtr.Value.ToString() == "amount")
                    {
                        await jtr.ReadAsync().ConfigureAwait(false);
                        NewTx.Amount = Convert.ToDouble(jtr.Value.ToString());
                    }
                    else if (jtr.Value.ToString() == "timestamp")
                    {
                        await jtr.ReadAsync().ConfigureAwait(false);
                        NewTx.TimeStamp = Convert.ToDouble(jtr.Value.ToString());
                    }
                    else continue;
                }
            }
            if (!string.IsNullOrEmpty(NewTx.CurrencyInput))
                TxList.Add(NewTx);

            return TxList;
        }
    }
}
