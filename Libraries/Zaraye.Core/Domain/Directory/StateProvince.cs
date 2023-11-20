using Zaraye.Core.Domain.Common;
using Zaraye.Core.Domain.Localization;
using System;

namespace Zaraye.Core.Domain.Directory
{
    /// <summary>
    /// Represents a state/province
    /// </summary>
    public partial class StateProvince : BaseEntity, ILocalizedEntity, DefaultColumns, IActiveActivityLogEntity
    {
        /// <summary>
        /// Gets or sets the country identifier
        /// </summary>
        public int CountryId { get; set; }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the abbreviation
        /// </summary>
        public string Abbreviation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is published
        /// </summary>
        public bool Published { get; set; }

        public bool PublishedOnPriceDiscovery { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }

        public int ParentId { get; set; }

        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
        public bool Deleted { get; set; }
        public int DeletedById { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
    }
}
