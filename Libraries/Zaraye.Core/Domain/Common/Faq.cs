using Zaraye.Core.Domain.Localization;
using System;
using System.Linq;

namespace Zaraye.Core.Domain.Common
{
    public partial class Faq : BaseEntity, StoreEntity, ILocalizedEntity, ISoftDeletedEntity, DefaultColumns
    {
        public string Question { get; set; }
        public string Answer { get; set; }
        public bool Type { get; set; }
        public bool Published { get; set; }
        public int DisplayOrder { get; set; }
        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
        public bool Deleted { get; set; }
        public int DeletedById { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
        public int StoreId { get; set; }
    }
}