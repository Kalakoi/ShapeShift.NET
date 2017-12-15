using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Kalakoi.Crypto.ShapeShift
{
    /// <summary>
    /// Provides easy access to GET and POST functionality for REST services.
    /// </summary>
    internal static class RestServices
    {
        /// <summary>
        /// Sends GET request to REST service and returns response.
        /// </summary>
        /// <param name="uri">Address to send request to.</param>
        /// <returns>JSON data as string.</returns>
        internal static async Task<string> GetResponseAsync(Uri uri)
        {
            //Create client to send GET request
            using (HttpClient client = new HttpClient())
            {
                //Add request headers
                client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome / 58.0.3029.110 Safari / 537.36");
                //Send request and await response
                string response = await client.GetStringAsync(uri).ConfigureAwait(false);
                return response;
            }
        }

        /// <summary>
        /// Sends POST request to REST service with JSON data and returns response.
        /// </summary>
        /// <param name="uri">Web address to send request.</param>
        /// <param name="Data">JSON data to send with request.</param>
        /// <returns>JSON data as string.</returns>
        internal static async Task<string> GetPostResponseAsync(Uri uri, string Data)
        {
            //Create Client to send and receive data from REST service
            using (HttpClient client = new HttpClient())
            {
                //Add headers for request
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome / 58.0.3029.110 Safari / 537.36");
                client.DefaultRequestHeaders.Host = uri.Host;
                //Create content to send from JSON string
                HttpContent content = new StringContent(Data, Encoding.UTF8, @"application/json");
                //Send POST request and await response
                HttpResponseMessage response = await client.PostAsync(uri, content).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                //Return response JSON as string
                return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            }
        }
    }
}
