namespace Zaraye.Core.Domain.Common
{
    public partial class DashboardDataModel
    {
        public BuyerData BuyerData { get; set; }
        public SupplierData SupplierData { get; set; }
        public ShipmentData ShipmentData { get; set; }
    }

    public class BuyerData
    {
        public decimal? Engagement { get; set; }
        public decimal? Active { get; set; }
        public decimal? NonEngaged { get; set; }
    }

    public class SupplierData
    {
        public decimal? Engagement { get; set; }
        public decimal? Active { get; set; }
        public decimal? NonEngaged { get; set; }
    }

    public class ShipmentData
    {
        public decimal? Pending { get; set; }
        public decimal? Dispatched { get; set; }
        public decimal? InTransit { get; set; }
        public decimal? Delivered { get; set; }
    }
}
