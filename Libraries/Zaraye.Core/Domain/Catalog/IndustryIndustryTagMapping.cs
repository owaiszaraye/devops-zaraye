namespace Zaraye.Core.Domain.Catalog
{
    public partial class IndustryIndustryTagMapping : BaseEntity
    {
        /// <summary>
        /// Gets or sets the category identifier
        /// </summary>
        public int IndustryId { get; set; }

        /// <summary>
        /// Gets or sets the category tag identifier
        /// </summary>
        public int IndustryTagId { get; set; }
    }
}
