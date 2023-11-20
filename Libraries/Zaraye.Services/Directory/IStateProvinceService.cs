using Zaraye.Core.Caching;
using Zaraye.Core.Domain.Common;
using Zaraye.Core.Domain.Directory;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Zaraye.Services.Directory
{
    /// <summary>
    /// State province service interface
    /// </summary>
    public partial interface IStateProvinceService
    {
        /// <summary>
        /// Deletes a state/province
        /// </summary>
        /// <param name="stateProvince">The state/province</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteStateProvinceAsync(StateProvince stateProvince);

        /// <summary>
        /// Gets a state/province
        /// </summary>
        /// <param name="stateProvinceId">The state/province identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the state/province
        /// </returns>
        Task<StateProvince> GetStateProvinceByIdAsync(int stateProvinceId);

        /// <summary>
        /// Gets a state/province by abbreviation
        /// </summary>
        /// <param name="abbreviation">The state/province abbreviation</param>
        /// <param name="countryId">Country identifier; pass null to load the state regardless of a country</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the state/province
        /// </returns>
        Task<StateProvince> GetStateProvinceByAbbreviationAsync(string abbreviation, int? countryId = null);

        /// <summary>
        /// Gets a state/province by address 
        /// </summary>
        /// <param name="address">Address</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the country
        /// </returns>
        Task<StateProvince> GetStateProvinceByAddressAsync(Address address);

        /// <summary>
        /// Gets a state/province collection by country identifier
        /// </summary>
        /// <param name="countryId">Country identifier</param>
        /// <param name="languageId">Language identifier. It's used to sort states by localized names (if specified); pass 0 to skip it</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the states
        /// </returns>
        Task<IList<StateProvince>> GetStateProvincesByCountryIdAsync(int countryId, int languageId = 0, bool showHidden = false);

        Task<IList<StateProvince>> GetPriceDiscoveryStateProvincesByCountryIdAsync(int countryId);

        /// <summary>
        /// Gets all states/provinces
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the states
        /// </returns>
        Task<IList<StateProvince>> GetStateProvincesAsync(bool showHidden = false);

        /// <summary>
        /// Inserts a state/province
        /// </summary>
        /// <param name="stateProvince">State/province</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertStateProvinceAsync(StateProvince stateProvince);

        /// <summary>
        /// Updates a state/province
        /// </summary>
        /// <param name="stateProvince">State/province</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UpdateStateProvinceAsync(StateProvince stateProvince);

        Task<IList<StateProvince>> GetAllAreasByCityIdAsync(int cityId = 0, bool showHidden = false);

        Task<StateProvince> GetStateProvinceByNameAsync(string name, int countryId);

        Task<StateProvince> GetAreaByNameAsync(string name, int cityId, int countryId);

        Task<IList<CityCombinationList>> GetAllCityCombinationsListAsync(int industryId = 0, int categoryId = 0, int userId = 0);
    }
}
