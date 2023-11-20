using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using MailKit.Search;
using Zaraye.Core;
using Zaraye.Core.Caching;
using Zaraye.Core.Configuration;
using Zaraye.Core.Domain.Catalog;
using Zaraye.Core.Domain.Configuration;
using Zaraye.Core.Domain.Orders;
using Zaraye.Core.Domain.Shipping;
using Zaraye.Data;

namespace Zaraye.Services.Configuration
{
    /// <summary>
    /// Setting manager
    /// </summary>
    public partial class SettingService : ISettingService
    {
        #region Fields

        private readonly IRepository<Setting> _settingRepository;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IRepository<UserType> _userTypeRepository;
        private readonly IRepository<ShipmentReturnReason> _shipmentReturnReasonRepository;
        private readonly IRepository<ReturnRequestReason> _returnRequestReasonRepository;
        private readonly IRepository<DeliveryCostReason> _deliveryCostReasonRepository;
        private readonly IRepository<DeliveryTimeReason> _deliveryTimeReasonRepository;

        #endregion

        #region Ctor

        public SettingService(IRepository<Setting> settingRepository,
            IStaticCacheManager staticCacheManager,
            IRepository<UserType> userTypeRepository,
            IRepository<ShipmentReturnReason> shipmentReturnReasonRepository,
            IRepository<DeliveryCostReason> deliveryCostReasonRepository,
            IRepository<DeliveryTimeReason> deliveryTimeReasonRepository,
            IRepository<ReturnRequestReason> returnRequestReasonRepository
            )
        {
            _settingRepository = settingRepository;
            _staticCacheManager = staticCacheManager;
            _userTypeRepository = userTypeRepository;
            _shipmentReturnReasonRepository = shipmentReturnReasonRepository;
            _deliveryCostReasonRepository = deliveryCostReasonRepository;
            _deliveryTimeReasonRepository = deliveryTimeReasonRepository;
            _returnRequestReasonRepository = returnRequestReasonRepository;

        }

        #endregion

        #region Utilities

        /// <summary>
        /// Gets all settings
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the settings
        /// </returns>
        protected virtual async Task<IDictionary<string, IList<Setting>>> GetAllSettingsDictionaryAsync()
        {
            return await _staticCacheManager.GetAsync(ZarayeSettingsDefaults.SettingsAllAsDictionaryCacheKey, async () =>
            {
                var settings = await GetAllSettingsAsync();

                var dictionary = new Dictionary<string, IList<Setting>>();
                foreach (var s in settings)
                {
                    var resourceName = s.Name.ToLowerInvariant();
                    var settingForCaching = new Setting
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Value = s.Value,
                        StoreId = s.StoreId
                    };
                    if (!dictionary.ContainsKey(resourceName))
                        //first setting
                        dictionary.Add(resourceName, new List<Setting>
                        {
                            settingForCaching
                        });
                    else
                        //already added
                        //most probably it's the setting with the same name but for some certain store (storeId > 0)
                        dictionary[resourceName].Add(settingForCaching);
                }

                return dictionary;
            });
        }

