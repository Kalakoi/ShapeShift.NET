using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Kalakoi.Crypto.ShapeShift
{
    /// <summary>
    /// Provides ability to view previous transactions by address or API key.
    /// </summary>
    public class Tx
    {
        /// <summary>
        /// Transaction ID of the input coin going into shapeshift.
        /// </summary>
        public string InputTxID { get; private set; }
        /// <summary>
        /// Address that the input coin was paid to for this shift.
        /// </summary>
        public string InputAddress { get; private set; }
        /// <summary>
        /// Currency type of the input coin.
        /// </summary>
        public string InputCoin { get; private set; }
        /// <summary>
        /// Amount of input coin that was paid in on this shift.
        /// </summary>
        public double InputAmount { get; private set; }
        /// <summary>
        /// Transaction ID of the output coin going out to user.
        /// </summary>
        public string OutputTxID { get; private set; }
        /// <summary>
        /// Address that the output coin was sent to for this shift.
        /// </summary>
        public string OutputAddress { get; private set; }
        /// <summary>
        /// Currency type of the output coin.
        /// </summary>
        public string OutputCoin { get; private set; }
        /// <summary>
        /// Amount of output coin that was paid out on this shift.
        /// </summary>
        public double OutputAmount { get; private set; }
        /// <summary>
        /// The effective rate the user got on this shift.
        /// </summary>
        public double ShiftRate { get; private set; }
        /// <summary>
        /// Status of the shift.
        /// </summary>
        public TxStatuses Status { get; private set; }

        private Tx() { }

        /*url: shapeshift.io/txbyapikey/[apiKey]
        method: GET
 
        [apiKey] is the affiliate's PRIVATE api key.
 
            [
                {
                    inputTXID: [Transaction ID of the input coin going into shapeshift],
                    inputAddress: [Address that the input coin was paid to for this shift],
                    inputCurrency: [Currency type of the input coin],
                    inputAmount: [Amount of input coin that was paid in on this shift],
                    outputTXID: [Transaction ID of the output coin going out to user],
                    outputAddress: [Address that the output coin was sent to for this shift],
                    outputCurrency: [Currency type of the output coin],
                    outputAmount: [Amount of output coin that was paid out on this shift],
                    shiftRate: [The effective rate the user got on this shift.],
                    status: [status of the shift]
                }
                (one listing per transaction returned)
            ]
 
        The status can be  "received", "complete", "returned", "failed".
        */

        /// <summary>
        /// Finds all transactions sent using the specified API key.
        /// </summary>
        /// <param name="APIKey">The affiliate's PRIVATE api key.</param>
        /// <returns>List of transactions.</returns>
        internal static async Task<List<Tx>> GetTransactionsByAPIKeyAsync(string APIKey)
        {
            Uri uri = GetKeyUri(APIKey);
            string response = await RestServices.GetResponseAsync(uri).ConfigureAwait(false);
            return await ParseResponseAsync(response).ConfigureAwait(false);
        }

        private static Uri GetKeyUri(string APIKey) =>
            new Uri(string.Format(@"https://shapeshift.io/txbyapikey/{0}", APIKey));

        /*url: shapeshift.io/txbyaddress/[address]/[apiKey]
        method: GET
 
        [address] the address that output coin was sent to for the shift
        [apiKey] is the affiliate's PRIVATE api key.
 
        Success Output:
 
            [
                {
                    inputTXID: [Transaction ID of the input coin going into shapeshift],
                    inputAddress: [Address that the input coin was paid to for this shift],
                    inputCurrency: [Currency type of the input coin],
                    inputAmount: [Amount of input coin that was paid in on this shift],
                    outputTXID: [Transaction ID of the output coin going out to user],
                    outputAddress: [Address that the output coin was sent to for this shift],
                    outputCurrency: [Currency type of the output coin],
                    outputAmount: [Amount of output coin that was paid out on this shift],
                    shiftRate: [The effective rate the user got on this shift.],
                    status: [status of the shift]
                }
                (one listing per transaction returned)
            ]
 
        The status can be  "received", "complete", "returned", "failed".
        */

        /// <summary>
        /// Finds all transactions sent to specified address.
        /// </summary>
        /// <param name="Address">The address that output coin was sent to for the shift.</param>
        /// <param name="APIKey">The affiliate's PRIVATE api key.</param>
        /// <returns>List of transactions.</returns>
        internal static async Task<List<Tx>> GetTransactionsByAddressAsync(string Address, string APIKey)
        {
            Uri uri = GetAddressUri(Address, APIKey);
            string response = await RestServices.GetResponseAsync(uri).ConfigureAwait(false);
            return await ParseResponseAsync(response).ConfigureAwait(false);
        }

        private static Uri GetAddressUri(string Address, string APIKey) =>
            new Uri(string.Format(@"https://shapeshift.io/txbyaddress/{0}/{1}", Address, APIKey));

        private static async Task<List<Tx>> ParseResponseAsync(string response)
        {
            List<Tx> TxList = new List<Tx>();
            Tx NewTx = new Tx();
            using (JsonTextReader jtr = new JsonTextReader(new StringReader(response)))
            {
                while (await jtr.ReadAsync().ConfigureAwait(false))
                {
                    if (jtr.TokenType.ToString() == "StartObject")
                    {
                        if (!string.IsNullOrEmpty(NewTx.InputTxID))
                            TxList.Add(NewTx);
                        NewTx = new Tx();
                    }
                    else if (jtr.Value == null) continue;
                    else if (jtr.Value.ToString() == "inputTXID")
                    {
                        await jtr.ReadAsync().ConfigureAwait(false);
                        NewTx.InputTxID = jtr.Value.ToString();
                    }
                    else if (jtr.Value.ToString() == "inputAddress")
                    {
                        await jtr.ReadAsync().ConfigureAwait(false);
                        NewTx.InputAddress = jtr.Value.ToString();
                    }
                    else if (jtr.Value.ToString() == "inputCurrency")
                    {
                        await jtr.ReadAsync().ConfigureAwait(false);
                        NewTx.InputCoin = jtr.Value.ToString();
                    }
                    else if (jtr.Value.ToString() == "inputAmount")
                    {
                        await jtr.ReadAsync().ConfigureAwait(false);
                        NewTx.InputAmount = Convert.ToDouble(jtr.Value.ToString());
                    }
                    else if (jtr.Value.ToString() == "outputTXID")
                    {
                        await jtr.ReadAsync().ConfigureAwait(false);
                        NewTx.OutputTxID = jtr.Value.ToString();
                    }
                    else if (jtr.Value.ToString() == "outputAddress")
                    {
                        await jtr.ReadAsync().ConfigureAwait(false);
                        NewTx.OutputAddress = jtr.Value.ToString();
                    }
                    else if (jtr.Value.ToString() == "outputCurrency")
                    {
                        await jtr.ReadAsync().ConfigureAwait(false);
                        NewTx.OutputCoin = jtr.Value.ToString();
                    }
                    else if (jtr.Value.ToString() == "outputAmount")
                    {
                        await jtr.ReadAsync().ConfigureAwait(false);
                        NewTx.OutputAmount = Convert.ToDouble(jtr.Value.ToString());
                    }
                    else if (jtr.Value.ToString() == "shiftRate")
                    {
                        await jtr.ReadAsync().ConfigureAwait(false);
                        NewTx.ShiftRate = Convert.ToDouble(jtr.Value.ToString());
                    }
                    else if (jtr.Value.ToString() == "status")
                    {
                        await jtr.ReadAsync().ConfigureAwait(false);
                        NewTx.Status =
                            jtr.Value.ToString() == "received" ? TxStatuses.Received :
                            jtr.Value.ToString() == "complete" ? TxStatuses.Complete :
                            jtr.Value.ToString() == "returned" ? TxStatuses.Returned :
                            jtr.Value.ToString() == "failed" ? TxStatuses.Failed : TxStatuses.NoDeposits;
                    }
                    else continue;
                }
            }
            if (!string.IsNullOrEmpty(NewTx.InputTxID))
                TxList.Add(NewTx);

            return TxList;
        }
    }
}
