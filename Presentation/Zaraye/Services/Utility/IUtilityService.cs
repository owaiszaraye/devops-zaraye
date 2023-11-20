using Zaraye.Core.Domain.Customers;
using Zaraye.Core.Domain.Inventory;
using Zaraye.Core.Domain.Orders;
using Zaraye.Models.Api.V4.Buyer;
using Zaraye.Models.Api.V4.OrderManagement;
using static Zaraye.Models.Api.V4.Common.CommonApiModel;
using static Zaraye.Models.Api.V4.OrderManagement.OrderManagementApiModel;

namespace Zaraye.Services.Utility
{
    public interface IUtilityService
    {
        Task<string> ConvertToXmlAsync(List<BuyerRequestApiModel.AttributesApiModel> attributeDtos, int productId);
        Task<List<ProductItemAttributeApiModel>> ParseAttributeXml(string attributesXml);
        Task<decimal> GetBuyerProgress(Customer buyer = null);
        Task<(int totalDays, int totalHours, int minutes, int percentage)> GetPercentageAndTimeRemaining(DateTime priceValidity, DateTime createdOnUtc);
        Task<int> Buyer_UploadPicture(byte[] imgBytes, string fileName);
        Task SavePurchaseOrderAddressAsync(Customer supplier);
        string TimeAgo(DateTime? dt);
        string GetMimeTypeFromImageByteArray(byte[] byteArray);
        string getRelativeDateTime(DateTime date);
        string getRelativeDateTime(DateTime date, bool isUrdu);
        Task<decimal> GetFinalRate(int rateId, Customer buyer, int supplierId, decimal rate, int categoryId, bool includeFM);
        Task<string> Common_ConvertToXmlAsync(List<CombinationAttributeApiModel> attributeDtos, int productId);
        Task<decimal> CalculateSellingPriceOfProductByTaggings(List<DirectCogsInventoryTagging> cogsInventoryTaggings, bool gstInclude = false, decimal sellingPriceOfProduct = 0);
        Task<decimal> CalculateBuyingPriceByTaggings(List<DirectCogsInventoryTagging> cogsInventoryTaggings, bool gstInclude = false);
        Task<int> OrderManagement_UploadPicture(byte[] imgBytes, string fileName);
        int OrderManagement_Upload(byte[] imgBytes, string fileName);
        Task SaveSaleOrderAddressAsync(Request request, Customer buyer);
        Task<IList<string>> SaveSalesOrderCalculationAsync(Order order, Request request, DirectOrderCalculation directOrderCalculation);
        Task<IList<string>> PrepareSaleOrderCalculationAsync(Order order, Request request, DirectOrderCalculation model);
        Task SavePurchaseOrderCalculationAsync(Order order, DirectOrderCalculation directOrderCalculation);
        Task<string> OrderManagement_ConvertToXmlAsync(List<AttributesModel> attributeDtos, int productId);
        Task<BusinessModelApiModel> SaleOrder_BusinessModelFormCalculatedJson(DirectOrder directOrder);
        Task<BusinessModelApiModel> PurchaseOrder_BusinessModelFormCalculatedJson(DirectOrder directOrder);
        decimal ConvertToDecimal(object value);
        Task<IList<string>> PreparePurchaseOrderCalculationAsync(Quotation quotation, DirectOrderCalculation directOrderCalculation);

    }
}
