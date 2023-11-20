using Zaraye.Framework.Models;

namespace Zaraye.Models.Catalog
{
    /// <summary>
    /// Represents a specification attribute model
    /// </summary>
    public partial record ProductSpecificationAttributeModel : BaseZarayeEntityModel
    {
        #region Properties

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the values
        /// </summary>
        public IList<ProductSpecificationAttributeValueModel> Values { get; set; }

        #endregion

        #region Ctor

        public ProductSpecificationAttributeModel()
        {
            Values = new List<ProductSpecificationAttributeValueModel>();
        }

        #endregion
    }
}