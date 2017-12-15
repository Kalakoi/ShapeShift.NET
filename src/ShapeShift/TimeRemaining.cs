using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Kalakoi.Crypto.ShapeShift
{
    /// <summary>
    /// Statuses available for Time Remaining requests.
    /// </summary>
    public enum TimeStatuses { Pending, Expired }

    /// <summary>
    /// Provides access to information on time remaining for pending deposits.
    /// </summary>
    public class TimeRemaining
    {
        /*url: shapeshift.io/timeremaining/[address]
        method: GET
 
        [address] is the deposit address to look up.
 
        Success Output:
 
            {
                status:"pending",
                seconds_remaining: 600
            }
 
        The status can be either "pending" or "expired".
        If the status is expired then seconds_remaining will show 0.
        */

        /// <summary>
        /// Status of deposit.
        /// </summary>
        public TimeStatuses Status { get; private set; }
        /// <summary>
        /// Seconds remaining before transaction expires.
        /// </summary>
        public double SecondsRemaining { get; private set; }

        private TimeRemaining() { }

        /// <summary>
        /// Gets status of deposit and time remaining to complete deposit before expiration.
        /// </summary>
        /// <param name="Address">The deposit address to look up.</param>
        /// <returns>Time remaining for deposit.</returns>
        internal static async Task<TimeRemaining> GetTimeRemainingAsync(string Address)
        {
            Uri uri = GetUri(Address);
            string response = await RestServices.GetResponseAsync(uri).ConfigureAwait(false);
            return await ParseResponseAsync(response).ConfigureAwait(false);
        }

        private static Uri GetUri(string Address) =>
            new Uri(string.Format(@"https://shapeshift.io/timeremaining/{0}", Address));

        private static async Task<TimeRemaining> ParseResponseAsync(string response)
        {
            TimeRemaining tr = new TimeRemaining();
            using (JsonTextReader jtr = new JsonTextReader(new StringReader(response)))
            {
                while (await jtr.ReadAsync().ConfigureAwait(false))
                {
                    if (jtr.Value == null) continue;
                    else if (jtr.Value.ToString() == "status")
                    {
                        await jtr.ReadAsync().ConfigureAwait(false);
                        tr.Status = jtr.Value.ToString() == "pending" ? TimeStatuses.Pending : TimeStatuses.Expired;
                    }
                    else if (jtr.Value.ToString() == "seconds_remaining")
                    {
                        await jtr.ReadAsync().ConfigureAwait(false);
                        tr.SecondsRemaining = Convert.ToDouble(jtr.Value.ToString());
                    }
                    else continue;
                }
            }
            return tr;
        }
    }
}
