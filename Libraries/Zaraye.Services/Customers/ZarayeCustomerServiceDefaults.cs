using Zaraye.Core.Caching;
using Zaraye.Core.Domain.Customers;

namespace Zaraye.Services.Customers
{
    /// <summary>
    /// Represents default values related to customer services
    /// </summary>
    public static partial class ZarayeCustomerServicesDefaults
    {
        /// <summary>
        /// Gets a password salt key size
        /// </summary>
        public static int PasswordSaltKeySize => 5;

        /// <summary>
        /// Gets a max username length
        /// </summary>
        public static int CustomerUsernameLength => 100;

        /// <summary>
        /// Gets a default hash format for customer password
        /// </summary>
        public static string DefaultHashedPasswordFormat => "SHA512";

        /// <summary>
        /// Gets default prefix for customer
        /// </summary>
        public static string CustomerAttributePrefix => "customer_attribute_";

        #region Caching defaults

        #region Customer

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : system name
        /// </remarks>
        public static CacheKey CustomerBySystemNameCacheKey => new("Zaraye.customer.bysystemname.{0}");

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : customer GUID
        /// </remarks>
        public static CacheKey CustomerByGuidCacheKey => new("Zaraye.customer.byguid.{0}");

        #endregion

        #region Customer attributes

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : customer attribute ID
        /// </remarks>
        public static CacheKey CustomerAttributeValuesByAttributeCacheKey => new("Zaraye.customerattributevalue.byattribute.{0}");

        #endregion

        #region Customer roles

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : show hidden records?
        /// </remarks>
        public static CacheKey CustomerRolesAllCacheKey => new("Zaraye.customerrole.all.{0}-{1}", ZarayeEntityCacheDefaults<CustomerRole>.AllPrefix);

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : system name
        /// </remarks>
        public static CacheKey CustomerRolesBySystemNameCacheKey => new("Zaraye.customerrole.bysystemname.{0}", CustomerRolesBySystemNamePrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string CustomerRolesBySystemNamePrefix => "Zaraye.customerrole.bysystemname.";

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : customer identifier
        /// {1} : show hidden
        /// </remarks>
        public static CacheKey CustomerRoleIdsCacheKey => new("Zaraye.customer.customerrole.ids.{0}-{1}", CustomerCustomerRolesPrefix);

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : customer identifier
        /// {1} : show hidden
        /// </remarks>
        public static CacheKey CustomerRolesCacheKey => new("Zaraye.customer.customerrole.{0}-{1}", CustomerCustomerRolesByCustomerPrefix, CustomerCustomerRolesPrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string CustomerCustomerRolesPrefix => "Zaraye.customer.customerrole.";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : customer identifier
        /// </remarks>
        public static string CustomerCustomerRolesByCustomerPrefix => "Zaraye.customer.customerrole.{0}";
        
        #endregion

        #region Addresses

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : customer identifier
        /// </remarks>
        public static CacheKey CustomerAddressesCacheKey => new("Zaraye.customer.addresses.{0}", CustomerAddressesPrefix);

        /// <summary>
        /// Gets a key for caching
        /// </summary>
        /// <remarks>
        /// {0} : customer identifier
        /// {1} : address identifier
        /// </remarks>
        public static CacheKey CustomerAddressCacheKey => new("Zaraye.customer.addresses.{0}-{1}", CustomerAddressesByCustomerPrefix, CustomerAddressesPrefix);

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        public static string CustomerAddressesPrefix => "Zaraye.customer.addresses.";

        /// <summary>
        /// Gets a key pattern to clear cache
        /// </summary>
        /// <remarks>
        /// {0} : customer identifier
        /// </remarks>
        public static string CustomerAddressesByCustomerPrefix => "Zaraye.customer.addresses.{0}";
        
        #endregion

        #region Customer password

        /// <summary>
        /// Gets a key for caching current customer password lifetime
        /// </summary>
        /// <remarks>
        /// {0} : customer identifier
        /// </remarks>
        public static CacheKey CustomerPasswordLifetimeCacheKey => new("Zaraye.customerpassword.lifetime.{0}");

        #endregion

        #endregion

    }
}