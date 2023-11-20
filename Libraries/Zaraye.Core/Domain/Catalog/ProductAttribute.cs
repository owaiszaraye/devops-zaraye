using Zaraye.Core.Domain.Common;
using Zaraye.Core.Domain.Localization;
using System;

namespace Zaraye.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a product attribute
    /// </summary>
    public partial class ProductAttribute : BaseEntity,StoreEntity, ILocalizedEntity, DefaultColumns, IActiveActivityLogEntity
    {
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        public string Description { get; set; }

        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
        public bool Deleted { get; set; }
        public int DeletedById { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }

        public int StoreId { get; set; }
    }
}
