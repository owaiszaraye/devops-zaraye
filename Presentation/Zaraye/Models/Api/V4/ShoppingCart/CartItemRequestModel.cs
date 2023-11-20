using System.ComponentModel.DataAnnotations;
using static Zaraye.Models.Api.V4.Buyer.BuyerRequestApiModel;

namespace Zaraye.Models.Api.V4.ShoppingCart
{
    public class CartItemRequestModel
    {
        public CartItemRequestModel()
        {
            AttributesData = new List<AttributesApiModel>();
        }
        [Range(1, int.MaxValue, ErrorMessage = "Product Id should be greater than zero")]
        public int ProductId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Brand Id should be greater than zero")]
        public int BrandId { get; set; }
        [Range(1, Double.PositiveInfinity)]
        public decimal Quantity { get; set; }
        public List<AttributesApiModel> AttributesData { get; set; }

    }

}
