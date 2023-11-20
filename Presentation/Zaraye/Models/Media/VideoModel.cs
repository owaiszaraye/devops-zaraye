using Zaraye.Framework.Models;

namespace Zaraye.Models.Media
{
    public partial record VideoModel : BaseZarayeModel
    {
        public string VideoUrl { get; set; }

        public string Allow { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }
    }
}
