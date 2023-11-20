namespace Zaraye.Models.Api.V4.Common
{
    public partial class CategoryFilterModel
    {
        public CategoryFilterModel()
        {
            PageSize = 12;
            PageIndex = 0;
        }
        public string IndustryIds { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
    }
}
