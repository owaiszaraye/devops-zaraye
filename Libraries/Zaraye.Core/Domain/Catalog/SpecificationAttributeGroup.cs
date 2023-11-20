using Zaraye.Core.Domain.Localization;

namespace Zaraye.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a specification attribute group
    /// </summary>
    public partial class SpecificationAttributeGroup : BaseEntity,StoreEntity, ILocalizedEntity
    {
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }
        public int StoreId { get; set; }
    }
}
