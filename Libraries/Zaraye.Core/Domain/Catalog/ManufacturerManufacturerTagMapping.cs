namespace Zaraye.Core.Domain.Catalog
{
    public partial class ManufacturerManufacturerTagMapping : BaseEntity
    {
        /// <summary>
        /// Gets or sets the category identifier
        /// </summary>
        public int ManufacturerId { get; set; }

        /// <summary>
        /// Gets or sets the category tag identifier
        /// </summary>
        public int ManufacturerTagId { get; set; }
    }
}
