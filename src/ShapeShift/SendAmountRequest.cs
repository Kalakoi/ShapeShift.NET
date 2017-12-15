using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Kalakoi.Crypto.ShapeShift
{
    /// <summary>
    /// Provides ability to request amount needed to be sent to complete transaction.
    /// </summary>
    public class SendAmountRequest
    {
        /*url: shapeshift.io/sendamount
        method: POST
        data type: JSON
 
          Data required:
 
        amount          = the amount to be sent to the withdrawal address
        withdrawal      = the address for coin to be sent to
        pair            = what coins are being exchanged in the form [input coin]_[output coin]  ie ltc_btc
        returnAddress   = (Optional) address to return deposit to if anything goes wrong with exchange
        destTag         = (Optional) Destination tag that you want appended to a Ripple payment to you
        rsAddress       = (Optional) For new NXT accounts to be funded, supply this on NXT payment to you
        apiKey          = (Optional) Your affiliate PUBLIC KEY, for volume tracking, affiliate payments, split-shifts, etc...
 
        example data {"amount":123, "withdrawal":"123ABC", "pair":"ltc_btc", returnAddress:"BBBBBBB"}
 
          Success Output:
 
        {
             success:
              {
                pair: [pair],
                withdrawal: [Withdrawal Address], //-- will match address submitted in post
                withdrawalAmount: [Withdrawal Amount], // Amount of the output coin you will receive
                deposit: [Deposit Address (or memo field if input coin is BTS / BITUSD)],
                depositAmount: [Deposit Amount], // Exact amount of input coin to send in
                expiration: [timestamp when this will expire],
                quotedRate: [the exchange rate to be honored]
                apiPubKey: [public API attached to this shift, if one was given]
              }
        }
        */

        /// <summary>
        /// Coin pair requested for exchange.
        /// </summary>
        public string Pair { get; private set; }
        /// <summary>
        /// Address to send exchanged coins to.
        /// </summary>
        public string WithdrawalAddress { get; private set; }
        /// <summary>
        /// Amount of coins to be received from exchange.
        /// </summary>
        public double WithdrawalAmount { get; private set; }
        /// <summary>
        /// Address to send coins to be exchanged.
        /// </summary>
        public string DepositAddress { get; private set; }
        /// <summary>
        /// Amount of coins required to complete exchange.
        /// </summary>
        public double DepositAmount { get; private set; }
        /// <summary>
        /// Exchange expiration timestamp.
        /// </summary>
        public double Expiration { get; private set; }
        /// <summary>
        /// Quoted rate of exchange.
        /// </summary>
        public double QuotedRate { get; private set; }
        /// <summary>
        /// Public API key attached to this exchange, if any.
        /// </summary>
        public string APIKey { get; private set; }
        /// <summary>
        /// Error thrown by request, if any.
        /// </summary>
        public string Error { get; private set; }

        private SendAmountRequest() { }

        /// <summary>
        /// Gets information on pending exchange.
        /// </summary>
        /// <param name="Amount">Amount to be sent to withdrawal address.</param>
        /// <param name="Address">The withdrawal address.</param>
        /// <param name="Pair">The coin pair.</param>
        /// <param name="ReturnAddress">Address to return coins to if exchange fails.</param>
        /// <param name="RippleTag">Destination tag that you want appended to a Ripple payment to you.</param>
        /// <param name="NXTRsAddress">For new NXT accounts to be funded, supply this on NXT payment to you.</param>
        /// <param name="APIKey">Your affiliate PUBLIC KEY, for volume tracking, affiliate payments, split-shifts, etc...</param>
        /// <returns>Information on pending exchange.</returns>
        internal static async Task<SendAmountRequest> RequestAsync(double Amount, string Address, string Pair, string ReturnAddress = "", string RippleTag = "", string NXTRsAddress = "", string APIKey = "")
        {
            Uri uri = GetUri();
            string data = CreateData(Amount, Address, Pair, ReturnAddress, RippleTag, NXTRsAddress, APIKey);
            string response = await RestServices.GetPostResponseAsync(uri, data).ConfigureAwait(false);
            return await ParseResponseAsync(response).ConfigureAwait(false);
        }

        private static Uri GetUri() =>
            new Uri(@"https://shapeshift.io/sendamount");

        private static string CreateData(double Amount, string Address, string Pair, string ReturnAddress, string RippleTag, string NXTRsAddress, string APIKey)
        {
            string dataFormat = "\"{0}\":\"{1}\"";
            string data = "{";
            data += string.Format(dataFormat, "amount", Amount.ToString());
            data += ", " + string.Format(dataFormat, "withdrawal", Address);
            data += ", " + string.Format(dataFormat, "pair", Pair);
            if (!string.IsNullOrEmpty(ReturnAddress))
                data += ", " + string.Format(dataFormat, "returnAddress", ReturnAddress);
            if (!string.IsNullOrEmpty(RippleTag))
                data += ", " + string.Format(dataFormat, "destTag", RippleTag);
            if (!string.IsNullOrEmpty(NXTRsAddress))
                data += ", " + string.Format(dataFormat, "rsAddress", NXTRsAddress);
            if (!string.IsNullOrEmpty(APIKey))
                data += ", " + string.Format(dataFormat, "apiKey", APIKey);
            data += "}";
            return data;
        }

        private static async Task<SendAmountRequest> ParseResponseAsync(string response)
        {
            SendAmountRequest request = new SendAmountRequest();
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
                    else if (jtr.Value.ToString() == "withdrawal")
                    {
                        await jtr.ReadAsync().ConfigureAwait(false);
                        request.WithdrawalAddress = jtr.Value.ToString();
                    }
                    else if (jtr.Value.ToString() == "withdrawalAmount")
                    {
                        await jtr.ReadAsync().ConfigureAwait(false);
                        request.WithdrawalAmount = Convert.ToDouble(jtr.Value.ToString());
                    }
                    else if (jtr.Value.ToString() == "deposit")
                    {
                        await jtr.ReadAsync().ConfigureAwait(false);
                        request.DepositAddress = jtr.Value.ToString();
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
                    else if (jtr.Value.ToString() == "apiPubKey")
                    {
                        await jtr.ReadAsync().ConfigureAwait(false);
                        request.APIKey = jtr.Value.ToString();
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
