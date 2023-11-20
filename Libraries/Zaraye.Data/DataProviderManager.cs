using Zaraye.Core;
using Zaraye.Core.Infrastructure;
using Zaraye.Data.Configuration;
using Zaraye.Data.DataProviders;

namespace Zaraye.Data
{
    /// <summary>
    /// Represents the data provider manager
    /// </summary>
    public partial class DataProviderManager : IDataProviderManager
    {
        #region Methods

        /// <summary>
        /// Gets data provider by specific type
        /// </summary>
        /// <param name="dataProviderType">Data provider type</param>
        /// <returns></returns>
        public static IZarayeDataProvider GetDataProvider(DataProviderType dataProviderType)
        {
            return dataProviderType switch
            {
                DataProviderType.SqlServer => new MsSqlZarayeDataProvider(),
                DataProviderType.MySql => new MySqlZarayeDataProvider(),
                DataProviderType.PostgreSQL => new PostgreSqlDataProvider(),
                _ => throw new ZarayeException($"Not supported data provider name: '{dataProviderType}'"),
            };
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets data provider
        /// </summary>
        public IZarayeDataProvider DataProvider
        {
            get
            {
                var dataProviderType = Singleton<DataConfig>.Instance.DataProvider;

                return GetDataProvider(dataProviderType);
            }
        }

        #endregion
    }
}
