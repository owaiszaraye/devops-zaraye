using Microsoft.AspNetCore.Mvc;
using Zaraye.Core.Domain.Customers;
using Zaraye.Core.Domain.Orders;
using Zaraye.Core;
using Zaraye.Models.Api.V4.Request;
using Zaraye.Services.Customers;
using Zaraye.Core.Domain.Localization;
using Zaraye.Core.Events;
using Zaraye.Services.Catalog;
using Zaraye.Services.Common;
using Zaraye.Services.Localization;
using Zaraye.Services.Logging;
using Zaraye.Services.Messages;
using Zaraye.Services.Orders;

namespace Zaraye.Services.Requests
{
    public class RequestsService : IRequestsService
    {
        #region Fields
        private readonly IRequestService _requestService;
        private readonly IWorkContext _workContext;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly ICustomNumberFormatter _customNumberFormatter;
        private readonly ICustomerService _customerService;
        private readonly CustomerSettings _customerSettings;
        private readonly IStoreContext _storeContext;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly ICustomerRegistrationService _customerRegistrationService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IAddressService _addressService;
        private readonly LocalizationSettings _localizationSettings;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly ICustomerActivityService _customerActivityService;

        #endregion
        #region Ctor
        public RequestsService(
            IRequestService requestService, IWorkContext workContext,
            IShoppingCartService shoppingCartService, IProductService productService,
            ICategoryService categoryService, ICustomNumberFormatter customNumberFormatter,
            ICustomerService customerService, CustomerSettings customerSettings,
            IStoreContext storeContext, IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            ICustomerRegistrationService customerRegistrationService, IEventPublisher eventPublisher,
            IAddressService addressService, LocalizationSettings localizationSettings,
            IWorkflowMessageService workflowMessageService, ICustomerActivityService customerActivityService
            )
        {
            _requestService = requestService;
            _workContext = workContext;
            _shoppingCartService = shoppingCartService;
            _productService = productService;
            _categoryService = categoryService;
            _customNumberFormatter = customNumberFormatter;
            _customerService = customerService;
            _customerSettings = customerSettings;
            _storeContext = storeContext;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _customerRegistrationService = customerRegistrationService;
            _eventPublisher = eventPublisher;
            _addressService = addressService;
            _localizationSettings = localizationSettings;
            _workflowMessageService = workflowMessageService;
            _customerActivityService = customerActivityService;
        }
        #endregion
        public virtual async Task<object> AddRequestAsync([FromBody] RegisterRequestModel registerRequestModel)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var existingCustomer = await _customerService.GetCustomerByEmailAsync(registerRequestModel.Email);

            if (existingCustomer == null)
            {
                var randomGeneratedPassword = CommonHelper.GenerateRandomDigitCode(8);
                var customerEmail = registerRequestModel.Email?.Trim();
                var customerUserName = registerRequestModel.Phone?.Trim();

                var registrationRequest = new CustomerRegistrationRequest(customer,
                customerEmail,
                    customerUserName,
                randomGeneratedPassword,
                    _customerSettings.DefaultPasswordFormat,
                    (await _storeContext.GetCurrentStoreAsync()).Id,
                    true,
                    isBuyer: true);

                var registrationResult = await _customerRegistrationService.RegisterCustomerAsync(registrationRequest);
                if (!registrationResult.Success)
                    return registrationResult.Errors;
                //throw new AggregateException(registrationResult.Errors);
                else
                {

                    customer.Phone = registerRequestModel.Phone;
                    //custom working for name
                    var spaceExist = registerRequestModel.FullName.Any(x => Char.IsWhiteSpace(x));
                    if (spaceExist)
                    {
                        var splittedValue = registerRequestModel.FullName.Split(' ', 2);
                        customer.FirstName = splittedValue[0];
                        customer.LastName = splittedValue[1];
                    }
                    else
                    {
                        customer.FirstName = registerRequestModel.FullName;
                        customer.LastName = "";
                    }
                    await _customerService.UpdateCustomerAsync(customer);

                }

            }
            else
            {
                await _shoppingCartService.MigrateShoppingCartAsync(customer, existingCustomer, false);
                customer = existingCustomer;
            }

            var cartitems = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart);
            if (cartitems.Count <= 0)
                return "Cart items not found";

            foreach (var cartitem in cartitems)
            {
                var product = await _productService.GetProductByIdAsync(cartitem.ProductId);
                if (product == null)
                    throw new ApplicationException("Product not found");
                var productCategory = (await _categoryService.GetProductCategoriesByProductIdAsync(cartitem.ProductId, isAppPublished: true)).FirstOrDefault();
                if (productCategory is null || product.Id != productCategory.ProductId)
                    throw new ApplicationException("product Category not found");
                var category = await _categoryService.GetCategoryByIdAsync(productCategory.CategoryId);
                if (category is null)
                    throw new ApplicationException("category not found");
                var buyerRequest = new Request
                {
                    BuyerId = customer.Id,
                    IndustryId = product.IndustryId,
                    CategoryId = category.Id,
                    ProductId = cartitem.ProductId,
                    ProductAttributeXml = cartitem.AttributesXml,
                    RequestStatus = RequestStatus.Pending,
                    Quantity = cartitem.Quantity,
                    BrandId = cartitem.BrandId,
                    CreatedOnUtc = DateTime.UtcNow,
                    ExpiryDate = DateTime.UtcNow.AddDays(category.ExpiryDays > 0 ? category.ExpiryDays : 2),
                    IdealBuyingPrice = 0,
                    RequestTypeId = (int)RequestTypeEnum.External,
                    Source = "Web",
                    BusinessModelId = (int)BusinessModelEnum.Standard,
                };
                await _requestService.InsertRequestAsync(buyerRequest);
                buyerRequest.CustomRequestNumber = _customNumberFormatter.GenerateRequestCustomNumber(buyerRequest);
                await _requestService.UpdateRequestAsync(buyerRequest);
                await _shoppingCartService.DeleteShoppingCartItemAsync(cartitem.Id);
            }

            return "Request is successfully inserted.";
        }
    }
}
