using Zaraye.Core.Domain.Orders;
using Zaraye.Services.Localization;
using Zaraye.Services.Logging;
using Zaraye.Services.ScheduleTasks;
using System;

namespace Zaraye.Services.Orders
{
    public partial class RequestQuotationExpiryTask : IScheduleTask
    {
        #region Fields

        private readonly IRequestService _requestService;
        private readonly IQuotationService _quotationService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;

        #endregion

        #region Ctor

        public RequestQuotationExpiryTask(
            IRequestService requestService,
            IQuotationService quotationService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            ILogger logger)
        {
            _requestService = requestService;
            _quotationService = quotationService;
            _customerActivityService = customerActivityService;
            _localizationService = localizationService;
            _logger = logger;
        }

        #endregion

        #region Methods

        public async System.Threading.Tasks.Task ExecuteAsync()
        {
            var allRequests = await _requestService.GetAllRequestAsync(/*loadOnlyExpired: true*/);
            foreach (var request in allRequests)
            {
                if (request.ExpiryDate.HasValue && DateTime.UtcNow > request.ExpiryDate.Value && (request.RequestStatusId == (int)RequestStatus.Pending || request.RequestStatusId == (int)RequestStatus.Verified))
                {
                    request.RequestStatus = RequestStatus.Expired;
                    await _requestService.UpdateRequestAsync(request);
                }
            }

            //var allQuotations = await _quotationService.GetAllQuotationAsync(loadOnlyExpired: true);
            //foreach (var quotation in allQuotations)
            //{
            //    var buyerRequest = await _requestService.GetRequestByIdAsync(quotation);
            //    if (buyerRequest is not null && buyerRequest.OrderId == 0 && buyerRequest.DuplicateOrderId == 0)
            //    {
            //        if (DateTime.UtcNow > quotation.PriceValidity && (quotation.StatusId == (int)SellerBidStatus.Pending || quotation.StatusId == (int)SellerBidStatus.Verified || quotation.StatusId == (int)SellerBidStatus.QuotedToBuyer))
            //        {
            //            var oldstatus = await _localizationService.GetLocalizedEnumAsync(quotation.SellerBidStatus);

            //            quotation.SellerBidStatus = SellerBidStatus.Expired;
            //            await _sellerService.UpdateSellerBidAsync(quotation);

            //            await _customerActivityService.InsertActivityAsync("ChangeStatusSupplierQuotation",
            //                string.Format(await _localizationService.GetResourceAsync("ActivityLog.QuotationExpired"), oldstatus, await _localizationService.GetLocalizedEnumAsync(quotation.SellerBidStatus)), quotation);
            //        }
            //    }
            //}
        }

        #endregion
    }
}