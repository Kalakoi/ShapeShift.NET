using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kalakoi.Crypto.ShapeShift
{
    /// <summary>
    /// Provides access to verified currency pairs available for exchange.
    /// </summary>
    public class TradingPair
    {
        /// <summary>
        /// The Pair string used by ShapeShift.
        /// </summary>
        public string Pair { get; private set; }
        /// <summary>
        /// The Ticker of the currency to be converted.
        /// </summary>
        public string Ticker1 { get; private set; }
        /// <summary>
        /// The Ticker of the currency to convert to.
        /// </summary>
        public string Ticker2 { get; private set; }

        private TradingPair() { }

        /// <summary>
        /// Generates a list of all TradingPairs supported by ShapeShift.
        /// </summary>
        /// <returns>A List of TradingPairs.</returns>
        internal static async Task<List<TradingPair>> GetAllPairsAsync()
        {
            //Initialize an empty List of TradingPairs
            List<TradingPair> PairList = new List<TradingPair>();
            //Generate a List of all SupportedCoins
            List<SupportedCoin> CoinList = await SupportedCoin.GetCoinsAsync().ConfigureAwait(false);
            //Loop through all SupportedCoins
            foreach (SupportedCoin Coin1 in CoinList)
            {
                //Check if coin is available for Shifting
                if (Coin1.Status == CoinStatuses.Available)
                {
                    //Loop through all SupportedCoins again for generating all TradingPairs
                    foreach (SupportedCoin Coin2 in CoinList)
                    {
                        //Check if both coins are not the same and both coins are available to Shift
                        if (Coin1 != Coin2 && Coin2.Status == CoinStatuses.Available)
                        {
                            //Create new TradingPair object and assign values
                            TradingPair NewPair = new TradingPair();
                            NewPair.Ticker1 = Coin1.Symbol;
                            NewPair.Ticker2 = Coin2.Symbol;
                            NewPair.Pair = string.Format("{0}_{1}", Coin1.Symbol, Coin2.Symbol);
                            //Add new TradingPair object to List of TradingPairs
                            PairList.Add(NewPair);
                        }
                        else continue;
                    }
                }
                else continue;
            }
            //Return complete TradingPair List
            return PairList;
        }
    }
}
