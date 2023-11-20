using Zaraye.Core.Domain.Common;
using Zaraye.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Zaraye.Services.Common
{
    public partial class DashboardService : IDashboardService
    {
        #region Fileds

        private readonly IZarayeDataProvider _zarayeDataProvider;

        #endregion

        #region Ctor

        public DashboardService(IZarayeDataProvider zarayeDataProvider)
        {
            _zarayeDataProvider = zarayeDataProvider;
        }

        #endregion

        #region Mehtod

        public virtual async Task<DashboardDataModel> GetAllDashboarDataAsync()
        {
            var buyerData = (await _zarayeDataProvider.QueryProcAsync<BuyerData>("CalculateDashboard_BuyerData")).FirstOrDefault();
            var supplierData = (await _zarayeDataProvider.QueryProcAsync<SupplierData>("CalculateDashboard_SupplierData")).FirstOrDefault();
            var shipmentData = (await _zarayeDataProvider.QueryProcAsync<ShipmentData>("CalculateDashboard_ShipmentData")).FirstOrDefault();

            var dashboardModel = new DashboardDataModel
            {
                BuyerData = new BuyerData
                {

                    Engagement = buyerData?.Engagement,
                    Active = buyerData?.Active,
                    NonEngaged = buyerData?.NonEngaged,
                },
                SupplierData = new SupplierData
                {
                    Engagement = supplierData?.Engagement,
                    Active = supplierData?.Active,
                    NonEngaged = supplierData?.NonEngaged,
                },
                ShipmentData = new ShipmentData
                {
                    Pending = shipmentData?.Pending,
                    Dispatched = shipmentData?.Dispatched,
                    InTransit = shipmentData?.InTransit,
                    Delivered = shipmentData?.Delivered
                }
            };

            return dashboardModel;
        }

        #endregion
    }
}