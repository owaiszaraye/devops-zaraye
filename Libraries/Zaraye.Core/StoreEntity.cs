using System;
namespace Zaraye.Core
{
    /// <summary>
    /// Represents the base class for entities
    /// </summary>
    public partial interface StoreEntity
    {
        /// <summary>
        /// Gets or sets the entity identifier
        /// </summary>
        int StoreId { get; set; }

    }
}