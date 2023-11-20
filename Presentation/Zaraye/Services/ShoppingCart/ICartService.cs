using Zaraye.Models.Api.V4.ShoppingCart;

namespace Zaraye.Services.ShoppingCart
{
    public interface ICartService
    {
        Task<object> GetAllCartItems();
        Task<string> AddCartItem(CartItemRequestModel cartItemRequestModel);
        Task<string> UpdateCartItem(int cartItemId, decimal quantity);
        Task<string> DeleteCartItem(int cartItemId);
    }
}
