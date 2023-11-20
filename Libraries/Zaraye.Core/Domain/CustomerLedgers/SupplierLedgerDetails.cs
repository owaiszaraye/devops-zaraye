using System;

namespace Zaraye.Core.Domain.CustomerLedgers
{
    public partial class SupplierLedgerDetails : BaseEntity
    {
        public DateTime? Date { get; set; }
        public string Description { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal Balance { get; set; }
        public int? OrderId { get; set; }
        public string CustomOrderNumber { get; set; }
        public int? ShipmentId { get; set; }
        public string CustomShipmentNumber { get; set; }
        public int? ProductId { get; set; }
        public string ProductName { get; set; }
        public int? BrandId { get; set; }
        public string BrandName { get; set; }
        public decimal? Quantity { get; set; }
        public string QuantityType { get; set; }
        public int? PaymentId { get; set; }
    }
}
