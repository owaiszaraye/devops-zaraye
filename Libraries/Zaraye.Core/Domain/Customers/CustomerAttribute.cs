using Zaraye.Core.Domain.Catalog;
using Zaraye.Core.Domain.Localization;

namespace Zaraye.Core.Domain.Customers
{
    /// <summary>
    /// Represents a customer attribute
    /// </summary>
    public partial class CustomerAttribute : BaseEntity,StoreEntity, ILocalizedEntity
    {
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the attribute is required
        /// </summary>
        public bool IsRequired { get; set; }

        /// <summary>
        /// Gets or sets the attribute control type identifier
        /// </summary>
        public int AttributeControlTypeId { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }

        public int StoreId { get; set; }

        /// <summary>
        /// Gets the attribute control type
        /// </summary>
        public AttributeControlType AttributeControlType
        {
            get => (AttributeControlType)AttributeControlTypeId;
            set => AttributeControlTypeId = (int)value;
        }
    }
}
