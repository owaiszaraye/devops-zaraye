using Zaraye.Models.Api.V4.MarketPlace;

namespace Zaraye.Services.MarketPlace
{
    public interface IMarketPlaceService
    {
        Task<IList<object>> SearchMarketPlace(string keyword = "");
        Task<object> GetAllCustomerTestimonials(int pageIndex, int pageSize);
        Task<object> GetAllEmployeeInsights(int pageIndex, int pageSize);
        Task<object> GetMarketPlaceExchangerate();
        Task<string> AddOnlineLeadAsync(OnlineLeadRequestModel leads);
    }
}
