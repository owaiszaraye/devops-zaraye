using Zaraye.Core.Domain.Localization;
using Zaraye.Core.Domain.Seo;

namespace Zaraye.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a category tag
    /// </summary>
    public partial class CategoryTag : BaseEntity, StoreEntity, ILocalizedEntity, ISlugSupported
    {
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        public int StoreId { get; set; }
    }
}
