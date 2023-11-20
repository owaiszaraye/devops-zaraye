using Zaraye.Core.Configuration;

namespace Zaraye.Core.Domain.Catalog
{
    public class CalculationSettings : ISettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether blog is enabled
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the GST Percentage For Quotation for posts
        /// </summary>
        public decimal GSTPercentageForQuotation { get; set; }

        public decimal GSTPercentageForStandardQuotation { get; set; }

        /// <summary>
        /// Gets or sets the WHT Percentage For Quotation for posts
        /// </summary>
        public decimal WHTPercentageForQuotation { get; set; }

        /// <summary>
        /// Gets or sets the Wholesale Percentage For Quotation for posts
        /// </summary>
        public decimal WholesalepercentageForQuotation { get; set; }
    }
}
