using Microsoft.AspNetCore.Mvc;
using Zaraye.Models.Api.V4.MarketPlace;
using Zaraye.Services.Logging;
using Zaraye.Services.MarketPlace;

namespace Zaraye.Controllers.V4.Marketplace
{
    [ApiController]
    [Produces("application/json")]
    [ApiVersion("4")]
    [Route("v{version:apiVersion}/marketplace")]
    public class MarketplaceController : BaseApiController
    {
        #region Fields
        private readonly IMarketPlaceService _marketPlaceService;
        private readonly IAppLoggerService _appLoggerService;
        #endregion

        #region Ctor
        public MarketplaceController(IMarketPlaceService marketPlaceService, IAppLoggerService appLoggerService)
        {
            _marketPlaceService = marketPlaceService;
            _appLoggerService = appLoggerService;
        }
        #endregion

        [HttpGet("search-market-place/{keyword}")]
        public async Task<IActionResult> SearchMarketPlace(string keyword = "")
        {
            try
            {
                var data = await _marketPlaceService.SearchMarketPlace(keyword);
                if (data.Count <= 0)
                    return Ok(new { success = false, message = "no data found" });

                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }
        [HttpPost("add-online-lead")]
        public async Task<IActionResult> AddOnlineLeadAsync(OnlineLeadRequestModel model)
        {
            try
            {
                await _marketPlaceService.AddOnlineLeadAsync(model);

                return Ok(new { success = true, message = "Lead added successfully" });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("get-all-customer-testimonials")]
        public async Task<IActionResult> GetAllCustomerTestimonials(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            try
            {
                var data = await _marketPlaceService.GetAllCustomerTestimonials(pageIndex, pageSize);
                if (data == null)
                    return Ok(new { success = false, message = "no data found" });

                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("get-all-employee-insights")]
        public async Task<IActionResult> GetAllEmployeeInsights(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            try
            {
                var data = await _marketPlaceService.GetAllEmployeeInsights(pageIndex, pageSize);
                if (data == null)
                    return Ok(new { success = false, message = "no data found" });

                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("get-all-marketplace-exchangerate")]
        public async Task<IActionResult> GetAllMarketplaceExchangeRate()
        {
            try
            {
                var data = await _marketPlaceService.GetMarketPlaceExchangerate();
                if (data == null)
                    return Ok(new { success = false, message = "no data found" });

                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex.Message });
            }
        }
    }
}
