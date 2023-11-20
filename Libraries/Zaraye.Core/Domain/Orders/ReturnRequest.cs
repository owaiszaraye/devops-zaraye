using Zaraye.Core.Domain.Common;
using System;

namespace Zaraye.Core.Domain.Orders
{
    /// <summary>
    /// Represents a return request
    /// </summary>
    public partial class ReturnRequest : BaseEntity,StoreEntity, IActiveActivityLogEntity, DefaultColumns, ISoftDeletedEntity
    {
        /// <summary>
        /// Custom number of return request
        /// </summary>
        public string CustomNumber { get; set; }

        /// <summary>
        /// Gets or sets the store identifier
        /// </summary>
        public int StoreId { get; set; }

        /// <summary>
        /// Gets or sets the customer identifier
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the order item identifier
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// Gets or sets the quantity returned to stock
        /// </summary>
        public decimal ReturnedQuantity { get; set; }

        /// <summary>
        /// Gets or sets the comments
        /// </summary>
        public string Comments { get; set; }

        /// <summary>
        /// Gets or sets the return status identifier
        /// </summary>
        public int ReturnRequestStatusId { get; set; }

        public int ReturnReasonId { get; set; }

        public int ReturnOrderId { get; set; }

        /// <summary>
        /// Gets or sets the return status
        /// </summary>
        public ReturnRequestStatus ReturnRequestStatus
        {
            get => (ReturnRequestStatus)ReturnRequestStatusId;
            set => ReturnRequestStatusId = (int)value;
        }

        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
        public bool Deleted { get; set; }
        public int DeletedById { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
    }
}
