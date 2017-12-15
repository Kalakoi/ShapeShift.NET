using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Kalakoi.Crypto.ShapeShift
{
    /// <summary>
    /// Provides ability to validate addresses for supported coins.
    /// </summary>
    public class ValidateAddress
    {
        /*url: shapeshift.io/validateAddress/[address]/[coinSymbol]
        method: GET
 
        [address] the address that the user wishes to validate
        [coinSymbol] the currency symbol of the coin
 
        Success Output:
 
  
                {
                    isValid: [true / false],
                    error: [(if isvalid is false, there will be an error message)]
                }
     
 
        isValid will either be true or false. If isvalid returns false, an error parameter will be present and will contain a descriptive error message.
        */

        /// <summary>
        /// True if address is valid.
        /// </summary>
        public bool IsValid { get; private set; }
        /// <summary>
        /// Error thrown by validation check.
        /// </summary>
        public string Error { get; private set; }

        private ValidateAddress() { }

        /// <summary>
        /// Validates address belongs to specified currency.
        /// </summary>
        /// <param name="Address">Address to check.</param>
        /// <param name="Symbol">Ticker symbol for currency to check.</param>
        /// <returns>Validation results.</returns>
        internal static async Task<ValidateAddress> ValidateAsync(string Address, string Symbol)
        {
            Uri uri = GetUri(Address, Symbol);
            string response = await RestServices.GetResponseAsync(uri).ConfigureAwait(false);
            return await ParseResponseAsync(response).ConfigureAwait(false);
        }

        private static Uri GetUri(string Address, string Symbol) =>
            new Uri(string.Format(@"https://shapeshift.io/validateaddress/{0}/{1}", Address, Symbol));

        private static async Task<ValidateAddress> ParseResponseAsync(string response)
        {
            ValidateAddress va = new ValidateAddress();
            using (JsonTextReader jtr = new JsonTextReader(new StringReader(response)))
            {
                while (await jtr.ReadAsync().ConfigureAwait(false))
                {
                    if (jtr.Value == null) continue;
                    else if (jtr.Value.ToString() == "isValid")
                    {
                        await jtr.ReadAsync().ConfigureAwait(false);
                        va.IsValid = jtr.Value.ToString() == "true";
                    }
                    else if (jtr.Value.ToString() == "error")
                    {
                        await jtr.ReadAsync().ConfigureAwait(false);
                        va.Error = jtr.Value.ToString();
                    }
                    else continue;
                }
            }
            return va;
        }
    }
}
