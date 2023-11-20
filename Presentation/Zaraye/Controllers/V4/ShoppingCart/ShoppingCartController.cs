using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Zaraye.Models.Api.V4.ShoppingCart;
using Zaraye.Services.Logging;
using Zaraye.Services.ShoppingCart;

namespace Zaraye.Controllers.V4.ShoppingCart
{
    [ApiController]
    [Produces("application/json")]
    [ApiVersion("4")]
    [Route("v{version:apiVersion}/shopping-cart")]
    public class ShoppingCartController : BaseApiController
    {
        private readonly ICartService _cartService;
        private readonly IAppLoggerService _appLoggerService;
        public ShoppingCartController(ICartService cartService, IAppLoggerService appLoggerService)
        {
            _cartService = cartService;
            _appLoggerService = appLoggerService;
        }

        [HttpGet("get-all-cart-items")]
        public async Task<IActionResult> GetAllCartItems()
        {
            try
            {
                var data = await _cartService.GetAllCartItems();
                return Ok(new { success = true, data = data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }
        [HttpPost("add-cart-item")]
        public async Task<IActionResult> AddCartItem([FromBody] CartItemRequestModel cartItemRequestModel)
        {
            try
            {
                var data = await _cartService.AddCartItem(cartItemRequestModel);
                return Ok(new { success = true, message = data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }
        [HttpPut("update-cart-item")]
        public async Task<IActionResult> UpdateCartItem([Required] int cartItemId, [Required] decimal quantity)
        {
            try
            {
                var data = await _cartService.UpdateCartItem(cartItemId, quantity);
                return Ok(new { success = true, message = data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }
        [HttpDelete("delete-cart-item")]
        public async Task<IActionResult> DeleteCartItem([Required] int cartItemId)
        {
            try
            {
                var data = await _cartService.DeleteCartItem(cartItemId);
                return Ok(new { success = true, message = data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }


    }
}
