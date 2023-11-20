namespace Zaraye.Models.Api.V4.Common
{
    public partial class ProductFilterModel
    {

        public ProductFilterModel()
        {
            PageSize = 12;
            PageIndex = 0;
            OrderBy = "asc";
        }

        public string IndustryIds { get; set; }
        public string CategoriesIds { get; set; }
        public string BrandsIds { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public string OrderBy { get; set; }

    }
}
