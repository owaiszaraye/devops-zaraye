using Zaraye.Core.Domain.Localization;
using System;

namespace Zaraye.Core.Domain.Media
{
    /// <summary>
    /// Represents a picture
    /// </summary>
    public partial class AppSlider : BaseEntity,StoreEntity, ILocalizedEntity, DefaultColumns
    {
        public int PictureId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string DisplayOrder { get; set; }
        public bool Published { get; set; }

        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
        public bool Deleted { get; set; }
        public int DeletedById { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }

        public int StoreId { get; set; }
    }
}
