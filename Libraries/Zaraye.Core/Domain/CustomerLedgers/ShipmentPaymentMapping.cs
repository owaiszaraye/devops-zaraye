using System;

namespace Zaraye.Core.Domain.CustomerLedgers
{
    public partial class ShipmentPaymentMapping : BaseEntity
    {
        public int ShipmentId { get; set; }
        public int PaymentId { get; set; }
        public decimal Amount { get; set; }
        public bool IsDeliveryCost { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public int CreatedById { get; set; }
    }
}
