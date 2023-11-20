using Zaraye.Framework.Models;

namespace Zaraye.Models.Catalog
{
    /// <summary>
    /// Represents a grouped product specification attribute model
    /// </summary>
    public partial record ProductSpecificationAttributeGroupModel : BaseZarayeEntityModel
    {
        #region Properties

        /// <summary>
        /// Gets or sets the specification attribute group name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the specification attribute group attributes
        /// </summary>
        public IList<ProductSpecificationAttributeModel> Attributes { get; set; }

        #endregion

        #region Ctor

        public ProductSpecificationAttributeGroupModel()
        {
            Attributes = new List<ProductSpecificationAttributeModel>();
        }

        #endregion
    }
}