        /// <summary>
        /// Gets all settings
        /// </summary>
        /// <returns>
        /// Settings
        /// </returns>
        protected virtual IDictionary<string, IList<Setting>> GetAllSettingsDictionary()
        {
            return _staticCacheManager.Get(ZarayeSettingsDefaults.SettingsAllAsDictionaryCacheKey, () =>
            {
                var settings = GetAllSettings();

                var dictionary = new Dictionary<string, IList<Setting>>();
                foreach (var s in settings)
                {
                    var resourceName = s.Name.ToLowerInvariant();
                    var settingForCaching = new Setting
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Value = s.Value,
                        StoreId = s.StoreId
                    };
                    if (!dictionary.ContainsKey(resourceName))
                        //first setting
                        dictionary.Add(resourceName, new List<Setting>
                        {
                            settingForCaching
                        });
                    else
                        //already added
                        //most probably it's the setting with the same name but for some certain store (storeId > 0)
                        dictionary[resourceName].Add(settingForCaching);
                }

                return dictionary;
            });
        }

        /// <summary>
        /// Set setting value
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task SetSettingAsync(Type type, string key, object value, int storeId = 0, bool clearCache = true)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            key = key.Trim().ToLowerInvariant();
            var valueStr = TypeDescriptor.GetConverter(type).ConvertToInvariantString(value);

            var allSettings = await GetAllSettingsDictionaryAsync();
            var settingForCaching = allSettings.ContainsKey(key) ?
                allSettings[key].FirstOrDefault(x => x.StoreId == storeId) : null;
            if (settingForCaching != null)
            {
                //update
                var setting = await GetSettingByIdAsync(settingForCaching.Id);
                setting.Value = valueStr;
                await UpdateSettingAsync(setting, clearCache);
            }
            else
            {
                //insert
                var setting = new Setting
                {
                    Name = key,
                    Value = valueStr,
                    StoreId = storeId
                };
                await InsertSettingAsync(setting, clearCache);
            }
        }

        /// <summary>
        /// Set setting value
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
        protected virtual void SetSetting(Type type, string key, object value, int storeId = 0, bool clearCache = true)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            key = key.Trim().ToLowerInvariant();
            var valueStr = TypeDescriptor.GetConverter(type).ConvertToInvariantString(value);

            var allSettings = GetAllSettingsDictionary();
            var settingForCaching = allSettings.ContainsKey(key) ?
                allSettings[key].FirstOrDefault(x => x.StoreId == storeId) : null;
            if (settingForCaching != null)
            {
                //update
                var setting = GetSettingById(settingForCaching.Id);
                setting.Value = valueStr;
                UpdateSetting(setting, clearCache);
            }
            else
            {
                //insert
                var setting = new Setting
                {
                    Name = key,
                    Value = valueStr,
                    StoreId = storeId
                };
                InsertSetting(setting, clearCache);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds a setting
        /// </summary>
        /// <param name="setting">Setting</param>
        /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertSettingAsync(Setting setting, bool clearCache = true)
        {
            await _settingRepository.InsertAsync(setting);

            //cache
            if (clearCache)
                await ClearCacheAsync();
        }

        /// <summary>
        /// Adds a setting
        /// </summary>
        /// <param name="setting">Setting</param>
        /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
        public virtual void InsertSetting(Setting setting, bool clearCache = true)
        {
            _settingRepository.Insert(setting);

            //cache
            if (clearCache)
                ClearCache();
        }

        /// <summary>
        /// Updates a setting
        /// </summary>
        /// <param name="setting">Setting</param>
        /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateSettingAsync(Setting setting, bool clearCache = true)
        {
            if (setting == null)
                throw new ArgumentNullException(nameof(setting));

            await _settingRepository.UpdateAsync(setting);

            //cache
            if (clearCache)
                await ClearCacheAsync();
        }

        /// <summary>
        /// Updates a setting
        /// </summary>
        /// <param name="setting">Setting</param>
        /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
        public virtual void UpdateSetting(Setting setting, bool clearCache = true)
        {
            if (setting == null)
                throw new ArgumentNullException(nameof(setting));

            _settingRepository.Update(setting);

            //cache
            if (clearCache)
                ClearCache();
        }

        /// <summary>
        /// Deletes a setting
        /// </summary>
        /// <param name="setting">Setting</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteSettingAsync(Setting setting)
        {
            await _settingRepository.DeleteAsync(setting);

            //cache
            await ClearCacheAsync();
        }

        /// <summary>
        /// Deletes a setting
        /// </summary>
        /// <param name="setting">Setting</param>
        public virtual void DeleteSetting(Setting setting)
        {
            _settingRepository.Delete(setting);

            //cache
            ClearCache();
        }

        /// <summary>
        /// Deletes settings
        /// </summary>
        /// <param name="settings">Settings</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteSettingsAsync(IList<Setting> settings)
        {
            await _settingRepository.DeleteAsync(settings);

            //cache
            await ClearCacheAsync();
        }

        /// <summary>
        /// Gets a setting by identifier
        /// </summary>
        /// <param name="settingId">Setting identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the setting
        /// </returns>
        public virtual async Task<Setting> GetSettingByIdAsync(int settingId)
        {
            return await _settingRepository.GetByIdAsync(settingId, cache => default);
        }

        /// <summary>
        /// Gets a setting by identifier
        /// </summary>
        /// <param name="settingId">Setting identifier</param>
        /// <returns>
        /// The setting
        /// </returns>
        public virtual Setting GetSettingById(int settingId)
        {
            return _settingRepository.GetById(settingId, cache => default);
        }

        /// <summary>
        /// Get setting by key
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="loadSharedValueIfNotFound">A value indicating whether a shared (for all stores) value should be loaded if a value specific for a certain is not found</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the setting
        /// </returns>
        public virtual async Task<Setting> GetSettingAsync(string key, int storeId = 0, bool loadSharedValueIfNotFound = false)
        {
            if (string.IsNullOrEmpty(key))
                return null;

            var settings = await GetAllSettingsDictionaryAsync();
            key = key.Trim().ToLowerInvariant();
            if (!settings.ContainsKey(key)) 
                return null;

            var settingsByKey = settings[key];
            var setting = settingsByKey.FirstOrDefault(x => x.StoreId == storeId);

            //load shared value?
            if (setting == null && storeId > 0 && loadSharedValueIfNotFound)
                setting = settingsByKey.FirstOrDefault(x => x.StoreId == 0);

            return setting != null ? await GetSettingByIdAsync(setting.Id) : null;
        }

        /// <summary>
        /// Get setting by key
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="loadSharedValueIfNotFound">A value indicating whether a shared (for all stores) value should be loaded if a value specific for a certain is not found</param>
        /// <returns>
        /// The setting
        /// </returns>
        public virtual Setting GetSetting(string key, int storeId = 0, bool loadSharedValueIfNotFound = false)
        {
            if (string.IsNullOrEmpty(key))
                return null;

            var settings = GetAllSettingsDictionary();
            key = key.Trim().ToLowerInvariant();
            if (!settings.ContainsKey(key))
                return null;

            var settingsByKey = settings[key];
            var setting = settingsByKey.FirstOrDefault(x => x.StoreId == storeId);

            //load shared value?
            if (setting == null && storeId > 0 && loadSharedValueIfNotFound)
                setting = settingsByKey.FirstOrDefault(x => x.StoreId == 0);

            return setting != null ? GetSettingById(setting.Id) : null;
        }

        /// <summary>
        /// Get setting value by key
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">Key</param>
        /// <param name="defaultValue">Default value</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="loadSharedValueIfNotFound">A value indicating whether a shared (for all stores) value should be loaded if a value specific for a certain is not found</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the setting value
        /// </returns>
        public virtual async Task<T> GetSettingByKeyAsync<T>(string key, T defaultValue = default,
            int storeId = 0, bool loadSharedValueIfNotFound = false)
        {
            if (string.IsNullOrEmpty(key))
                return defaultValue;

            var settings = await GetAllSettingsDictionaryAsync();
            key = key.Trim().ToLowerInvariant();
            if (!settings.ContainsKey(key)) 
                return defaultValue;

            var settingsByKey = settings[key];
            var setting = settingsByKey.FirstOrDefault(x => x.StoreId == storeId);

            //load shared value?
            if (setting == null && storeId > 0 && loadSharedValueIfNotFound)
                setting = settingsByKey.FirstOrDefault(x => x.StoreId == 0);

            return setting != null ? CommonHelper.To<T>(setting.Value) : defaultValue;
        }

        /// <summary>
        /// Get setting value by key
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">Key</param>
        /// <param name="defaultValue">Default value</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="loadSharedValueIfNotFound">A value indicating whether a shared (for all stores) value should be loaded if a value specific for a certain is not found</param>
        /// <returns>
        /// Setting value
        /// </returns>
        public virtual T GetSettingByKey<T>(string key, T defaultValue = default,
            int storeId = 0, bool loadSharedValueIfNotFound = false)
        {
            if (string.IsNullOrEmpty(key))
                return defaultValue;

            var settings = GetAllSettingsDictionary();
            key = key.Trim().ToLowerInvariant();
            if (!settings.ContainsKey(key))
                return defaultValue;

            var settingsByKey = settings[key];
            var setting = settingsByKey.FirstOrDefault(x => x.StoreId == storeId);

            //load shared value?
            if (setting == null && storeId > 0 && loadSharedValueIfNotFound)
                setting = settingsByKey.FirstOrDefault(x => x.StoreId == 0);

            return setting != null ? CommonHelper.To<T>(setting.Value) : defaultValue;
        }

        /// <summary>
        /// Set setting value
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task SetSettingAsync<T>(string key, T value, int storeId = 0, bool clearCache = true)
        {
            await SetSettingAsync(typeof(T), key, value, storeId, clearCache);
        }

        /// <summary>
        /// Set setting value
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
        public virtual void SetSetting<T>(string key, T value, int storeId = 0, bool clearCache = true)
        {
            SetSetting(typeof(T), key, value, storeId, clearCache);
        }

        /// <summary>
        /// Gets all settings
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the settings
        /// </returns>
        public virtual async Task<IList<Setting>> GetAllSettingsAsync()
        {
            var settings = await _settingRepository.GetAllAsync(query =>
            {
                return from s in query
                       orderby s.Name, s.StoreId
                    select s;
            }, cache => default);
            
            return settings;
        }

        /// <summary>
        /// Gets all settings
        /// </summary>
        /// <returns>
        /// Settings
        /// </returns>
        public virtual IList<Setting> GetAllSettings()
        {
            var settings = _settingRepository.GetAll(query => from s in query
                orderby s.Name, s.StoreId
                select s, cache => default);

            return settings;
        }

        /// <summary>
        /// Determines whether a setting exists
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <typeparam name="TPropType">Property type</typeparam>
        /// <param name="settings">Entity</param>
        /// <param name="keySelector">Key selector</param>
        /// <param name="storeId">Store identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the true -setting exists; false - does not exist
        /// </returns>
        public virtual async Task<bool> SettingExistsAsync<T, TPropType>(T settings,
            Expression<Func<T, TPropType>> keySelector, int storeId = 0)
            where T : ISettings, new()
        {
            var key = GetSettingKey(settings, keySelector);

            var setting = await GetSettingByKeyAsync<string>(key, storeId: storeId);
            return setting != null;
        }

        /// <summary>
        /// Determines whether a setting exists
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <typeparam name="TPropType">Property type</typeparam>
        /// <param name="settings">Entity</param>
        /// <param name="keySelector">Key selector</param>
        /// <param name="storeId">Store identifier</param>
        /// <returns>
        /// The true -setting exists; false - does not exist
        /// </returns>
        public virtual bool SettingExists<T, TPropType>(T settings,
            Expression<Func<T, TPropType>> keySelector, int storeId = 0)
            where T : ISettings, new()
        {
            var key = GetSettingKey(settings, keySelector);

            var setting = GetSettingByKey<string>(key, storeId: storeId);
            return setting != null;
        }
        
        /// <summary>
        /// Load settings
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="storeId">Store identifier for which settings should be loaded</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task<T> LoadSettingAsync<T>(int storeId = 0) where T : ISettings, new()
        {
            return (T)await LoadSettingAsync(typeof(T), storeId);
        }

        /// <summary>
        /// Load settings
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="storeId">Store identifier for which settings should be loaded</param>
        public virtual T LoadSetting<T>(int storeId = 0) where T : ISettings, new()
        {
            return (T)LoadSetting(typeof(T), storeId);
        }

        /// <summary>
        /// Load settings
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="storeId">Store identifier for which settings should be loaded</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task<ISettings> LoadSettingAsync(Type type, int storeId = 0)
        {
            var settings = Activator.CreateInstance(type);

            if (!DataSettingsManager.IsDatabaseInstalled())
                return settings as ISettings;

            foreach (var prop in type.GetProperties())
            {
                // get properties we can read and write to
                if (!prop.CanRead || !prop.CanWrite)
                    continue;

                var key = type.Name + "." + prop.Name;
                //load by store
                var setting = await GetSettingByKeyAsync<string>(key, storeId: storeId, loadSharedValueIfNotFound: true);
                if (setting == null)
                    continue;

                if (!TypeDescriptor.GetConverter(prop.PropertyType).CanConvertFrom(typeof(string)))
                    continue;

                if (!TypeDescriptor.GetConverter(prop.PropertyType).IsValid(setting))
                    continue;

                var value = TypeDescriptor.GetConverter(prop.PropertyType).ConvertFromInvariantString(setting);

                //set property
                prop.SetValue(settings, value, null);
            }

            return settings as ISettings;
        }

        /// <summary>
        /// Load settings
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="storeId">Store identifier for which settings should be loaded</param>
        /// <returns>Settings</returns>
        public virtual ISettings LoadSetting(Type type, int storeId = 0)
        {
            var settings = Activator.CreateInstance(type);

            if (!DataSettingsManager.IsDatabaseInstalled())
                return settings as ISettings;

            foreach (var prop in type.GetProperties())
            {
                // get properties we can read and write to
                if (!prop.CanRead || !prop.CanWrite)
                    continue;

                var key = type.Name + "." + prop.Name;
                //load by store
                var setting = GetSettingByKey<string>(key, storeId: storeId, loadSharedValueIfNotFound: true);
                if (setting == null)
                    continue;

                if (!TypeDescriptor.GetConverter(prop.PropertyType).CanConvertFrom(typeof(string)))
                    continue;

                if (!TypeDescriptor.GetConverter(prop.PropertyType).IsValid(setting))
                    continue;

                var value = TypeDescriptor.GetConverter(prop.PropertyType).ConvertFromInvariantString(setting);

                //set property
                prop.SetValue(settings, value, null);
            }

            return settings as ISettings;
        }

        /// <summary>
        /// Save settings object
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="storeId">Store identifier</param>
        /// <param name="settings">Setting instance</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task SaveSettingAsync<T>(T settings, int storeId = 0) where T : ISettings, new()
        {
            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            foreach (var prop in typeof(T).GetProperties())
            {
                // get properties we can read and write to
                if (!prop.CanRead || !prop.CanWrite)
                    continue;

                if (!TypeDescriptor.GetConverter(prop.PropertyType).CanConvertFrom(typeof(string)))
                    continue;

                var key = typeof(T).Name + "." + prop.Name;
                var value = prop.GetValue(settings, null);
                if (value != null)
                    await SetSettingAsync(prop.PropertyType, key, value, storeId, false);
                else
                    await SetSettingAsync(key, string.Empty, storeId, false);
            }

            //and now clear cache
            await ClearCacheAsync();
        }

        /// <summary>
        /// Save settings object
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="storeId">Store identifier</param>
        /// <param name="settings">Setting instance</param>
        public virtual void SaveSetting<T>(T settings, int storeId = 0) where T : ISettings, new()
        {
            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            foreach (var prop in typeof(T).GetProperties())
            {
                // get properties we can read and write to
                if (!prop.CanRead || !prop.CanWrite)
                    continue;

                if (!TypeDescriptor.GetConverter(prop.PropertyType).CanConvertFrom(typeof(string)))
                    continue;

                var key = typeof(T).Name + "." + prop.Name;
                var value = prop.GetValue(settings, null);
                if (value != null)
                    SetSetting(prop.PropertyType, key, value, storeId, false);
                else
                    SetSetting(key, string.Empty, storeId, false);
            }

            //and now clear cache
            ClearCache();
        }

        /// <summary>
        /// Save settings object
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <typeparam name="TPropType">Property type</typeparam>
        /// <param name="settings">Settings</param>
        /// <param name="keySelector">Key selector</param>
        /// <param name="storeId">Store ID</param>
        /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task SaveSettingAsync<T, TPropType>(T settings,
            Expression<Func<T, TPropType>> keySelector,
            int storeId = 0, bool clearCache = true) where T : ISettings, new()
        {
            if (keySelector.Body is not MemberExpression member) 
                throw new ArgumentException($"Expression '{keySelector}' refers to a method, not a property.");

            var propInfo = member.Member as PropertyInfo;
            if (propInfo == null) 
                throw new ArgumentException($"Expression '{keySelector}' refers to a field, not a property.");

            var key = GetSettingKey(settings, keySelector);
            var value = (TPropType)propInfo.GetValue(settings, null);
            if (value != null)
                await SetSettingAsync(key, value, storeId, clearCache);
            else
                await SetSettingAsync(key, string.Empty, storeId, clearCache);
        }

        /// <summary>
        /// Save settings object
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <typeparam name="TPropType">Property type</typeparam>
        /// <param name="settings">Settings</param>
        /// <param name="keySelector">Key selector</param>
        /// <param name="storeId">Store ID</param>
        /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
        public virtual void SaveSetting<T, TPropType>(T settings,
            Expression<Func<T, TPropType>> keySelector,
            int storeId = 0, bool clearCache = true) where T : ISettings, new()
        {
            if (keySelector.Body is not MemberExpression member)
                throw new ArgumentException($"Expression '{keySelector}' refers to a method, not a property.");

            var propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
                throw new ArgumentException($"Expression '{keySelector}' refers to a field, not a property.");

            var key = GetSettingKey(settings, keySelector);
            var value = (TPropType)propInfo.GetValue(settings, null);
            if (value != null)
                SetSetting(key, value, storeId, clearCache);
            else
                SetSetting(key, string.Empty, storeId, clearCache);
        }

        /// <summary>
        /// Save settings object (per store). If the setting is not overridden per store then it'll be delete
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <typeparam name="TPropType">Property type</typeparam>
        /// <param name="settings">Settings</param>
        /// <param name="keySelector">Key selector</param>
        /// <param name="overrideForStore">A value indicating whether to setting is overridden in some store</param>
        /// <param name="storeId">Store ID</param>
        /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task SaveSettingOverridablePerStoreAsync<T, TPropType>(T settings,
            Expression<Func<T, TPropType>> keySelector,
            bool overrideForStore, int storeId = 0, bool clearCache = true) where T : ISettings, new()
        {
            if (overrideForStore || storeId == 0)
                await SaveSettingAsync(settings, keySelector, storeId, clearCache);
            else if (storeId > 0)
                await DeleteSettingAsync(settings, keySelector, storeId);
        }

        /// <summary>
        /// Delete all settings
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteSettingAsync<T>() where T : ISettings, new()
        {
            var settingsToDelete = new List<Setting>();
            var allSettings = await GetAllSettingsAsync();
            foreach (var prop in typeof(T).GetProperties())
            {
                var key = typeof(T).Name + "." + prop.Name;
                settingsToDelete.AddRange(allSettings.Where(x => x.Name.Equals(key, StringComparison.InvariantCultureIgnoreCase)));
            }

            await DeleteSettingsAsync(settingsToDelete);
        }

        /// <summary>
        /// Delete settings object
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <typeparam name="TPropType">Property type</typeparam>
        /// <param name="settings">Settings</param>
        /// <param name="keySelector">Key selector</param>
        /// <param name="storeId">Store ID</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteSettingAsync<T, TPropType>(T settings,
            Expression<Func<T, TPropType>> keySelector, int storeId = 0) where T : ISettings, new()
        {
            var key = GetSettingKey(settings, keySelector);
            key = key.Trim().ToLowerInvariant();

            var allSettings = await GetAllSettingsDictionaryAsync();
            var settingForCaching = allSettings.ContainsKey(key) ?
                allSettings[key].FirstOrDefault(x => x.StoreId == storeId) : null;
            if (settingForCaching == null) 
                return;

            //update
            var setting = await GetSettingByIdAsync(settingForCaching.Id);
            await DeleteSettingAsync(setting);
        }

        /// <summary>
        /// Clear cache
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task ClearCacheAsync()
        {
            await _staticCacheManager.RemoveByPrefixAsync(ZarayeEntityCacheDefaults<Setting>.Prefix);
        }

        /// <summary>
        /// Clear cache
        /// </summary>
        public virtual void ClearCache()
        {
            _staticCacheManager.RemoveByPrefix(ZarayeEntityCacheDefaults<Setting>.Prefix);
        }

        /// <summary>
        /// Get setting key (stored into database)
        /// </summary>
        /// <typeparam name="TSettings">Type of settings</typeparam>
        /// <typeparam name="T">Property type</typeparam>
        /// <param name="settings">Settings</param>
        /// <param name="keySelector">Key selector</param>
        /// <returns>Key</returns>
        public virtual string GetSettingKey<TSettings, T>(TSettings settings, Expression<Func<TSettings, T>> keySelector)
            where TSettings : ISettings, new()
        {
            if (keySelector.Body is not MemberExpression member)
                throw new ArgumentException($"Expression '{keySelector}' refers to a method, not a property.");

            if (member.Member is not PropertyInfo propInfo)
                throw new ArgumentException($"Expression '{keySelector}' refers to a field, not a property.");

            var key = $"{typeof(TSettings).Name}.{propInfo.Name}";
            
            return key;
        }
        #endregion

        #region User Type

        public virtual async Task DeleteUserTypeAsync(UserType userType)
        {
            await _userTypeRepository.DeleteAsync(userType, false);
        }

        public virtual async Task<UserType> GetUserTypeByIdAsync(int userTypeId)
        {
            return await _userTypeRepository.GetByIdAsync(userTypeId);
        }

        public virtual async Task InsertUserTypeAsync(UserType userType)
        {
            userType.UpdatedOnUtc = DateTime.UtcNow;
            await _userTypeRepository.InsertAsync(userType);
        }

        public virtual async Task UpdateUserTypeAsync(UserType userType)
        {
            userType.UpdatedOnUtc = DateTime.UtcNow;
            await _userTypeRepository.UpdateAsync(userType);
        }

        public virtual async Task<IPagedList<UserType>> GetAllUserTypesAsync(string type = "", int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            var query = from ut in _userTypeRepository.Table
                        where ut.Type == type
                        && !ut.Deleted
                        select ut;

            if (!showHidden)
                query = query.Where(c => c.Published);

            var userTypes = await query.ToListAsync();

            //paging
            return new PagedList<UserType>(userTypes, pageIndex, pageSize);
        }

        public virtual async Task<UserType> GetUserTypeByNameAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            var query = _userTypeRepository.Table.Where(b => b.Name == name);
            return await query.FirstOrDefaultAsync();
        }

        #endregion

        #region Shipment Return Reason

        public virtual async Task DeleteShipmentReturnReasonAsync(ShipmentReturnReason shipmentReturnReason)
        {
            shipmentReturnReason.Deleted = true;
            await _shipmentReturnReasonRepository.UpdateAsync(shipmentReturnReason);
        }

        public virtual async Task<ShipmentReturnReason> GetShipmentReturnReasonByIdAsync(int shipmentReturnReasonId)
        {
            return await _shipmentReturnReasonRepository.GetByIdAsync(shipmentReturnReasonId);
        }

        public virtual async Task InsertShipmentReturnReasonAsync(ShipmentReturnReason shipmentReturnReason)
        {
            await _shipmentReturnReasonRepository.InsertAsync(shipmentReturnReason);
        }

        public virtual async Task UpdateShipmentReturnReasonAsync(ShipmentReturnReason shipmentReturnReason)
        {
            await _shipmentReturnReasonRepository.UpdateAsync(shipmentReturnReason);
        }

        public virtual async Task<IPagedList<ShipmentReturnReason>> GetAllShipmentReturnReasonAsync(int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            var query = from dc in _shipmentReturnReasonRepository.Table
                        where !dc.Deleted
                        select dc;

            if (!showHidden)
                query = query.Where(c => c.Published);

            //paging
            return await query.ToPagedListAsync(pageIndex, pageSize);
        }
        #endregion

        #region Delivery Cost Reason

        public virtual async Task DeleteDeliveryCostReasonAsync(DeliveryCostReason deliveryCostReason)
        {
            deliveryCostReason.Deleted = true;
            await _deliveryCostReasonRepository.UpdateAsync(deliveryCostReason);
        }

        public virtual async Task<DeliveryCostReason> GetDeliveryCostReasonByIdAsync(int deliveryCostReasonId)
        {
            return await _deliveryCostReasonRepository.GetByIdAsync(deliveryCostReasonId);
        }

        public virtual async Task InsertDeliveryCostReasonAsync(DeliveryCostReason deliveryCostReason)
        {
            await _deliveryCostReasonRepository.InsertAsync(deliveryCostReason);
        }

        public virtual async Task UpdateDeliveryCostReasonAsync(DeliveryCostReason deliveryCostReason)
        {
            await _deliveryCostReasonRepository.UpdateAsync(deliveryCostReason);
        }

        public virtual async Task<IPagedList<DeliveryCostReason>> GetAllDeliveryCostReasonAsync(int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            var query = from dc in _deliveryCostReasonRepository.Table
                        where !dc.Deleted
                        select dc;

            if (!showHidden)
                query = query.Where(c => c.Published);

            var deliveryCostReason = await query.ToListAsync();

            //paging
            return new PagedList<DeliveryCostReason>(deliveryCostReason, pageIndex, pageSize);
        }


        #endregion

        #region Delivery Time Reason

        public virtual async Task DeleteDeliveryTimeReasonAsync(DeliveryTimeReason deliveryTimeReason)
        {
            deliveryTimeReason.Deleted = true;
            await _deliveryTimeReasonRepository.UpdateAsync(deliveryTimeReason);
        }

        public virtual async Task<DeliveryTimeReason> GetDeliveryTimeReasonByIdAsync(int deliveryTimeReasonId)
        {
            return await _deliveryTimeReasonRepository.GetByIdAsync(deliveryTimeReasonId);
        }

        public virtual async Task InsertDeliveryTimeReasonAsync(DeliveryTimeReason deliveryTimeReason)
        {
            await _deliveryTimeReasonRepository.InsertAsync(deliveryTimeReason);
        }

        public virtual async Task UpdateDeliveryTimeReasonAsync(DeliveryTimeReason deliveryTimeReason)
        {
            await _deliveryTimeReasonRepository.UpdateAsync(deliveryTimeReason);
        }

        public virtual async Task<IPagedList<DeliveryTimeReason>> GetAllDeliveryTimeReasonAsync(int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            var query = from dc in _deliveryTimeReasonRepository.Table
                        where !dc.Deleted
                        select dc;

            if (!showHidden)
                query = query.Where(c => c.Published);

            //paging
            return await query.ToPagedListAsync(pageIndex, pageSize);
        }
        #endregion

        #region Return Request Reason

        public virtual async Task DeleteReturnRequestReasonAsync(ReturnRequestReason returnRequestReason)
        {
            returnRequestReason.Deleted = true;
            await _returnRequestReasonRepository.UpdateAsync(returnRequestReason);
        }

        public virtual async Task<ReturnRequestReason> GetReturnRequestReasonByIdAsync(int returnRequestReasonId)
        {
            return await _returnRequestReasonRepository.GetByIdAsync(returnRequestReasonId);
        }

        public virtual async Task InsertReturnRequestReasonAsync(ReturnRequestReason returnRequestReason)
        {
            await _returnRequestReasonRepository.InsertAsync(returnRequestReason);
        }

        public virtual async Task UpdateReturnRequestReasonAsync(ReturnRequestReason returnRequestReason)
        {
            await _returnRequestReasonRepository.UpdateAsync(returnRequestReason);
        }

        public virtual async Task<IPagedList<ReturnRequestReason>> GetAllReturnRequestReasonAsync(int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            var query = from dc in _returnRequestReasonRepository.Table
                        where !dc.Deleted
                        orderby dc.DisplayOrder
                        select dc;

            if (!showHidden)
                query = (IOrderedQueryable<ReturnRequestReason>)query.Where(c => c.Published);

            //paging
            return await query.ToPagedListAsync(pageIndex, pageSize);
        }
        #endregion

    }
}