using Microsoft.AspNetCore.Mvc;
using Zaraye.Models.Api.V4.Request;

namespace Zaraye.Services.Requests
{
    public interface IRequestsService
    {
        Task<object> AddRequestAsync([FromBody] RegisterRequestModel registerRequestModel);
    }
}
