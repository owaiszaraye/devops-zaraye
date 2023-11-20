
namespace Zaraye.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a category-category tag mapping class
    /// </summary>
    public partial class CategoryCategoryTagMapping : BaseEntity
    {
        /// <summary>
        /// Gets or sets the category identifier
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// Gets or sets the category tag identifier
        /// </summary>
        public int CategoryTagId { get; set; }
    }
}
