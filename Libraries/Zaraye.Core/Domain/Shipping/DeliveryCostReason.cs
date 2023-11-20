﻿using System;

namespace Zaraye.Core.Domain.Shipping
{
    /// <summary>
    /// Pickup point
    /// </summary>
    public partial class DeliveryCostReason : BaseEntity,StoreEntity, DefaultColumns
    {
        public string Name { get; set; }

        public bool Published { get; set; }

        public int DisplayOrder { get; set; }

        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
        public bool Deleted { get; set; }
        public int DeletedById { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }

        public int StoreId { get; set; }
    }
}
