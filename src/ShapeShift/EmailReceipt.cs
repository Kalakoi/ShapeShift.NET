using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Kalakoi.Crypto.ShapeShift
{
    /// <summary>
    /// Status of Email request.
    /// </summary>
    public enum EmailStatuses { Success, Failure }

    /// <summary>
    /// Provides ability to request a receipt for a transaction to be sent via email.
    /// </summary>
    public class EmailReceipt
    {
        /*url:  shapeshift.io/mail
        method: POST
        data type: JSON
        data required:
        email    = the address for receipt email to be sent to
        txid       = the transaction id of the transaction TO the user (ie the txid for the withdrawal NOT the deposit)
        example data {"email":"mail@example.com", "txid":"123ABC"}
 
        Success Output:
        {"email":
            {
                "status":"success",
                "message":"Email receipt sent"
            }
        }
        */

        /// <summary>
        /// Status of receipt request.
        /// </summary>
        public EmailStatuses Status { get; private set; }
        /// <summary>
        /// Message returned by request, if any.
        /// </summary>
        public string Message { get; private set; }
        /// <summary>
        /// Error thrown by request, if any.
        /// </summary>
        public string Error { get; private set; }

        private EmailReceipt() { }

        /// <summary>
        /// Requests a receipt for transaction to be sent via email.
        /// </summary>
        /// <param name="Email">Email address to send receipt to.</param>
        /// <param name="TxID">Transaction ID of the transaction sent to the user.</param>
        /// <returns>Result of receipt request.</returns>
        internal static async Task<EmailReceipt> RequestAsync(string Email, string TxID)
        {
            //Get URI for POST request
            Uri uri = GetUri();
            //Generate JSON data as string to send
            string data = CreateData(Email, TxID);
            //Send POST request and awaits response
            string response = await RestServices.GetPostResponseAsync(uri, data).ConfigureAwait(false);
            //Parse response for results
            return await ParseResponseAsync(response).ConfigureAwait(false);
        }

        private static Uri GetUri() =>
            new Uri(@"https://shapeshift.io/mail");

        private static string CreateData(string Email, string TxID) =>
            "{" + string.Format("\"email\":\"{0}\", \"txid\":\"{1}\"", Email, TxID) + "}";

        private static async Task<EmailReceipt> ParseResponseAsync(string response)
        {
            EmailReceipt receipt = new EmailReceipt();
            using (JsonTextReader jtr = new JsonTextReader(new StringReader(response)))
            {
                while (await jtr.ReadAsync().ConfigureAwait(false))
                {
                    if (jtr.Value == null) continue;
                    else if (jtr.Value.ToString() == "status")
                    {
                        await jtr.ReadAsync().ConfigureAwait(false);
                        receipt.Status = jtr.Value.ToString() == "success" ? EmailStatuses.Success : EmailStatuses.Failure;
                    }
                    else if (jtr.Value.ToString() == "message")
                    {
                        await jtr.ReadAsync().ConfigureAwait(false);
                        receipt.Message = jtr.Value.ToString();
                    }
                    else if (jtr.Value.ToString() == "error")
                    {
                        await jtr.ReadAsync().ConfigureAwait(false);
                        receipt.Error = jtr.Value.ToString();
                    }
                    else continue;
                }
            }
            return receipt;
        }
    }
}
