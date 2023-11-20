using System;

namespace Zaraye.Core.Domain.Directory
{
    /// <summary>
    /// Represents a measure weight
    /// </summary>
    public partial class MeasureWeight : BaseEntity,StoreEntity, DefaultColumns
    {
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the system keyword
        /// </summary>
        public string SystemKeyword { get; set; }

        /// <summary>
        /// Gets or sets the ratio
        /// </summary>
        public decimal Ratio { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
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
