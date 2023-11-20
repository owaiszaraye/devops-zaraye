using Zaraye.Core.Domain.Common;
using Zaraye.Core.Domain.Localization;
using System;

namespace Zaraye.Core.Domain.Orders
{
    /// <summary>
    /// Represents a return request reason
    /// </summary>
    public partial class ReturnRequestReason : BaseEntity,StoreEntity, ILocalizedEntity, ISoftDeletedEntity
    {
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }

        public bool Published { get; set; }

        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
        public bool Deleted { get; set; }

        public int StoreId { get; set; }
    }
}
