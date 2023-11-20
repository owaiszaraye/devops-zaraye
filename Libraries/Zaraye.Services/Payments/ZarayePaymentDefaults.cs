namespace Zaraye.Services.Payments
{
    /// <summary>
    /// Represents default values related to payment services
    /// </summary>
    public static partial class ZarayePaymentDefaults
    {
        /// <summary>
        /// Gets a setting name to store countries in which a payment method is not allowed
        /// </summary>
        /// <remarks>
        /// {0} : payment method name
        /// </remarks>
        public static string RestrictedCountriesSettingName => "PaymentMethodRestictions.{0}";
    }
}