using Zaraye.Core.Domain.Customers;
using Zaraye.Core.Domain.Directory;
using Zaraye.Core.Domain.Localization;
using Zaraye.Core.Domain.Tax;
using System.Threading.Tasks;

namespace Zaraye.Core
{
    /// <summary>
    /// Represents work context
    /// </summary>
    public interface IWorkContext
    {
        /// <summary>
        /// Gets the current customer
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task<Customer> GetCurrentCustomerAsync();

        /// <summary>
        /// Sets the current customer
        /// </summary>
        /// <param name="customer">Current customer</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task SetCurrentCustomerAsync(Customer customer = null);

        /// <summary>
        /// Gets the original customer (in case the current one is impersonated)
        /// </summary>
        Customer OriginalCustomerIfImpersonated { get; }

        /// <summary>
        /// Gets current user working language
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task<Language> GetWorkingLanguageAsync();

        /// <summary>
        /// Sets current user working language
        /// </summary>
        /// <param name="language">Language</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task SetWorkingLanguageAsync(Language language);

        /// <summary>
        /// Gets or sets current user working currency
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task<Currency> GetWorkingCurrencyAsync();

        /// <summary>
        /// Sets current user working currency
        /// </summary>
        /// <param name="currency">Currency</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task SetWorkingCurrencyAsync(Currency currency);

        /// <summary>
        /// Gets or sets current tax display type
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task<TaxDisplayType> GetTaxDisplayTypeAsync();

        /// <summary>
        /// Sets current tax display type
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task SetTaxDisplayTypeAsync(TaxDisplayType taxDisplayType);        
    }
}
