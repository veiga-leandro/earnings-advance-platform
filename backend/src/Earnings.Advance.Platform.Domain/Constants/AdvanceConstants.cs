namespace Earnings.Advance.Platform.Domain.Constants
{
    /// <summary>
    /// Global business constants for advance operations
    /// </summary>
    public static class AdvanceConstants
    {
        /// <summary>
        /// Fee rate applied to advance requests (5%)
        /// </summary>
        public const decimal FEE_RATE = 0.05m;

        /// <summary>
        /// Minimum amount allowed for advance requests (R$ 100.00)
        /// </summary>
        public const decimal MINIMUM_AMOUNT = 100.00m;

        /// <summary>
        /// Currency symbol for displaying amounts
        /// </summary>
        public const string CURRENCY_SYMBOL = "R$";

        /// <summary>
        /// Formats amount with currency symbol
        /// </summary>
        public static string FormatAmount(decimal amount) =>
            $"{CURRENCY_SYMBOL} {amount:F2}";
    }
}
