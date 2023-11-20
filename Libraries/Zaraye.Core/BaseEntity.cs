using System;
namespace Zaraye.Core
{
    /// <summary>
    /// Represents the base class for entities
    /// </summary>
    public abstract partial class BaseEntity
    {
        /// <summary>
        /// Gets or sets the entity identifier
        /// </summary>
        public int Id { get; set; }

        //public DateTime CreatedOnUtc { get; set; }
        //public DateTime UpdatedOnUtc { get; set; }
        //public int CreatedById { get; set; }
        //public int UpdatedById { get; set; }
        //public bool Deleted { get; set; }
        //public int DeletedById { get; set; }
    }
}