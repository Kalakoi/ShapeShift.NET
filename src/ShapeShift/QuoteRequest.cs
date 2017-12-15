using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Kalakoi.Crypto.ShapeShift
{
    /// <summary>
    /// Provides ability to request quotes for specific pair exchanges.
    /// </summary>
    public class QuoteRequest
    {
        /*url: shapeshift.io/sendamount
        method: POST
        data type: JSON

          Data required:
 
        amount  = the amount to be sent to the withdrawal address
        pair    = what coins are being exchanged in the form [input coin]_[output coin]  ie ltc_btc
 
        example data {"amount":123, "pair":"ltc_btc"}
 
          Success Output:
 
        {
             success:
              {
                pair: [pair],
                withdrawalAmount: [Withdrawal Amount], // Amount of the output coin you will receive
                depositAmount: [Deposit Amount], // Exact amount of input coin to send in
                expiration: [timestamp when this will expire],
                quotedRate: [the exchange rate to be honored]
                minerFee: [miner fee for this transaction]
              }
        }
        */

        /// <summary>
        /// Coin pair to exchange between.
        /// </summary>
        public string Pair { get; private set; }
        /// <summary>
        /// Amount of coin to be received by user.
        /// </summary>
        public double WithdrawalAmount { get; private set; }
        /// <summary>
        /// Amount of coin required to be deposited.
        /// </summary>
        public double DepositAmount { get; private set; }
        /// <summary>
        /// Expiration timestamp of quote.
        /// </summary>
        public double Expiration { get; private set; }
        /// <summary>
        /// Quoted rate of exchange.
        /// </summary>
        public double QuotedRate { get; private set; }
        /// <summary>
        /// Fee to be sent to miners to fascilitate exchange.
        /// </summary>
        public double MinerFee { get; private set; }
        /// <summary>
        /// Error thrown by request, if any.
        /// </summary>
        public string Error { get; private set; }

        private QuoteRequest() { }

        /// <summary>
        /// Requests a quote for an exchange without exchanging.
        /// </summary>
        /// <param name="Pair">Coin pair to exchange between.</param>
        /// <param name="Amount">Amount of coin to be sent to withdrawal address.</param>
        /// <returns>Quote for exchange information.</returns>
        internal static async Task<QuoteRequest> RequestAsync(string Pair, double Amount)
        {
            Uri uri = GetUri();
            string data = CreateData(Pair, Amount);
            string response = await RestServices.GetPostResponseAsync(uri, data).ConfigureAwait(false);
            return await ParseResponseAsync(response).ConfigureAwait(false);
        }

        private static Uri GetUri() =>
            new Uri(@"https://shapeshift.io/sendamount");

        private static string CreateData(string Pair, double Amount) =>
            "{" + string.Format("\"amount\":\"{0}\", \"pair\":\"{1}\"", Amount.ToString(), Pair) + "}";

        private static async Task<QuoteRequest> ParseResponseAsync(string response)
        {
            QuoteRequest request = new QuoteRequest();
            using (JsonTextReader jtr = new JsonTextReader(new StringReader(response)))
            {
                while (await jtr.ReadAsync().ConfigureAwait(false))
                {
                    if (jtr.Value == null) continue;
                    else if (jtr.Value.ToString() == "pair")
                    {
                        await jtr.ReadAsync().ConfigureAwait(false);
                        request.Pair = jtr.Value.ToString();
                    }
                    else if (jtr.Value.ToString() == "withdrawalAmount")
                    {
                        await jtr.ReadAsync().ConfigureAwait(false);
                        request.WithdrawalAmount = Convert.ToDouble(jtr.Value.ToString());
                    }
                    else if (jtr.Value.ToString() == "depositAmount")
                    {
                        await jtr.ReadAsync().ConfigureAwait(false);
                        request.DepositAmount = Convert.ToDouble(jtr.Value.ToString());
                    }
                    else if (jtr.Value.ToString() == "expiration")
                    {
                        await jtr.ReadAsync().ConfigureAwait(false);
                        request.Expiration = Convert.ToDouble(jtr.Value.ToString());
                    }
                    else if (jtr.Value.ToString() == "quotedRate")
                    {
                        await jtr.ReadAsync().ConfigureAwait(false);
                        request.QuotedRate = Convert.ToDouble(jtr.Value.ToString());
                    }
                    else if (jtr.Value.ToString() == "minerFee")
                    {
                        await jtr.ReadAsync().ConfigureAwait(false);
                        request.MinerFee = Convert.ToDouble(jtr.Value.ToString());
                    }
                    else if (jtr.Value.ToString() == "error")
                    {
                        await jtr.ReadAsync().ConfigureAwait(false);
                        request.Error = jtr.Value.ToString();
                    }
                    else continue;
                }
            }
            return request;
        }
    }
}
