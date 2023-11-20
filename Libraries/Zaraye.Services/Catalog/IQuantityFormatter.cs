using System.Threading.Tasks;

namespace Zaraye.Services.Catalog
{
    public partial interface IQuantityFormatter
    {
        /// <summary>
        /// Formats the quantity
        /// </summary>
        /// <param name="quantity">Quantity</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the quantity
        /// </returns>
        string FormatQuantity(decimal quantity);

        /// <summary>
        /// Formats the quantity
        /// </summary>
        /// <param name="quantity">Quantity</param>
        /// <param name="showCurrency">A value indicating whether to show a currency</param>
        /// <param name="targetCurrency">Target currency</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the quantity
        /// </returns>
        Task<string> FormatQuantity(decimal quantity, int productId);

        /// <summary>
        /// Formats the quantity
        /// </summary>
        /// <param name="quantity">Quantity</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the quantity
        /// </returns>
        decimal FormatDecimalQuantity(decimal quantity);
    }
}
