using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kalakoi.Crypto.ShapeShift
{
    /// <summary>
    /// Provides access the the ShapeShift service.
    /// </summary>
    public static class ShapeShift
    {
        /// <summary>
        /// Attempts to cancel pending exchange
        /// </summary>
        /// <param name="Address">The deposit address associated with the pending transaction.</param>
        /// <returns>Result of cancel operation.</returns>
        public static async Task<CancelResult> CancelExchangeAsync(string Address) => 
            await CancelResult.CancelAsync(Address).ConfigureAwait(false);

        /// <summary>
        /// Requests a receipt for transaction to be sent via email.
        /// </summary>
        /// <param name="Email">Email address to send receipt to.</param>
        /// <param name="TxID">Transaction ID of the transaction sent to the user.</param>
        /// <returns>Result of receipt request.</returns>
        public static async Task<EmailReceipt> RequestEmailReceiptAsync(string Email, string TxID) => 
            await EmailReceipt.RequestAsync(Email, TxID).ConfigureAwait(false);

        /// <summary>
        /// Requests a quote for an exchange without exchanging.
        /// </summary>
        /// <param name="Pair">Coin pair to exchange between.</param>
        /// <param name="Amount">Amount of coin to be sent to withdrawal address.</param>
        /// <returns>Quote for exchange information.</returns>
        public static async Task<QuoteRequest> RequestQuoteAsync(string Pair, double Amount) =>
            await QuoteRequest.RequestAsync(Pair, Amount).ConfigureAwait(false);

        /// <summary>
        /// Gets information on recent transactions completed by ShapeShift.
        /// </summary>
        /// <param name="Max">Maximum number of transactions to return. Must be betweeen 1 and 50, inclusive.</param>
        /// <returns>List of recent transactions.</returns>
        public static async Task<List<RecentTx>> GetRecentTransactionsAsync(int Max = 5) =>
            await RecentTx.GetRecentTransactionsAsync(Max).ConfigureAwait(false);

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
        public static async Task<SendAmountRequest> GetSendAmountAsync(double Amount, string Address, string Pair, string ReturnAddress = "", string RippleTag = "", string NXTRsAddress = "", string APIKey = "") =>
            await SendAmountRequest.RequestAsync(Amount, Address, Pair, ReturnAddress, RippleTag, NXTRsAddress, APIKey).ConfigureAwait(false);

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
        public static async Task<ShiftResult> ShiftAsync(string Withdrawal, string Pair, string Return = "", string RippleTag = "", string NXTRsAddress = "", string APIKey = "") =>
            await ShiftResult.ShiftAsync(Withdrawal, Pair, Return, RippleTag, NXTRsAddress, APIKey).ConfigureAwait(false);

        /// <summary>
        /// Provides information on a specific currency supported by ShapeShift.
        /// </summary>
        /// <param name="Symbol">Ticker symbol of currency.</param>
        /// <returns>Information on specific supported currency.</returns>
        public static async Task<SupportedCoin> GetCoinAsync(string Symbol) =>
            await SupportedCoin.GetCoinAsync(Symbol).ConfigureAwait(false);

        /// <summary>
        /// Provides information on all currencies supported by ShapeShift.
        /// </summary>
        /// <returns>List of all supported currencies.</returns>
        public static async Task<List<SupportedCoin>> GetAllCoinsAsync() =>
            await SupportedCoin.GetCoinsAsync().ConfigureAwait(false);

        /// <summary>
        /// Gets status of deposit and time remaining to complete deposit before expiration.
        /// </summary>
        /// <param name="Address">The deposit address to look up.</param>
        /// <returns>Time remaining for deposit.</returns>
        public static async Task<TimeRemaining> CheckTimeRemainingAsync(string Address) =>
            await TimeRemaining.GetTimeRemainingAsync(Address).ConfigureAwait(false);

        /// <summary>
        /// Gets trade limit for specified currency pair.
        /// </summary>
        /// <param name="Pair">Currency pair to exchange.</param>
        /// <returns>Trading limit information.</returns>
        public static async Task<TradingLimit> GetTradeLimitAsync(string Pair) =>
            await TradingLimit.GetLimitAsync(Pair).ConfigureAwait(false);

        /// <summary>
        /// Gets trade limit for specified currency pair.
        /// </summary>
        /// <param name="Ticker1">Ticker for currency to exchange from.</param>
        /// <param name="Ticker2">Ticker for currency to exchange to.</param>
        /// <returns>Trading limit information.</returns>
        public static async Task<TradingLimit> GetTradeLimitAsync(string Ticker1, string Ticker2) =>
            await TradingLimit.GetLimitAsync(Ticker1, Ticker2).ConfigureAwait(false);

        /// <summary>
        /// Gets list of all trade limits.
        /// </summary>
        /// <returns>List of all trade limits.</returns>
        public static async Task<List<TradingLimit>> GetAllTradeLimitsAsync() =>
            await TradingLimit.GetAllLimitsAsync().ConfigureAwait(false);

        /// <summary>
        /// Gets market info for specific currency pair.
        /// </summary>
        /// <param name="Pair">Pair to get information for.</param>
        /// <returns>Market Information.</returns>
        public static async Task<TradingMarketInfo> GetMarketInfoAsync(string Pair) =>
            await TradingMarketInfo.GetMarketInfoAsync(Pair).ConfigureAwait(false);

        /// <summary>
        /// Gets market info for specific currency pair.
        /// </summary>
        /// <param name="Ticker1">Ticker for currency to exchange from.</param>
        /// <param name="Ticker2">Ticker for currency to exchange to.</param>
        /// <returns>Market Information.</returns>
        public static async Task<TradingMarketInfo> GetMarketInfoAsync(string Ticker1, string Ticker2) =>
            await TradingMarketInfo.GetMarketInfoAsync(Ticker1, Ticker2).ConfigureAwait(false);

        /// <summary>
        /// Gets market info for all currency pairs.
        /// </summary>
        /// <returns>List of Market Information.</returns>
        public static async Task<List<TradingMarketInfo>> GetAllMarketInfosAsync() =>
            await TradingMarketInfo.GetAllMarketsAsync().ConfigureAwait(false);

        /// <summary>
        /// Generates a list of all TradingPairs supported by ShapeShift.
        /// </summary>
        /// <returns>A List of TradingPairs.</returns>
        public static async Task<List<TradingPair>> GetAllPairsAsync() =>
            await TradingPair.GetAllPairsAsync().ConfigureAwait(false);

        /// <summary>
        /// Finds exchange rate for specified coin pair.
        /// </summary>
        /// <param name="Pair">Coin pair to find rate for.</param>
        /// <returns>Exchange rate.</returns>
        public static async Task<TradingRate> GetExchangeRateAsync(string Pair) =>
            await TradingRate.GetRateAsync(Pair).ConfigureAwait(false);

        /// <summary>
        /// Finds exchange rate for specified coin pair.
        /// </summary>
        /// <param name="Ticker1">Ticker symbol for coin to exchange.</param>
        /// <param name="Ticker2">Ticker symbol for resulting coin.</param>
        /// <returns>Exchange rate.</returns>
        public static async Task<TradingRate> GetExchangeRateAsync(string Ticker1, string Ticker2) =>
            await TradingRate.GetRateAsync(Ticker1, Ticker2).ConfigureAwait(false);

        /// <summary>
        /// Finds exchange rates for all valid coin pairs.
        /// </summary>
        /// <returns>List of exchange rates.</returns>
        public static async Task<List<TradingRate>> GetAllExchangeRatesAsync() =>
            await TradingRate.GetAllRatesAsync().ConfigureAwait(false);

        /// <summary>
        /// Finds all transactions sent using the specified API key.
        /// </summary>
        /// <param name="APIKey">The affiliate's PRIVATE api key.</param>
        /// <returns>List of transactions.</returns>
        public static async Task<List<Tx>> GetTransactionsAsync(string APIKey) =>
            await Tx.GetTransactionsByAPIKeyAsync(APIKey).ConfigureAwait(false);

        /// <summary>
        /// Finds all transactions sent to specified address.
        /// </summary>
        /// <param name="Address">The address that output coin was sent to for the shift.</param>
        /// <param name="APIKey">The affiliate's PRIVATE api key.</param>
        /// <returns>List of transactions.</returns>
        public static async Task<List<Tx>> GetTransactionsAsync(string Address, string APIKey) =>
            await Tx.GetTransactionsByAddressAsync(Address, APIKey).ConfigureAwait(false);

        /// <summary>
        /// Gets status of transaction (to be) deposited to supplied address.
        /// </summary>
        /// <param name="Address">Deposit address.</param>
        /// <returns>Transaction status.</returns>
        public static async Task<TxStatus> GetTransactionStatusAsync(string Address) =>
            await TxStatus.GetStatusAsync(Address).ConfigureAwait(false);

        /// <summary>
        /// Validates address belongs to specified currency.
        /// </summary>
        /// <param name="Address">Address to check.</param>
        /// <param name="Symbol">Ticker symbol for currency to check.</param>
        /// <returns>Validation results.</returns>
        public static async Task<ValidateAddress> ValidateAddressAsync(string Address, string Symbol) =>
            await ValidateAddress.ValidateAsync(Address, Symbol).ConfigureAwait(false);
    }
}
