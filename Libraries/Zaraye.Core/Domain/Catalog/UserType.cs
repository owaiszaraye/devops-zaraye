using Zaraye.Core.Domain.Common;
using System;

namespace Zaraye.Core.Domain.Catalog
{
    public partial class UserType : BaseEntity,StoreEntity, ISoftDeletedEntity
    {
        public string Name { get; set; }
        public int DisplayOrder { get; set; }
        public bool Published { get; set; }
        public string Type { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
        public bool Deleted { get; set; }
        public int StoreId { get; set; }
    }
}
