using Microsoft.AspNetCore.Mvc;
using Zaraye.Models.Api.V4.Request;
using Zaraye.Services.Logging;
using Zaraye.Services.Requests;

namespace Zaraye.Controllers.V4.Requests
{
    [ApiController]
    [Produces("application/json")]
    [ApiVersion("4")]
    [Route("v{version:apiVersion}/request")]
    public class RequestController : ControllerBase
    {

        private readonly IRequestsService _requestsService;
        private readonly IAppLoggerService _appLoggerService;
        public RequestController(IRequestsService requestsService, IAppLoggerService appLoggerService)
        {
            _requestsService = requestsService;
            _appLoggerService = appLoggerService;
        }

        [HttpPost("add-request")]
        public async Task<IActionResult> AddRequestAsync([FromBody] RegisterRequestModel registerRequestModel)
        {
            try
            {
                var data = await _requestsService.AddRequestAsync(registerRequestModel);
                return Ok(new { success = true, message = data });
            }
            catch (Exception ex)
            {
                await _appLoggerService.WriteLogs(ex);
                return Ok(new { success = false, message = ex });
            }

        }
    }
}
