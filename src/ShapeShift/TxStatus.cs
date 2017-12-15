using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Kalakoi.Crypto.ShapeShift
{
    /// <summary>
    /// Transaction Statuses.
    /// </summary>
    public enum TxStatuses { NoDeposits, Received, Returned, Complete, Failed }

    /// <summary>
    /// Provides access to transaction status.
    /// </summary>
    public class TxStatus
    {
        /*url: shapeshift.io/txStat/[address]
        method: GET
 
        [address] is the deposit address to look up.
 
        Success Output:  (various depending on status)
 
        Status: No Deposits Received
            {
                status:"no_deposits",
                address:[address]           //matches address submitted
            }
 
        Status: Received (we see a new deposit but have not finished processing it)
            {
                status:"received",
                address:[address]           //matches address submitted
            }
 
        Status: Complete
        {
            status : "complete",
            address: [address],
            withdraw: [withdrawal address],
            incomingCoin: [amount deposited],
            incomingType: [coin type of deposit],
            outgoingCoin: [amount sent to withdrawal address],
            outgoingType: [coin type of withdrawal],
            transaction: [transaction id of coin sent to withdrawal address]
        }
 
        Status: Failed
        {
            status : "failed",
            error: [Text describing failure]
        }
 
        //Note: this can still get the normal style error returned. For example if request is made without an address.
        */

        /// <summary>
        /// Status of exchange.
        /// </summary>
        public TxStatuses Status { get; private set; }
        /// <summary>
        /// Deposit address coins (need to be) sent to.
        /// </summary>
        public string Address { get; private set; }
        /// <summary>
        /// Address to send coins after exchange.
        /// </summary>
        public string WithdrawalAddress { get; private set; }
        /// <summary>
        /// Amount to send to deposit address.
        /// </summary>
        public double IncomingAmount { get; private set; }
        /// <summary>
        /// Currency type to send.
        /// </summary>
        public string IncomingCoin { get; private set; }
        /// <summary>
        /// Amount (to be) received from exchange.
        /// </summary>
        public double OutgoingAmount { get; private set; }
        /// <summary>
        /// Currency type to receive.
        /// </summary>
        public string OutgoingCoin { get; private set; }
        /// <summary>
        /// Transaction ID of coin sent to withdrawal address.
        /// </summary>
        public string TxID { get; private set; }
        /// <summary>
        /// Error thrown by status check, if any.
        /// </summary>
        public string Error { get; private set; }

        private TxStatus() { }

        /// <summary>
        /// Gets status of transaction (to be) deposited to supplied address.
        /// </summary>
        /// <param name="Address">Deposit address.</param>
        /// <returns>Transaction status.</returns>
        internal static async Task<TxStatus> GetStatusAsync(string Address)
        {
            Uri uri = GetUri(Address);
            string response = await RestServices.GetResponseAsync(uri).ConfigureAwait(false);
            return await ParseResponseAsync(response).ConfigureAwait(false);
        }

        private static Uri GetUri(string address) =>
            new Uri(string.Format(@"https://shapeshift.io/txStat/{0}", address));

        private static async Task<TxStatus> ParseResponseAsync(string response)
        {
            TxStatus status = new TxStatus();
            using (JsonTextReader jtr = new JsonTextReader(new StringReader(response)))
            {
                while (await jtr.ReadAsync().ConfigureAwait(false))
                {
                    if (jtr.Value == null) continue;
                    else if (jtr.Value.ToString() == "status")
                    {
                        await jtr.ReadAsync().ConfigureAwait(false);
                        status.Status =
                            jtr.Value.ToString() == "no_deposits" ? TxStatuses.NoDeposits :
                            jtr.Value.ToString() == "received" ? TxStatuses.Received :
                            jtr.Value.ToString() == "complete" ? TxStatuses.Complete :
                            jtr.Value.ToString() == "failed" ? TxStatuses.Failed : TxStatuses.Returned;
                    }
                    else if (jtr.Value.ToString() == "address")
                    {
                        await jtr.ReadAsync().ConfigureAwait(false);
                        status.Address = jtr.Value.ToString();
                    }
                    else if (jtr.Value.ToString() == "withdraw")
                    {
                        await jtr.ReadAsync().ConfigureAwait(false);
                        status.WithdrawalAddress = jtr.Value.ToString();
                    }
                    else if (jtr.Value.ToString() == "incomingCoin")
                    {
                        await jtr.ReadAsync().ConfigureAwait(false);
                        status.IncomingAmount = Convert.ToDouble(jtr.Value.ToString());
                    }
                    else if (jtr.Value.ToString() == "incomingType")
                    {
                        await jtr.ReadAsync().ConfigureAwait(false);
                        status.IncomingCoin = jtr.Value.ToString();
                    }
                    else if (jtr.Value.ToString() == "outgoingCoin")
                    {
                        await jtr.ReadAsync().ConfigureAwait(false);
                        status.OutgoingAmount = Convert.ToDouble(jtr.Value.ToString());
                    }
                    else if (jtr.Value.ToString() == "outgoingType")
                    {
                        await jtr.ReadAsync().ConfigureAwait(false);
                        status.OutgoingCoin = jtr.Value.ToString();
                    }
                    else if (jtr.Value.ToString() == "transaction")
                    {
                        await jtr.ReadAsync().ConfigureAwait(false);
                        status.TxID = jtr.Value.ToString();
                    }
                    else if (jtr.Value.ToString() == "error")
                    {
                        await jtr.ReadAsync().ConfigureAwait(false);
                        status.Error = jtr.Value.ToString();
                    }
                    else continue;
                }
            }
            return status;
        }
    }
}
