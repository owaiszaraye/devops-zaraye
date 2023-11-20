
namespace Zaraye.Framework.Models
{
    /// <summary>
    /// Represents base nopCommerce entity model
    /// </summary>
    public partial record BaseZarayeEntityModel : BaseZarayeModel
    {
        /// <summary>
        /// Gets or sets model identifier
        /// </summary>
        public virtual int Id { get; set; }
    }
}