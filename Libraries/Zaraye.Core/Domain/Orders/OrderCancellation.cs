using System;

namespace Zaraye.Core.Domain.Orders
{
    public partial class OrderCancellation : BaseEntity
    {
        #region Properties
        
        public int OrderId { get; set; }
        public decimal Quantity { get; set; }
        public string Reason { get; set; }
        public int CreateById { get; set; }
        public DateTime CreatedOnUtc { get; set; }

        #endregion
    }
}