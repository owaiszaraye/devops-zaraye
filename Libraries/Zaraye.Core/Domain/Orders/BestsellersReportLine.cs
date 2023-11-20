using System;

namespace Zaraye.Core.Domain.Orders
{
    /// <summary>
    /// Represents a best suppliers report line
    /// </summary>
    [Serializable]
    public partial class BestsuppliersReportLine
    {
        /// <summary>
        /// Gets or sets the product identifier
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Gets or sets the product name
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// Gets or sets the total amount
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// Gets or sets the total quantity
        /// </summary>
        public decimal TotalQuantity { get; set; }
    }
}
