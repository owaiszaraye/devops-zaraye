using ExCSS;
using Newtonsoft.Json;
using Zaraye.Core;
using Zaraye.Core.Domain.Customers;
using Zaraye.Core.Domain.Messages;
using Zaraye.Core.Domain.Orders;
using Zaraye.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Zaraye.Services.Messages
{
    public partial class PushNotificationService : IPushNotificationService
    {
        #region Fields

        private readonly IRepository<PushNotificationDevice> _pushNotificationDeviceRepository;
        private readonly IRepository<CustomerCustomerRoleMapping> _customerCustomerRoleMappingRepository;
        private readonly IWorkContext _workContext;
        private readonly IRepository<PushNotification> _pushNotificationRepository;
        private readonly OrderSettings _orderSettings;

        #endregion

        #region Ctor

        public PushNotificationService(
            IRepository<PushNotificationDevice> pushNotificationDeviceRepository,
            IRepository<CustomerCustomerRoleMapping> customerCustomerRoleMappingRepository,
            IWorkContext workContext,
            IRepository<PushNotification> pushNotificationRepository,
            OrderSettings orderSettings
            )
        {
            _pushNotificationDeviceRepository = pushNotificationDeviceRepository;
            _customerCustomerRoleMappingRepository = customerCustomerRoleMappingRepository;
            _workContext = workContext;
            _pushNotificationRepository = pushNotificationRepository;
            _orderSettings = orderSettings;
        }

        #endregion

        #region PushNotificationDevice Methods

        public virtual async Task InsertPushNotificationDeviceAsync(PushNotificationDevice pushNotificationDevice)
        {
            await _pushNotificationDeviceRepository.InsertAsync(pushNotificationDevice);
        }

        public virtual async Task UpdatePushNotificationDeviceAsync(PushNotificationDevice pushNotificationDevice)
        {
            await _pushNotificationDeviceRepository.UpdateAsync(pushNotificationDevice);
        }

        public virtual async Task DeletePushNotificationDeviceAsync(PushNotificationDevice pushNotificationDevice)
        {
            await _pushNotificationDeviceRepository.DeleteAsync(pushNotificationDevice);
        }

        public virtual async Task<PushNotificationDevice> GetPushNotificationDeviceByIdAsync(int pushNotificationDeviceId)
        {
            return await _pushNotificationDeviceRepository.GetByIdAsync(pushNotificationDeviceId, cache => default);
        }

        public virtual async Task<IList<PushNotificationDevice>> GetAllPushNotificationDevicesAsync(int customerId = 0, int[] customerRoleIds = null)
        {
            var pushNotificationDevices = await _pushNotificationDeviceRepository.GetAllAsync(query =>
            {
                if (customerId > 0) 
                    query = query.Where(c => c.CustomerId == customerId);

                if (customerRoleIds != null && customerRoleIds.Length > 0 && !customerRoleIds.Contains(0))
                {
                    query = query.Join(_customerCustomerRoleMappingRepository.Table, x => x.CustomerId, y => y.CustomerId,
                            (x, y) => new { Customer = x, Mapping = y })
                        .Where(z => customerRoleIds.Contains(z.Mapping.CustomerRoleId))
                        .Select(z => z.Customer)
                        .Distinct();
                }

                query = query.OrderBy(c => c.CreatedOnUtc);
                return query;
            });

            return pushNotificationDevices;
        }

        public virtual async Task<PushNotificationDevice> FindPushNotificationDevice(string deviceId)
        {
            var query = _pushNotificationDeviceRepository.Table;
            var data = await query.FirstOrDefaultAsync(record => record.DeviceId == deviceId);
            return data;
        }

        #endregion

        #region PushNotification Methods

        //public virtual async Task<PushNotification> SendPushNotificationAsync(string systemKeyword, string comment, BaseEntity entity = null)
        //{
        //    return await SendPushNotificationAsync(await _workContext.GetCurrentCustomerAsync(), systemKeyword, comment, entity);
        //}

        public virtual async Task<PushNotification> InsertPushNotificationAsync(int customerId, string title, string body, int? entityId, string entity = null, string extraData = null)
        {
            if (customerId <= 0)
                return null;

            //insert log item
            var pushNotification = new PushNotification
            {
                CustomerId = customerId,
                EntityId = entityId,
                EntityName = entity,
                Title = title,
                Body = body,
                CreatedOnUtc = DateTime.UtcNow,
                IsRead = false,
                ExtraData = extraData
            };
            await _pushNotificationRepository.InsertAsync(pushNotification);

            return pushNotification;
        }

        public virtual async Task UpdatePushNotificationAsync(PushNotification pushNotification)
        {
            await _pushNotificationRepository.UpdateAsync(pushNotification);
        }

        public virtual async Task<PushNotification> GetPushNotificationIdAsync(int pushNotificationId)
        {
            return await _pushNotificationRepository.GetByIdAsync(pushNotificationId, cache => default);
        }

        public virtual async Task<IPagedList<PushNotification>> GetAllPushNotificationsAsync(DateTime? createdOnFrom = null, DateTime? createdOnTo = null,
            int? customerId = null, string entityName = null, int? entityId = null,
            int pageIndex = 0, int pageSize = int.MaxValue)
        {
            return await _pushNotificationRepository.GetAllPagedAsync(query =>
            {
                //filter by creation date
                if (createdOnFrom.HasValue)
                    query = query.Where(logItem => createdOnFrom.Value <= logItem.CreatedOnUtc);
                if (createdOnTo.HasValue)
                    query = query.Where(logItem => createdOnTo.Value >= logItem.CreatedOnUtc);

                //filter by customer
                if (customerId.HasValue && customerId.Value > 0)
                    query = query.Where(logItem => customerId.Value == logItem.CustomerId);

                //filter by entity
                if (!string.IsNullOrEmpty(entityName))
                    query = query.Where(logItem => logItem.EntityName.Equals(entityName));
                if (entityId.HasValue && entityId.Value > 0)
                    query = query.Where(logItem => entityId.Value == logItem.EntityId);

                query = query.OrderByDescending(logItem => logItem.CreatedOnUtc).ThenBy(logItem => logItem.Id);

                return query;
            }, pageIndex, pageSize);
        }

        public async Task<PushNotificationresponse> SendPushNotificationAsync(AppType appType, string title, string body, string[] token)
        {
            var serverKey = "";
            if (appType == AppType.ConsumerApp)
                serverKey = _orderSettings.ConsumerAppServerKey;
            else if (appType == AppType.TijaraApp)
                serverKey = _orderSettings.TijaraAppServerKey;

            if (string.IsNullOrWhiteSpace(serverKey))
                return new PushNotificationresponse() { IsSuccessStatusCode = false };

            var client = new HttpClient();
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
            client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {serverKey}");

            var temp = new
            {
                notification = new
                {
                    title = title,
                    body = body,
                    sound = "default",
                    android = new
                    {
                        notification = new
                        {
                            channel_id = "500"
                        }
                    },
                    priority = "high"
                },
                to = "/topics/zaraye"
            };

            string jsonString = JsonConvert.SerializeObject(temp);
            var requestContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://fcm.googleapis.com/fcm/send", requestContent);

            return new PushNotificationresponse() { IsSuccessStatusCode = response.IsSuccessStatusCode, StatusCode = response.StatusCode, ResponseContent = await response.Content.ReadAsStringAsync() };
        }

        #endregion
    }
}