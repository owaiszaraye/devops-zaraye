using Zaraye.Framework.Models;

namespace Zaraye.Models.Media
{
    public partial record PictureModel : BaseZarayeModel
    {
        public string ImageUrl { get; set; }

        public string ThumbImageUrl { get; set; }

        public string FullSizeImageUrl { get; set; }

        public string Title { get; set; }

        public string AlternateText { get; set; }
    }
}