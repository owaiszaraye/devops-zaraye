﻿using System;

namespace Zaraye.Core.Domain.Shipping
{
    public partial class ShipmentReturn : BaseEntity,StoreEntity, DefaultColumns
    {
        public int ShipmentId { get; set; }

        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
        public bool Deleted { get; set; }
        public int DeletedById { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
        public int StoreId { get; set; }
    }
}
