using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Kalakoi.Crypto.ShapeShift
{
    /// <summary>
    /// Provides ability to cancel pending exchanges.
    /// </summary>
    public class CancelResult
    {
        /*url: shapeshift.io/cancelpending
        method: POST
        data type: JSON
        data required: address  = The deposit address associated with the pending transaction
 
        Example data : {address : "1HB5XMLmzFVj8ALj6mfBsbifRoD4miY36v"}
 
        Success Output:
 
         {  success  : " Pending Transaction cancelled "  }
 
        Error Output:
 
         {  error  : {errorMessage}  }
        */

        /// <summary>
        /// True if cancel was successful.
        /// </summary>
        public bool Success { get; private set; }
        /// <summary>
        /// Message returned from cancel operation, if any.
        /// </summary>
        public string Message { get; private set; }
        /// <summary>
        /// Error message thrown during cancel, if any.
        /// </summary>
        public string Error { get; private set; }

        private CancelResult() { }

        /// <summary>
        /// Attempts to cancel pending exchange
        /// </summary>
        /// <param name="Address">The deposit address associated with the pending transaction.</param>
        /// <returns>Result of cancel operation.</returns>
        internal static async Task<CancelResult> CancelAsync(string Address)
        {
            //Get URI for POST request
            Uri uri = GetUri();
            //Generate JSON data string
            string data = CreateData(Address);
            //Send POST request
            string response = await RestServices.GetPostResponseAsync(uri, data).ConfigureAwait(false);
            //Parse response for results
            return await ParseResponseAsync(response).ConfigureAwait(false);
        }

        private static Uri GetUri() =>
            new Uri(@"https://shapeshift.io/cancelpending");

        private static string CreateData(string Address) =>
            "{" + string.Format("\"address\":\"{0}\"", Address) + "}";

        private static async Task<CancelResult> ParseResponseAsync(string response)
        {
            CancelResult result = new CancelResult();
            using (JsonTextReader jtr = new JsonTextReader(new StringReader(response)))
            {
                while (await jtr.ReadAsync().ConfigureAwait(false))
                {
                    if (jtr.Value == null) continue;
                    else if (jtr.Value.ToString() == "success")
                    {
                        result.Success = true;
                        await jtr.ReadAsync().ConfigureAwait(false);
                        result.Message = jtr.Value.ToString();
                    }
                    else if (jtr.Value.ToString() == "error")
                    {
                        result.Success = false;
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
