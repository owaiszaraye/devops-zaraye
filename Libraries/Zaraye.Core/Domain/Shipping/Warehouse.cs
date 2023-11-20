using Zaraye.Core.Domain.Common;
using System;

namespace Zaraye.Core.Domain.Shipping
{
    /// <summary>
    /// Represents a shipment
    /// </summary>
    public partial class Warehouse : BaseEntity,StoreEntity, DefaultColumns, IActiveActivityLogEntity
    {
        /// <summary>
        /// Gets or sets  the warehouse name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the admin comment
        /// </summary>
        public string AdminComment { get; set; }

        /// <summary>
        /// Gets or sets the address identifier of the warehouse
        /// </summary>
        public int AddressId { get; set; }

        /// <summary>
        /// Gets or sets the supplier identifier of the warehouse
        /// </summary>
        public int? SupplierId { get; set; }

        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
        public bool Deleted { get; set; }
        public int DeletedById { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
        public int StoreId { get; set; }
    }
}