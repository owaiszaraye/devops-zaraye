using System;

namespace Zaraye.Core.Domain.CustomerLedgers
{
    public partial class CustomerLedger : BaseEntity
    {
        public int CustomerId { get; set; }
        public string Description { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public int? PaymentId { get; set; }
        public int? ShipmentId { get; set; }
        public int? InventoryId { get; set; }
        //public int? SPMId { get; set; }
        public int ParentId { get; set; }

        public DateTime Date { get; set; }
    }
}
