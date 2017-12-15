using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Kalakoi.Crypto.ShapeShift
{
    /// <summary>
    /// Provides ability to send exchange requests to ShapeShift.
    /// </summary>
    public class ShiftResult
    {
        /*url:  shapeshift.io/shift
        method: POST
        data type: JSON
        data required:
        withdrawal      = the address for resulting coin to be sent to
        pair            = what coins are being exchanged in the form [input coin]_[output coin]  ie btc_ltc
        returnAddress   = (Optional) address to return deposit to if anything goes wrong with exchange
        destTag         = (Optional) Destination tag that you want appended to a Ripple payment to you
        rsAddress       = (Optional) For new NXT accounts to be funded, you supply this on NXT payment to you
        apiKey          = (Optional) Your affiliate PUBLIC KEY, for volume tracking, affiliate payments, split-shifts, etc...
 
        example data: {"withdrawal":"AAAAAAAAAAAAA", "pair":"btc_ltc", returnAddress:"BBBBBBBBBBB"}
 
        Success Output:
            {
                deposit: [Deposit Address (or memo field if input coin is BTS / BITUSD)],
                depositType: [Deposit Type (input coin symbol)],
                withdrawal: [Withdrawal Address], //-- will match address submitted in post
                withdrawalType: [Withdrawal Type (output coin symbol)],
                public: [NXT RS-Address pubkey (if input coin is NXT)],
                xrpDestTag : [xrpDestTag (if input coin is XRP)],
                apiPubKey: [public API attached to this shift, if one was given]
            } 
        */

        /// <summary>
        /// Address to send coins to exchange.
        /// </summary>
        public string DepositAddress { get; private set; }
        /// <summary>
        /// Currency to exchange from.
        /// </summary>
        public string DepositCoin { get; private set; }
        /// <summary>
        /// Address to send exchanged coins to.
        /// </summary>
        public string WithdrawalAddress { get; private set; }
        /// <summary>
        /// Currency to exchange to.
        /// </summary>
        public string WithdrawalCoin { get; private set; }
        /// <summary>
        /// Destination tag to be appended to Ripple payment.
        /// </summary>
        public string RippleTag { get; private set; }
        /// <summary>
        /// NXT RS-Address public key.
        /// </summary>
        public string NXTRsAddress { get; private set; }
        /// <summary>
        /// Public API key attached to this exchange, if any.
        /// </summary>
        public string APIKey { get; private set; }
        /// <summary>
        /// Error thrown by exchange request, if any.
        /// </summary>
        public string Error { get; private set; }

        private ShiftResult() { }

        /// <summary>
        /// Sends Shift request.
        /// </summary>
        /// <param name="Withdrawal">Address for resulting coins to be sent to.</param>
        /// <param name="Pair">Currency pair for exchange.</param>
        /// <param name="Return">Address to return coins to if exchange fails.</param>
        /// <param name="RippleTag">Destination tag that you want appended to a Ripple payment to you.</param>
        /// <param name="NXTRsAddress">For new NXT accounts to be funded, you supply this on NXT payment to you.</param>
        /// <param name="APIKey">Your affiliate PUBLIC KEY, for volume tracking, affiliate payments, split-shifts, etc...</param>
        /// <returns>Result of Shift request.</returns>
        internal static async Task<ShiftResult> ShiftAsync(string Withdrawal, string Pair, string Return = "", string RippleTag = "", string NXTRsAddress = "", string APIKey = "")
        {
            Uri uri = GetUri();
            string data = CreateData(Withdrawal, Pair, Return, RippleTag, NXTRsAddress, APIKey);
            string response = await RestServices.GetPostResponseAsync(uri, data).ConfigureAwait(false);
            return await ParseResponseAsync(response).ConfigureAwait(false);
        }

        private static Uri GetUri() =>
            new Uri(@"https://shapeshift.io/shift");

        private static string CreateData(string Withdrawal, string Pair, string Return, string RippleTag, string NXTRsAddress, string APIKey)
        {
            string dataFormat = "\"{0}\":\"{1}\"";
            string data = "{";
            data += string.Format(dataFormat, "withdrawal", Withdrawal);
            data += ", " + string.Format(dataFormat, "pair", Pair);
            if (!string.IsNullOrEmpty(Return))
                data += ", " + string.Format(dataFormat, "returnAddress", Return);
            if (!string.IsNullOrEmpty(RippleTag))
                data += ", " + string.Format(dataFormat, "destTag", RippleTag);
            if (!string.IsNullOrEmpty(NXTRsAddress))
                data += ", " + string.Format(dataFormat, "rsAddress", NXTRsAddress);
            if (!string.IsNullOrEmpty(APIKey))
                data += ", " + string.Format(dataFormat, "apiKey", APIKey);
            data += "}";
            return data;
        }

        private static async Task<ShiftResult> ParseResponseAsync(string response)
        {
            ShiftResult result = new ShiftResult();
            using (JsonTextReader jtr = new JsonTextReader(new StringReader(response)))
            {
                while (await jtr.ReadAsync().ConfigureAwait(false))
                {
                    if (jtr.Value == null) continue;
                    else if (jtr.Value.ToString() == "deposit")
                    {
                        await jtr.ReadAsync().ConfigureAwait(false);
                        result.DepositAddress = jtr.Value.ToString();
                    }
                    else if (jtr.Value.ToString() == "depositType")
                    {
                        await jtr.ReadAsync().ConfigureAwait(false);
                        result.DepositCoin = jtr.Value.ToString();
                    }
                    else if (jtr.Value.ToString() == "withdrawal")
                    {
                        await jtr.ReadAsync().ConfigureAwait(false);
                        result.WithdrawalAddress = jtr.Value.ToString();
                    }
                    else if (jtr.Value.ToString() == "withdrawalType")
                    {
                        await jtr.ReadAsync().ConfigureAwait(false);
                        result.WithdrawalCoin = jtr.Value.ToString();
                    }
                    else if (jtr.Value.ToString() == "public")
                    {
                        await jtr.ReadAsync().ConfigureAwait(false);
                        result.NXTRsAddress = jtr.Value.ToString();
                    }
                    else if (jtr.Value.ToString() == "xrpDestTag")
                    {
                        await jtr.ReadAsync().ConfigureAwait(false);
                        result.RippleTag = jtr.Value.ToString();
                    }
                    else if (jtr.Value.ToString() == "apiPubKey")
                    {
                        await jtr.ReadAsync().ConfigureAwait(false);
                        result.APIKey = jtr.Value.ToString();
                    }
                    else if (jtr.Value.ToString() == "error")
                    {
                        await jtr.ReadAsync().ConfigureAwait(false);
                        result.Error = jtr.Value.ToString();
                    }
                    else continue;
                }
            }
            return result;
        }
    }
}
