using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Zaraye.Core;
using Zaraye.Core.Domain.Catalog;
using Zaraye.Core.Domain.Common;
using Zaraye.Core.Domain.Customers;
using Zaraye.Core.Domain.Inventory;
using Zaraye.Core.Domain.Localization;
using Zaraye.Core.Domain.Orders;
using Zaraye.Core.Domain.Payments;
using Zaraye.Core.Domain.Shipping;
using Zaraye.Core.Events;
using Zaraye.Core.Shipping;
using Zaraye.Services.Authentication;
using Zaraye.Services.Catalog;
using Zaraye.Services.Common;
using Zaraye.Services.CustomerLedgers;
using Zaraye.Services.Customers;
using Zaraye.Services.Directory;
using Zaraye.Services.Helpers;
using Zaraye.Services.Inventory;
using Zaraye.Services.Localization;
using Zaraye.Services.Logging;
using Zaraye.Services.Media;
using Zaraye.Services.Messages;
using Zaraye.Services.Orders;
using Zaraye.Services.Payments;
using Zaraye.Services.Security;
using Zaraye.Services.Shipping;
using Zaraye.Models.Api.V4.OrderManagement;
using Zaraye.Models.Api.V4.Security;
using static Zaraye.Models.Api.V4.OrderManagement.OrderManagementApiModel;
using Zaraye.Services.Utility;

namespace Zaraye.Services.OrderManagement
{
    public class OrderManagermentService : IOrderManagermentService
    {
        #region Fields
        private readonly IStoreContext _storeContext;
        private readonly ICustomerRegistrationService _customerRegistrationService;
        private readonly ICustomerService _customerService;
        private readonly CustomerSettings _customerSettings;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IWorkContext _workContext;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IAddressService _addressService;
        private readonly LocalizationSettings _localizationSettings;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IPictureService _pictureService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IDownloadService _downloadService;
        private readonly IMeasureService _measureService;
        private readonly ICustomNumberFormatter _customNumberFormatter;
        private readonly IOrderService _orderService;
        private readonly IManufacturerService _brandService;
        private readonly IPaymentService _paymentService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IPermissionService _permissionService;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IShipmentService _shipmentService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly HttpClient _httpClient;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly CalculationSettings _calculationSettings;
        private readonly IRequestService _requestService;
        private readonly IInventoryService _inventoryService;
        private readonly IQuotationService _quotationService;
        private readonly IIndustryService _industryService;
        private readonly IShippingService _shippingService;
        private readonly ICustomerLedgerService _customerLedgerService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUtilityService _utilityService;
        #endregion

        #region Ctor
        public OrderManagermentService(
            ICustomerRegistrationService customerRegistrationService,
            ICustomerService customerService,
            CustomerSettings customerSettings,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IWorkflowMessageService workflowMessageService,
            IStoreContext storeContext,
            IWorkContext workContext,
            ICustomerActivityService customerActivityService,
            IConfiguration config,
            IAuthenticationService authenticationService,
            IEventPublisher eventPublisher,
            IAddressService addressService,
            LocalizationSettings localizationSettings,
            IProductService productService,
            ICategoryService categoryService,
            IDateTimeHelper dateTimeHelper,
            IPriceFormatter priceFormatter,
            IPictureService pictureService,
            IProductAttributeParser productAttributeParser,
            IShoppingCartService shoppingCartService,
            IProductAttributeService productAttributeService,
            IDownloadService downloadService,
            IMeasureService measureService,
            ICustomNumberFormatter customNumberFormatter,
            IOrderService orderService,
            IManufacturerService brandService,
            IPaymentService paymentService,
            IOrderProcessingService orderProcessingService,
            IPermissionService permissionService,
            ICountryService countryService,
            IStateProvinceService stateProvinceService,
            IShipmentService shipmentService,
            IPriceCalculationService priceCalculationService,
            HttpClient client,
            ShoppingCartSettings shoppingCartSettings,
            CalculationSettings calculationSettings,
            IRequestService requestService,
            IInventoryService inventoryService,
            IQuotationService quotationService,
            IIndustryService industryService,
            IShippingService shippingService,
            ICustomerLedgerService customerLedgerService,
            IHttpContextAccessor httpContextAccessor, IUtilityService utilityService
            )
        {
            _storeContext = storeContext;
            _customerRegistrationService = customerRegistrationService;
            _customerService = customerService;
            _customerSettings = customerSettings;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _workflowMessageService = workflowMessageService;
            _workContext = workContext;
            _customerActivityService = customerActivityService;
            _authenticationService = authenticationService;
            _eventPublisher = eventPublisher;
            _addressService = addressService;
            _localizationSettings = localizationSettings;
            _productService = productService;
            _categoryService = categoryService;
            _dateTimeHelper = dateTimeHelper;
            _priceFormatter = priceFormatter;
            _pictureService = pictureService;
            _productAttributeParser = productAttributeParser;
            _shoppingCartService = shoppingCartService;
            _productAttributeService = productAttributeService;
            _downloadService = downloadService;
            _measureService = measureService;
            _customNumberFormatter = customNumberFormatter;
            _orderService = orderService;
            _brandService = brandService;
            _paymentService = paymentService;
            _orderProcessingService = orderProcessingService;
            _permissionService = permissionService;
            _countryService = countryService;
            _stateProvinceService = stateProvinceService;
            _shipmentService = shipmentService;
            _priceCalculationService = priceCalculationService;
            _httpClient = client;
            _shoppingCartSettings = shoppingCartSettings;
            _calculationSettings = calculationSettings;
            _requestService = requestService;
            _inventoryService = inventoryService;
            _quotationService = quotationService;
            _industryService = industryService;
            _shippingService = shippingService;
            _customerLedgerService = customerLedgerService;
            _httpContextAccessor = httpContextAccessor;
            _utilityService = utilityService;
        }



        #endregion

        #region CommonApi

        public async Task<object> Info()
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                throw new Exception("user not found");

            var data = new
            {
                info = new
                {
                    user.Id,
                    user.Email,
                    Phone = user.Username,
                    user.FirstName,
                    user.LastName,
                    CompanyName = user.Company,
                    user.CountryId,
                    StateId = user.StateProvinceId,
                    Address = user.StreetAddress,
                    user.City,
                    user.BookerType,
                    role = new
                    {
                        //New
                        isDemandAssociate = await _customerService.IsDemandAssociateAsync(user),
                        isSupply = await _customerService.IsSupplyAsync(user),
                        isSupplyAssociate = await _customerService.IsSupplyAssociateAsync(user),
                        isGroundOperations = await _customerService.IsGroundOperationsAsync(user),
                        isOPerationAssociate = await _customerService.IsOPerationAssociateAsync(user),
                        isOPerationLead = await _customerService.IsOPerationLeadAsync(user),
                        isBusinessLead = await _customerService.IsBusinessLeadAsync(user),
                        isLiveOps = await _customerService.IsLiveOpsAsync(user),
                        isControlTower = await _customerService.IsControlTowerAsync(user),

                        //Old
                        isDemand = await _customerService.IsDemandAsync(user),
                        //isSupply = await _customerService.IsSupplyAsync(user),
                        isOperations = await _customerService.IsOperationsAsync(user),
                        isFinance = await _customerService.IsFinanceAsync(user),
                        isBusinessHead = await _customerService.IsBusinessHeadAsync(user),
                        isFinanceHead = await _customerService.IsFinanceHeadAsync(user),
                        isOpsHead = await _customerService.IsOpsHeadAsync(user)
                    },
                    permission = new
                    {
                        CreateRequest = await _permissionService.AuthorizeAsync(StandardPermissionProvider.CreateRequest),
                        RejectRequest = await _permissionService.AuthorizeAsync(StandardPermissionProvider.RejectRequest),
                        InitiateRfq = await _permissionService.AuthorizeAsync(StandardPermissionProvider.InitiateRfq),
                        CreateSalesOrder = await _permissionService.AuthorizeAsync(StandardPermissionProvider.CreateSalesOrder),
                        CreateQuotation = await _permissionService.AuthorizeAsync(StandardPermissionProvider.CreateQuotation),
                        ChooseQuotationToCreatePurchaseOrder = await _permissionService.AuthorizeAsync(StandardPermissionProvider.ChooseQuotationToCreatePurchaseOrder),
                        //CreatePurchaseOrder = await _permissionService.AuthorizeAsync(StandardPermissionProvider.CreatePurchaseOrder),
                        GenerateDeliveryRequestOnSaleOrder = await _permissionService.AuthorizeAsync(StandardPermissionProvider.GenerateDeliveryRequestOnSaleOrder),
                        GeneratePickupRequestOnPurchaseOrder = await _permissionService.AuthorizeAsync(StandardPermissionProvider.GeneratePickupRequestOnPurchaseOrder),
                        SignSalesOrderContract = await _permissionService.AuthorizeAsync(StandardPermissionProvider.SignSalesOrderContract),
                        SignPurchaseOrderContract = await _permissionService.AuthorizeAsync(StandardPermissionProvider.SignPurchaseOrderContract),
                        RespondToDeleiveryRequest = await _permissionService.AuthorizeAsync(StandardPermissionProvider.RespondToDeleiveryRequest),
                        RespondToPickupRequest = await _permissionService.AuthorizeAsync(StandardPermissionProvider.RespondToPickupRequest),
                        ReassignAgentOnOperationTicket = await _permissionService.AuthorizeAsync(StandardPermissionProvider.ReassignAgentOnOperationTicket),
                        BuyerRegistration = await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageBuyerRegistration),
                        SupplierRegistration = await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSupplierRegistration),

                        //Demand
                        //ManageBuyerRegistration = await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageBuyerRegistration),
                        ManagePlaceRequest = await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlaceRequest),
                        ManagePlaceOrder = await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlaceOrder),
                        ManagePlaceDeliveryRequest = await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlaceDeliveryRequest),
                        ManagePlaceSalesReturnRequest = await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlaceSalesReturnRequest),

                        //Supply
                        //ManageSupplierRegistration = await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSupplierRegistration),
                        ManagePlaceQuotation = await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlaceQuotation),
                        ManageRaisePo = await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageRaisePo),

                        //
                        ManageUploadGrns = await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUploadGrn),
                        ManageUploadProofofPayments = await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageUploadProofofPayment),
                        //ManageRespondtoAssignedTicketOps = await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageRespondtoAssignedTicketOps),
                        ManageApproveorRejectPowithAttachments = await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageApproveorRejectPowithAttachment),
                        //ManageReassignedTicketsFinance = await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageRespondToAssignedTicketsFinance),
                        ManageReassignonTickets = await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageReassignonTicket),

                        ManagePoTickets = await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePOTicket),
                        ManageDeliveryRequestTicket = await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageDeliveryRequestTicket),
                        ManageSaleReturnTicket = await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSaleReturnTicket),
                        ManageAgent = await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAgent),
                        ManageRaisePoCompleteAndIncomplete = await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageRaisePoCompleteAndIncomplete),
                        ManageSalesReturnCompleteAndIncomplete = await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSalesReturnCompleteAndIncomplete),
                        ManageInvantoryRate = await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageInventoryRate),
                        InventoryRate = await _permissionService.AuthorizeAsync(StandardPermissionProvider.InventoryRate),

                        PurchaseOrderShipmentRequestApproved = await _permissionService.AuthorizeAsync(StandardPermissionProvider.PurchaseOrderShipmentRequestApproved),
                        PurchaseOrderShipmentShipped = await _permissionService.AuthorizeAsync(StandardPermissionProvider.PurchaseOrderShipmentShipped),
                        PurchaseOrderShipmentDelivered = await _permissionService.AuthorizeAsync(StandardPermissionProvider.PurchaseOrderShipmentDelivered),
                        SaleOrderShipmentRequestApproved = await _permissionService.AuthorizeAsync(StandardPermissionProvider.SaleOrderShipmentRequestApproved),
                        SaleOrderShipmentShipped = await _permissionService.AuthorizeAsync(StandardPermissionProvider.SaleOrderShipmentShipped),
                        SaleOrderShipmentDelivered = await _permissionService.AuthorizeAsync(StandardPermissionProvider.SaleOrderShipmentDelivered)
                    }
                }
            };

            return data;
        }

        public async Task<string> InfoAsync(BuyerInfoModel model)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                throw new Exception("user not found");

            if (!CommonHelper.IsValidEmail(model.Email))
                throw new Exception(await _localizationService.GetResourceAsync("Common.WrongEmail"));

            if (string.IsNullOrWhiteSpace(model.Phone))
                throw new Exception(await _localizationService.GetResourceAsync("Account.Fields.Username.Required"));

            //username 
            if (_customerSettings.UsernamesEnabled && _customerSettings.AllowUsersToChangeUsernames)
            {
                var userName = model.Phone.Trim();
                if (!user.Username.Equals(userName, StringComparison.InvariantCultureIgnoreCase))
                {
                    //change username
                    await _customerRegistrationService.SetUsernameAsync(user, userName);

                    //re-authenticate
                    //do not authenticate users in impersonation mode
                    if (_workContext.OriginalCustomerIfImpersonated == null)
                        await _authenticationService.SignInAsync(user, true);
                }
            }

            //email
            var email = model.Email.Trim();
            if (!user.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase))
            {
                //change email
                var requireValidation = _customerSettings.UserRegistrationType == UserRegistrationType.EmailValidation;
                await _customerRegistrationService.SetEmailAsync(user, email, requireValidation);

                //do not authenticate users in impersonation mode
                if (_workContext.OriginalCustomerIfImpersonated == null)
                {
                    //re-authenticate (if usernames are disabled)
                    if (!_customerSettings.UsernamesEnabled && !requireValidation)
                        await _authenticationService.SignInAsync(user, true);
                }
            }

            //form fields
            if (_customerSettings.FirstNameEnabled)
                user.FirstName = model.FirstName;
            if (_customerSettings.LastNameEnabled)
                user.LastName = model.LastName;
            if (_customerSettings.StreetAddressEnabled)
                user.StreetAddress = model.Address;
            if (_customerSettings.CompanyEnabled)
                user.Company = model.Company;
            if (_customerSettings.CountryEnabled)
                user.CountryId = model.CountryId;
            if (_customerSettings.CountryEnabled && _customerSettings.StateProvinceEnabled)
                user.StateProvinceId = model.StateId;
            if (_customerSettings.PhoneEnabled)
                user.Phone = model.Phone;

            await _customerService.UpdateCustomerAsync(user);

            return await _localizationService.GetResourceAsync("Booker.Info.Edit.Success");

        }

        public async Task<object> AllOpsAgents()
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                throw new Exception("user not found");

            var allOpsAgents = (await _customerService.GetAllCustomersAsync(customerRoleIds: new[] { (await _customerService.GetCustomerRoleBySystemNameAsync(ZarayeCustomerDefaults.OperationsRoleName)).Id })).ToList();
            var data = allOpsAgents.Select(e =>
            {
                return new
                {
                    Value = e.Id,
                    Text = e.FullName
                };
            }).ToList();

            return data;
        }

        public async Task<object> GetAllBankDetailsBySupplier(int supplierId)
        {
            var supplier = await _customerService.GetCustomerByIdAsync(supplierId);
            if (supplier == null)
                throw new Exception("Supplier not found.");

            var bankDetails = (await _customerService.GetAllBankDetailAsync(supplier.Id)).ToList();
            var result = (from bankDetail in bankDetails
                          select new { Value = bankDetail.Id, Text = bankDetail.BankName + ", " + bankDetail.AccountTitle + ", " + bankDetail.AccountNumber }).ToList();

            return result;
        }

        public async Task<object> AllFinanceAgents()
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                throw new Exception("user not found");

            var allFinanaceAgents = (await _customerService.GetAllCustomersAsync(customerRoleIds: new[] { (await _customerService.GetCustomerRoleBySystemNameAsync(ZarayeCustomerDefaults.FinanceRoleName)).Id })).ToList();
            var data = allFinanaceAgents.Select(e =>
            {
                return new
                {
                    Value = e.Id,
                    Text = e.FullName
                };
            }).ToList();

            return data;

        }

        #endregion

        #region Buyer Booker

        public async Task<string> BuyerRegistration(AccountApiModel.BookerBuyerRegisterApiModel model)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                throw new Exception("user not found");


            //check whether registration is allowed
            if (_customerSettings.UserRegistrationType == UserRegistrationType.Disabled)
                throw new Exception(await _localizationService.GetResourceAsync("Account.Register.Result.Disabled"));


            if (!CommonHelper.IsValidEmail(model.Email))
                throw new Exception(await _localizationService.GetResourceAsync("Common.WrongEmail"));


            if (string.IsNullOrWhiteSpace(model.Phone))
                throw new Exception(await _localizationService.GetResourceAsync("Account.Fields.Username.Required"));


            var customer = new Customer
            {
                Active = true,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
                RegisteredInStoreId = (await _storeContext.GetCurrentStoreAsync()).Id,
                Source = "Tijara-APP",
                CreatedBy = user.Id
            };
            await _customerService.InsertCustomerAsync(customer);

            var customerEmail = model.Email?.Trim();
            var customerUserName = model.Phone?.Trim();

            var registrationRequest = new CustomerRegistrationRequest(customer,
                customerEmail,
                customerUserName,
                model.Password,
                _customerSettings.DefaultPasswordFormat,
                (await _storeContext.GetCurrentStoreAsync()).Id,
                true,
                isBuyer: true);

            var registrationResult = await _customerRegistrationService.RegisterCustomerAsync(registrationRequest);
            if (registrationResult.Success)
            {
                //form fields
                if (_customerSettings.FirstNameEnabled)
                    customer.FirstName = model.FirstName;
                if (_customerSettings.LastNameEnabled)
                    customer.LastName = model.LastName;
                if (_customerSettings.StreetAddressEnabled)
                    customer.StreetAddress = model.Address;
                if (_customerSettings.CompanyEnabled)
                    customer.Company = model.Company;
                if (_customerSettings.CountryEnabled)
                    customer.CountryId = model.CountryId;
                if (_customerSettings.CountryEnabled && _customerSettings.StateProvinceEnabled)
                    customer.StateProvinceId = model.StateProvinceId;
                if (_customerSettings.PhoneEnabled)
                    customer.Phone = model.Phone;
                customer.Gst = model.BuyerGST;
                if (model.IndustryId > 0)
                    customer.IndustryId = model.IndustryId;
                if (model.BuyerType > 0)
                    customer.UserTypeId = model.BuyerType;

                await _genericAttributeService.SaveAttributeAsync(customer, ZarayeCustomerDefaults.VatNumberAttribute, model.BuyerNTN);

                if (model.BookerCurrentLocation != null)
                {
                    await _genericAttributeService.SaveAttributeAsync(customer, ZarayeCustomerDefaults.RegisteredBookerCurrentLocationAttribute, $"{model.BookerCurrentLocation.Latitude},{model.BookerCurrentLocation.Longitude}");
                    await _genericAttributeService.SaveAttributeAsync(customer, ZarayeCustomerDefaults.RegisteredBookerCurrentLocationAddressAttribute, model.BookerCurrentLocation.Location);
                }

                if (model.BuyerPinLocation != null)
                {
                    await _genericAttributeService.SaveAttributeAsync(customer, ZarayeCustomerDefaults.RegisteredPinLocationAttribute, $"{model.BuyerPinLocation.Latitude},{model.BuyerPinLocation.Longitude}");
                    await _genericAttributeService.SaveAttributeAsync(customer, ZarayeCustomerDefaults.RegisteredPinLocationAddressAttribute, model.BuyerPinLocation.Location);
                }

                //insert default address (if possible)
                var defaultAddress = new Address
                {
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    Email = customer.Email,
                    Company = customer.Company,
                    CountryId = customer.CountryId > 0
                            ? customer.CountryId
                            : null,
                    StateProvinceId = customer.StateProvinceId > 0
                            ? customer.StateProvinceId
                            : null,
                    County = customer.County,
                    City = customer.City,
                    Address1 = customer.StreetAddress,
                    Address2 = customer.StreetAddress2,
                    ZipPostalCode = customer.ZipPostalCode,
                    PhoneNumber = customer.Phone,
                    FaxNumber = customer.Fax,
                    CreatedOnUtc = customer.CreatedOnUtc
                };
                if (await _addressService.IsAddressValidAsync(defaultAddress))
                {
                    //some validation
                    if (defaultAddress.CountryId == 0)
                        defaultAddress.CountryId = null;
                    if (defaultAddress.StateProvinceId == 0)
                        defaultAddress.StateProvinceId = null;
                    //set default address
                    //customer.Addresses.Add(defaultAddress);

                    await _addressService.InsertAddressAsync(defaultAddress);
                    await _customerService.InsertCustomerAddressAsync(customer, defaultAddress);

                    customer.BillingAddressId = defaultAddress.Id;
                    customer.ShippingAddressId = defaultAddress.Id;

                    await _customerService.UpdateCustomerAsync(customer);
                }

                //notifications
                if (_customerSettings.NotifyNewCustomerRegistration)
                    await _workflowMessageService.SendBuyerRegisteredNotificationMessageAsync(customer,
                        _localizationSettings.DefaultAdminLanguageId);

                //raise event       
                await _eventPublisher.PublishAsync(new CustomerRegisteredEvent(customer));

                //send customer welcome message
                await _workflowMessageService.SendCustomerWelcomeMessageAsync(customer, (await _workContext.GetWorkingLanguageAsync()).Id);

                //raise event       
                await _eventPublisher.PublishAsync(new CustomerActivatedEvent(customer));

                await _customerRegistrationService.SignInCustomerAsync(customer, null, true);

                //activity log
                await _customerActivityService.InsertActivityAsync("AddNewBuyer",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewBuyer"), customer.Id), customer);

                return await _localizationService.GetResourceAsync("Account.Register.Result.Standard");
            }

            return string.Join(",", registrationResult.Errors);
        }


        public async Task<object> SearchBuyers(string name = "")
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                throw new Exception("user not found");

            if (string.IsNullOrEmpty(name))
                throw new Exception("Buyer name is required");

            var buyers = (await _customerService.GetAllCustomersAsync(isActive: true,
                customerRoleIds: new int[] { (await _customerService.GetCustomerRoleBySystemNameAsync(ZarayeCustomerDefaults.BuyerRoleName)).Id },
                fullName: name)).ToList();

            var data = await buyers.SelectAwait(async b =>
            {
                var buyerTypeAttributeId = await _genericAttributeService.GetAttributeAsync<string>(b, ZarayeCustomerDefaults.BuyerTypeIdAttribute);

                return new
                {
                    b.Id,
                    b.FullName,
                    b.Email,
                    Phone = b.Username,
                    BuyerType = (await _customerService.GetUserTypeByIdAsync(buyerTypeAttributeId != null ? Convert.ToInt32(buyerTypeAttributeId) : 0))?.Name
                };
            }).ToListAsync();

            return data;
        }


        public async Task<object> AddBuyerRequest(BuyerRequestModel model)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                throw new Exception("user not found");


            var product = await _productService.GetProductByIdAsync(model.ProductId);
            if (product is null)
                throw new Exception("Product not found");

            var industry = await _industryService.GetIndustryByIdAsync(model.IndustryId);
            if (industry is null)
                throw new Exception("Industry not found");

            var category = await _categoryService.GetCategoryByIdAsync(model.CategoryId);
            if (category is null)
                throw new Exception("Category not found");

            var brand = await _brandService.GetManufacturerByIdAsync(model.BrandId);
            if (brand is null)
                throw new Exception("Brand not found");

            var warnings = new List<string>();
            var attributesData = model.AttributesData.Select(async attribute =>
            {
                return new AttributesModel()
                {
                    Name = attribute.Name,
                    Value = attribute.Value
                };
            }).Select(t => t.Result).ToList();

            var attributesXml = await _utilityService.OrderManagement_ConvertToXmlAsync(attributesData, product.Id);
            warnings.AddRange(await _shoppingCartService.GetShoppingCartItemAttributeWarningsAsync(await _workContext.GetCurrentCustomerAsync(), ShoppingCartType.ShoppingCart, product, 1, attributesXml));
            if (warnings.Any())
                return warnings.ToArray();

            if (model.Quantity <= 0)
                throw new Exception("Quantity is required");

            var request = new Request
            {
                BookerId = user.Id,
                BuyerId = model.BuyerId,
                IndustryId = model.IndustryId,
                CategoryId = model.CategoryId,
                ProductId = model.ProductId,
                ProductAttributeXml = attributesXml,
                RequestStatus = RequestStatus.Verified,
                DeliveryDate = model.DeliveryDate,// (DateTime)_dateTimeHelper.ConvertToUtcTime(model.DeliveryDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()),
                Quantity = model.Quantity,
                BrandId = model.BrandId,
                DeliveryAddress = model.DeliveryAddress,
                CountryId = model.CountryId,
                CityId = model.CityId,
                AreaId = model.AreaId,
                BusinessModelId = (int)BusinessModelEnum.Standard,
                PaymentDuration = model.PaymentTerms,
                Deleted = false,
                CreatedOnUtc = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddDays(category.ExpiryDays > 0 ? category.ExpiryDays : 2),
                Source = "Tijara",
                RequestTypeId = (int)RequestTypeEnum.External
            };
            await _requestService.InsertRequestAsync(request);

            //generate and set custom request number
            request.CustomRequestNumber = _customNumberFormatter.GenerateRequestCustomNumber(request);
            await _requestService.UpdateRequestAsync(request);

            await _customerActivityService.InsertActivityAsync("AddBuyerRequest",
                await _localizationService.GetResourceAsync("ActivityLog.AddBuyerRequest"), request);

            var totalUsedQuantity = await _requestService.GetRfqUsedQuantity(request.Id);
            var totalRfqRemainingQty = request.Quantity - totalUsedQuantity;

            bool isAvailable = false;
            decimal remainingQty = 0;

            var inventoryGroup = await _inventoryService.GetInventoryGroupByBrandIdAndProductIdAsync(request.ProductId, request.BrandId, request.ProductAttributeXml);
            if (inventoryGroup is not null)
            {
                var totalInventoryInboundQty = (await _inventoryService.GetAllInventoryInboundsAsync(groupId: inventoryGroup.Id)).Sum(x => x.StockQuantity);
                remainingQty = totalInventoryInboundQty - request.Quantity;
                isAvailable = true;
            }

            return new
            {
                success = true,
                message = string.Format(await _localizationService.GetResourceAsync("TijaraApp.CustomMessage"), isAvailable ? remainingQty : request.Quantity, (await _measureService.GetMeasureWeightByIdAsync(product.UnitId))?.Name ?? string.Empty, await _localizationService.GetLocalizedAsync(await _brandService.GetManufacturerByIdAsync(request.BrandId), x => x.Name)),
                inventoryAvailable = isAvailable,
                requestId = request.Id,
                inventoryRemainingQty = isAvailable ? remainingQty <= 0 ? Math.Abs(remainingQty) : remainingQty : request.Quantity,
                totalRfqRemainingQty
            };
        }


        public async Task<string> RejectRequest(RejectModel model)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                throw new Exception("user not found");

            if (model.Id <= 0)
                throw new Exception("Request id is required");

            var request = await _requestService.GetRequestByIdAsync(model.Id);
            if (request is null)
                throw new Exception("Request not found");

            if (string.IsNullOrWhiteSpace(model.RejectedReason))
                throw new Exception("Rejected reason is required");

            if (model.RejectedReason == "Other" && string.IsNullOrWhiteSpace(model.RejectedOtherReason))
                throw new Exception("Rejected other reason is required");

            request.RequestStatus = RequestStatus.Cancelled;

            if (model.RejectedReason == "Other" && !string.IsNullOrWhiteSpace(model.RejectedOtherReason))
                request.RejectedReason = $"Other - {model.RejectedOtherReason}";
            else
                request.RejectedReason = model.RejectedReason;

            await _requestService.UpdateRequestAsync(request);

            //Cancelled rfq
            var requestForQuotations = await _requestService.GetAllRequestForQuotationAsync(requestId: request.Id);
            requestForQuotations.Select(async requestForQuotation =>
            {
                requestForQuotation.RequestForQuotationStatus = RequestForQuotationStatus.Cancelled;
                await _requestService.UpdateRequestForQuotationAsync(requestForQuotation);

                //Cancelled quotation
                var quotations = await _quotationService.GetAllQuotationAsync(RfqId: requestForQuotation.Id);
                quotations.Select(async quotation =>
                {
                    quotation.QuotationStatus = QuotationStatus.Cancelled;
                    await _quotationService.UpdateQuotationAsync(quotation);
                }).ToList();

            }).ToList();
            //foreach (var requestForQuotation in requestForQuotations)
            //{
            //    requestForQuotation.RequestForQuotationStatus = RequestForQuotationStatus.Cancelled;
            //    await _requestService.UpdateRequestForQuotationAsync(requestForQuotation);

            //    //Cancelled quotation
            //    var quotations = await _quotationService.GetAllQuotationAsync(RfqId: requestForQuotation.Id);
            //    foreach (var quotation in quotations)
            //    {
            //        quotation.QuotationStatus = QuotationStatus.Cancelled;
            //        await _quotationService.UpdateQuotationAsync(quotation);
            //    }
            //}

            return await _localizationService.GetResourceAsync("RequestForQuotation.Rejected.Success");
        }


        public async Task<object> AddRequestForQuotation(RequestForQuotationModel model)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                throw new Exception("user not found");

            if (model.RequestId <= 0)
                throw new Exception("Request id is required");


            var request = await _requestService.GetRequestByIdAsync(model.RequestId);
            if (request is null)
                throw new Exception("Request not found");

            var totalUsedQuantity = await _requestService.GetRfqUsedQuantity(request.Id);
            var totalRemaining = request.Quantity - totalUsedQuantity;

            if (model.Quantity > totalRemaining || model.Quantity <= 0)
                throw new Exception("request.Quantity.Error");


            var requestForQuotation = new RequestForQuotation()
            {
                RequestId = request.Id,
                Quantity = model.Quantity,
                RfqStatusId = (int)RequestForQuotationStatus.Verified,
                CreatedOnUtc = DateTime.UtcNow,
                BookerId = user.Id,
                Source = "Tijara"
            };
            await _requestService.InsertRequestForQuotationAsync(requestForQuotation);

            //generate and set custom rfq number
            requestForQuotation.CustomRfqNumber = _customNumberFormatter.GenerateRequestForQuotationCustomNumber(requestForQuotation);
            await _requestService.UpdateRequestForQuotationAsync(requestForQuotation);

            //await _customerActivityService.InsertActivityAsync("AddRequestForQuotation",
            //    await _localizationService.GetResourceAsync("ActivityLog.AddRequestForQuotation"), request);

            return new { message = await _localizationService.GetResourceAsync("Buyer.RequestForQuotation.Add.Successfully"), rfqId = requestForQuotation.Id };

        }


        public async Task<object> RequestHistory(int pageIndex = 0, int pageSize = 10)
        {
            var booker = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(booker) /*|| !await _customerService.IsBookerAsync(booker)*/)
                throw new Exception("Booker not foun");

            List<Request> allBuyerRequests = new List<Request>();
            List<OrderManagementApiModel.Requests> activeBuyerRequests = new List<OrderManagementApiModel.Requests>();
            List<OrderManagementApiModel.Requests> pastBuyerRequets = new List<OrderManagementApiModel.Requests>();
            List<int> requestStatusIds = new List<int> { (int)RequestStatus.Verified, (int)RequestStatus.Processing, (int)RequestStatus.Approved, (int)RequestStatus.Complete, (int)RequestStatus.Cancelled, (int)RequestStatus.UnVerified, (int)RequestStatus.Expired };
            //Get all buyer request
            var requests = await _requestService.GetAllRequestAsync(pageIndex: pageIndex, pageSize: pageSize, bsIds: requestStatusIds/*, bookerId: booker.Id*/);
            allBuyerRequests = requests.ToList();
            if (allBuyerRequests.Any())
            {
                var currentTime = await _dateTimeHelper.ConvertToUserTimeAsync(DateTime.UtcNow, DateTimeKind.Utc);

                //Get Active Request List Filter By Date 
                var activeBuyerRequestDates = allBuyerRequests.OrderByDescending(x => x.CreatedOnUtc).Where(x => x.RequestStatusId == (int)RequestStatus.Verified || x.RequestStatusId == (int)RequestStatus.Processing || x.RequestStatusId == (int)RequestStatus.Approved || x.RequestStatusId == (int)RequestStatus.Complete).GroupBy(p => _dateTimeHelper.ConvertToUserTimeAsync(p.CreatedOnUtc, DateTimeKind.Utc).Result.ToString("dd/MM/yyyy")).ToList();
                await Task.WhenAll(activeBuyerRequestDates.Select(async activeBuyerRequestDate =>
                {
                    var model = new OrderManagementApiModel.Requests();
                    var yesterday = currentTime.AddDays(-1).ToString("dd/MM/yyyy");
                    if (currentTime.ToString("dd/MM/yyyy") == activeBuyerRequestDate.Key)
                        model.Date = "Today";
                    else if (currentTime.AddDays(-1).ToString("dd/MM/yyyy") == activeBuyerRequestDate.Key)
                        model.Date = "Yesterday";
                    else
                        model.Date = activeBuyerRequestDate.Key;

                    activeBuyerRequestDate.Select(async activeBuyerRequest =>
                    {
                        var product = await _productService.GetProductByIdAsync(activeBuyerRequest.ProductId);

                        var category = await _categoryService.GetCategoryByIdAsync(activeBuyerRequest.CategoryId);

                        var industry = await _industryService.GetIndustryByIdAsync(activeBuyerRequest.IndustryId);

                        var buyer = await _customerService.GetCustomerByIdAsync(activeBuyerRequest.BuyerId);

                        var totalUsedQuantity = await _requestService.GetRfqUsedQuantity(activeBuyerRequest.Id);
                        var totalRfqRemainingQty = activeBuyerRequest.Quantity - totalUsedQuantity;

                        bool isAvailable = false;
                        decimal remainingQty = 0;
                        int orderId = 0;
                        string customOrderNumber = "-";
                        string InventoryStatus = "";

                        var inventoryGroup = await _inventoryService.GetInventoryGroupByBrandIdAndProductIdAsync(activeBuyerRequest.ProductId, activeBuyerRequest.BrandId, activeBuyerRequest.ProductAttributeXml);
                        if (inventoryGroup is not null)
                        {
                            var totalInventoryInboundQty = (await _inventoryService.GetAllInventoryInboundsAsync(groupId: inventoryGroup.Id)).Sum(x => x.StockQuantity);
                            remainingQty = totalInventoryInboundQty - activeBuyerRequest.Quantity;
                            isAvailable = true;
                        }

                        if (activeBuyerRequest.RequestStatusId == (int)RequestStatus.Processing || activeBuyerRequest.RequestStatusId == (int)RequestStatus.Approved || activeBuyerRequest.RequestStatusId == (int)RequestStatus.Complete)
                        {
                            var order = await _orderService.GetOrderByRequestIdAsync(activeBuyerRequest.Id);
                            if (order is not null)
                            {
                                orderId = order.Id;
                                customOrderNumber = order.CustomOrderNumber;
                            }
                        }

                        if (inventoryGroup is null)
                            InventoryStatus = "Not Available";
                        if (remainingQty >= 0)
                            InventoryStatus = "Available";
                        else if (remainingQty < 0)
                            InventoryStatus = "Partially available";

                        model.Data.Add(new BuyerRequestData
                        {
                            Id = activeBuyerRequest.Id,
                            CustomRequestNumber = activeBuyerRequest.CustomRequestNumber,
                            ProductId = product.Id,
                            ProductName = product.Name,
                            IndustryId = industry.Id,
                            IndustryName = industry.Name,
                            BuyerId = buyer.Id,
                            BuyerName = buyer.FullName,
                            BrandName = activeBuyerRequest.BrandId > 0 ? await _localizationService.GetLocalizedAsync(await _brandService.GetManufacturerByIdAsync(activeBuyerRequest.BrandId), x => x.Name) : "-",
                            BrandId = activeBuyerRequest.BrandId,
                            Quantity = activeBuyerRequest.Quantity,
                            DeliveryAddress = activeBuyerRequest.DeliveryAddress,
                            ExpiryDate = activeBuyerRequest.ExpiryDate.HasValue ? (await _dateTimeHelper.ConvertToUserTimeAsync(activeBuyerRequest.ExpiryDate.Value, DateTimeKind.Utc)).ToString("dd/MM/yy hh:mm tt") : "",
                            StatusId = activeBuyerRequest.RequestStatusId,
                            Status = await _localizationService.GetLocalizedEnumAsync(activeBuyerRequest.RequestStatus),
                            AttributesInfo = await _utilityService.ParseAttributeXml(activeBuyerRequest.ProductAttributeXml),
                            InventoryStatus = InventoryStatus,
                            RemainingQuantity = isAvailable ? remainingQty : activeBuyerRequest.Quantity,
                            TotalRfqRemainingQty = totalRfqRemainingQty,
                            OrderId = orderId,
                            CustomOrderNumber = customOrderNumber,
                        });
                    }).ToList();
                    activeBuyerRequests.Add(model);
                }));

                //Get Active Request List Filter By Date 
                var pastBuyerRequests = allBuyerRequests.OrderByDescending(x => x.CreatedOnUtc).Where(x => x.RequestStatusId == (int)RequestStatus.UnVerified || x.RequestStatusId == (int)RequestStatus.Cancelled || x.RequestStatusId == (int)RequestStatus.Expired).GroupBy(p => _dateTimeHelper.ConvertToUserTimeAsync(p.CreatedOnUtc, DateTimeKind.Utc).Result.ToString("dd/MM/yyyy")).ToList();
                await Task.WhenAll(pastBuyerRequests.Select(async pastRequest =>
                {
                    var model = new OrderManagementApiModel.Requests();
                    if (currentTime.ToString("dd/MM/yyyy") == pastRequest.Key)
                        model.Date = "Today";
                    else if (currentTime.AddDays(-1).ToString("dd/MM/yyyy") == pastRequest.Key)
                        model.Date = "Yesterday";
                    else
                        model.Date = pastRequest.Key;

                    foreach (var pastBuyerRequest in pastRequest)
                    {
                        var product = await _productService.GetProductByIdAsync(pastBuyerRequest.ProductId);
                        if (product is null)
                            continue;

                        var category = await _categoryService.GetCategoryByIdAsync(pastBuyerRequest.CategoryId);
                        if (category is null)
                            continue;

                        var industry = await _industryService.GetIndustryByIdAsync(pastBuyerRequest.IndustryId);
                        if (industry is null)
                            continue;

                        var buyer = await _customerService.GetCustomerByIdAsync(pastBuyerRequest.BuyerId);
                        if (buyer is null)
                            continue;

                        var totalUsedQuantity = await _requestService.GetRfqUsedQuantity(pastBuyerRequest.Id);
                        var totalRfqRemainingQty = pastBuyerRequest.Quantity - totalUsedQuantity;

                        bool isAvailable = false;
                        decimal remainingQty = 0;
                        bool isGroup = false;
                        int orderId = 0;
                        string customOrderNumber = "-";
                        string InventoryStatus = "";

                        var inventoryGroup = await _inventoryService.GetInventoryGroupByBrandIdAndProductIdAsync(pastBuyerRequest.ProductId, pastBuyerRequest.BrandId, pastBuyerRequest.ProductAttributeXml);
                        if (inventoryGroup is not null)
                        {
                            var totalInventoryInboundQty = (await _inventoryService.GetAllInventoryInboundsAsync(groupId: inventoryGroup.Id)).Sum(x => x.StockQuantity);
                            remainingQty = pastBuyerRequest.Quantity - totalInventoryInboundQty;
                            isAvailable = true;
                        }

                        if (pastBuyerRequest.RequestStatusId == (int)RequestStatus.Processing || pastBuyerRequest.RequestStatusId == (int)RequestStatus.Approved || pastBuyerRequest.RequestStatusId == (int)RequestStatus.Complete)
                        {
                            var order = await _orderService.GetOrderByRequestIdAsync(pastBuyerRequest.Id);
                            if (order is not null)
                            {
                                orderId = order.Id;
                                customOrderNumber = order.CustomOrderNumber;
                            }
                        }

                        if (inventoryGroup is null)
                            InventoryStatus = "Not Available";
                        if (remainingQty >= 0)
                            InventoryStatus = "Available";
                        else if (remainingQty < 0)
                            InventoryStatus = "Partially available";

                        model.Data.Add(new BuyerRequestData
                        {
                            Id = pastBuyerRequest.Id,
                            CustomRequestNumber = pastBuyerRequest.CustomRequestNumber,
                            ProductId = product.Id,
                            ProductName = product.Name,
                            IndustryId = industry.Id,
                            IndustryName = industry.Name,
                            BuyerId = buyer.Id,
                            BuyerName = buyer.FullName,
                            BrandName = pastBuyerRequest.BrandId > 0 ? await _localizationService.GetLocalizedAsync(await _brandService.GetManufacturerByIdAsync(pastBuyerRequest.BrandId), x => x.Name) : "-",
                            BrandId = pastBuyerRequest.BrandId,
                            Quantity = pastBuyerRequest.Quantity,
                            DeliveryAddress = pastBuyerRequest.DeliveryAddress,
                            //TotalQuotations = (await _sellerBidService.GetAllSellerBidAsync(buyerRequestId: pastBuyerRequest.Id, sbIds: new List<int> { (int)RequestStatus.QuotedToBuyer, (int)RequestStatus. })).Count,
                            ExpiryDate = pastBuyerRequest.ExpiryDate.HasValue ? (await _dateTimeHelper.ConvertToUserTimeAsync(pastBuyerRequest.ExpiryDate.Value, DateTimeKind.Utc)).ToString("dd/MM/yy hh:mm tt") : "",
                            StatusId = pastBuyerRequest.RequestStatusId,
                            Status = await _localizationService.GetLocalizedEnumAsync(pastBuyerRequest.RequestStatus),
                            AttributesInfo = await _utilityService.ParseAttributeXml(pastBuyerRequest.ProductAttributeXml),
                            InventoryStatus = InventoryStatus,
                            RemainingQuantity = isAvailable ? remainingQty : pastBuyerRequest.Quantity,
                            TotalRfqRemainingQty = totalRfqRemainingQty,
                            OrderId = orderId,
                            CustomOrderNumber = customOrderNumber,
                        });
                    }
                    pastBuyerRequets.Add(model);
                }));

                var data = new List<object>();
                if (activeBuyerRequests.Any())
                {
                    data.Add(new
                    {
                        DataType = await _localizationService.GetResourceAsync("Buyer.ActiveRequestForQuotation"),
                        Data = activeBuyerRequests
                    });
                }
                if (pastBuyerRequets.Any())
                {
                    data.Add(new
                    {
                        DataType = await _localizationService.GetResourceAsync("Buyer.PastRequestForQuotation"),
                        Data = pastBuyerRequets
                    });
                }
                return new { success = true, message = "", data, totalPages = requests.TotalPages, currentPage = requests.PageIndex };
            }

            return new { success = false, message = "" };
        }

        public async Task<object> GetBuyerRequest(int requestId)
        {
            if (requestId <= 0)
                throw new Exception("Buyer request id is required");

            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                throw new Exception("user not found");

            var buyerRequest = await _requestService.GetRequestByIdAsync(requestId);
            if (buyerRequest is null)
                throw new Exception("Buyer request not found");

            var product = await _productService.GetProductByIdAsync(buyerRequest.ProductId);
            if (product is null || product.Id != buyerRequest.ProductId)
                throw new Exception("Buyer request product not found");

            var industry = await _industryService.GetIndustryByIdAsync(buyerRequest.IndustryId);
            if (industry is null)
                throw new Exception("Buyer request industry not found");

            var data = new
            {
                buyerRequest.Id,
                buyerRequest.CustomRequestNumber,
                buyerRequest.BuyerId,
                BuyerName = await _customerService.GetCustomerFullNameAsync(buyerRequest.BuyerId),
                ProductId = product.Id,
                ProductName = product.Name,
                IndustryId = industry.Id,
                IndustryName = industry.Name,
                Brand = buyerRequest.BrandId > 0 ? await _localizationService.GetLocalizedAsync(await _brandService.GetManufacturerByIdAsync(buyerRequest.BrandId), x => x.Name) : "-",
                buyerRequest.BrandId,
                Qty = buyerRequest.Quantity,
                QtyType = (await _measureService.GetMeasureWeightByIdAsync(product.UnitId))?.Name ?? string.Empty,
                buyerRequest.DeliveryAddress,
                DeliveryDate = (await _dateTimeHelper.ConvertToUserTimeAsync(buyerRequest.DeliveryDate, DateTimeKind.Utc)).ToString("dd MMM, yyyy"),
                //DeliveryDate = (await _dateTimeHelper.ConvertToUserTimeAsync(buyerRequest.DeliveryDate, DateTimeKind.Utc)).ToString("ddd MMM MM yyyy"),
                //TotalQuotations = await _directOrderCalculationService.GetQuotationsCountAsync(bsIds: new List<int> { (int)QuotationStatus.Verified }, buyerRequestId: buyerRequest.Id),
                StatusId = buyerRequest.RequestStatusId,
                Status = await _localizationService.GetLocalizedEnumAsync(buyerRequest.RequestStatus),
                //Comment = buyerRequest.Comment,
                ProductAttributesInfo = await _utilityService.ParseAttributeXml(buyerRequest.ProductAttributeXml),
            };
            return new { success = true, message = "", data };

        }
        public async Task<object> GetAllRequests(int industryId, int pageIndex = 0, int pageSize = 10)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                throw new Exception("user not found");

            if (industryId <= 0)
                throw new Exception("Industry id is required");

            var industry = await _industryService.GetIndustryByIdAsync(industryId);
            if (industry == null)
                throw new Exception("Industry not found");

            List<object> list = new List<object>();
            //IPagedList<object> pagedList = new PagedList<object>(list, pageIndex, pageSize);

            var requests = await _requestService.GetAllRequestAsync(pageIndex: pageIndex, pageSize: pageSize, bsIds: new List<int> { (int)RequestStatus.Verified, (int)RequestStatus.Processing, (int)RequestStatus.Approved, (int)RequestStatus.Complete },
                industryId: industryId, excludeRfqForApp: true);
            await Task.WhenAll(requests.Select(async request =>
            {
                var totalUsedQuantity = await _requestService.GetRfqUsedQuantity(request.Id);
                var totalRfqRemainingQty = request.Quantity - totalUsedQuantity;

                var category = await _categoryService.GetCategoryByIdAsync(request.CategoryId);

                var product = await _productService.GetProductByIdAsync(request.ProductId);

                var buyer = await _customerService.GetCustomerByIdAsync(request.BuyerId);

                bool isAvailable = false;
                decimal remainingQty = 0;
                bool isGroup = false;

                string InventoryStatus = "";

                var inventoryGroup = await _inventoryService.GetInventoryGroupByBrandIdAndProductIdAsync(request.ProductId, request.BrandId, request.ProductAttributeXml);
                if (inventoryGroup is not null)
                {
                    var totalInventoryInboundQty = (await _inventoryService.GetAllInventoryInboundsAsync(groupId: inventoryGroup.Id)).Sum(x => x.StockQuantity);
                    remainingQty = request.Quantity - totalInventoryInboundQty;
                    if (remainingQty >= 0)
                        isAvailable = true;
                    isGroup = true;
                }

                if (inventoryGroup is null)
                    InventoryStatus = "Not Available";
                if (remainingQty >= 0)
                    InventoryStatus = "Available";
                else if (remainingQty < 0)
                    InventoryStatus = "Partially available";

                list.Add(new
                {
                    request.Id,
                    request.CustomRequestNumber,
                    ProductId = product.Id,
                    ProductName = product.Name,
                    IndustryId = industry.Id,
                    IndustryName = industry.Name,
                    BuyerId = buyer.Id,
                    BuyerName = buyer.FullName,
                    BrandName = request.BrandId > 0 ? await _localizationService.GetLocalizedAsync(await _brandService.GetManufacturerByIdAsync(request.BrandId), x => x.Name) : "-",
                    request.BrandId,
                    request.Quantity,
                    request.DeliveryAddress,
                    ExpiryDate = request.ExpiryDate.HasValue ? (await _dateTimeHelper.ConvertToUserTimeAsync(request.ExpiryDate.Value, DateTimeKind.Utc)).ToString("dd/MM/yy hh:mm tt") : "",
                    StatusId = request.RequestStatusId,
                    Status = await _localizationService.GetLocalizedEnumAsync(request.RequestStatus),
                    AttributesInfo = await _utilityService.ParseAttributeXml(request.ProductAttributeXml),
                    InventoryStatus,
                    RemainingQuantity = isAvailable ? remainingQty : request.Quantity,
                    TotalRfqRemainingQty = totalRfqRemainingQty,
                });
            }));

            return new { success = true, message = "", data = list, totalPages = requests.TotalPages, currentPage = requests.PageIndex };
        }
        public async Task<object> GetPurchaseOrderDetailForDeliveryRequest(int expectedShipmentId)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                throw new Exception("user not found");

            var expectedShipment = await _orderService.GetDeliveryScheduleByIdAsync(expectedShipmentId);
            if (expectedShipment is null)
                throw new Exception("Expected shipment not found");

            var order = await _orderService.GetOrderByIdAsync(expectedShipment.OrderId);
            if (order is null)
                throw new Exception("Order not found");

            var orderItem = (await _orderService.GetOrderItemsAsync(order.Id)).FirstOrDefault();
            if (orderItem is null)
                throw new Exception("Order item not found");

            var quotation = await _quotationService.GetQuotationByIdAsync(order.QuotationId.Value);
            if (quotation is null)
                throw new Exception("Quotation not found");

            var buyerRequest = await _requestService.GetRequestByIdAsync(order.RequestId);
            if (buyerRequest is null)
                throw new Exception("Buyer Request not found");

            var product = await _productService.GetProductByIdAsync(buyerRequest.ProductId);
            if (product is null || product.Id != buyerRequest.ProductId)
                throw new Exception("Buyer request product not found");

            var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);
            if (customer is null)
                throw new Exception("Customer not found");

            var data = new object();
            data = new
            {
                orderId = order.Id,
                customOrderNumber = order.CustomOrderNumber,
                buyerId = customer.Id,
                buyerName = customer.FullName,
                //suplierId = customer.Id,
                //suplierName = customer.FullName,
                brand = buyerRequest.BrandId > 0 ? await _localizationService.GetLocalizedAsync(await _brandService.GetManufacturerByIdAsync(buyerRequest.BrandId), x => x.Name) : "-",
                leftQuantity = orderItem.Quantity - _orderService.GetTotalOrderDeliveryRequestQuantityByOrderIdAsync(order.Id, expectedShipment.Id),
                //leftQuantity = expectedShipment.ExpectedQuantity - remaining,
                totalQuantity = quotation.Quantity,
                expectedShipmentQuantity = expectedShipment.ExpectedQuantity,
                ProductName = product.Name,
                ProductAttributesInfo = await _utilityService.ParseAttributeXml(buyerRequest.ProductAttributeXml),
                countryId = buyerRequest.CountryId,
                stateId = buyerRequest.CityId,
                areaId = buyerRequest.AreaId,
                streetAddress = buyerRequest.DeliveryAddress,
                contactNo = customer.Phone
            };

            return new { success = true, data };
        }

        public async Task<object> GetSaleOrderDetailForDeliveryRequest(int expectedShipmentId)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                throw new Exception("user not found");

            var expectedShipment = await _orderService.GetDeliveryScheduleByIdAsync(expectedShipmentId);
            if (expectedShipment is null)
                throw new Exception("Expected shipment not found");

            var order = await _orderService.GetOrderByIdAsync(expectedShipment.OrderId);
            if (order is null)
                throw new Exception("Order not found");

            var orderItem = (await _orderService.GetOrderItemsAsync(order.Id)).FirstOrDefault();
            if (orderItem is null)
                throw new Exception("Order item not found");

            var buyerRequest = await _requestService.GetRequestByIdAsync(order.RequestId);
            if (buyerRequest is null)
                throw new Exception("Buyer Request not found");

            var product = await _productService.GetProductByIdAsync(buyerRequest.ProductId);
            if (product is null || product.Id != buyerRequest.ProductId)
                throw new Exception("Buyer request product not found");

            var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);
            if (customer is null)
                throw new Exception("Customer not found");


            var data = new
            {
                orderId = order.Id,
                customOrderNumber = order.CustomOrderNumber,
                buyerId = customer.Id,
                buyerName = customer.FullName,
                brand = buyerRequest.BrandId > 0 ? await _localizationService.GetLocalizedAsync(await _brandService.GetManufacturerByIdAsync(buyerRequest.BrandId), x => x.Name) : "-",
                leftQuantity = orderItem.Quantity - _orderService.GetTotalOrderDeliveryRequestQuantityByOrderIdAsync(order.Id, expectedShipment.Id),
                totalQuantity = buyerRequest.Quantity,
                expectedShipmentQuantity = expectedShipment.ExpectedQuantity,
                ProductName = product.Name,
                ProductAttributesInfo = await _utilityService.ParseAttributeXml(buyerRequest.ProductAttributeXml),
                countryId = buyerRequest.CountryId,
                stateId = buyerRequest.CityId,
                areaId = buyerRequest.AreaId,
                streetAddress = buyerRequest.DeliveryAddress,
                contactNo = customer.Phone
            };

            return new { success = true, data };
        }


        public async Task<object> AddShipmentRequest(OrderDeliveryRequestModel model)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                throw new Exception("user not found");


            var deliveryScedule = await _orderService.GetDeliveryScheduleByIdAsync(model.ExpectedShipmentId);
            if (deliveryScedule is null)
                throw new Exception("Expected shipment not found");

            var order = await _orderService.GetOrderByIdAsync(deliveryScedule.OrderId);
            if (order is null || order.Deleted)
                throw new Exception("Order not found");

            var orderItem = (await _orderService.GetOrderItemsAsync(order.Id)).FirstOrDefault();
            if (orderItem is null)
                throw new Exception("Order item not found");

            var buyerRequest = await _requestService.GetRequestByIdAsync(order.RequestId);
            if (buyerRequest is null)
                throw new Exception("Buyer Request not found");


            decimal totalLeftQuantity = orderItem.Quantity - _orderService.GetTotalOrderDeliveryRequestQuantityByOrderIdAsync(order.Id, deliveryScedule.Id);
            //decimal totalLeftQuantity = deliveryScedule.ExpectedQuantity - remaining;
            if (totalLeftQuantity <= 0)
                throw new Exception("There is no left quantity");

            if (model.Quantity > totalLeftQuantity)
                return new { success = false, message = "Left Quantity ", leftQuantity = totalLeftQuantity };

            var industry = await _industryService.GetIndustryByIdAsync(buyerRequest.IndustryId);
            if (industry is null)
                throw new Exception("Industry not found");

            var category = await _categoryService.GetCategoryByIdAsync(buyerRequest.CategoryId);
            if (category is null)
                throw new Exception("Category not found");

            var orderDeliveryRequest = new OrderDeliveryRequest
            {
                OrderId = order.Id,
                OrderDeliveryScheduleId = deliveryScedule.Id,
                StatusId = (int)OrderDeliveryRequestEnum.Pending,
                BagsDirectlyFromSupplier = model.BagsDirectlyFromSupplier,
                BagsDirectlyFromWarehouse = model.BagsDirectlyFromWarehouse,
                WarehouseId = model.WarehouseId,
                CountryId = model.CountryId,
                CityId = model.CityId,
                AreaId = model.AreaId,
                StreetAddress = model.StreetAddress,
                ContactNumber = model.ContactNumber,
                Quantity = model.Quantity,
                //ShipmentDateUtc = model.ShipmentDateUtc,
                Deleted = false,
                AgentId = model.AgentId,
                TicketExpiryDate = DateTime.UtcNow.AddDays(category.TicketExpiryDays > 0 ? category.TicketExpiryDays : 1),
                TicketPirority = category.TicketPirority > 0 ? category.TicketPirority : (int)TicketEnum.Medium
            };
            await _orderService.InsertOrderDeliveryRequestAsync(orderDeliveryRequest);

            return new { success = true, message = await _localizationService.GetResourceAsync("Order.Delivery.Request.Created.Success") };

        }

        public async Task<object> SaleOrderDeliveryRequestDeatil(int expectedShipmentRequestId, string type = "ExpectedShipment")
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                throw new Exception("user not found");

            if (string.IsNullOrWhiteSpace(type))
                throw new Exception("type is required");

            var data = new object();
            if (type == "ExpectedShipment")
            {
                var deliveryRequest = await _orderService.GetOrderDeliveryRequestByIdAsync(expectedShipmentRequestId);
                if (deliveryRequest is null)
                    throw new Exception("delivery request not found");

                var order = await _orderService.GetOrderByIdAsync(deliveryRequest.OrderId);
                if (order is null)
                    throw new Exception("delivery request order not found");

                var buyerRequest = await _requestService.GetRequestByIdAsync(order.RequestId);
                if (buyerRequest is null)
                    throw new Exception("Buyer Request not found");

                var product = await _productService.GetProductByIdAsync(buyerRequest.ProductId);
                if (product is null || product.Id != buyerRequest.ProductId)
                    throw new Exception("Buyer request product not found");

                data = new
                {
                    id = deliveryRequest.Id,
                    orderId = deliveryRequest.OrderId,
                    orderCustomNumber = order.CustomOrderNumber,
                    shipmentId = deliveryRequest.StatusId == (int)OrderDeliveryRequestEnum.Complete ? (await _shipmentService.GetShipmentByDeliveryRequestId(deliveryRequest.Id))?.Id : 0,
                    wareHouseId = deliveryRequest.WarehouseId,
                    wareHouseName = deliveryRequest.WarehouseId > 0 ? (await _shippingService.GetWarehouseByIdAsync(deliveryRequest.WarehouseId)).Name : "",
                    statusId = deliveryRequest.StatusId,
                    status = await _localizationService.GetLocalizedEnumAsync(deliveryRequest.OrderDeliveryRequestEnum),
                    bagsDirectlyFromSupplier = deliveryRequest.BagsDirectlyFromSupplier,
                    bagsDirectlyFromWarehouse = deliveryRequest.BagsDirectlyFromWarehouse,
                    countryName = await _localizationService.GetLocalizedAsync(await _countryService.GetCountryByIdAsync(deliveryRequest.CountryId), x => x.Name),
                    countryId = deliveryRequest.CountryId,
                    cityName = await _localizationService.GetLocalizedAsync(await _stateProvinceService.GetStateProvinceByIdAsync(deliveryRequest.CityId), x => x.Name),
                    cityId = deliveryRequest.CityId,
                    areaName = await _localizationService.GetLocalizedAsync(await _stateProvinceService.GetStateProvinceByIdAsync(deliveryRequest.AreaId), x => x.Name),
                    areaId = deliveryRequest.AreaId,
                    streetaddress = deliveryRequest.StreetAddress,
                    contactnumber = deliveryRequest.ContactNumber,
                    totalQuantity = buyerRequest.Quantity,
                    deliveredQuantity = deliveryRequest.Quantity,
                    agentId = deliveryRequest.AgentId,
                    agent = (await _customerService.GetCustomerByIdAsync(deliveryRequest.AgentId))?.FullName,
                    timeRemaining = deliveryRequest.TicketExpiryDate.HasValue ? deliveryRequest.TicketExpiryDate.Value.Subtract(DateTime.Now).TotalHours > 0 || deliveryRequest.TicketExpiryDate.Value.Subtract(DateTime.Now).TotalMinutes > 0 ? deliveryRequest.TicketExpiryDate.Value.Subtract(DateTime.Now).ToString("hh") + " : hrs " + deliveryRequest.TicketExpiryDate.Value.Subtract(DateTime.Now).ToString("mm") + ": mns" : "0" : "0",
                    priority = await _localizationService.GetLocalizedEnumAsync(deliveryRequest.TicketEnum),
                    requester = (await _customerService.GetCustomerByIdAsync(deliveryRequest.CreatedById))?.FullName,
                    buyerName = (await _customerService.GetCustomerByIdAsync(customerId: buyerRequest.BuyerId))?.FullName,
                    brand = buyerRequest.BrandId > 0 ? await _localizationService.GetLocalizedAsync(await _brandService.GetManufacturerByIdAsync(buyerRequest.BrandId), x => x.Name) : "-",
                    rejectedReason = deliveryRequest.RejectedReason,
                    productName = product.Name,
                    productAttributesInfo = await _utilityService.ParseAttributeXml(buyerRequest.ProductAttributeXml),
                };
            }

            if (type == "Shipment")
            {
                var shipment = await _shipmentService.GetShipmentByIdAsync(expectedShipmentRequestId);
                if (shipment is null)
                    throw new Exception("Shipment not found");

                var deliveryRequest = await _orderService.GetOrderDeliveryRequestByIdAsync(shipment.DeliveryRequestId.Value);
                if (deliveryRequest is null)
                    throw new Exception("delivery request not found");

                var order = await _orderService.GetOrderByIdAsync(deliveryRequest.OrderId);
                if (order is null)
                    throw new Exception("delivery request order not found");

                var request = await _requestService.GetRequestByIdAsync(order.RequestId);
                if (request is null)
                    throw new Exception("Buyer Request not found");

                var product = await _productService.GetProductByIdAsync(request.ProductId);
                if (product is null || product.Id != request.ProductId)
                    throw new Exception("Buyer request product not found");

                var inventories = new List<object>();

                //Prepare inventories
                var inventoryGroup = await _inventoryService.GetInventoryGroupByBrandIdAndProductIdAsync(request.ProductId, request.BrandId, request.ProductAttributeXml);
                if (inventoryGroup is not null)
                {
                    var inventoryInbounds = new List<InventoryInbound>();

                    if (request.BusinessModelId == (int)BusinessModelEnum.Broker)
                        inventoryInbounds = (await _inventoryService.GetAllInventoryInboundsAsync(groupId: inventoryGroup.Id)).Where(x => x.InventoryInboundStatusEnum == InventoryInboundStatusEnum.Physical && x.BusinessModelId == (int)BusinessModelEnum.Broker).OrderBy(x => x.Id).ToList();
                    else
                        inventoryInbounds = (await _inventoryService.GetAllInventoryInboundsAsync(groupId: inventoryGroup.Id)).Where(x => x.InventoryInboundStatusEnum == InventoryInboundStatusEnum.Physical && x.BusinessModelId != (int)BusinessModelEnum.Broker).OrderBy(x => x.Id).ToList();

                    foreach (var inventoryInbound in inventoryInbounds)
                    {
                        if (inventoryInbound.ShipmentId.HasValue)
                        {
                            var inventoryShipment = await _shipmentService.GetShipmentByIdAsync(inventoryInbound.ShipmentId.Value);
                            if (inventoryShipment is not null)
                            {
                                if (inventoryShipment.IsDirectOrder && inventoryInbound.InventoryInboundStatusEnum == InventoryInboundStatusEnum.Virtual)
                                {
                                    if (inventoryShipment.BuyerId != order.CustomerId)
                                        continue;
                                }
                            }
                        }

                        var outboundQty = (await _inventoryService.GetAllInventoryOutboundsAsync(InventoryInboundId: inventoryInbound.Id)).Sum(x => x.OutboundQuantity);
                        var balanceQuantity = inventoryInbound.StockQuantity - outboundQty;
                        if (balanceQuantity > 0)
                            inventories.Add(new { InventoryId = inventoryInbound.Id, TotalQuantity = inventoryInbound.StockQuantity, BalanceQuantity = balanceQuantity, OutboundQuantity = 0 });
                    }
                }
                var picture = await _pictureService.GetPictureByIdAsync(shipment.PictureId);
                data = new
                {
                    id = deliveryRequest.Id,
                    orderId = deliveryRequest.OrderId,
                    orderCustomNumber = order.CustomOrderNumber,
                    wareHouseId = shipment.WarehouseId,
                    wareHouseName = shipment.WarehouseId > 0 ? (await _shippingService.GetWarehouseByIdAsync(shipment.WarehouseId)).Name : "",
                    statusId = deliveryRequest.StatusId,
                    status = await _localizationService.GetLocalizedEnumAsync(deliveryRequest.OrderDeliveryRequestEnum),
                    bagsDirectlyFromSupplier = deliveryRequest.BagsDirectlyFromSupplier,
                    bagsDirectlyFromWarehouse = deliveryRequest.BagsDirectlyFromWarehouse,
                    countryName = await _localizationService.GetLocalizedAsync(await _countryService.GetCountryByIdAsync(deliveryRequest.CountryId), x => x.Name),
                    countryId = deliveryRequest.CountryId,
                    cityName = await _localizationService.GetLocalizedAsync(await _stateProvinceService.GetStateProvinceByIdAsync(deliveryRequest.CityId), x => x.Name),
                    cityId = deliveryRequest.CityId,
                    areaName = await _localizationService.GetLocalizedAsync(await _stateProvinceService.GetStateProvinceByIdAsync(deliveryRequest.AreaId), x => x.Name),
                    areaId = deliveryRequest.AreaId,
                    streetaddress = deliveryRequest.StreetAddress,
                    contactnumber = deliveryRequest.ContactNumber,
                    totalQuantity = request.Quantity,
                    deliveredQuantity = deliveryRequest.Quantity,
                    shipmentdate = (await _dateTimeHelper.ConvertToUserTimeAsync(deliveryRequest.CreatedOnUtc, DateTimeKind.Utc)).ToString("MM/dd/yyyy h:mm:ss tt") /*deliveryRequest.ShipmentDateUtc*/,
                    agentId = deliveryRequest.AgentId,
                    agent = (await _customerService.GetCustomerByIdAsync(deliveryRequest.AgentId))?.FullName,
                    timeRemaining = deliveryRequest.TicketExpiryDate.HasValue ? deliveryRequest.TicketExpiryDate.Value.Subtract(DateTime.Now).TotalHours > 0 || deliveryRequest.TicketExpiryDate.Value.Subtract(DateTime.Now).TotalMinutes > 0 ? deliveryRequest.TicketExpiryDate.Value.Subtract(DateTime.Now).ToString("hh") + " : hrs " + deliveryRequest.TicketExpiryDate.Value.Subtract(DateTime.Now).ToString("mm") + ": mns" : "0" : "0",
                    priority = await _localizationService.GetLocalizedEnumAsync(deliveryRequest.TicketEnum),
                    requester = (await _customerService.GetCustomerByIdAsync(deliveryRequest.CreatedById))?.FullName,
                    buyerName = (await _customerService.GetCustomerByIdAsync(customerId: request.BuyerId))?.FullName,
                    brand = request.BrandId > 0 ? await _localizationService.GetLocalizedAsync(await _brandService.GetManufacturerByIdAsync(request.BrandId), x => x.Name) : "-",
                    rejectedReason = deliveryRequest.RejectedReason,
                    productName = product.Name,
                    productAttributesInfo = await _utilityService.ParseAttributeXml(request.ProductAttributeXml),
                    canShip = shipment.ShippedDateUtc.HasValue,
                    canDelivered = shipment.DeliveryDateUtc.HasValue,
                    inventories,

                    //Shipped Detail
                    shippedDetail = new
                    {
                        expectedDateShipped = shipment.ExpectedDateShipped.HasValue ? (await _dateTimeHelper.ConvertToUserTimeAsync(shipment.ExpectedDateShipped.Value, DateTimeKind.Utc)).ToString() : "",
                        expectedDateDelivered = shipment.ExpectedDateDelivered.HasValue ? (await _dateTimeHelper.ConvertToUserTimeAsync(shipment.ExpectedDateDelivered.Value, DateTimeKind.Utc)).ToString() : "",
                        expectedDeliveryCost = shipment.ExpectedDeliveryCost,
                        expectedQuantity = shipment.ExpectedQuantity,
                        transporterId = shipment.TransporterId,
                        transporterName = shipment.TransporterId > 0 ? (await _customerService.GetCustomerByIdAsync(shipment.TransporterId))?.FullName : "",
                        vehicleId = shipment.VehicleId,
                        transportvehicleName = await _customerService.GetTransporterVehicleMappingByIdAsync(shipment.VehicleId) != null ? $"{(await _customerService.GetVehiclePortfolioByIdAsync((await _customerService.GetTransporterVehicleMappingByIdAsync(shipment.VehicleId)).VehicleId)).Name} - {(await _customerService.GetTransporterVehicleMappingByIdAsync(shipment.VehicleId))?.VehicleNumber}" : null,
                        vehicleNumber = shipment.VehicleNumber,
                        routeType = shipment.RouteTypeId > 0 ? await _localizationService.GetLocalizedEnumAsync(shipment.RouteType) : "",
                        deliveryStatus = await _localizationService.GetLocalizedEnumAsync(shipment.DeliveryStatus),
                        pickupAddress = shipment.PickupAddress,
                        actualShippedQuantity = shipment.ActualShippedQuantity,
                        actualShippedQuantityReason = shipment.ActualShippedQuantityReason,
                        costOnZaraye = shipment.CostOnZarayeId > 0 ? await _localizationService.GetLocalizedEnumAsync(shipment.CostOnZaraye) : "",
                        shippedDateUtc = shipment.ShippedDateUtc.HasValue ? (await _dateTimeHelper.ConvertToUserTimeAsync(shipment.ShippedDateUtc.Value, DateTimeKind.Utc)).ToString() : "",
                    },

                    //Delivered Detail
                    deliveredDetail = new
                    {
                        deliveryDateUtc = shipment.DeliveryDateUtc.HasValue ? (await _dateTimeHelper.ConvertToUserTimeAsync(shipment.DeliveryDateUtc.Value, DateTimeKind.Utc)).ToString() : "",
                        shipmentDeliveryAddress = shipment.ShipmentDeliveryAddress,
                        picture = shipment.PictureId > 0 ? await _pictureService.GetPictureUrlAsync(shipment.PictureId) : "",
                        fullPictureUrl = picture != null ? (await _pictureService.GetPictureUrlAsync(picture)).Url : "",
                        transporterType = shipment.TransporterTypeId > 0 ? await _localizationService.GetLocalizedEnumAsync(shipment.TransporterType) : "",
                        deliveryType = shipment.DeliveryTypeId > 0 ? await _localizationService.GetLocalizedEnumAsync(shipment.DeliveryType) : "",
                        deliveryTiming = shipment.DeliveryTimingId > 0 ? await _localizationService.GetLocalizedEnumAsync(shipment.DeliveryTimingEnum) : "",
                        deliveryDelayedReason = shipment.DeliveryDelayedReasonId > 0 ? (await _shipmentService.GetDeliveryTimeReasonByIdAsync(shipment.DeliveryDelayedReasonId))?.Name : "",
                        deliveryCostType = shipment.DeliveryCostTypeId > 0 ? await _localizationService.GetLocalizedEnumAsync(shipment.DeliveryCostType) : "",
                        deliveryCost = shipment.DeliveryCost,
                        warehouseId = shipment.WarehouseId,
                        freightCharges = shipment.FreightCharges,
                        labourCharges = shipment.LabourCharges,
                        deliveryCostReason = shipment.DeliveryCostReasonId > 0 ? (await _shipmentService.GetDeliveryCostReasonByIdAsync(shipment.DeliveryCostReasonId))?.Name : "",
                        actualDeliveredQuantity = shipment.ActualDeliveredQuantity,
                        actualDeliveredQuantityReason = shipment.ActualDeliveredQuantityReason,
                        labourType = shipment.LabourTypeId > 0 ? await _localizationService.GetLocalizedEnumAsync(shipment.LabourType) : "",
                    },
                };
            }

            return new { success = true, data };
        }

        #endregion

        #region Direct Sale Order


        public async Task<object> CheckDirectSaleOrderExist(int requestId)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                throw new Exception("user not found");

            var request = await _requestService.GetRequestByIdAsync(requestId);
            if (request is null)
                throw new Exception("Request not found");

            var directOrder = await _orderService.GetDirectOrderByRequestId(request.Id);
            if (directOrder is null)
                return new { success = true, message = "Data not found", data = false };

            return new { success = true, message = "Data found", data = true, directOrderId = directOrder.Id };
        }

        public async Task<object> DirectSaleOrderProcess(int requestId)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                throw new Exception("user not found");

            var request = await _requestService.GetRequestByIdAsync(requestId);
            if (request is null)
                throw new Exception("Buyer request not found");

            var directOrder = await _orderService.GetDirectOrderByRequestId(request.Id);
            if (directOrder is not null)
            {
                var directOrderDeliverySchedules = await _orderService.GetAllDirectOrderDeliveryScheduleAsync(directOrder.Id);
                foreach (var directOrderDeliverySchedule in directOrderDeliverySchedules)
                    await _orderService.DeleteDirectOrderDeliveryScheduleAsync(directOrderDeliverySchedule);

                //Remove direct order supplier info
                var directOrderCalculations = await _orderService.GetAllDirectOrderCalculationAsync(directOrder.Id);
                foreach (var directOrderCalculation in directOrderCalculations)
                    await _orderService.DeleteDirectOrderCalculationAsync(directOrderCalculation);

                var directCogsInventoryTaggings = await _inventoryService.GetAllDirectCogsInventoryTaggingsAsync(requestId: request.Id);
                foreach (var tagging in directCogsInventoryTaggings)
                {
                    //delete Direct Cogs Inventory Taggings
                    await _inventoryService.DeleteDirectCogsInventoryTaggingAsync(tagging);
                }

                await _orderService.DeleteDirectOrderAsync(directOrder);
            }
            //New Direct Order
            var newDirectOrder = new DirectOrder
            {
                BookerId = user.Id,
                BuyerId = request.BuyerId,
                RequestId = request.Id,
                TransactionModelId = request.BusinessModelId,
                OrderTypeId = (int)OrderType.SaleOrder
            };
            await _orderService.InsertDirectOrderAsync(newDirectOrder);
            //Add default direct order delivery schedule 
            await _orderService.InsertDirectOrderDeliveryScheduleAsync(new DirectOrderDeliverySchedule
            {
                CreatedOnUtc = DateTime.UtcNow,
                ExpectedDeliveryDateUtc = null,
                ExpectedShipmentDateUtc = null,
                ExpectedDeliveryCost = 0,
                ExpectedQuantity = null,
                DirectOrderId = newDirectOrder.Id
            });
            //Add default direct order calculation 
            await _orderService.InsertDirectOrderCalculationAsync(new DirectOrderCalculation
            {
                CreatedOnUtc = DateTime.UtcNow,
                DirectOrderId = newDirectOrder.Id,
                Quantity = request.Quantity
            });
            return new { success = true, message = await _localizationService.GetResourceAsync("New.Direct.Order.Created"), directOrderId = newDirectOrder.Id };

        }

        public async Task<object> GetSaleOrderFormLoad(int requestId)
        {
            if (requestId <= 0)
                throw new Exception("Buyer request id is required");

            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                throw new Exception("user not found");

            var buyerRequest = await _requestService.GetRequestByIdAsync(requestId);
            if (buyerRequest is null)
                throw new Exception("Buyer request not found");

            var directOrder = await _orderService.GetDirectOrderByRequestId(buyerRequest.Id);
            if (directOrder is null)
                throw new Exception("Direct order not found");

            var data = new
            {
                requestId = buyerRequest.Id,
                countryId = buyerRequest.CountryId,
                countryName = buyerRequest.CountryId > 0 ? (await _countryService.GetCountryByIdAsync(buyerRequest.CountryId))?.Name : "-",
                cityId = buyerRequest.CityId,
                cityName = buyerRequest.CityId > 0 ? (await _stateProvinceService.GetStateProvinceByIdAsync(buyerRequest.CityId))?.Name : "-",
                areaId = buyerRequest.CountryId,
                areaName = buyerRequest.AreaId > 0 ? (await _stateProvinceService.GetStateProvinceByIdAsync(buyerRequest.AreaId))?.Name : "-",
                quantity = buyerRequest.Quantity,
                buyerName = (await _customerService.GetCustomerByIdAsync(buyerRequest.BuyerId))?.FullName,
                businessModel = await _localizationService.GetLocalizedEnumAsync(directOrder.TransactionModelEnum),
                businessModelId = directOrder.TransactionModelId,
                streetAddress = directOrder.StreetAddress,
                industrtyId = buyerRequest.IndustryId,
                PinLocation = new
                {
                    location = directOrder.PinLocation_Location,
                    longitude = directOrder.PinLocation_Longitude,
                    latitude = directOrder.PinLocation_Latitude
                },
                interGeography = directOrder.InterGeography.HasValue ? directOrder.InterGeography.Value : false,
                expectedShipments = (await _orderService.GetAllDirectOrderDeliveryScheduleAsync(directOrder.Id)).Select(async d =>
                {
                    return new
                    {
                        id = d.Id,
                        expectedDeliveryDateUtc = d.ExpectedDeliveryDateUtc.HasValue ? await _dateTimeHelper.ConvertToUserTimeAsync(d.ExpectedDeliveryDateUtc.Value, DateTimeKind.Utc) : (DateTime?)null,
                        expectedShipmentDateUtc = d.ExpectedShipmentDateUtc.HasValue ? await _dateTimeHelper.ConvertToUserTimeAsync(d.ExpectedShipmentDateUtc.Value, DateTimeKind.Utc) : (DateTime?)null,
                        expectedQuantity = d.ExpectedQuantity.HasValue ? d.ExpectedQuantity.ToString() : "",
                        expectedDeliveryCost = d.ExpectedDeliveryCost.HasValue ? d.ExpectedDeliveryCost.ToString() : "",
                        createdOnUtc = d.CreatedOnUtc,
                    };
                }).Select(t => t.Result).ToList(),
                directOrderId = directOrder.Id
            };
            return new { success = true, message = "", data };

        }

        public async Task<object> SaleOrderInfo(DirectOrderApiModel.DirectOrderInfoModel model)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                throw new Exception("user not found");

            if (model.RequestId <= 0)
                throw new Exception("Buyer request id is required");

            var buyerRequest = await _requestService.GetRequestByIdAsync(model.RequestId);
            if (buyerRequest is null)
                throw new Exception("Buyer request not found");

            var directOrder = await _orderService.GetDirectOrderByRequestId(model.RequestId);
            if (directOrder is null)
                throw new Exception(await _localizationService.GetResourceAsync("DirectOrder.NotFound"));

            //Update direct order
            if (model.StreetAddress is not null)
                directOrder.StreetAddress = model.StreetAddress;

            if (model.TransactionModelId is not null && model.TransactionModelId.HasValue)
            {
                directOrder.TransactionModelId = model.TransactionModelId.Value;
                buyerRequest.BusinessModelId = model.TransactionModelId.Value;
                await _requestService.UpdateRequestAsync(buyerRequest);

                var cogsInventoryTaggings = await _inventoryService.GetAllDirectCogsInventoryTaggingsAsync(requestId: buyerRequest.Id);
                if (cogsInventoryTaggings.Any())
                {
                    foreach (var cogsInventoryTagging in cogsInventoryTaggings)
                        await _inventoryService.DeleteDirectCogsInventoryTaggingAsync(cogsInventoryTagging);
                }
            }

            if (model.PinLocation is not null)
            {
                directOrder.PinLocation_Latitude = model.PinLocation.Latitude;
                directOrder.PinLocation_Longitude = model.PinLocation.Longitude;
                directOrder.PinLocation_Location = model.PinLocation.Location;
            }

            if (model.InterGeography.HasValue)
                directOrder.InterGeography = model.InterGeography.Value;

            await _orderService.UpdateDirectOrderAsync(directOrder);

            return new { success = true, message = await _localizationService.GetResourceAsync("DirectOrder.infos.Update") };
        }
        public async Task<object> SaleOrderBusinessModelFormJson(int requestId)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                throw new Exception("user not found");

            var directOrder = await _orderService.GetDirectOrderByRequestId(requestId);
            if (directOrder is null)
                throw new Exception("Direct order not found");

            var request = await _requestService.GetRequestByIdAsync(requestId);
            if (request is null)
                throw new Exception("Request not found");

            var directOrderCalculations = await _orderService.GetAllDirectOrderCalculationAsync(directOrder.Id);

            var businessModels = new List<BusinessModelApiModel>();
            await Task.WhenAll(directOrderCalculations.Select(async orderCalculation =>
            {
                var model = new BusinessModelApiModel();

                var directOrderSupplierInfo = await _orderService.GetAllDirectOrderCalculationAsync(directOrder.Id);
                if (directOrderSupplierInfo is not null)
                    businessModels.Add(await _utilityService.SaleOrder_BusinessModelFormCalculatedJson(directOrder));
            }));
            return new { success = true, message = "", data = new { bunsinessModelFields = businessModels, businessModelName = await _localizationService.GetLocalizedEnumAsync(directOrder.TransactionModelEnum), businessModelId = directOrder.TransactionModelId } };
        }


        public async Task<object> SaleOrderBusinessModelFormCalculation(BusinessModelApiModel model)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                throw new Exception("user not found");

            var directOrderCalculation = await _orderService.GetDirectOrderCalculationByIdAsync(model.OrderCalculationId);
            if (directOrderCalculation is null)
                throw new Exception("Order calculation not found");

            var directOrder = await _orderService.GetDirectOrderByRequestId(model.RequestId);
            if (directOrder is null)
                throw new Exception("Direct order not found");

            var request = await _requestService.GetRequestByIdAsync(model.RequestId);
            if (request is null)
                throw new Exception("Request not found");

            var taggedInventories = await _inventoryService.GetAllDirectCogsInventoryTaggingsAsync(requestId: request.Id);
            if (directOrder.TransactionModelId != (int)BusinessModelEnum.ForwardSelling)
            {
                if (taggedInventories.Sum(x => x.Quantity) > request.Quantity)
                    throw new Exception("tagged inventory quantity is greater than request quantity");

                if (taggedInventories.Sum(x => x.Quantity) != request.Quantity)
                    throw new Exception("Sum of inventory should be equal to request quantity");
            }

            var gstRateValue = 0m;
            var gstRate = model.Receivables.Where(x => x.Name == "GSTAmount").FirstOrDefault().Value;
            if (gstRate is not null)
                gstRateValue = _utilityService.ConvertToDecimal(gstRate);

            var brokerWhtRateValue = 0m;
            var brokerWholesaleTaxRateValue = 0m;

            if (request.BusinessModelId == (int)BusinessModelEnum.Broker)
            {
                var brokerWhtRate = model.Receivables.Where(x => x.Name == "WHTRate").FirstOrDefault().Value;
                if (brokerWhtRate is not null)
                    brokerWhtRateValue = _utilityService.ConvertToDecimal(brokerWhtRate);

                var wholesaleTaxRate = model.Receivables.Where(x => x.Name == "WholesaleTaxRate").FirstOrDefault().Value;
                if (wholesaleTaxRate is not null)
                    brokerWholesaleTaxRateValue = _utilityService.ConvertToDecimal(wholesaleTaxRate);

                var brokerInventory = await _inventoryService.GetInventoryInboundByIdAsync(taggedInventories.Select(x => x.InventoryId).FirstOrDefault());
                if (brokerInventory is not null)
                {
                    gstRateValue = brokerInventory.GstRate;
                    brokerWhtRateValue = brokerInventory.WhtRate;
                    brokerWholesaleTaxRateValue = brokerInventory.WholeSaleTaxRate;
                    directOrderCalculation.InvoicedAmount = directOrderCalculation.BuyingPrice;
                }
            }

            var salePriceValue = 0m;
            var salePrice = model.Receivables.Where(x => x.Name == "SellingPriceOfProduct").FirstOrDefault().Value;
            if (salePrice is not null)
                salePriceValue = _utilityService.ConvertToDecimal(salePrice);

            var buyingPrice = await _utilityService.CalculateBuyingPriceByTaggings(taggedInventories.ToList(), gstRateValue > 0);
            var sellingPriceOfProduct = await _utilityService.CalculateSellingPriceOfProductByTaggings(taggedInventories.ToList(), gstRateValue > 0, salePriceValue);

            var calculatedBuyingPrice = buyingPrice;
            var calculatedSellingPriceOfProduct = sellingPriceOfProduct;

            var margin = 0m;
            if (calculatedSellingPriceOfProduct > calculatedBuyingPrice)
                margin = calculatedSellingPriceOfProduct - calculatedBuyingPrice;

            var discount = 0m;
            if (calculatedSellingPriceOfProduct < calculatedBuyingPrice)
                discount = calculatedSellingPriceOfProduct - calculatedBuyingPrice;

            model.SalePrice = salePriceValue;
            #region Standard

            if (directOrder.TransactionModelId == (int)BusinessModelEnum.Standard)
            {
                var totalPayble = model.SalePrice * request.Quantity;
                var totalReceivable = model.SalePrice * request.Quantity;

                var includeGstInTotalAmount = model.Payables.Where(x => x.Name == "GSTIncludedInTotalAmount")?.FirstOrDefault()?.Value;

                #region Receivable

                foreach (var item in model.Receivables)
                {
                    if (item.Name == "BuyingPrice")
                        directOrderCalculation.BuyingPrice = buyingPrice;

                    if (item.Name == "SellingPriceOfProduct")
                        directOrderCalculation.SellingPriceOfProduct = model.SalePrice;

                    else if (item.Name == "MarginRate")
                    {

                        if (calculatedSellingPriceOfProduct > calculatedBuyingPrice)
                        {

                            directOrderCalculation.MarginAmount = margin * request.Quantity;
                            directOrderCalculation.MarginRateType = "PKR";
                            directOrderCalculation.MarginRate = margin;

                            directOrderCalculation.NetRateWithMargin = totalReceivable /*+ directOrderCalculation.MarginAmount*/;

                            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                            {
                                directOrderCalculation.MarginRate = await _priceCalculationService.RoundPriceAsync(directOrderCalculation.MarginRate);
                                directOrderCalculation.NetRateWithMargin = await _priceCalculationService.RoundPriceAsync(directOrderCalculation.NetRateWithMargin);
                            }
                        }
                        else
                        {
                            directOrderCalculation.MarginAmount = 0;
                            directOrderCalculation.MarginRate = 0;
                            directOrderCalculation.NetRateWithMargin = totalReceivable;
                        }
                    }

                    //Discount Recievable
                    else if (item.Name == "DiscountRate")
                    {
                        if (calculatedSellingPriceOfProduct < calculatedBuyingPrice)
                        {
                            directOrderCalculation.DiscountAmount = discount * request.Quantity;
                            directOrderCalculation.DiscountRateType = "PKR";
                            directOrderCalculation.DiscountRate = discount;

                            directOrderCalculation.NetRateWithMargin -= Math.Abs(directOrderCalculation.DiscountAmount);

                            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                            {
                                directOrderCalculation.DiscountAmount = await _priceCalculationService.RoundPriceAsync(directOrderCalculation.DiscountAmount);
                                directOrderCalculation.NetRateWithMargin = await _priceCalculationService.RoundPriceAsync(directOrderCalculation.NetRateWithMargin);
                            }
                        }
                        else
                        {
                            directOrderCalculation.DiscountRate = 0;
                            //directOrderCalculation.Receivable_Promotype_0 = "";
                            directOrderCalculation.DiscountAmount = 0;
                        }
                    }

                    //GST Recievable
                    else if (item.Name == "GSTAmount")
                    {
                        if (_utilityService.ConvertToDecimal(item.Value) > 0)
                        {
                            directOrderCalculation.GSTRate = _utilityService.ConvertToDecimal(item.Value);
                            directOrderCalculation.GSTAmount = directOrderCalculation.NetRateWithMargin * (directOrderCalculation.GSTRate / 100m);

                            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                                directOrderCalculation.GSTAmount = await _priceCalculationService.RoundPriceAsync(directOrderCalculation.GSTAmount);
                        }
                        else
                        {
                            directOrderCalculation.GSTRate = 0;
                            directOrderCalculation.GSTAmount = 0;
                        }
                    }

                    //WHT Recievable
                    else if (item.Name == "WHTAmount")
                    {
                        if (_utilityService.ConvertToDecimal(item.Value) > 0)
                        {
                            directOrderCalculation.WHTRate = _utilityService.ConvertToDecimal(item.Value);
                            directOrderCalculation.WHTAmount = (directOrderCalculation.NetRateWithMargin + directOrderCalculation.GSTAmount) * (_utilityService.ConvertToDecimal(item.Value) / 100m);

                            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                                directOrderCalculation.WHTAmount = await _priceCalculationService.RoundPriceAsync(directOrderCalculation.WHTAmount);
                        }
                        else
                        {
                            directOrderCalculation.WHTRate = 0;
                            directOrderCalculation.WHTAmount = 0;
                        }

                        //if (directOrderCalculation.BusinessModelId != (int)BusinessModelEnum.Standard)
                        //{
                        //    totalReceivable += directOrderCalculation.MarginAmount;
                        //    totalReceivable -= directOrderCalculation.DiscountAmount;
                        //}

                        totalReceivable += directOrderCalculation.GSTAmount;
                        totalReceivable -= directOrderCalculation.WHTAmount;
                        directOrderCalculation.SubTotal = totalReceivable;
                    }
                }

                #endregion
            }

            #endregion

            #region All other business module

            else if (directOrder.TransactionModelId == (int)BusinessModelEnum.OneOnOne || directOrder.TransactionModelId == (int)BusinessModelEnum.Agency || directOrder.TransactionModelId == (int)BusinessModelEnum.FineCounts || directOrder.TransactionModelId == (int)BusinessModelEnum.ForwardBuying || directOrder.TransactionModelId == (int)BusinessModelEnum.ForwardSelling)
            {
                #region Receivable
                var totalReceivable = model.SalePrice * request.Quantity;
                var gstAmount_Receivable = 0m;
                var whtAmount_Receivable = 0m;
                var totalReceivableFromBuyer = 0m;
                var totalReceivableFromBuyerAfterMultiply = 0m;

                foreach (var item in model.Receivables)
                {
                    if (item.Name is not null)
                    {
                        var receivable_SellingPriceOfProduct = _utilityService.ConvertToDecimal(model.Receivables.Where(x => x.Name == "SellingPriceOfProduct")?.FirstOrDefault().Value);

                        if (item.Name == "SellingPriceOfProduct")
                        {
                            directOrderCalculation.SellingPriceOfProduct = receivable_SellingPriceOfProduct;
                        }

                        else if (item.Name == "BuyingPrice")
                        {
                            directOrderCalculation.BuyingPrice = calculatedBuyingPrice;
                        }
                        else if (item.Name == "InvoicedAmount")
                            directOrderCalculation.InvoicedAmount = _utilityService.ConvertToDecimal(item.Value);

                        //GST Rate
                        else if (item.Name == "GSTAmount")
                        {
                            var gstRatePercent = _utilityService.ConvertToDecimal(model.Receivables.Where(x => x.Name == "GSTRate")?.FirstOrDefault().Value);
                            if (gstRatePercent > 0)
                            {
                                var gstRateCalculationValue = directOrderCalculation.InvoicedAmount * gstRatePercent / 100;
                                gstAmount_Receivable = Math.Round(gstRateCalculationValue, 2);

                                directOrderCalculation.GSTIncludedInTotalAmount = true;
                                directOrderCalculation.GSTRate = _utilityService.ConvertToDecimal(gstRatePercent);
                                directOrderCalculation.GSTAmount = gstAmount_Receivable;

                            }
                            else
                            {
                                gstAmount_Receivable = 0m;
                                directOrderCalculation.GSTIncludedInTotalAmount = false;
                                directOrderCalculation.GSTRate = 0m;
                                directOrderCalculation.GSTAmount = 0m;
                            }
                        }

                        else if (item.Name == "BrokerCash")
                            directOrderCalculation.BrokerCash = directOrderCalculation.SellingPriceOfProduct - directOrderCalculation.InvoicedAmount;

                        else if (item.Name == "BrokerId")
                            directOrderCalculation.BrokerId = Convert.ToInt32(item.Value);

                        //WHT Rate
                        else if (item.Name == "WHTAmount")
                        {
                            var whtRate = _utilityService.ConvertToDecimal(model.Receivables.Where(x => x.Name == "WHTRate")?.FirstOrDefault().Value);
                            if (whtRate > 0)
                            {
                                var whtRateClaculationValue = (directOrderCalculation.InvoicedAmount + gstAmount_Receivable) * whtRate / 100;
                                whtAmount_Receivable = Math.Round(whtRateClaculationValue, 2);

                                directOrderCalculation.WhtIncluded = true;
                                directOrderCalculation.WHTRate = _utilityService.ConvertToDecimal(whtRate);
                                directOrderCalculation.WHTAmount = whtAmount_Receivable;
                            }
                            else
                            {
                                whtAmount_Receivable = 0m;
                                directOrderCalculation.WhtIncluded = false;
                                directOrderCalculation.WHTRate = 0m;
                                directOrderCalculation.WHTAmount = 0m;
                            }
                        }

                        //Buyer comission receivable per bag
                        else if (item.Name == "BuyerCommissionReceivablePerBag")
                        {
                            directOrderCalculation.BuyerCommissionReceivablePerBag = _utilityService.ConvertToDecimal(item.Value);
                            directOrderCalculation.BuyerCommissionReceivable_Summary = directOrderCalculation.BuyerCommissionReceivablePerBag * request.Quantity;
                        }

                        //Buyer comission payable per bag
                        else if (item.Name == "BuyerCommissionPayablePerBag")
                        {
                            directOrderCalculation.BuyerCommissionPayablePerBag = _utilityService.ConvertToDecimal(item.Value);
                            directOrderCalculation.BuyerCommissionPayable_Summary = directOrderCalculation.BuyerCommissionPayablePerBag * request.Quantity;
                        }

                        else if (item.Name == "BuyerCommissionPayableUserId")
                            directOrderCalculation.BuyerCommissionPayableUserId = Convert.ToInt32(item.Value);

                        else if (item.Name == "BuyerCommissionReceivableUserId")
                            directOrderCalculation.BuyerCommissionReceivableUserId = Convert.ToInt32(item.Value);

                        else if (item.Name == "TotalReceivableBuyer")
                        {
                            totalReceivableFromBuyer = receivable_SellingPriceOfProduct + gstAmount_Receivable - whtAmount_Receivable;
                            directOrderCalculation.TotalPerBag = totalReceivableFromBuyer + directOrderCalculation.BuyerCommissionReceivablePerBag - directOrderCalculation.BuyerCommissionPayablePerBag;
                            totalReceivableFromBuyerAfterMultiply = totalReceivableFromBuyer * request.Quantity;
                            directOrderCalculation.TotalReceivableBuyer = totalReceivableFromBuyerAfterMultiply;
                        }

                        else if (item.Name == "MarginAmount")
                        {
                            if (calculatedSellingPriceOfProduct > calculatedBuyingPrice)
                            {
                                directOrderCalculation.MarginRateType = "PKR";
                                directOrderCalculation.MarginAmount = margin * request.Quantity;
                                directOrderCalculation.MarginRate = margin;
                            }
                            else
                            {
                                directOrderCalculation.MarginRateType = "";
                                directOrderCalculation.MarginAmount = 0m;
                                directOrderCalculation.MarginRate = 0m;
                            }
                        }

                        else if (item.Name == "DiscountAmount")
                        {
                            if (calculatedSellingPriceOfProduct < calculatedBuyingPrice)
                            {
                                directOrderCalculation.DiscountRateType = "PKR";
                                directOrderCalculation.DiscountAmount = discount * request.Quantity;
                                directOrderCalculation.DiscountRate = discount;
                            }
                            else
                            {
                                directOrderCalculation.DiscountRateType = "";
                                directOrderCalculation.DiscountRate = 0m;
                                directOrderCalculation.DiscountAmount = 0m;
                            }
                        }

                        else if (item.Name == "OrderTotal")
                        {
                            var buyerCommissionAfterMultiply = directOrderCalculation.BuyerCommissionPayablePerBag * request.Quantity;
                            var buyerCommission_ReceivableAfterMultiply = directOrderCalculation.BuyerCommissionReceivablePerBag * request.Quantity;
                            directOrderCalculation.SubTotal = totalReceivableFromBuyerAfterMultiply;
                            directOrderCalculation.OrderTotal = directOrderCalculation.SubTotal;
                        }
                    }
                }

                #endregion
            }

            #endregion

            #region Lending

            else if (directOrder.TransactionModelId == (int)BusinessModelEnum.Lending)
            {
                #region Receivable

                var totalReceivable = model.SalePrice * request.Quantity;
                var gstAmount_Receivable = 0m;
                var whtAmount_Receivable = 0m;
                //var margin = 0m;
                //var discount = 0m;
                var totalReceivableFromBuyerAfterMultiply = 0m;
                var sellingPrice_FinanceIncome = _utilityService.ConvertToDecimal(model.Receivables.Where(x => x.Name == "InvoicedAmount")?.FirstOrDefault().Value) + _utilityService.ConvertToDecimal(model.Receivables.Where(x => x.Name == "FinanceIncome")?.FirstOrDefault().Value);

                foreach (var item in model.Receivables)
                {
                    if (item.Name is not null)
                    {
                        var receivable_SellingPriceOfProduct = _utilityService.ConvertToDecimal(model.Receivables.Where(x => x.Name == "SellingPriceOfProduct")?.FirstOrDefault().Value);

                        if (item.Name == "SellingPriceOfProduct")
                            directOrderCalculation.SellingPriceOfProduct = _utilityService.ConvertToDecimal(item.Value);

                        else if (item.Name == "BuyingPrice")
                        {
                            directOrderCalculation.BuyingPrice = calculatedBuyingPrice;
                        }

                        //GST Rate
                        else if (item.Name == "GSTAmount")
                        {
                            var gstRatePercent = _utilityService.ConvertToDecimal(model.Receivables.Where(x => x.Name == "GSTRate")?.FirstOrDefault().Value);
                            if (gstRatePercent > 0)
                            {
                                var invoiceAmount = _utilityService.ConvertToDecimal(model.Receivables.Where(x => x.Name == "InvoicedAmount")?.FirstOrDefault().Value);
                                var financeCostPayment = Convert.ToInt32(model.Receivables.Where(x => x.Name == "FinanceCostPayment")?.FirstOrDefault().Value);
                                var gstRateCalculationValue = invoiceAmount * gstRatePercent / 100m;

                                if (financeCostPayment == (int)FinanceCostPaymentStatus.DirectlyToMill)
                                    gstRateCalculationValue = sellingPrice_FinanceIncome * gstRatePercent / 100m;

                                gstAmount_Receivable = Math.Round(gstRateCalculationValue, 2);

                                directOrderCalculation.GSTIncludedInTotalAmount = true;
                                directOrderCalculation.GSTRate = _utilityService.ConvertToDecimal(gstRatePercent);
                                directOrderCalculation.GSTAmount = gstAmount_Receivable;

                            }
                            else
                            {
                                gstAmount_Receivable = 0m;
                                directOrderCalculation.GSTIncludedInTotalAmount = false;
                                directOrderCalculation.GSTRate = 0m;
                                directOrderCalculation.GSTAmount = 0m;
                            }
                        }


                        //WHT Rate
                        else if (item.Name == "WHTAmount")
                        {
                            var whtRate = _utilityService.ConvertToDecimal(model.Receivables.Where(x => x.Name == "WHTRate")?.FirstOrDefault().Value);
                            if (whtRate > 0)
                            {
                                var whtRateClaculationValue = (sellingPrice_FinanceIncome + gstAmount_Receivable) * whtRate / 100;
                                whtAmount_Receivable = Math.Round(whtRateClaculationValue, 2);

                                directOrderCalculation.WhtIncluded = true;
                                directOrderCalculation.WHTRate = _utilityService.ConvertToDecimal(whtRate);
                                directOrderCalculation.WHTAmount = whtAmount_Receivable;
                            }
                            else
                            {
                                whtAmount_Receivable = 0m;
                                directOrderCalculation.WhtIncluded = false;
                                directOrderCalculation.WHTRate = 0m;
                                directOrderCalculation.WHTAmount = 0m;
                            }
                        }

                        //Buyer comission receivable per bag
                        else if (item.Name == "BuyerCommissionReceivablePerBag")
                        {
                            directOrderCalculation.BuyerCommissionReceivablePerBag = _utilityService.ConvertToDecimal(item.Value);
                            directOrderCalculation.BuyerCommissionReceivable_Summary = directOrderCalculation.BuyerCommissionReceivablePerBag * request.Quantity;
                        }

                        //Buyer comission payable per bag
                        else if (item.Name == "BuyerCommissionPayablePerBag")
                        {
                            directOrderCalculation.BuyerCommissionPayablePerBag = _utilityService.ConvertToDecimal(item.Value);
                            directOrderCalculation.BuyerCommissionPayable_Summary = directOrderCalculation.BuyerCommissionPayablePerBag * request.Quantity;
                        }

                        else if (item.Name == "BuyerCommissionPayableUserId")
                            directOrderCalculation.BuyerCommissionPayableUserId = Convert.ToInt32(item.Value);

                        else if (item.Name == "BuyerCommissionReceivableUserId")
                            directOrderCalculation.BuyerCommissionReceivableUserId = Convert.ToInt32(item.Value);

                        else if (item.Name == "InvoicedAmount")
                            directOrderCalculation.InvoicedAmount = _utilityService.ConvertToDecimal(item.Value);

                        else if (item.Name == "BrokerCash")
                            directOrderCalculation.BrokerCash = directOrderCalculation.SellingPriceOfProduct - directOrderCalculation.InvoicedAmount;


                        else if (item.Name == "BrokerId")
                            directOrderCalculation.BrokerId = Convert.ToInt32(item.Value);

                        else if (item.Name == "BuyerPaymentTerms")
                            directOrderCalculation.BuyerPaymentTerms = Convert.ToInt32(item.Value);

                        else if (item.Name == "TotalFinanceCost")
                            directOrderCalculation.TotalFinanceCost = _utilityService.ConvertToDecimal(item.Value);

                        else if (item.Name == "SupplierCreditTerms")
                            directOrderCalculation.SupplierCreditTerms = _utilityService.ConvertToDecimal(item.Value);

                        else if (item.Name == "FinanceCostPayment")
                            directOrderCalculation.FinanceCostPayment = Convert.ToInt32(item.Value);

                        else if (item.Name == "FinanceCost")
                            directOrderCalculation.FinanceCost = directOrderCalculation.SupplierCreditTerms > 0 && directOrderCalculation.TotalFinanceCost > 0 ? directOrderCalculation.TotalFinanceCost / directOrderCalculation.SupplierCreditTerms * directOrderCalculation.BuyerPaymentTerms : 0;

                        else if (item.Name == "FinanceIncome")
                            directOrderCalculation.FinanceIncome = _utilityService.ConvertToDecimal(item.Value);

                        else if (item.Name == "SellingPrice_FinanceIncome")
                            directOrderCalculation.SellingPrice_FinanceIncome = directOrderCalculation.SellingPriceOfProduct + directOrderCalculation.FinanceIncome;

                        else if (item.Name == "MarginAmount")
                        {
                            if (calculatedSellingPriceOfProduct > calculatedBuyingPrice)
                            {
                                directOrderCalculation.MarginRateType = "PKR";
                                directOrderCalculation.MarginAmount = margin * request.Quantity;
                                directOrderCalculation.MarginRate = margin;
                            }
                            else
                            {
                                directOrderCalculation.MarginRateType = "";
                                directOrderCalculation.MarginAmount = 0m;
                                directOrderCalculation.MarginRate = 0m;
                            }
                        }
                        else if (item.Name == "DiscountAmount")
                        {
                            if (calculatedSellingPriceOfProduct < calculatedBuyingPrice)
                            {
                                directOrderCalculation.DiscountRateType = "PKR";
                                directOrderCalculation.DiscountAmount = discount * request.Quantity;
                                directOrderCalculation.DiscountRate = discount;
                            }
                            else
                            {
                                directOrderCalculation.DiscountRateType = "";
                                directOrderCalculation.DiscountRate = 0m;
                                directOrderCalculation.DiscountAmount = 0m;
                            }
                        }

                        else if (item.Name == "TotalReceivableBuyer")
                        {
                            directOrderCalculation.TotalReceivableBuyer = directOrderCalculation.SellingPrice_FinanceIncome + gstAmount_Receivable - whtAmount_Receivable;
                            totalReceivableFromBuyerAfterMultiply = directOrderCalculation.TotalReceivableBuyer * request.Quantity;
                            directOrderCalculation.TotalReceivableBuyer = totalReceivableFromBuyerAfterMultiply;
                        }
                        else if (item.Name == "OrderTotal")
                        {
                            var buyerCommission_ReceivableAfterMultiply = directOrderCalculation.BuyerCommissionReceivablePerBag * request.Quantity;
                            var buyerCommissionAfterMultiply = directOrderCalculation.BuyerCommissionPayablePerBag * request.Quantity;
                            directOrderCalculation.SubTotal = directOrderCalculation.TotalReceivableBuyer;
                            directOrderCalculation.OrderTotal = directOrderCalculation.SubTotal;
                        }
                    }
                }

                #endregion
            }

            #endregion

            #region Broker

            if (directOrder.TransactionModelId == (int)BusinessModelEnum.Broker)
            {
                var totalReceivable = model.SalePrice * request.Quantity;
                #region Receivable

                var gstAmount_Receivable = 0m;
                var whtAmount_Receivable = 0m;
                var wholeSaleTaxAmount_Receivable = 0m;
                //var calculatedSellingPriceOfProduct_Receivable = 0m;

                foreach (var item in model.Receivables)
                {
                    if (item.Name is not null)
                    {
                        var receivable_SellingPriceOfProduct = _utilityService.ConvertToDecimal(model.Receivables.Where(x => x.Name == "SellingPriceOfProduct")?.FirstOrDefault().Value);
                        //calculatedSellingPriceOfProduct_Receivable = receivable_SellingPriceOfProduct;
                        if (item.Name == "SellingPriceOfProduct")
                        {
                            directOrderCalculation.SellingPriceOfProduct = receivable_SellingPriceOfProduct;
                        }
                        else if (item.Name == "BuyingPrice")
                        {
                            directOrderCalculation.BuyingPrice = calculatedBuyingPrice;
                        }
                        else if (item.Name == "InvoicedAmount")
                            directOrderCalculation.InvoicedAmount = _utilityService.ConvertToDecimal(item.Value);

                        else if (item.Name == "GSTRate")
                            directOrderCalculation.GSTRate = gstRateValue;

                        //GST Rate
                        else if (item.Name == "GSTAmount")
                        {
                            var gstRatePercent = _utilityService.ConvertToDecimal(model.Receivables.Where(x => x.Name == "GSTRate")?.FirstOrDefault().Value);
                            if (gstRatePercent > 0)
                            {
                                var gstRateCalculationValue = directOrderCalculation.InvoicedAmount * gstRatePercent / 100;
                                gstAmount_Receivable = Math.Round(gstRateCalculationValue, 2);

                                directOrderCalculation.GSTIncludedInTotalAmount = true;
                                directOrderCalculation.GSTRate = _utilityService.ConvertToDecimal(gstRatePercent);
                                directOrderCalculation.GSTAmount = gstAmount_Receivable;

                            }
                            else
                            {
                                gstAmount_Receivable = 0m;
                                directOrderCalculation.GSTIncludedInTotalAmount = false;
                                directOrderCalculation.GSTRate = 0m;
                                directOrderCalculation.GSTAmount = 0m;
                            }
                        }

                        //WHT Rate
                        else if (item.Name == "WHTAmount")
                        {
                            //var whtRate = _utilityService.ConvertToDecimal(model.Receivables.Where(x => x.Name == "WHTRate")?.FirstOrDefault().Value);
                            if (brokerWhtRateValue > 0)
                            {
                                var whtRateClaculationValue = (directOrderCalculation.InvoicedAmount + gstAmount_Receivable) * brokerWhtRateValue / 100;
                                whtAmount_Receivable = whtRateClaculationValue;

                                directOrderCalculation.WhtIncluded = true;
                                directOrderCalculation.WHTRate = _utilityService.ConvertToDecimal(brokerWhtRateValue);
                                directOrderCalculation.WHTAmount = whtAmount_Receivable;
                            }
                            else
                            {
                                whtAmount_Receivable = 0m;
                                directOrderCalculation.WhtIncluded = false;
                                directOrderCalculation.WHTRate = 0m;
                                directOrderCalculation.WHTAmount = 0m;
                            }
                        }

                        //Wholesale Tax
                        else if (item.Name == "WholesaleTaxAmount")
                        {
                            //var wholesaletaxRate = _utilityService.ConvertToDecimal(model.Receivables.Where(x => x.Name == "WholesaleTaxRate")?.FirstOrDefault().Value);
                            if (brokerWholesaleTaxRateValue > 0)
                            {
                                //var wholesaletaxCalculationValue = ((decimal)((float)(InvoicedAmount_Payable + gstAmount_Payable - Math.Abs(whtAmount_Payable)) * 0.001f));
                                var wholesaletaxCalculationValue = (decimal)((float)(directOrderCalculation.InvoicedAmount + gstAmount_Receivable - Math.Abs(whtAmount_Receivable)) * (float)brokerWholesaleTaxRateValue / 100);
                                wholeSaleTaxAmount_Receivable = wholesaletaxCalculationValue;

                                directOrderCalculation.WholesaletaxIncluded = true;
                                directOrderCalculation.WholesaleTaxRate = _utilityService.ConvertToDecimal(brokerWholesaleTaxRateValue);
                                directOrderCalculation.WholesaleTaxAmount = wholeSaleTaxAmount_Receivable;
                            }
                            else
                            {
                                wholeSaleTaxAmount_Receivable = 0m;
                                directOrderCalculation.WholesaletaxIncluded = false;
                                directOrderCalculation.WholesaleTaxRate = 0;
                                directOrderCalculation.WholesaleTaxAmount = 0m;
                            }
                        }

                        //Buyer commission receivable per bag
                        else if (item.Name == "BuyerCommissionReceivablePerBag")
                            directOrderCalculation.BuyerCommissionReceivablePerBag = _utilityService.ConvertToDecimal(item.Value);

                        else if (item.Name == "BuyerCommissionPayablePerBag")
                            directOrderCalculation.BuyerCommissionPayablePerBag = _utilityService.ConvertToDecimal(item.Value);

                        else if (item.Name == "BrokerCash")
                            directOrderCalculation.BrokerCash = directOrderCalculation.SellingPriceOfProduct - directOrderCalculation.InvoicedAmount;

                        else if (item.Name == "BrokerId")
                            directOrderCalculation.BrokerId = Convert.ToInt32(item.Value);

                        else if (item.Name == "BuyerCommissionPayableUserId")
                            directOrderCalculation.BuyerCommissionPayableUserId = Convert.ToInt32(item.Value);

                        else if (item.Name == "BuyerCommissionReceivableUserId")
                            directOrderCalculation.BuyerCommissionReceivableUserId = Convert.ToInt32(item.Value);

                        //Margin
                        else if (item.Name == "MarginAmount")
                        {
                            if (calculatedSellingPriceOfProduct > calculatedBuyingPrice)
                            {
                                directOrderCalculation.MarginRateType = "PKR";
                                directOrderCalculation.MarginAmount = margin * request.Quantity;
                                directOrderCalculation.MarginRate = margin;
                            }
                            else
                            {
                                directOrderCalculation.MarginRateType = "";
                                directOrderCalculation.MarginAmount = 0m;
                                directOrderCalculation.MarginRate = 0m;
                            }

                        }

                        //Promo
                        else if (item.Name == "DiscountAmount")
                        {
                            if (calculatedSellingPriceOfProduct < calculatedBuyingPrice)
                            {
                                directOrderCalculation.DiscountRateType = "PKR";
                                directOrderCalculation.DiscountAmount = discount * request.Quantity;
                                directOrderCalculation.DiscountRate = discount;
                            }
                            else
                            {
                                directOrderCalculation.DiscountRateType = "";
                                directOrderCalculation.DiscountRate = 0m;
                                directOrderCalculation.DiscountAmount = 0m;
                            }
                        }

                        //Total receivable from buyer directly to supplier
                        else if (item.Name == "TotalReceivableFromBuyerDirectlyToSupplier")
                        {
                            var payableToMill = (decimal)(float)(directOrderCalculation.InvoicedAmount + directOrderCalculation.GSTAmount - directOrderCalculation.WHTAmount + wholeSaleTaxAmount_Receivable);
                            directOrderCalculation.TotalReceivableFromBuyerDirectlyToSupplier = payableToMill * request.Quantity;
                        }

                        //Total commission receivable from buyer to zaraye
                        else if (item.Name == "TotalCommissionReceivableFromBuyerToZaraye")
                            directOrderCalculation.TotalCommissionReceivableFromBuyerToZaraye = directOrderCalculation.MarginAmount * request.Quantity;

                        //Buyer commission receivable 
                        else if (item.Name == "BuyerCommissionReceivable_Summary")
                            directOrderCalculation.BuyerCommissionReceivable_Summary = directOrderCalculation.BuyerCommissionReceivablePerBag * request.Quantity;

                        //Buyer commission payable
                        else if (item.Name == "BuyerCommissionPayable_Summary")
                            directOrderCalculation.BuyerCommissionPayable_Summary = directOrderCalculation.BuyerCommissionPayablePerBag * request.Quantity;

                        //Total receivable
                        else if (item.Name == "OrderTotal")
                        {
                            var payableToMill = directOrderCalculation.InvoicedAmount + directOrderCalculation.GSTAmount - directOrderCalculation.WHTAmount;
                            directOrderCalculation.PayableToMill = payableToMill;
                            var totalReceivableFromBuyerDirectlyToSupplier = payableToMill;
                            var payableToMillAfterMultiply = directOrderCalculation.PayableToMill * request.Quantity;
                            var totalCommissionReceivableFromBuyerToZaraye_ReceivableAfterMultiply = margin * request.Quantity;
                            var totalReceivableFromBuyerDirectlyToSupplier_ReceivableAfterMultiply = payableToMillAfterMultiply;
                            directOrderCalculation.TotalPerBag = directOrderCalculation.BuyerCommissionReceivablePerBag - directOrderCalculation.BuyerCommissionPayablePerBag;
                            directOrderCalculation.SubTotal = totalReceivableFromBuyerDirectlyToSupplier_ReceivableAfterMultiply + totalCommissionReceivableFromBuyerToZaraye_ReceivableAfterMultiply /*+ directOrderCalculation.BuyerCommissionReceivable_Summary - directOrderCalculation.BuyerCommissionPayable_Summary*/;
                            directOrderCalculation.GrossAmount = directOrderCalculation.SubTotal;
                            directOrderCalculation.OrderTotal = directOrderCalculation.BuyerCommissionReceivable_Summary - directOrderCalculation.BuyerCommissionPayable_Summary;
                        }

                    }
                }

                #endregion
            }

            #endregion

            await _orderService.UpdateDirectOrderCalculationAsync(directOrderCalculation);

            return new { success = true, message = "", data = await _utilityService.SaleOrder_BusinessModelFormCalculatedJson(directOrder) };
        }

        public async Task<object> DirectSaleOrder(int requestId)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                throw new Exception("user not found");

            var request = await _requestService.GetRequestByIdAsync(requestId);
            if (request is null)
                throw new Exception("Request not found");

            var buyer = await _customerService.GetCustomerByIdAsync(request.BuyerId);
            if (buyer is null)
                throw new Exception("Buyer not found");

            var directOrder = await _orderService.GetDirectOrderByRequestId(request.Id);
            if (directOrder is null)
                throw new Exception("Direct order not found");

            var directOrderCalculation = await _orderService.GetDirectOrderCalculationByDirectOrderIdAsync(directOrder.Id);
            if (directOrderCalculation is null)
                throw new Exception("Direct order calculation not found");

            if (directOrderCalculation.BuyingPrice == 0 && directOrder.TransactionModelId != (int)BusinessModelEnum.ForwardSelling)
                throw new Exception("Buying price is required");

            if (directOrderCalculation.BusinessModelId != (int)BusinessModelEnum.Standard)
            {
                if (directOrderCalculation.BrokerCash > 0 && directOrderCalculation.BrokerId == 0)
                    throw new Exception(await _localizationService.GetResourceAsync("Admin.Request.Order.SalesOrder.Fields.BrokerId.Required"));

                if (directOrderCalculation.BuyerCommissionPayableUserId > 0 && directOrderCalculation.BuyerCommissionPayablePerBag == 0)
                    throw new Exception(await _localizationService.GetResourceAsync("Admin.Request.Order.SalesOrder.Fields.BuyerCommissionPayableUserId.Required"));

                if (directOrderCalculation.BuyerCommissionReceivableUserId > 0 && directOrderCalculation.BuyerCommissionReceivablePerBag == 0)
                    throw new Exception(await _localizationService.GetResourceAsync("Admin.Request.Order.SalesOrder.Fields.BuyerCommissionReceivableUserId.Required"));
            }

            //clear cart 
            var cartItems = await _shoppingCartService.GetShoppingCartAsync(buyer, ShoppingCartType.ShoppingCart, storeId: (await _storeContext.GetCurrentStoreAsync()).Id);
            foreach (var cartItem in cartItems)
                await _shoppingCartService.DeleteShoppingCartItemAsync(cartItem);

            var product = await _productService.GetProductByIdAsync(request.ProductId);
            if (product != null)
            {
                //validate order calculation
                directOrderCalculation.BusinessModelId = directOrder.TransactionModelId;
                var warnings = await _utilityService.PrepareSaleOrderCalculationAsync(new Order(), request, directOrderCalculation);
                if (warnings.Any())
                    return new { success = false, message = warnings };

                if (!warnings.Any())
                {
                    //now let's try adding product to the cart (now including product attribute validation, etc)
                    var addToCartWarnings = await _shoppingCartService.AddToCartAsync(customer: buyer,
                    product: product,
                    shoppingCartType: ShoppingCartType.ShoppingCart,
                    storeId: (await _storeContext.GetCurrentStoreAsync()).Id,
                    attributesXml: request.ProductAttributeXml,
                    quantity: request.Quantity, overridePrice: directOrderCalculation.SellingPriceOfProduct, brandId: request.BrandId);

                    if (addToCartWarnings.Any())
                        return new { success = false, message = addToCartWarnings };

                    //save address for buyer 
                    await _utilityService.SaveSaleOrderAddressAsync(request, buyer);

                    //place order
                    var processPaymentRequest = new ProcessPaymentRequest
                    {
                        StoreId = (await _storeContext.GetCurrentStoreAsync()).Id,
                    };

                    _paymentService.GenerateOrderGuid(processPaymentRequest);
                    processPaymentRequest.CustomerId = request.BuyerId;
                    processPaymentRequest.RequestId = request.Id;
                    processPaymentRequest.OrderTypeId = (int)OrderType.SaleOrder;
                    processPaymentRequest.OrderTotal = directOrderCalculation.SellingPriceOfProduct * request.Quantity;
                    processPaymentRequest.SalePrice = directOrderCalculation.BuyingPrice;
                    processPaymentRequest.Quotation = null;
                    processPaymentRequest.Source = "Tijara";

                    var placeOrderResult = await _orderProcessingService.PlaceOrderAsync(processPaymentRequest);
                    if (placeOrderResult.Success)
                    {
                        directOrderCalculation.BusinessModelId = directOrder.TransactionModelId;

                        //save sales order calculation
                        await _utilityService.SaveSalesOrderCalculationAsync(placeOrderResult.PlacedOrder, request, directOrderCalculation);

                        //Save Cogs Inventory Taggings
                        var directCogsInventoryTaggings = await _inventoryService.GetAllDirectCogsInventoryTaggingsAsync(requestId: requestId);
                        foreach (var tagging in directCogsInventoryTaggings)
                        {
                            await _inventoryService.InsertCogsInventoryTaggingAsync(new CogsInventoryTagging
                            {
                                InventoryId = tagging.InventoryId,
                                Quantity = tagging.Quantity,
                                Rate = tagging.Rate,
                                RequestId = tagging.RequestId,
                                GrossQuantity = tagging.Quantity
                            });

                            //delete Direct Cogs Inventory Taggings
                            await _inventoryService.DeleteDirectCogsInventoryTaggingAsync(tagging);
                        }

                        //Save delivery schedule
                        foreach (var schedule in await _orderService.GetAllDirectOrderDeliveryScheduleAsync(directOrder.Id))
                        {
                            await _orderService.InsertDeliveryScheduleAsync(new OrderDeliverySchedule
                            {
                                OrderId = placeOrderResult.PlacedOrder.Id,
                                CreatedById = user.Id,
                                ExpectedDeliveryDateUtc = schedule.ExpectedDeliveryDateUtc.Value,
                                ExpectedShipmentDateUtc = schedule.ExpectedShipmentDateUtc.Value,
                                ExpectedQuantity = schedule.ExpectedQuantity.Value,
                                ExpectedDeliveryCost = schedule.ExpectedDeliveryCost.Value,
                                CreatedOnUtc = DateTime.UtcNow
                            });
                        }

                        //Update request status
                        request.RequestStatus = RequestStatus.Approved;
                        request.BusinessModelId = directOrder.TransactionModelId;
                        request.PinLocation_Latitude = directOrder.PinLocation_Latitude;
                        request.PinLocation_Location = directOrder.PinLocation_Location;
                        request.PinLocation_Longitude = directOrder.PinLocation_Longitude;
                        await _requestService.UpdateRequestAsync(request);

                        //Remove temp
                        var directOrderDeliverySchedules = await _orderService.GetAllDirectOrderDeliveryScheduleAsync(directOrder.Id);
                        foreach (var directOrderDeliverySchedule in directOrderDeliverySchedules)
                            await _orderService.DeleteDirectOrderDeliveryScheduleAsync(directOrderDeliverySchedule);

                        //Remove direct order supplier info
                        var directOrderCalculations = await _orderService.GetAllDirectOrderCalculationAsync(directOrder.Id);
                        foreach (var directorderCalculation in directOrderCalculations)
                            await _orderService.DeleteDirectOrderCalculationAsync(directorderCalculation);

                        await _orderService.DeleteDirectOrderAsync(directOrder);

                        return new { success = true, message = await _localizationService.GetResourceAsync("Request.SalesOrder.Genearted.Successfully") };
                    }
                    else
                        return new { success = false, message = await _localizationService.GetResourceAsync("Order.Does'nt.Placed.Success") };
                }
            }

            return new { success = false, message = "Product not found" };
        }

        public async Task<object> SaleOrderList(bool showActiveOrders = true, int pageIndex = 0, int pageSize = 10)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                throw new Exception("user not found");

            List<Order> allOrders = new List<Order>();
            List<OrderManagementApiModel.Orders> activeOrders = new List<OrderManagementApiModel.Orders>();
            List<OrderManagementApiModel.Orders> pastOrders = new List<OrderManagementApiModel.Orders>();
            List<int> orderStatusIds = new List<int>();

            if (showActiveOrders)
                orderStatusIds.AddRange(new List<int> { (int)OrderStatus.Pending, (int)OrderStatus.Processing });
            else
                orderStatusIds.AddRange(new List<int> { (int)OrderStatus.Cancelled, (int)OrderStatus.Complete });

            var allOrdersPageList = await _orderService.SearchOrdersAsync(osIds: orderStatusIds, orderTypeId: (int)OrderType.SaleOrder, pageIndex: pageIndex, pageSize: pageSize);

            allOrders = allOrdersPageList.ToList();
            if (allOrders.Any())
            {
                var currentTime = await _dateTimeHelper.ConvertToUserTimeAsync(DateTime.UtcNow, DateTimeKind.Utc);
                //Get Active Request List Filter By Date 
                var activeOrderDates = allOrders.OrderByDescending(x => x.CreatedOnUtc).Where(x => x.OrderStatusId == (int)OrderStatus.Pending || x.OrderStatusId == (int)OrderStatus.Processing).GroupBy(p => _dateTimeHelper.ConvertToUserTimeAsync(p.CreatedOnUtc, DateTimeKind.Utc).Result.ToString("dd/MM/yyyy")).ToList();
                await Task.WhenAll(activeOrderDates.Select(async activeOrderDate =>
                {
                    var model = new OrderManagementApiModel.Orders();
                    var yesterday = currentTime.AddDays(-1).ToString("dd/MM/yyyy");
                    if (currentTime.ToString("dd/MM/yyyy") == activeOrderDate.Key)
                        model.Date = "Today";
                    else if (currentTime.AddDays(-1).ToString("dd/MM/yyyy") == activeOrderDate.Key)
                        model.Date = "Yesterday";
                    else
                        model.Date = activeOrderDate.Key;
                    await Task.WhenAll(activeOrderDate.Select(async order =>
                    {
                        var paymentDueDate = "";
                        var industry = "";

                        if (order.RequestId > 0)
                        {
                            var buyerRequest = await _requestService.GetRequestByIdAsync(order.RequestId);
                            if (buyerRequest is not null)
                            {
                                industry = (await _categoryService.GetCategoryByIdAsync(buyerRequest.IndustryId))?.Name ?? "-";


                                var orderShipments = await _shipmentService.GetAllShipmentsAsync(orderId: order.Id, shipmentTypeId: (int)ShipmentType.SaleOrder);
                                if (!orderShipments.Any())
                                {
                                    paymentDueDate = (await _dateTimeHelper.ConvertToUserTimeAsync(order.CreatedOnUtc.AddDays(buyerRequest.PaymentDuration), DateTimeKind.Utc)).ToString("dd MMM yy");
                                }
                                else
                                {
                                    var orderShipment = orderShipments.FirstOrDefault();
                                    paymentDueDate = orderShipment.DeliveryDateUtc.HasValue ?
                                        (await _dateTimeHelper.ConvertToUserTimeAsync(orderShipment.DeliveryDateUtc.Value.AddDays(buyerRequest.PaymentDuration), DateTimeKind.Utc)).ToString("dd MMM yy") :
                                        (await _dateTimeHelper.ConvertToUserTimeAsync(order.CreatedOnUtc.AddDays(buyerRequest.PaymentDuration), DateTimeKind.Utc)).ToString("dd MMM yy");
                                }
                            }

                            var product = await _productService.GetProductByIdAsync((await _orderService.GetOrderItemsAsync(order.Id)).FirstOrDefault().ProductId);

                            model.Data.Add(new OrdersData
                            {
                                OrderId = order.Id,
                                OrderCustomNumber = order.CustomOrderNumber,
                                CreatedOnUtc = (await _dateTimeHelper.ConvertToUserTimeAsync(order.CreatedOnUtc, DateTimeKind.Utc)).ToString("hh:mm tt"),
                                OrderStatus = await _localizationService.GetLocalizedEnumAsync(order.OrderStatus),
                                OrderStatusId = order.OrderStatusId,
                                ProductId = product.Id,
                                ProductName = product.Name,
                                BuyerId = buyerRequest.BuyerId,
                                BuyerName = (await _customerService.GetCustomerByIdAsync(customerId: buyerRequest.BuyerId))?.FullName,
                                BrandId = buyerRequest.BrandId,
                                BrandName = buyerRequest.BrandId > 0 ? await _localizationService.GetLocalizedAsync(await _brandService.GetManufacturerByIdAsync(buyerRequest.BrandId), x => x.Name) : "-",
                                BookerId = buyerRequest.BookerId,
                                BookerName = (await _customerService.GetCustomerByIdAsync(customerId: buyerRequest.BookerId))?.FullName,
                                Quantity = buyerRequest.Quantity,
                                PaymentDueDate = paymentDueDate,
                                Industry = industry,
                            });
                        }
                    }));

                    activeOrders.Add(model);
                }));

                var pastOrderDates = allOrders.OrderByDescending(x => x.CreatedOnUtc).Where(x => x.OrderStatusId == (int)OrderStatus.Cancelled).GroupBy(p => _dateTimeHelper.ConvertToUserTimeAsync(p.CreatedOnUtc, DateTimeKind.Utc).Result.ToString("dd/MM/yyyy")).ToList();
                await Task.WhenAll(pastOrderDates.Select(async pastOrderDate =>
                {
                    var model = new OrderManagementApiModel.Orders();
                    var yesterday = currentTime.AddDays(-1).ToString("dd/MM/yyyy");
                    if (currentTime.ToString("dd/MM/yyyy") == pastOrderDate.Key)
                        model.Date = "Today";
                    else if (currentTime.AddDays(-1).ToString("dd/MM/yyyy") == pastOrderDate.Key)
                        model.Date = "Yesterday";
                    else
                        model.Date = pastOrderDate.Key;

                    foreach (var order in pastOrderDate)
                    {
                        var paymentDueDate = "";
                        var industry = "";

                        if (order.RequestId > 0)
                        {
                            var buyerRequest = await _requestService.GetRequestByIdAsync(order.RequestId);
                            if (buyerRequest is not null)
                            {
                                industry = (await _categoryService.GetCategoryByIdAsync(buyerRequest.IndustryId))?.Name ?? "-";


                                var orderShipments = await _shipmentService.GetAllShipmentsAsync(orderId: order.Id, shipmentTypeId: (int)ShipmentType.SaleOrder);
                                if (!orderShipments.Any())
                                {
                                    paymentDueDate = (await _dateTimeHelper.ConvertToUserTimeAsync(order.CreatedOnUtc.AddDays(buyerRequest.PaymentDuration), DateTimeKind.Utc)).ToString("dd MMM yy");
                                }
                                else
                                {
                                    var orderShipment = orderShipments.FirstOrDefault();
                                    paymentDueDate = orderShipment.DeliveryDateUtc.HasValue ?
                                        (await _dateTimeHelper.ConvertToUserTimeAsync(orderShipment.DeliveryDateUtc.Value.AddDays(buyerRequest.PaymentDuration), DateTimeKind.Utc)).ToString("dd MMM yy") :
                                        (await _dateTimeHelper.ConvertToUserTimeAsync(order.CreatedOnUtc.AddDays(buyerRequest.PaymentDuration), DateTimeKind.Utc)).ToString("dd MMM yy");
                                }
                            }

                            var product = await _productService.GetProductByIdAsync((await _orderService.GetOrderItemsAsync(order.Id)).FirstOrDefault().ProductId);
                            if (product == null)
                                continue;

                            model.Data.Add(new OrdersData
                            {
                                OrderId = order.Id,
                                OrderCustomNumber = order.CustomOrderNumber,
                                CreatedOnUtc = (await _dateTimeHelper.ConvertToUserTimeAsync(order.CreatedOnUtc, DateTimeKind.Utc)).ToString("hh:mm tt"),
                                OrderStatus = await _localizationService.GetLocalizedEnumAsync(order.OrderStatus),
                                OrderStatusId = order.OrderStatusId,
                                ProductId = product.Id,
                                ProductName = product.Name,
                                BuyerId = buyerRequest.BuyerId,
                                BuyerName = (await _customerService.GetCustomerByIdAsync(customerId: buyerRequest.BuyerId))?.FullName,
                                BrandId = buyerRequest.BrandId,
                                BrandName = buyerRequest.BrandId > 0 ? await _localizationService.GetLocalizedAsync(await _brandService.GetManufacturerByIdAsync(buyerRequest.BrandId), x => x.Name) : "-",
                                BookerId = buyerRequest.BookerId,
                                BookerName = (await _customerService.GetCustomerByIdAsync(customerId: buyerRequest.BookerId))?.FullName,
                                Quantity = buyerRequest.Quantity,
                                PaymentDueDate = paymentDueDate,
                                Industry = industry,
                            });
                        }
                    }
                    pastOrders.Add(model);
                }));

                var data = new List<object>();
                if (activeOrders.Any())
                {
                    data.Add(new
                    {
                        DataType = await _localizationService.GetResourceAsync("Order.ActiveOrder"),
                        Data = activeOrders
                    });
                }
                if (pastOrders.Any())
                {
                    data.Add(new
                    {
                        DataType = await _localizationService.GetResourceAsync("Order.PastOrder"),
                        Data = pastOrders
                    });
                }

                return new { success = true, message = "", data, totalPages = allOrdersPageList.TotalPages, currentPage = allOrdersPageList.PageIndex };
            }
            else
            {
                return new { success = false, message = "No orders found." };
            }
        }

        public async Task<object> SaleOrderDetail(int orderId)
        {
            if (orderId <= 0)
                throw new Exception("Buyer order id is required");

            var buyer = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(buyer) /*|| !await _customerService.IsBuyerAsync(buyer)*/)
                throw new Exception("Buyer not found");

            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order is null || order.Deleted)
                throw new Exception("Order not found");

            var buyerRequest = await _requestService.GetRequestByIdAsync(order.RequestId);
            if (buyerRequest is null || buyerRequest.Deleted)
                throw new Exception("Order request not found");

            var industry = "";
            var productName = "";
            var brand = "";
            var qtyType = "";
            var deliveryDate = "";
            var deliveryAddress = "";
            var quantity = 0m;
            var orderShipped = "";
            var orderDelivered = "";

            industry = (await _industryService.GetIndustryByIdAsync(buyerRequest.IndustryId))?.Name ?? "-";
            quantity = buyerRequest.Quantity;

            var product = await _productService.GetProductByIdAsync(buyerRequest.ProductId);
            if (product is not null)
            {
                productName = await _localizationService.GetLocalizedAsync(product, x => x.Name);
                qtyType = (await _measureService.GetMeasureWeightByIdAsync(product.UnitId))?.Name ?? string.Empty;
            }

            brand = buyerRequest.BrandId > 0 ? await _localizationService.GetLocalizedAsync(await _brandService.GetManufacturerByIdAsync(buyerRequest.BrandId), x => x.Name) : "-";
            deliveryDate = (await _dateTimeHelper.ConvertToUserTimeAsync(buyerRequest.DeliveryDate, DateTimeKind.Utc)).ToString("dd MMM yy");

            var city = await _stateProvinceService.GetStateProvinceByIdAsync(buyerRequest.CityId);
            if (city != null)
                deliveryAddress += city.Name + ", ";

            var area = await _stateProvinceService.GetStateProvinceByIdAsync(buyerRequest.AreaId);
            if (area != null)
                deliveryAddress += area.Name + ", ";

            if (!string.IsNullOrWhiteSpace(buyerRequest.DeliveryAddress))
                deliveryAddress += buyerRequest.DeliveryAddress + ", " + buyerRequest.DeliveryAddress2;

            if (order.ShippingStatusId == (int)ShippingStatus.Delivered || order.ShippingStatusId == (int)ShippingStatus.Shipped)
            {
                var orderShipment = (await _shipmentService.GetAllShipmentsAsync(orderId: order.Id, shipmentTypeId: (int)ShipmentType.SaleOrder)).FirstOrDefault();
                if (orderShipment is not null)
                {
                    orderShipped = orderShipment.ShippedDateUtc.HasValue ?
                        (await _dateTimeHelper.ConvertToUserTimeAsync(orderShipment.ShippedDateUtc.Value, DateTimeKind.Utc)).ToString("dd MMM yy - hh:mm tt") : "";

                    orderDelivered = orderShipment.DeliveryDateUtc.HasValue && order.ShippingStatusId == (int)ShippingStatus.Delivered ?
                        (await _dateTimeHelper.ConvertToUserTimeAsync(orderShipment.DeliveryDateUtc.Value, DateTimeKind.Utc)).ToString("dd MMM yy - hh:mm tt") : "";
                }
            }

            //balance quantity working
            decimal shipmentBalanceQuantity = 0;
            foreach (var orderItem in await _orderService.GetOrderItemsAsync(order.Id, isShipEnabled: true)) //we can ship only shippable products
            {
                var totalNumberOfItemsCanBeAddedToShipment = await _orderService.GetTotalNumberOfItemsCanBeAddedToShipmentAsync(orderItem);
                if (totalNumberOfItemsCanBeAddedToShipment <= 0)
                    continue;

                //yes, we have at least one item to create a new shipment
                shipmentBalanceQuantity += await _priceCalculationService.RoundPriceAsync(totalNumberOfItemsCanBeAddedToShipment);
            }

            var buyerContract = await _orderService.GetContractByOrderIdAsync(order.Id);
            if (buyerContract is not null)
                await _httpClient.GetStringAsync(_storeContext.GetCurrentStore().Url + $"download/get-contract/{buyerContract.ContractGuid}");

            var data = new
            {
                OrderInfo = new
                {
                    OrderId = order.Id,
                    order.CustomOrderNumber,
                    order.OrderStatusId,
                    OrderStatus = await _localizationService.GetLocalizedEnumAsync(order.OrderStatus),
                    Industry = industry,
                    Product = productName,
                    Brand = brand,
                    TotalQuantity = quantity,
                    QtyType = qtyType,
                    DeliveryDate = deliveryDate,
                    OrderCreatedDate = (await _dateTimeHelper.ConvertToUserTimeAsync(order.CreatedOnUtc, DateTimeKind.Utc)).ToString("dd MMM yy"),
                    OrderCreatedTime = (await _dateTimeHelper.ConvertToUserTimeAsync(order.CreatedOnUtc, DateTimeKind.Utc)).ToString("hh:mm tt"),
                    BuyerDeliveryAddress = deliveryAddress,
                    buyerRequest.BookerId,
                    BookerName = (await _customerService.GetCustomerByIdAsync(customerId: buyerRequest.BookerId))?.FullName,
                    BuyerRequestId = buyerRequest.Id,
                    BuyerName = (await _customerService.GetCustomerByIdAsync(customerId: order.CustomerId))?.FullName,
                    BuyerEmail = (await _customerService.GetCustomerByIdAsync(customerId: order.CustomerId))?.Email,
                    ProductAttributesInfo = await _utilityService.ParseAttributeXml(buyerRequest.ProductAttributeXml),
                    BuyerContract = new
                    {
                        contractId = buyerContract != null ? buyerContract.Id : 0,
                        downloadUrl = buyerContract != null ? _storeContext.GetCurrentStore().Url + $"download/get-contract/{buyerContract.ContractGuid}" : "",
                        previewUrl = buyerContract != null ? _storeContext.GetCurrentStore().Url + $"files/contracts/{buyerContract.ContractGuid}.pdf" : "",
                        contractSignature = buyerContract != null && buyerContract.SignaturePictureId > 0 ? true : false
                    }
                },
                OrderStatus = new
                {
                    OrderPending = (await _dateTimeHelper.ConvertToUserTimeAsync(order.CreatedOnUtc, DateTimeKind.Utc)).ToString("dd MMM yy - hh:mm tt"),
                    OrderProcessed = order.ProcessingDateUtc.HasValue ? (await _dateTimeHelper.ConvertToUserTimeAsync(order.ProcessingDateUtc.Value, DateTimeKind.Utc)).ToString("dd MMM yy - hh:mm tt") : "",
                    OrderShipped = orderShipped,
                    OrderDelivered = orderDelivered
                },
                ShippmentDetail = new
                {
                    DeliverAddress = buyerRequest.DeliveryAddress,
                    ShippingStatus = await _localizationService.GetLocalizedEnumAsync(order.ShippingStatus),
                    NoOfShipmentNumber = (await _shipmentService.GetShipmentsByOrderIdAsync(order.Id)).Count(),
                    QuantityLeft = shipmentBalanceQuantity,
                    Shippments = (await _shipmentService.GetAllShipmentsAsync(orderId: order.Id, shipmentTypeId: (int)ShipmentType.SaleOrder)).Select(async s =>
                    {
                        return new
                        {
                            ShippmentId = s.Id,
                            ShipmentStatus = await _localizationService.GetLocalizedEnumAsync(s.DeliveryStatus),
                        };
                    }).ToList(),
                    PinLocation = new
                    {
                        Location = buyerRequest.PinLocation_Location,
                        Longitude = buyerRequest.PinLocation_Longitude,
                        Latitude = buyerRequest.PinLocation_Latitude
                    }
                },
                expectedShipments = await _orderService.GetAllDeliveryScheduleAsync(orderId: order.Id).Result.SelectAwait(async b =>
                {
                    return new
                    {
                        expectedDeliveryDateUtc = (await _dateTimeHelper.ConvertToUserTimeAsync(b.ExpectedDeliveryDateUtc, DateTimeKind.Utc)).ToString("dd MMM yy - hh:mm tt"),
                        expectedShipmentDateUtc = (await _dateTimeHelper.ConvertToUserTimeAsync(b.ExpectedShipmentDateUtc, DateTimeKind.Utc)).ToString("dd MMM yy - hh:mm tt"),
                        expectedDeliveryCost = b.ExpectedDeliveryCost,
                        Quantity = b.ExpectedQuantity
                    };
                }).ToListAsync(),
                //Requests = await _orderService.SearchOrderDeliveryRequestsAsync(orderId: orderId).Result.SelectAwait(async b =>
                //{
                //    return new
                //    {
                //        id = b.Id,
                //        orderId = b.OrderId,
                //        orderCustomNumber = (await _orderService.GetOrderByIdAsync(b.OrderId)).CustomOrderNumber,
                //        //directOrderCalculationId = b.QuotationId,
                //        //directOrderCalculationNumber = (await _quotationService.GetQuotationByIdAsync(b.QuotationId))?.CustomQuotationNumber,
                //        statusId = b.StatusId,
                //        status = await _localizationService.GetLocalizedEnumAsync(b.OrderDeliveryRequestEnum),
                //        bagsDirectlyFromSupplier = b.BagsDirectlyFromSupplier,
                //        bagsDirectlyFromWarehouse = b.BagsDirectlyFromWarehouse,
                //        countryName = await _localizationService.GetLocalizedAsync(await _countryService.GetCountryByIdAsync(b.CountryId), x => x.Name),
                //        countryId = b.CountryId,
                //        cityName = await _localizationService.GetLocalizedAsync(await _stateProvinceService.GetStateProvinceByIdAsync(b.CityId), x => x.Name),
                //        cityId = b.CityId,
                //        areaName = await _localizationService.GetLocalizedAsync(await _stateProvinceService.GetStateProvinceByIdAsync(b.AreaId), x => x.Name),
                //        areaId = b.AreaId,
                //        streetaddress = b.StreetAddress,
                //        contactnumber = b.ContactNumber,
                //        totalQuantity = quantity,
                //        deliveredQuantity = b.Quantity,
                //        shipmentdate = b.ShipmentDateUtc,
                //        agentId = b.AgentId,
                //        agent = (await _customerService.GetCustomerByIdAsync(b.AgentId))?.FullName,
                //        timeRemaining = b.TicketExpiryDate.HasValue ? b.TicketExpiryDate.Value.Subtract(DateTime.Now).TotalHours > 0 || b.TicketExpiryDate.Value.Subtract(DateTime.Now).TotalMinutes > 0 ? b.TicketExpiryDate.Value.Subtract(DateTime.Now).ToString("hh") + " : hrs " + b.TicketExpiryDate.Value.Subtract(DateTime.Now).ToString("mm") + ": mns" : "0" : "0",
                //        date = (await _dateTimeHelper.ConvertToUserTimeAsync(b.CreatedOnUtc, DateTimeKind.Utc)).ToString("dd/MM/yy"),
                //        priority = await _localizationService.GetLocalizedEnumAsync(b.TicketEnum),
                //        requester = (await _customerService.GetCustomerByIdAsync(b.CreatedById))?.FullName,
                //        buyerName = (await _customerService.GetCustomerByIdAsync(customerId: buyerRequest.BuyerId))?.FullName,
                //        brand = buyerRequest.BrandId > 0 ? await _localizationService.GetLocalizedAsync((await _brandService.GetManufacturerByIdAsync(buyerRequest.BrandId)), x => x.Name) : "-"
                //    };
                //}).ToListAsync(),
                //POHistory = await _orderService.GetAllOrderPaymentsAsync(paymentType: "Payable", orderId: orderId).Result.OrderByDescending(x => x.Id).SelectAwait(async b =>
                //{
                //    return new
                //    {
                //        id = b.Id,
                //        orderId = b.OrderId,
                //        orderCustomNumber = (await _orderService.GetOrderByIdAsync(b.OrderId)).CustomOrderNumber,
                //        supplierId = b.SupplierId,
                //        supllierName = (await _customerService.GetCustomerByIdAsync(b.SupplierId))?.FullName,
                //        directOrderCalculationId = b.QuotationId,
                //        directOrderCalculationNumber = (await _sellerBidService.GetSellerBidByIdAsync(b.QuotationId))?.CustomQuotationNumber,
                //        statusId = b.OrderPaymentStatusId,
                //        status = await _localizationService.GetLocalizedEnumAsync(b.OrderPaymentStatus),
                //        deliveredQuantity = b.Quantity,
                //        date = (await _dateTimeHelper.ConvertToUserTimeAsync(b.CreatedOnUtc, DateTimeKind.Utc)).ToString("dd/MM/yy"),
                //        time = (await _dateTimeHelper.ConvertToUserTimeAsync(b.CreatedOnUtc, DateTimeKind.Utc)).ToString("hh:mm tt"),
                //    };
                //}).ToListAsync(),
                //ReturnHistory = await _orderService.SearchOrderSalesReturnRequestsAsync(orderId: orderId).Result.SelectAwait(async salesReturnRequests =>
                //{
                //    return new
                //    {
                //        id = salesReturnRequests.Id,
                //        orderId = salesReturnRequests.OrderId,
                //        supplierId = salesReturnRequests.SupplierId,
                //        supllierName = (await _customerService.GetCustomerByIdAsync(salesReturnRequests.SupplierId))?.FullName,
                //        directOrderCalculationId = salesReturnRequests.QuotationId,
                //        directOrderCalculationNumber = (await _sellerBidService.GetSellerBidByIdAsync(salesReturnRequests.QuotationId))?.CustomQuotationNumber,
                //        returnRequestDateUtc = (await _dateTimeHelper.ConvertToUserTimeAsync(salesReturnRequests.ReturnRequestDateUtc, DateTimeKind.Utc)).ToString("dd/MM/yy"),
                //        quantity = salesReturnRequests.Quantity,
                //        pickupAddress = salesReturnRequests.PickupAddress,
                //        dropOffAddress = salesReturnRequests.DropOffAddress,
                //        isInventory = salesReturnRequests.IsInventory,
                //        returnReason = salesReturnRequests.ReturnReason,
                //        agentId = salesReturnRequests.AgentId,
                //        agent = (await _customerService.GetCustomerByIdAsync(salesReturnRequests.AgentId))?.FullName,
                //        createdOnUtc = (await _dateTimeHelper.ConvertToUserTimeAsync(salesReturnRequests.CreatedOnUtc, DateTimeKind.Utc)).ToString("dd/MM/yy"),
                //        statusId = salesReturnRequests.StatusId,
                //        status = await _localizationService.GetLocalizedEnumAsync(salesReturnRequests.orderSalesReturnRequestEnum),
                //    };
                //}).ToListAsync()
            };
            return new { success = true, message = "", data };
        }


        public async Task<object> BuyerOrderSummary(int orderId)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                throw new Exception("user not found");

            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order is null || order.Deleted)
                throw new Exception("Order not found");

            var orderCalculation = await _orderService.GetOrderCalculationByOrderIdAsync(orderId);
            if (orderCalculation is null)
                throw new Exception("Order calculation not found");

            var totalReceived = await _shipmentService.GetOrderPaidAmount(order);
            var totalAmountBalance = orderCalculation.OrderTotal - totalReceived;

            var data = new
            {
                TotalReceivables = await _priceFormatter.FormatPriceAsync(orderCalculation.OrderTotal),
                TotalReceived = await _priceFormatter.FormatPriceAsync(totalReceived),
                TotalBalance = await _priceFormatter.FormatPriceAsync(totalAmountBalance),
                PaymentStatus = await _localizationService.GetLocalizedEnumAsync(order.PaymentStatus)
            };
            return new { success = true, message = "", data };
        }

        #endregion

        #region Cost Of Goods Selling

        public async Task<object> CogsInventoryList(int requestId)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                throw new Exception("user not found");

            var request = await _requestService.GetRequestByIdAsync(requestId);
            if (request is null)
                throw new Exception("Request not found");

            var data = await (await _inventoryService.GetAllDirectCogsInventoryTaggingsAsync(requestId: requestId)).SelectAwait(async cg =>
            {
                return new
                {
                    id = cg.Id,
                    rate = (await _inventoryService.GetInventoryInboundByIdAsync(cg.InventoryId))?.PurchaseRate,
                    quantity = cg.Quantity,
                    inventoryId = cg.InventoryId
                };
            }).ToListAsync();

            return new { success = true, data, requestQuantity = request.Quantity };
        }

        public async Task<object> GetInventoriesByRequest(int requestId)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                throw new Exception("user not found");


            var request = await _requestService.GetRequestByIdAsync(requestId);
            if (request is null)
                throw new Exception("Request not found");

            var data = new List<object>();

            //prepare available warehouses
            var inventoryGroup = await _inventoryService.GetInventoryGroupByBrandIdAndProductIdAsync(request.ProductId, request.BrandId, request.ProductAttributeXml);
            if (inventoryGroup is not null)
            {
                var inventoryInbounds = (await _inventoryService.GetAllInventoryInboundsAsync(groupId: inventoryGroup.Id)).OrderBy(x => x.Id).ToList();

                if (request.BusinessModelId == (int)BusinessModelEnum.Broker)
                    inventoryInbounds = (await _inventoryService.GetAllInventoryInboundsAsync(groupId: inventoryGroup.Id)).Where(x => x.BusinessModelId == request.BusinessModelId).OrderBy(x => x.Id).ToList();
                else
                    inventoryInbounds = (await _inventoryService.GetAllInventoryInboundsAsync(groupId: inventoryGroup.Id)).Where(x => x.BusinessModelId != (int)BusinessModelEnum.Broker).OrderBy(x => x.Id).ToList();
                await Task.WhenAll(inventoryInbounds.Select(async inventoryInbound =>
                {
                    var inStockQuantity = inventoryInbound.StockQuantity; //- outboundQty;

                    var cogsInventoryTaggedQuantity = (await _inventoryService.GetAllCogsInventoryTaggingsAsync(inventoryInbound.Id)).Sum(x => x.Quantity);
                    var balanceQuantity = inStockQuantity - cogsInventoryTaggedQuantity;
                    if (balanceQuantity > 0)
                        data.Add(new { Text = "Inventory # : " + inventoryInbound.Id + " - " + "Quantity : " + balanceQuantity, Value = inventoryInbound.Id.ToString(), Quantity = balanceQuantity, totalQuantity = inStockQuantity, outBoundQuantity = balanceQuantity });

                }));
            }
            return new { success = true, message = "", data };
        }

        public async Task<object> AddCogsInventoryTagging([FromBody] DirectOrderApiModel.DirectCogsInventoryTaggingModel model)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                throw new Exception("user not found");

            var request = await _requestService.GetRequestByIdAsync(model.requestId);
            if (request is null)
                throw new Exception("Request not found");

            var inventoryInbound = await _inventoryService.GetInventoryInboundByIdAsync(model.inventoryId);
            if (inventoryInbound is not null)
            {
                if (model.quantity > inventoryInbound.StockQuantity)
                    throw new Exception("Quantity exceed from the inventory quantity");
            }

            var cogsInventoryTaggings = await _inventoryService.GetAllDirectCogsInventoryTaggingsAsync(requestId: request.Id);
            if (cogsInventoryTaggings.Any())
            {
                var checkQuantity = cogsInventoryTaggings.Sum(x => x.Quantity) + model.quantity;
                if (checkQuantity > request.Quantity)
                    throw new Exception("Quantity exceed from the request quantity");
            }

            var actualCogsInventoryTaggings = await _inventoryService.GetAllCogsInventoryTaggingsAsync(requestId: request.Id);

            if (cogsInventoryTaggings.Sum(x => x.Quantity) + actualCogsInventoryTaggings.Sum(x => x.Quantity) + model.quantity > request.Quantity)
                throw new Exception("Quantity exceed from the request quantity");

            if (!cogsInventoryTaggings.Any(x => x.InventoryId == model.inventoryId) && !actualCogsInventoryTaggings.Any(x => x.InventoryId == model.inventoryId))
            {
                if (request.BusinessModelId == (int)BusinessModelEnum.Broker && cogsInventoryTaggings.Count >= 1)
                    throw new Exception("Do not add multiple inventories in broker");
                else
                {
                    await _inventoryService.InsertDirectCogsInventoryTaggingAsync(new DirectCogsInventoryTagging
                    {
                        InventoryId = model.inventoryId,
                        Quantity = model.quantity,
                        Rate = (await _inventoryService.GetInventoryInboundByIdAsync(model.inventoryId)).PurchaseRate,
                        RequestId = request.Id,
                    });
                }
            }
            else
                return new { success = false, message = "Cogs tagging already exist for the selected inventory" };


            var taggedInventories = await _inventoryService.GetAllDirectCogsInventoryTaggingsAsync(requestId: request.Id);
            decimal buyingPrice = await _utilityService.CalculateBuyingPriceByTaggings(taggedInventories.ToList());
            var buyingPriceFormatted = await _priceFormatter.FormatPriceAsync(buyingPrice, true, false);

            return new { success = true, message = "Cogs inventory tagging add", buyingPrice, buyingPriceFormatted };
        }

        public async Task<object> DeleteCogsInventoryTagging(int id)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                throw new Exception("user not found");

            var cogsInventoryTagging = await _inventoryService.GetDirectCogsInventoryTaggingByIdAsync(id);
            if (cogsInventoryTagging is null)
                throw new Exception("Cogs inventory mapping not found");

            await _inventoryService.DeleteDirectCogsInventoryTaggingAsync(cogsInventoryTagging);

            return new { success = true, message = "cogs inventory taggings deleted successfully" };
        }

        #endregion

        #region Temp Order Expected shipments

        public async Task<object> TempOrder_AddExpectedShipment(int tempOrderId)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                throw new Exception("user not found");

            var tempOrder = await _orderService.GetDirectOrderByIdAsync(tempOrderId);
            if (tempOrder is null)
                throw new Exception("Direct order not found");

            //Add default direct order delivery schedule 
            await _orderService.InsertDirectOrderDeliveryScheduleAsync(new DirectOrderDeliverySchedule
            {
                CreatedOnUtc = DateTime.UtcNow,
                ExpectedDeliveryDateUtc = null,
                ExpectedShipmentDateUtc = null,
                ExpectedDeliveryCost = 0,
                ExpectedQuantity = null,
                DirectOrderId = tempOrder.Id
            });

            var deliveryschedules = (await _orderService.GetAllDirectOrderDeliveryScheduleAsync(tempOrder.Id)).Select(async d =>
            {
                return new
                {
                    id = d.Id,
                    expectedDeliveryDateUtc = d.ExpectedDeliveryDateUtc.HasValue ? await _dateTimeHelper.ConvertToUserTimeAsync(d.ExpectedDeliveryDateUtc.Value, DateTimeKind.Utc) : (DateTime?)null,
                    expectedShipmentDateUtc = d.ExpectedShipmentDateUtc.HasValue ? await _dateTimeHelper.ConvertToUserTimeAsync(d.ExpectedShipmentDateUtc.Value, DateTimeKind.Utc) : (DateTime?)null,
                    expectedQuantity = d.ExpectedQuantity.HasValue ? d.ExpectedQuantity.ToString() : "",
                    expectedDeliveryCost = d.ExpectedDeliveryCost.HasValue ? d.ExpectedDeliveryCost.ToString() : "",
                    createdOnUtc = d.CreatedOnUtc,
                    directOrderId = d.DirectOrderId
                };
            }).Select(t => t.Result).ToList();
            return new { success = true, message = "Expected shipment add", data = deliveryschedules };

        }

        public async Task<object> TempOrder_UpdateExpectedShipment(DirectOrderApiModel.DirectOrderDeliveryScheduleModel model)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                throw new Exception("user not found");

            var tempOrder = await _orderService.GetDirectOrderByIdAsync(model.tempOrderId);
            if (tempOrder is null)
                throw new Exception("Temp order not found");

            var directOrderDeliverySchedule = await _orderService.GetDirectOrderDeliveryScheduleByIdAsync(model.Id);
            if (directOrderDeliverySchedule is null)
                throw new Exception("Delivery schedule not found");

            if (model.DeliveryDate.HasValue)
            {
                var date = model.DeliveryDate.Value.Subtract(DateTime.UtcNow);
                if (date.Hours < 0 || date.Minutes < 0)
                    throw new Exception("Invalid Date");

                directOrderDeliverySchedule.ExpectedDeliveryDateUtc = model.DeliveryDate.Value;
            }
            if (model.ShipmentDate.HasValue)
            {
                var date = model.ShipmentDate.Value.Subtract(DateTime.UtcNow);
                if (date.Hours < 0 || date.Minutes < 0)
                    throw new Exception("Invalid Date");

                directOrderDeliverySchedule.ExpectedShipmentDateUtc = model.ShipmentDate.Value;
            }

            if (model.DeliveryCost > 0)
                directOrderDeliverySchedule.ExpectedDeliveryCost = model.DeliveryCost;

            if (model.Quantity > 0)
                directOrderDeliverySchedule.ExpectedQuantity = model.Quantity;

            await _orderService.UpdateDirectOrderDeliveryScheduleAsync(directOrderDeliverySchedule);

            return new { success = true, message = "Expected delivery schedule updated" };

        }


        public async Task<object> TempOrder_DeleteExpectedDeliverySchedule(int expectedShipmentId)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                throw new Exception("user not found");

            var expectedshipment = await _orderService.GetDirectOrderDeliveryScheduleByIdAsync(expectedShipmentId);
            if (expectedshipment is null)
                throw new Exception("Expected shipment not found");

            await _orderService.DeleteDirectOrderDeliveryScheduleAsync(expectedshipment);

            return new { success = true, message = "Expected delivery schedule deleted" };

        }

        #endregion

        #region Supplier Booker

        public async Task<object> SellerRegistration(AccountApiModel.BookerSellerRegisterApiModel model)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                throw new Exception("user not found");


            //check whether registration is allowed
            if (_customerSettings.UserRegistrationType == UserRegistrationType.Disabled)
                return new { success = false, message = await _localizationService.GetResourceAsync("Account.Register.Result.Disabled") };

            if (!CommonHelper.IsValidEmail(model.Email))
                return new { success = false, message = await _localizationService.GetResourceAsync("Common.WrongEmail") };

            if (string.IsNullOrWhiteSpace(model.Phone))
                return new { success = false, message = await _localizationService.GetResourceAsync("Account.Fields.Username.Required") };

            var customer = new Customer
            {
                Active = true,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
                RegisteredInStoreId = (await _storeContext.GetCurrentStoreAsync()).Id,
                Source = "Tijara-APP",
                CreatedBy = user.Id
            };
            await _customerService.InsertCustomerAsync(customer);

            var customerEmail = model.Email?.Trim();
            var customerUserName = model.Phone?.Trim();

            var registrationRequest = new CustomerRegistrationRequest(customer,
                customerEmail,
                customerUserName,
                model.Password,
                _customerSettings.DefaultPasswordFormat,
                (await _storeContext.GetCurrentStoreAsync()).Id,
                true,
                isSupplier: true);

            var registrationResult = await _customerRegistrationService.RegisterCustomerAsync(registrationRequest);
            if (registrationResult.Success)
            {
                //form fields
                if (_customerSettings.FirstNameEnabled)
                    customer.FirstName = model.FirstName;
                if (_customerSettings.LastNameEnabled)
                    customer.LastName = model.LastName;
                if (_customerSettings.StreetAddressEnabled)
                    customer.StreetAddress = model.Address;
                if (_customerSettings.CompanyEnabled)
                    customer.Company = model.Company;
                if (_customerSettings.CountryEnabled)
                    customer.CountryId = model.CountryId;
                if (_customerSettings.CountryEnabled && _customerSettings.StateProvinceEnabled)
                    customer.StateProvinceId = model.StateProvinceId;
                if (_customerSettings.PhoneEnabled)
                    customer.Phone = model.Phone;
                customer.Gst = model.SupplierGST;
                if (model.IndustryId > 0)
                    customer.IndustryId = model.IndustryId;
                if (model.SupplierType > 0)
                    customer.UserTypeId = model.SupplierType;

                await _genericAttributeService.SaveAttributeAsync(customer, ZarayeCustomerDefaults.VatNumberAttribute, model.SupplierNTN);

                if (model.BookerCurrentLocation != null)
                {
                    await _genericAttributeService.SaveAttributeAsync(customer, ZarayeCustomerDefaults.RegisteredBookerCurrentLocationAttribute, $"{model.BookerCurrentLocation.Latitude},{model.BookerCurrentLocation.Longitude}");
                    await _genericAttributeService.SaveAttributeAsync(customer, ZarayeCustomerDefaults.RegisteredBookerCurrentLocationAddressAttribute, model.BookerCurrentLocation.Location);
                }

                if (model.SellerPinLocation != null)
                {
                    await _genericAttributeService.SaveAttributeAsync(customer, ZarayeCustomerDefaults.RegisteredPinLocationAttribute, $"{model.SellerPinLocation.Latitude},{model.SellerPinLocation.Longitude}");
                    await _genericAttributeService.SaveAttributeAsync(customer, ZarayeCustomerDefaults.RegisteredPinLocationAddressAttribute, model.SellerPinLocation.Location);
                }

                //insert default address (if possible)
                var defaultAddress = new Address
                {
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    Email = customer.Email,
                    Company = customer.Company,
                    CountryId = customer.CountryId > 0
                            ? customer.CountryId
                            : null,
                    StateProvinceId = customer.StateProvinceId > 0
                            ? customer.StateProvinceId
                            : null,
                    County = customer.County,
                    City = customer.City,
                    Address1 = customer.StreetAddress,
                    Address2 = customer.StreetAddress2,
                    ZipPostalCode = customer.ZipPostalCode,
                    PhoneNumber = customer.Phone,
                    FaxNumber = customer.Fax,
                    CreatedOnUtc = customer.CreatedOnUtc
                };
                if (await _addressService.IsAddressValidAsync(defaultAddress))
                {
                    //some validation
                    if (defaultAddress.CountryId == 0)
                        defaultAddress.CountryId = null;
                    if (defaultAddress.StateProvinceId == 0)
                        defaultAddress.StateProvinceId = null;
                    //set default address
                    //customer.Addresses.Add(defaultAddress);

                    await _addressService.InsertAddressAsync(defaultAddress);

                    await _customerService.InsertCustomerAddressAsync(customer, defaultAddress);

                    customer.BillingAddressId = defaultAddress.Id;
                    customer.ShippingAddressId = defaultAddress.Id;
                }

                if (model.ProductIds != null && model.ProductIds.Any())
                {
                    foreach (var productId in model.ProductIds)
                    {
                        //insert the new product supplier mapping
                        await _productService.InsertSupplierProductAsync(new SupplierProduct
                        {
                            SupplierId = customer.Id,
                            ProductId = productId,
                            Published = true,
                            DisplayOrder = 1
                        });
                    }
                }

                await _customerService.UpdateCustomerAsync(customer);

                //notifications
                if (_customerSettings.NotifyNewCustomerRegistration)
                    await _workflowMessageService.SendSupplierRegisteredNotificationMessageAsync(customer,
                        _localizationSettings.DefaultAdminLanguageId);

                //raise event       
                await _eventPublisher.PublishAsync(new CustomerRegisteredEvent(customer));

                //send customer welcome message
                await _workflowMessageService.SendCustomerWelcomeMessageAsync(customer, (await _workContext.GetWorkingLanguageAsync()).Id);

                //raise event       
                await _eventPublisher.PublishAsync(new CustomerActivatedEvent(customer));

                await _customerRegistrationService.SignInCustomerAsync(customer, null, true);

                return new { success = true, message = await _localizationService.GetResourceAsync("Account.Register.Result.Standard") };
            }

            return new { success = false, message = string.Join(",", registrationResult.Errors) };
        }

        public async Task<object> SearchSuppliers(string name = "")
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                throw new Exception("user not found");


            if (string.IsNullOrEmpty(name))
                throw new Exception("Name is required");

            var customerRoles = await _customerService.GetCustomerRoleBySystemNameAsync(ZarayeCustomerDefaults.SupplierRoleName);

            var suppliers = await _customerService.GetAllCustomersAsync(isActive: true, customerRoleIds: new int[] { customerRoles.Id }, fullName: name);
            if (suppliers.Any())
            {
                var data = suppliers.ToList().Select(async b =>
                {
                    return new
                    {
                        b.Id,
                        b.FullName,
                        b.Email,
                        Phone = b.Username,
                        IndustryId = await _genericAttributeService.GetAttributeAsync<int>(b, ZarayeCustomerDefaults.SupplierIndustryIdAttribute)
                    };
                }).Select(t => t.Result).ToList();

                return new { success = true, data };
            }

            return new { success = false, message = "no supplier found" };

        }

        public async Task<object> SearchSuppliersByProductId(int productId)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                throw new Exception("user not found");

            if (productId == 0)
                throw new Exception("Product id is required");

            var suppliers = (await _customerService.GetAllCustomersAsync(
                customerRoleIds: new int[] { (await _customerService.GetCustomerRoleBySystemNameAsync(ZarayeCustomerDefaults.SupplierRoleName)).Id },
                productId: productId)).ToList();

            IList<object> data = new List<object>();

            await Task.WhenAll(suppliers.Select(async supplier =>
            {
                data.Add(new
                {
                    Value = supplier.Id,
                    Text = supplier.FullName,
                });
            }));
            return new { success = true, data };
        }

        public async Task<object> GetRequestForQuotationHistory()
        {
            var supplier = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(supplier) /*|| !await _customerService.IsBookerAsync(booker)*/)
                throw new Exception("user not found");

            List<RequestForQuotation> allrequestForQuotations = new List<RequestForQuotation>();
            List<RequestForQuotationsModel> activeRequestForQuotations = new List<RequestForQuotationsModel>();
            List<RequestForQuotationsModel> pastRequestForQuotations = new List<RequestForQuotationsModel>();
            List<int> requestStatusIds = new List<int> { (int)RequestForQuotationStatus.Verified, (int)RequestForQuotationStatus.Cancelled, (int)RequestForQuotationStatus.UnVerified, (int)RequestForQuotationStatus.Expired };
            //Get all buyer request
            allrequestForQuotations = (await _requestService.GetAllRequestForQuotationAsync(rfqsIds: requestStatusIds, bookerId: supplier.Id)).ToList();
            if (allrequestForQuotations.Any())
            {
                var currentTime = await _dateTimeHelper.ConvertToUserTimeAsync(DateTime.UtcNow, DateTimeKind.Utc);

                //Get Active Request List Filter By Date 
                var activeRequestForQuotationDates = allrequestForQuotations.OrderByDescending(x => x.CreatedOnUtc).Where(x => x.RfqStatusId == (int)RequestForQuotationStatus.Verified).GroupBy(p => _dateTimeHelper.ConvertToUserTimeAsync(p.CreatedOnUtc, DateTimeKind.Utc).Result.ToString("dd/MM/yyyy")).ToList();
                await Task.WhenAll(activeRequestForQuotationDates.Select(async activeRequestForQuotationDate =>
                {
                    var model = new RequestForQuotationsModel();
                    var yesterday = currentTime.AddDays(-1).ToString("dd/MM/yyyy");
                    if (currentTime.ToString("dd/MM/yyyy") == activeRequestForQuotationDate.Key)
                        model.Date = "Today";
                    else if (currentTime.AddDays(-1).ToString("dd/MM/yyyy") == activeRequestForQuotationDate.Key)
                        model.Date = "Yesterday";
                    else
                        model.Date = activeRequestForQuotationDate.Key;

                    foreach (var activeRequestForQuotation in activeRequestForQuotationDate)
                    {
                        var request = await _requestService.GetRequestByIdAsync(activeRequestForQuotation.RequestId);
                        if (request is null)
                            continue;

                        var product = await _productService.GetProductByIdAsync(request.ProductId);
                        if (product is null)
                            continue;

                        var category = await _categoryService.GetCategoryByIdAsync(request.CategoryId);
                        if (category is null)
                            continue;

                        var industry = await _industryService.GetIndustryByIdAsync(request.IndustryId);
                        if (industry is null)
                            continue;

                        var buyer = await _customerService.GetCustomerByIdAsync(request.BuyerId);
                        if (buyer is null)
                            continue;

                        model.Data.Add(new RequestForQuotationData
                        {
                            Id = activeRequestForQuotation.Id,
                            CustomNumber = activeRequestForQuotation.CustomRfqNumber,
                            ProductId = product.Id,
                            ProductName = product.Name,
                            IndustryId = industry.Id,
                            IndustryName = industry.Name,
                            BuyerId = buyer.Id,
                            BuyerName = buyer.FullName,
                            BrandName = request.BrandId > 0 ? await _localizationService.GetLocalizedAsync(await _brandService.GetManufacturerByIdAsync(request.BrandId), x => x.Name) : "-",
                            BrandId = request.BrandId,
                            Quantity = activeRequestForQuotation.Quantity,
                            DeliveryAddress = request.DeliveryAddress,
                            //TotalQuotations = (await _sellerBidService.GetAllSellerBidAsync(buyerRequestId: activeRequestForQuotation.Id, sbIds: new List<int> { (int)QuotationStatus.Verified })).Count,
                            ExpiryDate = request.ExpiryDate.HasValue ? (await _dateTimeHelper.ConvertToUserTimeAsync(request.ExpiryDate.Value, DateTimeKind.Utc)).ToString("dd/MM/yy hh:mm tt") : "",
                            StatusId = activeRequestForQuotation.RfqStatusId,
                            Status = await _localizationService.GetLocalizedEnumAsync(activeRequestForQuotation.RequestForQuotationStatus),
                            AttributesInfo = await _utilityService.ParseAttributeXml(request.ProductAttributeXml)
                        });
                    }
                    activeRequestForQuotations.Add(model);
                }));

                //Get Active Request List Filter By Date 
                var pastRequestForQuotationsDates = allrequestForQuotations.OrderByDescending(x => x.CreatedOnUtc).Where(x => x.RfqStatusId == (int)RequestForQuotationStatus.UnVerified || x.RfqStatusId == (int)RequestForQuotationStatus.Cancelled || x.RfqStatusId == (int)RequestForQuotationStatus.Expired).GroupBy(p => _dateTimeHelper.ConvertToUserTimeAsync(p.CreatedOnUtc, DateTimeKind.Utc).Result.ToString("dd/MM/yyyy")).ToList();

                await Task.WhenAll(pastRequestForQuotationsDates.Select(async pastRequestForQuotationsDate =>
                {
                    var model = new RequestForQuotationsModel();
                    if (currentTime.ToString("dd/MM/yyyy") == pastRequestForQuotationsDate.Key)
                        model.Date = "Today";
                    else if (currentTime.AddDays(-1).ToString("dd/MM/yyyy") == pastRequestForQuotationsDate.Key)
                        model.Date = "Yesterday";
                    else
                        model.Date = pastRequestForQuotationsDate.Key;

                    foreach (var pastRequestForQuotation in pastRequestForQuotationsDate)
                    {
                        var request = await _requestService.GetRequestByIdAsync(pastRequestForQuotation.RequestId);
                        if (request is null)
                            continue;

                        var product = await _productService.GetProductByIdAsync(request.ProductId);
                        if (product is null)
                            continue;

                        var category = await _categoryService.GetCategoryByIdAsync(request.CategoryId);
                        if (category is null)
                            continue;

                        var industry = await _industryService.GetIndustryByIdAsync(request.IndustryId);
                        if (industry is null)
                            continue;

                        var buyer = await _customerService.GetCustomerByIdAsync(request.BuyerId);
                        if (buyer is null)
                            continue;

                        model.Data.Add(new RequestForQuotationData
                        {
                            Id = pastRequestForQuotation.Id,
                            CustomNumber = pastRequestForQuotation.CustomRfqNumber,
                            ProductId = product.Id,
                            ProductName = product.Name,
                            IndustryId = industry.Id,
                            IndustryName = industry.Name,
                            BuyerId = buyer.Id,
                            BuyerName = buyer.FullName,
                            BrandName = request.BrandId > 0 ? await _localizationService.GetLocalizedAsync(await _brandService.GetManufacturerByIdAsync(request.BrandId), x => x.Name) : "-",
                            BrandId = request.BrandId,
                            Quantity = pastRequestForQuotation.Quantity,
                            DeliveryAddress = request.DeliveryAddress,
                            //TotalQuotations = (await _sellerBidService.GetAllSellerBidAsync(buyerRequestId: request.Id, sbIds: new List<int> { (int)SellerBidStatus.QuotedToBuyer })).Count,
                            ExpiryDate = request.ExpiryDate.HasValue ? (await _dateTimeHelper.ConvertToUserTimeAsync(request.ExpiryDate.Value, DateTimeKind.Utc)).ToString("dd/MM/yy hh:mm tt") : "",
                            StatusId = pastRequestForQuotation.RfqStatusId,
                            Status = await _localizationService.GetLocalizedEnumAsync(pastRequestForQuotation.RequestForQuotationStatus),
                            AttributesInfo = await _utilityService.ParseAttributeXml(request.ProductAttributeXml)
                        });
                    }
                    pastRequestForQuotations.Add(model);
                }));

                var data = new List<object>();
                if (activeRequestForQuotations.Any())
                {
                    data.Add(new
                    {
                        DataType = await _localizationService.GetResourceAsync("Buyer.ActiveRequests"),
                        Data = activeRequestForQuotations
                    });
                }
                if (pastRequestForQuotations.Any())
                {
                    data.Add(new
                    {
                        DataType = await _localizationService.GetResourceAsync("Buyer.PastRequests"),
                        Data = pastRequestForQuotations
                    });
                }

                return new { success = true, message = "", data };
            }

            return new { success = false, message = "" };
        }
        //continue
        public async Task<object> QuotationHistory()
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                throw new Exception("user not found");

            List<Bids> activeBids = new List<Bids>();
            List<Bids> pastBids = new List<Bids>();
            List<Quotation> allBids = new List<Quotation>();
            List<int> bidsStatusIds = new List<int> { (int)QuotationStatus.Verified, (int)QuotationStatus.Cancelled, (int)QuotationStatus.UnVerified, (int)QuotationStatus.Expired };

            allBids = (await _quotationService.GetAllQuotationAsync(sbIds: bidsStatusIds/*, bookerId: user.Id*/)).ToList();
            if (allBids.Any())
            {
                //Prepare Active quotations
                var activequotations = allBids.Where(x => x.QuotationStatusId == (int)QuotationStatus.Verified).GroupBy(p => _dateTimeHelper.ConvertToUserTimeAsync(p.CreatedOnUtc, DateTimeKind.Utc).Result.ToString("dd/MM/yyyy")).ToList();
                await Task.WhenAll(activequotations.Select(async activequotation =>
                {
                    var model = new Bids();
                    model.Date = activequotation.Key;
                    activequotation.Select(async quotation =>
                    {
                        var requestForQuotation = await _requestService.GetRequestForQuotationByIdAsync(quotation.RfqId);

                        var buyerRequest = await _requestService.GetRequestByIdAsync(requestForQuotation.RequestId);

                        var product = await _productService.GetProductByIdAsync(buyerRequest.ProductId);

                        var category = await _categoryService.GetCategoryByIdAsync(buyerRequest.CategoryId);

                        model.Data.Add(new Bids.BidsData
                        {
                            Id = quotation.Id,
                            CustomQuotationNumber = quotation.CustomQuotationNumber,
                            BuyerRequestId = buyerRequest.Id,
                            SupplierId = quotation.SupplierId,
                            SupplierName = await _customerService.GetCustomerFullNameAsync(quotation.SupplierId),
                            BuyerId = buyerRequest.BuyerId,
                            BuyerName = (await _customerService.GetCustomerByIdAsync(buyerRequest.BuyerId))?.FullName,
                            ProductName = product.Name,
                            Brand = buyerRequest.BrandId > 0 ? await _localizationService.GetLocalizedAsync(await _brandService.GetManufacturerByIdAsync(buyerRequest.BrandId), x => x.Name) : "-",
                            BrandId = quotation.BrandId,
                            Quantity = quotation.Quantity,
                            QtyType = (await _measureService.GetMeasureWeightByIdAsync(product.UnitId))?.Name ?? string.Empty,
                            UnitPrice = await _priceFormatter.FormatPriceAsync(quotation.QuotationPrice),
                            BidPrice = await _priceFormatter.FormatPriceAsync(quotation.QuotationPrice * quotation.Quantity),
                            ExpiryDate = (await _dateTimeHelper.ConvertToUserTimeAsync(quotation.PriceValidity, DateTimeKind.Utc)).ToString("dd MMM, yyyy hh:mm tt"),
                            CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(quotation.CreatedOnUtc, DateTimeKind.Utc),
                            Status = await _localizationService.GetLocalizedEnumAsync(quotation.QuotationStatus),
                            StatusId = quotation.QuotationStatusId,
                            IndustryName = (await _industryService.GetIndustryByIdAsync(buyerRequest.IndustryId))?.Name ?? "-",
                            CategoryName = (await _categoryService.GetCategoryByIdAsync(buyerRequest.CategoryId))?.Name ?? "-"
                        });
                    }).ToList();
                    //foreach (var quotation in activequotation)
                    //{
                    //    var requestForQuotation = await _requestService.GetRequestForQuotationByIdAsync(quotation.RfqId);
                    //    if (requestForQuotation is null)
                    //        continue;

                    //    var buyerRequest = await _requestService.GetRequestByIdAsync(requestForQuotation.RequestId);
                    //    if (buyerRequest is null)
                    //        continue;

                    //    var product = await _productService.GetProductByIdAsync(buyerRequest.ProductId);
                    //    if (product is null)
                    //        continue;

                    //    var category = await _categoryService.GetCategoryByIdAsync(buyerRequest.CategoryId);
                    //    if (category is null)
                    //        continue;

                    //    model.Data.Add(new Bids.BidsData
                    //    {
                    //        Id = quotation.Id,
                    //        CustomQuotationNumber = quotation.CustomQuotationNumber,
                    //        BuyerRequestId = buyerRequest.Id,
                    //        SupplierId = quotation.SupplierId,
                    //        SupplierName = await _customerService.GetCustomerFullNameAsync(quotation.SupplierId),
                    //        BuyerId = buyerRequest.BuyerId,
                    //        BuyerName = (await _customerService.GetCustomerByIdAsync(buyerRequest.BuyerId))?.FullName,
                    //        ProductName = product.Name,
                    //        Brand = buyerRequest.BrandId > 0 ? await _localizationService.GetLocalizedAsync(await _brandService.GetManufacturerByIdAsync(buyerRequest.BrandId), x => x.Name) : "-",
                    //        BrandId = quotation.BrandId,
                    //        Quantity = quotation.Quantity,
                    //        QtyType = (await _measureService.GetMeasureWeightByIdAsync(product.UnitId))?.Name ?? string.Empty,
                    //        UnitPrice = await _priceFormatter.FormatPriceAsync(quotation.QuotationPrice),
                    //        BidPrice = await _priceFormatter.FormatPriceAsync(quotation.QuotationPrice * quotation.Quantity),
                    //        ExpiryDate = (await _dateTimeHelper.ConvertToUserTimeAsync(quotation.PriceValidity, DateTimeKind.Utc)).ToString("dd MMM, yyyy hh:mm tt"),
                    //        CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(quotation.CreatedOnUtc, DateTimeKind.Utc),
                    //        Status = await _localizationService.GetLocalizedEnumAsync(quotation.QuotationStatus),
                    //        StatusId = quotation.QuotationStatusId,
                    //        IndustryName = (await _industryService.GetIndustryByIdAsync(buyerRequest.IndustryId))?.Name ?? "-",
                    //        CategoryName = (await _categoryService.GetCategoryByIdAsync(buyerRequest.CategoryId))?.Name ?? "-"
                    //    });
                    //}
                    activeBids.Add(model);
                }));

                //Prepare Active quotations
                var pastquotations = allBids.Where(x => x.QuotationStatusId == (int)QuotationStatus.Cancelled || x.QuotationStatusId == (int)QuotationStatus.UnVerified || x.QuotationStatusId == (int)QuotationStatus.Expired).GroupBy(p => _dateTimeHelper.ConvertToUserTimeAsync(p.CreatedOnUtc, DateTimeKind.Utc).Result.ToString("dd/MM/yyyy")).ToList();
                await Task.WhenAll(pastquotations.Select(async pastquotation =>
                {
                    var model = new Bids();
                    model.Date = pastquotation.Key;
                    foreach (var quotation in pastquotation)
                    {
                        var requestForQuotation = await _requestService.GetRequestForQuotationByIdAsync(quotation.RfqId);
                        if (requestForQuotation is null)
                            continue;

                        var buyerRequest = await _requestService.GetRequestByIdAsync(requestForQuotation.RequestId);
                        if (buyerRequest is null)
                            continue;

                        var product = await _productService.GetProductByIdAsync(buyerRequest.ProductId);
                        if (product is null)
                            continue;

                        var category = await _categoryService.GetCategoryByIdAsync(buyerRequest.CategoryId);
                        if (category is null)
                            continue;

                        model.Data.Add(new Bids.BidsData
                        {
                            Id = quotation.Id,
                            CustomQuotationNumber = quotation.CustomQuotationNumber,
                            BuyerRequestId = buyerRequest.Id,
                            SupplierId = quotation.SupplierId,
                            SupplierName = await _customerService.GetCustomerFullNameAsync(quotation.SupplierId),
                            BuyerId = buyerRequest.BuyerId,
                            BuyerName = (await _customerService.GetCustomerByIdAsync(buyerRequest.BuyerId))?.FullName,
                            ProductName = product.Name,
                            Brand = buyerRequest.BrandId > 0 ? await _localizationService.GetLocalizedAsync(await _brandService.GetManufacturerByIdAsync(buyerRequest.BrandId), x => x.Name) : "-",
                            BrandId = quotation.BrandId,
                            Quantity = quotation.Quantity,
                            QtyType = (await _measureService.GetMeasureWeightByIdAsync(product.UnitId))?.Name ?? string.Empty,
                            UnitPrice = await _priceFormatter.FormatPriceAsync(quotation.QuotationPrice),
                            BidPrice = await _priceFormatter.FormatPriceAsync(quotation.QuotationPrice * quotation.Quantity),
                            ExpiryDate = (await _dateTimeHelper.ConvertToUserTimeAsync(quotation.PriceValidity, DateTimeKind.Utc)).ToString("dd MMM, yyyy hh:mm tt"),
                            CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(quotation.CreatedOnUtc, DateTimeKind.Utc),
                            Status = await _localizationService.GetLocalizedEnumAsync(quotation.QuotationStatus),
                            StatusId = quotation.QuotationStatusId,
                            IndustryName = (await _categoryService.GetCategoryByIdAsync(buyerRequest.IndustryId))?.Name ?? "-",
                            CategoryName = (await _categoryService.GetCategoryByIdAsync(buyerRequest.CategoryId))?.Name ?? "-"
                        });
                    }
                    pastBids.Add(model);
                }));

                var data = new List<object>();
                if (activeBids.Count > 0)
                {
                    data.Add(new
                    {
                        DataType = await _localizationService.GetResourceAsync("Supplier.ActiveQuotations"),
                        Data = activeBids
                    });
                }
                if (pastBids.Count > 0)
                {
                    data.Add(new
                    {
                        DataType = await _localizationService.GetResourceAsync("Supplier.PastQuotations"),
                        Data = pastBids
                    });
                }

                return new { success = true, data };
            }
            return new { success = false, message = "" };
        }

        public async Task<object> MarketDataBySupplier(int industryId, int categoryId = 0, int productId = 0, DateTime? startDate = null, DateTime? endDate = null)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                throw new Exception("user not found");

            //get parameters to filter buyer request
            var startDateUtc = startDate.HasValue ?
                (DateTime?)_dateTimeHelper.ConvertToUtcTime(startDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()) : null;
            var endDateUtc = endDate.HasValue ?
                (DateTime?)_dateTimeHelper.ConvertToUtcTime(endDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1) : null;

            var industry = await _industryService.GetIndustryByIdAsync(industryId);
            if (industry == null)
                return new { success = false, message = "Industry not found" };

            List<object> list = new List<object>();

            var requestForQuotations = (await _requestService.GetAllRequestForQuotationAsync(rfqsIds: new List<int> { (int)RequestForQuotationStatus.Verified },
                industryId: industryId, categoryId: categoryId, productId: productId, startDateUtc: startDateUtc, endDateUtc: endDateUtc)).ToList();

            await Task.WhenAll(requestForQuotations.Select(async requestForQuotation =>
            {
                var request = await _requestService.GetRequestByIdAsync(requestForQuotation.RequestId);

                var category = await _categoryService.GetCategoryByIdAsync(request.CategoryId);

                var product = await _productService.GetProductByIdAsync(request.ProductId);

                var buyer = await _customerService.GetCustomerByIdAsync(request.BuyerId);

                list.Add(new
                {
                    requestForQuotation.Id,
                    CustomNumber = requestForQuotation.CustomRfqNumber,
                    ProductId = product.Id,
                    ProductName = product.Name,
                    IndustryId = industry.Id,
                    IndustryName = industry.Name,
                    BuyerId = buyer.Id,
                    BuyerName = buyer.FullName,
                    BrandName = request.BrandId > 0 ? await _localizationService.GetLocalizedAsync(await _brandService.GetManufacturerByIdAsync(request.BrandId), x => x.Name) : "-",
                    request.BrandId,
                    requestForQuotation.Quantity,
                    request.DeliveryAddress,
                    DeliveryDate = (await _dateTimeHelper.ConvertToUserTimeAsync(request.DeliveryDate, DateTimeKind.Utc)).ToString("dd MMM, yyyy"),
                    TotalQuotations = (await _quotationService.GetAllQuotationAsync(RfqId: requestForQuotation.Id, sbIds: new List<int> { (int)QuotationStatus.Pending, (int)QuotationStatus.Verified })).Count,
                    ExpiryDate = request.ExpiryDate.HasValue ? (await _dateTimeHelper.ConvertToUserTimeAsync(request.ExpiryDate.Value, DateTimeKind.Utc)).ToString("dd/MM/yy hh:mm tt") : "",
                    StatusId = requestForQuotation.RfqStatusId,
                    Status = await _localizationService.GetLocalizedEnumAsync(requestForQuotation.RequestForQuotationStatus),
                    AttributesInfo = await _utilityService.ParseAttributeXml(request.ProductAttributeXml)
                });
            }));

            return new { success = true, message = "", data = list };
        }

        public async Task<object> GetRequestForQuotation(int requestForQuotationId)
        {
            if (requestForQuotationId <= 0)
                return new { success = false, message = "Request for quotation id is required" };

            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                return new { success = false, message = "user not found" };

            var requestForQuotation = await _requestService.GetRequestForQuotationByIdAsync(requestForQuotationId);
            if (requestForQuotation is null)
                return new { success = false, message = "Request for quotation not found" };

            var request = await _requestService.GetRequestByIdAsync(requestForQuotation.RequestId);
            if (request is null)
                return new { success = false, message = "Request not found" };

            var category = await _categoryService.GetCategoryByIdAsync(request.CategoryId);
            if (category is null)
                return new { success = false, message = "Category not found" };

            var product = await _productService.GetProductByIdAsync(request.ProductId);
            if (product is null)
                return new { success = false, message = "Product not found" };

            var data = new
            {
                requestForQuotation.Id,
                buyerRequestId = requestForQuotation.RequestId,
                customRfqNumber = requestForQuotation.CustomRfqNumber,
                quantity = requestForQuotation.Quantity,
                category = category.Name,
                product = product.Name,
                productId = product.Id,
                deliveryDate = (await _dateTimeHelper.ConvertToUserTimeAsync(request.DeliveryDate, DateTimeKind.Utc)).ToString("dd MMM, yyyy"),
                status = await _localizationService.GetLocalizedEnumAsync(requestForQuotation.RequestForQuotationStatus),
            };
            return new { success = true, message = "", data };
        }

        public async Task<object> AddQuotationMultiple(int requestForQuotationId, [FromBody] List<RFQQuotationsModel> model)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                return new { success = false, message = "user not found" };

            if (requestForQuotationId <= 0)
                return new { success = false, message = "Request for quotation id is required" };

            var requestForQuotation = await _requestService.GetRequestForQuotationByIdAsync(requestForQuotationId);
            if (requestForQuotation is null)
                return new { success = false, message = "Request for quotation not found" };

            var request = await _requestService.GetRequestByIdAsync(requestForQuotation.RequestId);
            if (request is null)
                return new { success = false, message = "Request not found" };

            var product = await _productService.GetProductByIdAsync(request.ProductId);
            if (product == null || product.Deleted || !product.AppPublished)
                return new { success = false, message = "Buyer request product not found" };

            var currentUserTime = await _dateTimeHelper.ConvertToUserTimeAsync(DateTime.UtcNow, DateTimeKind.Utc);

            foreach (var directOrderCalculation in model)
            {
                var supplier = await _customerService.GetCustomerByIdAsync(directOrderCalculation.SupplierId);
                if (supplier == null)
                    return new { success = false, message = "Supplier not found" };

                if (directOrderCalculation.Quantity <= 0)
                    return new { success = false, message = "Quantity should be greater than 0" };

                if (directOrderCalculation.BusinessModelId <= 0)
                    return new { success = false, message = "Business model id is required" };

                if (directOrderCalculation.PriceValidity <= currentUserTime)
                    return new { success = false, message = "Invalid price validity" };

                var newdirectOrderCalculation = new Quotation
                {
                    RfqId = requestForQuotation.Id,
                    BookerId = user.Id,
                    SupplierId = directOrderCalculation.SupplierId,
                    QuotationStatus = QuotationStatus.Verified,
                    BrandId = request.BrandId,
                    QuotationPrice = directOrderCalculation.Price,
                    Quantity = directOrderCalculation.Quantity,
                    BusinessModelId = directOrderCalculation.BusinessModelId,
                    PriceValidity = directOrderCalculation.PriceValidity,
                    IsApproved = false,
                    Deleted = false,
                    CreatedOnUtc = DateTime.UtcNow,
                    Source = "Tijara"
                };
                await _quotationService.InsertQuotationAsync(newdirectOrderCalculation);

                //generate and set custom Quotation number
                newdirectOrderCalculation.CustomQuotationNumber = _customNumberFormatter.GenerateQuotationCustomNumber(newdirectOrderCalculation);
                await _quotationService.UpdateQuotationAsync(newdirectOrderCalculation);

            }

            return new { success = true, message = await _localizationService.GetResourceAsync("Seller.Bid.Created.Success") };
        }

        public async Task<object> GetAllQuotationsByRFQId(int requestForQuotationId)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                return new { success = false, message = "User not found" };

            if (requestForQuotationId <= 0)
                return new { success = false, message = "Request id is required" };

            var requestForQuotation = await _requestService.GetRequestForQuotationByIdAsync(requestForQuotationId);
            if (requestForQuotation is null)
                return new { success = false, message = "Request for directOrderCalculation not found" };

            var request = await _requestService.GetRequestByIdAsync(requestForQuotation.RequestId);
            if (request is null)
                return new { success = false, message = "Request not found" };

            var industry = await _industryService.GetIndustryByIdAsync(request.IndustryId);
            if (industry is null)
                return new { success = false, message = "Industry not found" };

            var category = await _categoryService.GetCategoryByIdAsync(request.CategoryId);
            if (category is null)
                return new { success = false, message = "Category not found" };

            var product = await _productService.GetProductByIdAsync(request.ProductId);
            if (product is null)
                return new { success = false, message = "Product not found" };

            var data = await (await _quotationService.GetAllQuotationAsync(sbIds: new List<int> { (int)QuotationStatus.Verified }, RfqId: requestForQuotation.Id)).SelectAwait(async qu =>
            {
                return new
                {
                    id = qu.Id,
                    customNumber = qu.CustomQuotationNumber,
                    rfqId = qu.RfqId,
                    status = await _localizationService.GetLocalizedEnumAsync(qu.QuotationStatus),
                    expiryDate = (await _dateTimeHelper.ConvertToUserTimeAsync(qu.PriceValidity, DateTimeKind.Utc)).ToString("dd MMM, yyyy"),
                    industry = industry.Name,
                    category = category.Name,
                    product = product.Name,
                    formatPrice = await _priceFormatter.FormatPriceAsync(qu.QuotationPrice, true, false),
                    price = qu.QuotationPrice,
                    totalFormatPrice = await _priceFormatter.FormatPriceAsync(qu.QuotationPrice * qu.Quantity, true, false),
                    brandId = qu.BrandId,
                    brand = (await _brandService.GetManufacturerByIdAsync(qu.BrandId))?.Name,
                    supplierId = qu.SupplierId,
                    supplierName = (await _customerService.GetCustomerByIdAsync(qu.SupplierId))?.FullName,
                    quantity = qu.Quantity,
                    businessModelId = qu.BusinessModelId,
                    businessModelName = await _localizationService.GetLocalizedEnumAsync(qu.BusinessModelEnum),
                    //priceValidity = (await _dateTimeHelper.ConvertToUserTimeAsync(qu.PriceValidity, DateTimeKind.Utc)).ToString()
                };
            }).ToListAsync();
            return new { success = true, data };
        }

        public async Task<object> RejectRequestForQuotation(RejectModel model)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                return new { success = false, message = "User not found" };

            if (model.Id <= 0)
                return new { success = false, message = "Request for quotation id is required" };

            var requestForQuotation = await _requestService.GetRequestForQuotationByIdAsync(model.Id);
            if (requestForQuotation is null)
                return new { success = false, message = "Request for quotation not found" };

            if (string.IsNullOrWhiteSpace(model.RejectedReason))
                return new { success = false, message = "Rejected reason is required" };

            if (model.RejectedReason == "Other" && string.IsNullOrWhiteSpace(model.RejectedOtherReason))
                return new { success = false, message = "Rejected other reason is required" };

            requestForQuotation.RequestForQuotationStatus = RequestForQuotationStatus.Cancelled;

            if (model.RejectedReason == "Other" && !string.IsNullOrWhiteSpace(model.RejectedOtherReason))
                requestForQuotation.RejectedReason = $"Other - {model.RejectedOtherReason}";
            else
                requestForQuotation.RejectedReason = model.RejectedReason;

            await _requestService.UpdateRequestForQuotationAsync(requestForQuotation);

            var quotations = await _quotationService.GetAllQuotationAsync(RfqId: requestForQuotation.Id);
            foreach (var quotation in quotations)
            {
                quotation.QuotationStatus = QuotationStatus.Cancelled;
                await _quotationService.UpdateQuotationAsync(quotation);
            }

            return new { success = true, message = await _localizationService.GetResourceAsync("RequestForQuotation.Rejected.Success") };
        }

        public async Task<object> GetSupplierContract(int orderId, int supplierId)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                return new { success = false, message = "user not found" };

            var supplier = await _customerService.GetCustomerByIdAsync(supplierId);
            if (!await _customerService.IsRegisteredAsync(supplier) && !await _customerService.IsSupplierAsync(supplier))
                return new { success = false, message = "supplier not found" };

            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order is null || order.Deleted || order.OrderStatus == OrderStatus.Cancelled)
                return new { success = false, message = "Order not found." };

            var supplierContract = (await _orderService.GetAllContractAsync(orderId: orderId, supplierId: supplierId)).FirstOrDefault();
            if (supplierContract is null)
                return new { success = false, message = "Contract not found." };

            return new
            {
                success = true,
                message = "",
                SuuplierContract = new
                {
                    contractId = supplierContract.Id,
                    downloadUrl = _storeContext.GetCurrentStore().Url + $"download/get-contract/{supplierContract.ContractGuid}",
                    previewUrl = _storeContext.GetCurrentStore().Url + $"files/contracts/{supplierContract.ContractGuid}.pdf",
                    contractSignature = supplierContract.SignaturePictureId > 0 ? true : false
                }
            };
        }

        public async Task<object> GetOrderDetailForPickupSchedule(int expectedShipmentId)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                return new { success = false, message = "User not found" };

            var expectedShipment = await _orderService.GetDeliveryScheduleByIdAsync(expectedShipmentId);
            if (expectedShipment is null)
                return new { success = false, message = "Expected shipment not found" };

            var order = await _orderService.GetOrderByIdAsync(expectedShipment.OrderId);
            if (order is null)
                return new { success = false, message = "Order not found" };

            var orderItem = (await _orderService.GetOrderItemsAsync(order.Id)).FirstOrDefault();
            if (orderItem is null)
                return new { success = false, message = "Order item not found" };

            var buyerRequest = await _requestService.GetRequestByIdAsync(order.RequestId);
            if (buyerRequest is null)
                return new { success = false, message = "Buyer Request not found" };

            var product = await _productService.GetProductByIdAsync(buyerRequest.ProductId);
            if (product is null || product.Id != buyerRequest.ProductId)
                return new { success = false, message = "Buyer request product not found" };

            var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);
            if (customer is null)
                return new { success = false, message = "Customer not found" };

            var data = new
            {
                orderId = order.Id,
                customOrderNumber = order.CustomOrderNumber,
                //buyerId = customer.Id,
                //buyerName = customer.FullName,
                suplierId = customer.Id,
                suplierName = customer.FullName,
                brand = buyerRequest.BrandId > 0 ? await _localizationService.GetLocalizedAsync(await _brandService.GetManufacturerByIdAsync(buyerRequest.BrandId), x => x.Name) : "-",
                leftQuantity = orderItem.Quantity - _orderService.GetTotalOrderDeliveryRequestQuantityByOrderIdAsync(order.Id, expectedShipment.Id),
                //leftQuantity = orderItem.Quantity - remaining,
                totalQuantity = buyerRequest.Quantity,
                expectedShipmentQuantity = expectedShipment.ExpectedQuantity,
                ProductName = product.Name,
                ProductAttributesInfo = await _utilityService.ParseAttributeXml(buyerRequest.ProductAttributeXml),
                countryId = buyerRequest.CountryId,
                stateId = buyerRequest.CityId,
                areaId = buyerRequest.AreaId,
                streetAddress = buyerRequest.DeliveryAddress,
                contactNo = customer.Phone
            };

            return new { success = true, data };
        }

        public async Task<object> AddPickupRequest(OrderDeliveryRequestModel model)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                return new { success = false, message = "user not found" };

            if (model.ExpectedShipmentId <= 0)
                return new { success = false, message = "Expected shipment id is required" };

            if (model.CountryId <= 0)
                return new { success = false, message = "Country is required" };

            if (model.CityId <= 0)
                return new { success = false, message = "City is required" };

            if (model.AreaId <= 0)
                return new { success = false, message = "Area is required" };

            if (model.AgentId <= 0)
                return new { success = false, message = "Agent is required" };

            if (model.Quantity <= 0)
                return new { success = false, message = "Quantity is required" };

            if (model.BagsDirectlyFromWarehouse && model.WarehouseId == 0)
                return new { success = false, message = "Warehouse is required" };

            var deliveryScedule = await _orderService.GetDeliveryScheduleByIdAsync(model.ExpectedShipmentId);
            if (deliveryScedule is null)
                return new { success = false, message = "Expected shipment not found" };

            var order = await _orderService.GetOrderByIdAsync(deliveryScedule.OrderId);
            if (order is null || order.Deleted)
                return new { success = false, message = "Order not found" };

            var orderItem = (await _orderService.GetOrderItemsAsync(order.Id)).FirstOrDefault();
            if (orderItem is null)
                return new { success = false, message = "Order item not found" };

            var buyerRequest = await _requestService.GetRequestByIdAsync(order.RequestId);
            if (buyerRequest is null)
                return new { success = false, message = "Buyer Request not found" };


            decimal totalLeftQuantity = orderItem.Quantity - _orderService.GetTotalOrderDeliveryRequestQuantityByOrderIdAsync(order.Id, model.ExpectedShipmentId);
            //decimal totalLeftQuantity = deliveryScedule.ExpectedQuantity - remaining;
            if (totalLeftQuantity <= 0)
                return new { success = false, message = "There is no left quantity " };

            if (model.Quantity > totalLeftQuantity)
                return new { success = false, message = "Left Quantity ", leftQuantity = totalLeftQuantity };

            var industry = await _industryService.GetIndustryByIdAsync(buyerRequest.IndustryId);
            if (industry is null)
                return new { success = false, message = "Industry not found" };

            var category = await _categoryService.GetCategoryByIdAsync(buyerRequest.CategoryId);
            if (category is null)
                return new { success = false, message = "Category not found" };

            var orderDeliveryRequest = new OrderDeliveryRequest
            {
                OrderId = order.Id,
                OrderDeliveryScheduleId = deliveryScedule.Id,
                StatusId = (int)OrderDeliveryRequestEnum.Pending,
                BagsDirectlyFromSupplier = model.BagsDirectlyFromSupplier,
                BagsDirectlyFromWarehouse = model.BagsDirectlyFromWarehouse,
                WarehouseId = model.WarehouseId,
                CountryId = model.CountryId,
                CityId = model.CityId,
                AreaId = model.AreaId,
                StreetAddress = model.StreetAddress,
                ContactNumber = model.ContactNumber,
                Quantity = model.Quantity,
                //ShipmentDateUtc = model.ShipmentDateUtc,
                Deleted = false,
                AgentId = model.AgentId,
                TicketExpiryDate = DateTime.UtcNow.AddDays(category.TicketExpiryDays > 0 ? category.TicketExpiryDays : 1),
                TicketPirority = category.TicketPirority > 0 ? category.TicketPirority : (int)TicketEnum.Medium
            };
            await _orderService.InsertOrderDeliveryRequestAsync(orderDeliveryRequest);

            return new { success = true, message = await _localizationService.GetResourceAsync("Order.Delivery.Request.Created.Success") };
        }

        public async Task<object> PickupRequestDeatil(int expectedShipmentId)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                return new { success = false, message = "User not found" };

            var deliveryRequest = await _orderService.GetOrderDeliveryRequestByIdAsync(expectedShipmentId);
            if (deliveryRequest is null)
                return new { success = false, message = "delivery request not found" };

            var order = await _orderService.GetOrderByIdAsync(deliveryRequest.OrderId);
            if (order is null)
                return new { success = false, message = "delivery request order not found" };

            var buyerRequest = await _requestService.GetRequestByIdAsync(order.RequestId);
            if (buyerRequest is null)
                return new { success = false, message = "Buyer Request not found" };

            var product = await _productService.GetProductByIdAsync(buyerRequest.ProductId);
            if (product is null || product.Id != buyerRequest.ProductId)
                return new { success = false, message = "Buyer request product not found" };

            var deliveryRequestDetail = new
            {
                id = deliveryRequest.Id,
                wareHouseId = deliveryRequest.WarehouseId,
                wareHouseName = deliveryRequest.WarehouseId > 0 ? (await _shippingService.GetWarehouseByIdAsync(deliveryRequest.WarehouseId)).Name : "",
                statusId = deliveryRequest.StatusId,
                status = await _localizationService.GetLocalizedEnumAsync(deliveryRequest.OrderDeliveryRequestEnum),
                bagsDirectlyFromSupplier = deliveryRequest.BagsDirectlyFromSupplier,
                bagsDirectlyFromWarehouse = deliveryRequest.BagsDirectlyFromWarehouse,
                countryName = await _localizationService.GetLocalizedAsync(await _countryService.GetCountryByIdAsync(deliveryRequest.CountryId), x => x.Name),
                countryId = deliveryRequest.CountryId,
                cityName = await _localizationService.GetLocalizedAsync(await _stateProvinceService.GetStateProvinceByIdAsync(deliveryRequest.CityId), x => x.Name),
                cityId = deliveryRequest.CityId,
                areaName = await _localizationService.GetLocalizedAsync(await _stateProvinceService.GetStateProvinceByIdAsync(deliveryRequest.AreaId), x => x.Name),
                areaId = deliveryRequest.AreaId,
                streetaddress = deliveryRequest.StreetAddress,
                contactnumber = deliveryRequest.ContactNumber,
                totalQuantity = buyerRequest.Quantity,
                deliveredQuantity = deliveryRequest.Quantity,
                shipmentdate = (await _dateTimeHelper.ConvertToUserTimeAsync(deliveryRequest.CreatedOnUtc, DateTimeKind.Utc)).ToString("MM/dd/yyyy h:mm:ss tt") /*deliveryRequest.ShipmentDateUtc*/,
                agentId = deliveryRequest.AgentId,
                agent = (await _customerService.GetCustomerByIdAsync(deliveryRequest.AgentId))?.FullName,
                timeRemaining = deliveryRequest.TicketExpiryDate.HasValue ? deliveryRequest.TicketExpiryDate.Value.Subtract(DateTime.Now).TotalHours > 0 || deliveryRequest.TicketExpiryDate.Value.Subtract(DateTime.Now).TotalMinutes > 0 ? deliveryRequest.TicketExpiryDate.Value.Subtract(DateTime.Now).ToString("hh") + " : hrs " + deliveryRequest.TicketExpiryDate.Value.Subtract(DateTime.Now).ToString("mm") + ": mns" : "0" : "0",
                date = (await _dateTimeHelper.ConvertToUserTimeAsync(deliveryRequest.CreatedOnUtc, DateTimeKind.Utc)).ToString("dd/MM/yy"),
                priority = await _localizationService.GetLocalizedEnumAsync(deliveryRequest.TicketEnum),
                requester = (await _customerService.GetCustomerByIdAsync(deliveryRequest.CreatedById))?.FullName,
                buyerName = (await _customerService.GetCustomerByIdAsync(customerId: buyerRequest.BuyerId))?.FullName,
                brand = buyerRequest.BrandId > 0 ? await _localizationService.GetLocalizedAsync(await _brandService.GetManufacturerByIdAsync(buyerRequest.BrandId), x => x.Name) : "-",
                rejectedReason = deliveryRequest.RejectedReason,
                productName = product.Name,
                productAttributesInfo = await _utilityService.ParseAttributeXml(buyerRequest.ProductAttributeXml),
                deliveryDate = deliveryRequest.CreatedOnUtc.ToString(),
            };

            return new { success = true, data = deliveryRequestDetail };
        }

        public async Task<object> AddPurchaseOrderShipmentRequest(OrderDeliveryRequestModel model)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                return new { success = false, message = "user not found" };


            var deliveryScedule = await _orderService.GetDeliveryScheduleByIdAsync(model.ExpectedShipmentId);
            if (deliveryScedule is null)
                return new { success = false, message = "Expected shipment not found" };

            var order = await _orderService.GetOrderByIdAsync(deliveryScedule.OrderId);
            if (order is null || order.Deleted)
                return new { success = false, message = "Order not found" };

            var orderItem = (await _orderService.GetOrderItemsAsync(order.Id)).FirstOrDefault();
            if (orderItem is null)
                return new { success = false, message = "Order item not found" };

            var buyerRequest = await _requestService.GetRequestByIdAsync(order.RequestId);
            if (buyerRequest is null)
                return new { success = false, message = "Buyer Request not found" };

            decimal totalLeftQuantity = orderItem.Quantity - _orderService.GetTotalOrderDeliveryRequestQuantityByOrderIdAsync(order.Id, deliveryScedule.Id);
            if (totalLeftQuantity <= 0)
                return new { success = false, message = "There is no left quantity " };

            if (model.Quantity > totalLeftQuantity)
                return new { success = false, message = "Left Quantity ", leftQuantity = totalLeftQuantity };

            var industry = await _industryService.GetIndustryByIdAsync(buyerRequest.IndustryId);
            if (industry is null)
                return new { success = false, message = "Industry not found" };

            var category = await _categoryService.GetCategoryByIdAsync(buyerRequest.CategoryId);
            if (category is null)
                return new { success = false, message = "Category not found" };

            List<object> list = new List<object>();

            var orderDeliveryRequest = new OrderDeliveryRequest
            {
                OrderId = order.Id,
                OrderDeliveryScheduleId = deliveryScedule.Id,
                StatusId = (int)OrderDeliveryRequestEnum.Pending,
                WarehouseId = model.WarehouseId,
                CountryId = model.CountryId,
                CityId = model.CityId,
                AreaId = model.AreaId,
                StreetAddress = model.StreetAddress,
                ContactNumber = model.ContactNumber,
                Quantity = model.Quantity,
                Deleted = false,
                AgentId = model.AgentId,
                TicketExpiryDate = DateTime.UtcNow.AddDays(category.TicketExpiryDays > 0 ? category.TicketExpiryDays : 1),
                TicketPirority = category.TicketPirority > 0 ? category.TicketPirority : (int)TicketEnum.Medium
            };
            await _orderService.InsertOrderDeliveryRequestAsync(orderDeliveryRequest);

            return new { success = true, message = await _localizationService.GetResourceAsync("Order.Delivery.Request.Created.Success") };

        }

        public async Task<object> PurchaseOrderDeliveryRequestDeatil(int expectedShipmentRequestId, string type = "ExpectedShipment")
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                return new { success = false, message = "User not found" };

            if (string.IsNullOrWhiteSpace(type))
                return new { success = false, message = "type is required" };

            var data = new object();
            if (type == "ExpectedShipment")
            {
                var deliveryRequest = await _orderService.GetOrderDeliveryRequestByIdAsync(expectedShipmentRequestId);
                if (deliveryRequest is null)
                    return new { success = false, message = "delivery request not found" };

                var order = await _orderService.GetOrderByIdAsync(deliveryRequest.OrderId);
                if (order is null)
                    return new { success = false, message = "delivery request order not found" };

                var buyerRequest = await _requestService.GetRequestByIdAsync(order.RequestId);
                if (buyerRequest is null)
                    return new { success = false, message = "Buyer Request not found" };

                var product = await _productService.GetProductByIdAsync(buyerRequest.ProductId);
                if (product is null || product.Id != buyerRequest.ProductId)
                    return new { success = false, message = "Buyer request product not found" };

                data = new
                {
                    id = deliveryRequest.Id,
                    orderId = deliveryRequest.OrderId,
                    orderCustomNumber = order.CustomOrderNumber,
                    shipmentId = deliveryRequest.StatusId == (int)OrderDeliveryRequestEnum.Complete ? (await _shipmentService.GetShipmentByDeliveryRequestId(deliveryRequest.Id))?.Id : 0,
                    wareHouseId = deliveryRequest.WarehouseId,
                    wareHouseName = deliveryRequest.WarehouseId > 0 ? (await _shippingService.GetWarehouseByIdAsync(deliveryRequest.WarehouseId)).Name : "",
                    statusId = deliveryRequest.StatusId,
                    status = await _localizationService.GetLocalizedEnumAsync(deliveryRequest.OrderDeliveryRequestEnum),
                    countryName = await _localizationService.GetLocalizedAsync(await _countryService.GetCountryByIdAsync(deliveryRequest.CountryId), x => x.Name),
                    countryId = deliveryRequest.CountryId,
                    cityName = await _localizationService.GetLocalizedAsync(await _stateProvinceService.GetStateProvinceByIdAsync(deliveryRequest.CityId), x => x.Name),
                    cityId = deliveryRequest.CityId,
                    areaName = await _localizationService.GetLocalizedAsync(await _stateProvinceService.GetStateProvinceByIdAsync(deliveryRequest.AreaId), x => x.Name),
                    areaId = deliveryRequest.AreaId,
                    streetaddress = deliveryRequest.StreetAddress,
                    contactnumber = deliveryRequest.ContactNumber,
                    totalQuantity = buyerRequest.Quantity,
                    deliveredQuantity = deliveryRequest.Quantity,
                    agentId = deliveryRequest.AgentId,
                    agent = (await _customerService.GetCustomerByIdAsync(deliveryRequest.AgentId))?.FullName,
                    timeRemaining = deliveryRequest.TicketExpiryDate.HasValue ? deliveryRequest.TicketExpiryDate.Value.Subtract(DateTime.Now).TotalHours > 0 || deliveryRequest.TicketExpiryDate.Value.Subtract(DateTime.Now).TotalMinutes > 0 ? deliveryRequest.TicketExpiryDate.Value.Subtract(DateTime.Now).ToString("hh") + " : hrs " + deliveryRequest.TicketExpiryDate.Value.Subtract(DateTime.Now).ToString("mm") + ": mns" : "0" : "0",
                    priority = await _localizationService.GetLocalizedEnumAsync(deliveryRequest.TicketEnum),
                    requester = (await _customerService.GetCustomerByIdAsync(deliveryRequest.CreatedById))?.FullName,
                    buyerName = (await _customerService.GetCustomerByIdAsync(customerId: buyerRequest.BuyerId))?.FullName,
                    brand = buyerRequest.BrandId > 0 ? await _localizationService.GetLocalizedAsync(await _brandService.GetManufacturerByIdAsync(buyerRequest.BrandId), x => x.Name) : "-",
                    rejectedReason = deliveryRequest.RejectedReason,
                    productName = product.Name,
                    productAttributesInfo = await _utilityService.ParseAttributeXml(buyerRequest.ProductAttributeXml),
                };
            }
            if (type == "Shipment")
            {
                var shipment = await _shipmentService.GetShipmentByIdAsync(expectedShipmentRequestId);
                if (shipment is null)
                    return new { success = false, message = "Shipment not found" };

                var deliveryRequest = await _orderService.GetOrderDeliveryRequestByIdAsync(shipment.DeliveryRequestId.Value);
                if (deliveryRequest is null)
                    return new { success = false, message = "delivery request not found" };

                var order = await _orderService.GetOrderByIdAsync(deliveryRequest.OrderId);
                if (order is null)
                    return new { success = false, message = "delivery request order not found" };

                var buyerRequest = await _requestService.GetRequestByIdAsync(order.RequestId);
                if (buyerRequest is null)
                    return new { success = false, message = "Buyer Request not found" };

                var product = await _productService.GetProductByIdAsync(buyerRequest.ProductId);
                if (product is null || product.Id != buyerRequest.ProductId)
                    return new { success = false, message = "Buyer request product not found" };

                var picture = await _pictureService.GetPictureByIdAsync(shipment.PictureId);
                data = new
                {
                    id = deliveryRequest.Id,
                    orderId = deliveryRequest.OrderId,
                    orderCustomNumber = order.CustomOrderNumber,
                    statusId = deliveryRequest.StatusId,
                    status = await _localizationService.GetLocalizedEnumAsync(deliveryRequest.OrderDeliveryRequestEnum),
                    //bagsDirectlyFromSupplier = deliveryRequest.BagsDirectlyFromSupplier,
                    //bagsDirectlyFromWarehouse = deliveryRequest.BagsDirectlyFromWarehouse,
                    countryName = await _localizationService.GetLocalizedAsync(await _countryService.GetCountryByIdAsync(deliveryRequest.CountryId), x => x.Name),
                    countryId = deliveryRequest.CountryId,
                    cityName = await _localizationService.GetLocalizedAsync(await _stateProvinceService.GetStateProvinceByIdAsync(deliveryRequest.CityId), x => x.Name),
                    cityId = deliveryRequest.CityId,
                    areaName = await _localizationService.GetLocalizedAsync(await _stateProvinceService.GetStateProvinceByIdAsync(deliveryRequest.AreaId), x => x.Name),
                    areaId = deliveryRequest.AreaId,
                    streetaddress = deliveryRequest.StreetAddress,
                    contactnumber = deliveryRequest.ContactNumber,
                    totalQuantity = buyerRequest.Quantity,
                    deliveredQuantity = deliveryRequest.Quantity,
                    shipmentdate = (await _dateTimeHelper.ConvertToUserTimeAsync(deliveryRequest.CreatedOnUtc, DateTimeKind.Utc)).ToString("MM/dd/yyyy h:mm:ss tt") /*deliveryRequest.ShipmentDateUtc*/,
                    agentId = deliveryRequest.AgentId,
                    agent = (await _customerService.GetCustomerByIdAsync(deliveryRequest.AgentId))?.FullName,
                    timeRemaining = deliveryRequest.TicketExpiryDate.HasValue ? deliveryRequest.TicketExpiryDate.Value.Subtract(DateTime.Now).TotalHours > 0 || deliveryRequest.TicketExpiryDate.Value.Subtract(DateTime.Now).TotalMinutes > 0 ? deliveryRequest.TicketExpiryDate.Value.Subtract(DateTime.Now).ToString("hh") + " : hrs " + deliveryRequest.TicketExpiryDate.Value.Subtract(DateTime.Now).ToString("mm") + ": mns" : "0" : "0",
                    priority = await _localizationService.GetLocalizedEnumAsync(deliveryRequest.TicketEnum),
                    requester = (await _customerService.GetCustomerByIdAsync(deliveryRequest.CreatedById))?.FullName,
                    buyerName = (await _customerService.GetCustomerByIdAsync(customerId: buyerRequest.BuyerId))?.FullName,
                    brand = buyerRequest.BrandId > 0 ? await _localizationService.GetLocalizedAsync(await _brandService.GetManufacturerByIdAsync(buyerRequest.BrandId), x => x.Name) : "-",
                    rejectedReason = deliveryRequest.RejectedReason,
                    productName = product.Name,
                    productAttributesInfo = await _utilityService.ParseAttributeXml(buyerRequest.ProductAttributeXml),
                    canShip = shipment.ShippedDateUtc.HasValue,
                    canDelivered = shipment.DeliveryDateUtc.HasValue,

                    //Shipped Detail
                    shippedDetail = new
                    {
                        expectedDateShipped = shipment.ExpectedDateShipped.HasValue ? (await _dateTimeHelper.ConvertToUserTimeAsync(shipment.ExpectedDateShipped.Value, DateTimeKind.Utc)).ToString() : "",
                        expectedDateDelivered = shipment.ExpectedDateDelivered.HasValue ? (await _dateTimeHelper.ConvertToUserTimeAsync(shipment.ExpectedDateDelivered.Value, DateTimeKind.Utc)).ToString() : "",
                        expectedDeliveryCost = shipment.ExpectedDeliveryCost,
                        expectedQuantity = shipment.ExpectedQuantity,
                        actualShippedQuantity = shipment.ActualShippedQuantity,
                        transporterId = shipment.TransporterId,
                        wareHouseId = shipment.WarehouseId,
                        wareHouseName = shipment.WarehouseId > 0 ? (await _shippingService.GetWarehouseByIdAsync(shipment.WarehouseId)).Name : "",
                        transporterName = shipment.TransporterId > 0 ? (await _customerService.GetCustomerByIdAsync(shipment.TransporterId))?.FullName : "",
                        vehicleId = shipment.VehicleId,
                        transportvehicleName = await _customerService.GetTransporterVehicleMappingByIdAsync(shipment.VehicleId) != null ? $"{(await _customerService.GetVehiclePortfolioByIdAsync((await _customerService.GetTransporterVehicleMappingByIdAsync(shipment.VehicleId)).VehicleId)).Name} - {(await _customerService.GetTransporterVehicleMappingByIdAsync(shipment.VehicleId))?.VehicleNumber}" : null,
                        vehicleNumber = shipment.VehicleNumber,
                        routeType = shipment.RouteTypeId > 0 ? await _localizationService.GetLocalizedEnumAsync(shipment.RouteType) : "",
                        deliveryStatus = await _localizationService.GetLocalizedEnumAsync(shipment.DeliveryStatus),
                        pickupAddress = shipment.PickupAddress,
                        shippedDateUtc = shipment.ShippedDateUtc.HasValue ? (await _dateTimeHelper.ConvertToUserTimeAsync(shipment.ShippedDateUtc.Value, DateTimeKind.Utc)).ToString() : "",
                        onLabourCharges = shipment.OnLabourCharges,
                        costOnZaraye = shipment.CostOnZarayeId > 0 ? await _localizationService.GetLocalizedEnumAsync(shipment.CostOnZaraye) : ""
                    },

                    //Delivered Detail
                    deliveredDetail = new
                    {
                        deliveryDateUtc = shipment.DeliveryDateUtc.HasValue ? (await _dateTimeHelper.ConvertToUserTimeAsync(shipment.DeliveryDateUtc.Value, DateTimeKind.Utc)).ToString() : "",
                        shipmentDeliveryAddress = shipment.ShipmentDeliveryAddress,
                        picture = shipment.PictureId > 0 ? await _pictureService.GetPictureUrlAsync(shipment.PictureId) : "",
                        fullPictureUrl = picture != null ? (await _pictureService.GetPictureUrlAsync(picture)).Url : "",
                        transporterType = shipment.TransporterTypeId > 0 ? await _localizationService.GetLocalizedEnumAsync(shipment.TransporterType) : "",
                        deliveryType = shipment.DeliveryTypeId > 0 ? await _localizationService.GetLocalizedEnumAsync(shipment.DeliveryType) : "",
                        deliveryTiming = shipment.DeliveryTimingId > 0 ? await _localizationService.GetLocalizedEnumAsync(shipment.DeliveryTimingEnum) : "",
                        deliveryDelayedReason = shipment.DeliveryDelayedReasonId > 0 ? (await _shipmentService.GetDeliveryTimeReasonByIdAsync(shipment.DeliveryDelayedReasonId))?.Name : "",
                        deliveryCostType = shipment.DeliveryCostTypeId > 0 ? await _localizationService.GetLocalizedEnumAsync(shipment.DeliveryCostType) : "",
                        deliveryCost = shipment.DeliveryCost,
                        warehouseId = shipment.WarehouseId,
                        freightCharges = shipment.FreightCharges,
                        labourCharges = shipment.LabourCharges,
                        actualDeliveredQuantity = shipment.ActualDeliveredQuantity,
                        deliveryCostReason = shipment.DeliveryCostReasonId > 0 ? (await _shipmentService.GetDeliveryCostReasonByIdAsync(shipment.DeliveryCostReasonId))?.Name : "",
                        costOnZaraye = shipment.LabourTypeId > 0 ? await _localizationService.GetLocalizedEnumAsync(shipment.LabourType) : ""
                    },
                };
            }

            return new { success = true, data };
        }

        #endregion

        #region Direct Purchase Order

        public async Task<object> CheckDirectPurchaseOrderExist(int requestForQuotationId)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                return new { success = false, message = "User not found" };

            var requestForQuotation = await _requestService.GetRequestForQuotationByIdAsync(requestForQuotationId);
            if (requestForQuotation is null)
                return new { success = false, message = "Request for quotation not found" };

            var request = await _requestService.GetRequestByIdAsync(requestForQuotation.RequestId);
            if (request is null)
                return new { success = false, message = "Request  not found" };

            var directOrders = await _orderService.SearchDirectOrdersAsync(rfqId: requestForQuotation.Id);
            if (!directOrders.Any())
                return new { success = true, message = "Data not found", data = false };

            return new { success = true, message = "Data found", data = true };
        }

        public async Task<object> DirecPurchaseOrderProcess(int requestForQuotationId, List<RFQQuotationsModel> model)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                return new { success = false, message = "user not found" };

            if (requestForQuotationId <= 0)
                return new { success = false, message = "Request for quotation id is required" };

            var requestForQuotation = await _requestService.GetRequestForQuotationByIdAsync(requestForQuotationId);
            if (requestForQuotation is null)
                return new { success = false, message = "Request for quotation not found" };

            var request = await _requestService.GetRequestByIdAsync(requestForQuotation.RequestId);
            if (request is null)
                return new { success = false, message = "Request not found" };

            var product = await _productService.GetProductByIdAsync(request.ProductId);
            if (product == null || product.Deleted || !product.AppPublished)
                return new { success = false, message = "Buyer request product not found" };

            var directOrderExists = (await _orderService.SearchDirectOrdersAsync(rfqId: requestForQuotation.Id)).ToList();
            foreach (var directOrderExist in directOrderExists)
            {
                var directOrderDeliverySchedules = await _orderService.GetAllDirectOrderDeliveryScheduleAsync(directOrderExist.Id);
                foreach (var directOrderDeliverySchedule in directOrderDeliverySchedules)
                    await _orderService.DeleteDirectOrderDeliveryScheduleAsync(directOrderDeliverySchedule);

                //Remove direct order supplier info
                var directOrderCalculationsExists = await _orderService.GetAllDirectOrderCalculationAsync(directOrderExist.Id);
                foreach (var directOrderCalculationsExist in directOrderCalculationsExists)
                    await _orderService.DeleteDirectOrderCalculationAsync(directOrderCalculationsExist);

                await _orderService.DeleteDirectOrderAsync(directOrderExist);
            }

            var quotationIds = model.Select(x => x.Id).ToArray();
            var allQuotations = (await _quotationService.GetQuotationByIdsAsync(quotationIds)).ToList();


            foreach (var quotation in allQuotations)
            {
                //New Direct Order
                var newDirectOrder = new DirectOrder
                {
                    BookerId = user.Id,
                    BuyerId = request.BuyerId,
                    RequestId = request.Id,
                    TransactionModelId = request.BusinessModelId,
                    QuotationId = quotation.Id,
                    BrandId = quotation.BrandId,
                    RequestForQuotationId = requestForQuotation.Id,
                    SupplierId = quotation.SupplierId,
                    Quantity = quotation.Quantity,
                    OrderTypeId = (int)OrderType.PurchaseOrder
                };
                await _orderService.InsertDirectOrderAsync(newDirectOrder);
                //Add default direct order delivery schedule 
                await _orderService.InsertDirectOrderDeliveryScheduleAsync(new DirectOrderDeliverySchedule
                {
                    CreatedOnUtc = DateTime.UtcNow,
                    ExpectedDeliveryDateUtc = null,
                    ExpectedShipmentDateUtc = null,
                    ExpectedQuantity = null,
                    ExpectedDeliveryCost = 0,
                    DirectOrderId = newDirectOrder.Id
                });
                //Add default direct order calculation 
                await _orderService.InsertDirectOrderCalculationAsync(new DirectOrderCalculation
                {
                    CreatedOnUtc = DateTime.UtcNow,
                    DirectOrderId = newDirectOrder.Id,
                    Quantity = quotation.Quantity,
                    Price = quotation.QuotationPrice,
                    BusinessModelId = quotation.BusinessModelId
                });
            }

            return new { success = true, message = await _localizationService.GetResourceAsync("New.Direct.Order.Created") };

        }

        public async Task<object> GeneratePurchaseDirectOrder(int requestForQuotationId, List<RFQQuotationsModel> model)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                return new { success = false, message = "user not found" };

            if (requestForQuotationId <= 0)
                return new { success = false, message = "Request for quotation id is required" };

            var requestForQuotation = await _requestService.GetRequestForQuotationByIdAsync(requestForQuotationId);
            if (requestForQuotation is null)
                return new { success = false, message = "Request for quotation not found" };

            var request = await _requestService.GetRequestByIdAsync(requestForQuotation.RequestId);
            if (request is null)
                return new { success = false, message = "Request not found" };

            if (model.Sum(x => x.Quantity) != requestForQuotation.Quantity)
                return new { success = false, message = "Quantity should be same to request for quotation quantity" };

            var product = await _productService.GetProductByIdAsync(request.ProductId);
            if (product == null || product.Deleted || !product.AppPublished)
                return new { success = false, message = "Buyer request product not found" };

            var currentUserTime = await _dateTimeHelper.ConvertToUserTimeAsync(DateTime.UtcNow, DateTimeKind.Utc);


            foreach (var quotation in model)
            {
                //New Direct Order
                var newDirectOrder = new DirectOrder
                {
                    BookerId = user.Id,
                    BuyerId = request.BuyerId,
                    RequestId = request.Id,
                    TransactionModelId = request.BusinessModelId,
                    QuotationId = quotation.Id,
                    RequestForQuotationId = requestForQuotation.Id,
                    SupplierId = quotation.SupplierId,
                    Quantity = quotation.Quantity,
                    OrderTypeId = (int)OrderType.PurchaseOrder
                };
                await _orderService.InsertDirectOrderAsync(newDirectOrder);
                //Add default direct order delivery schedule 
                await _orderService.InsertDirectOrderDeliveryScheduleAsync(new DirectOrderDeliverySchedule
                {
                    CreatedOnUtc = DateTime.UtcNow,
                    ExpectedDeliveryDateUtc = null,
                    ExpectedShipmentDateUtc = null,
                    ExpectedQuantity = null,
                    ExpectedDeliveryCost = 0,
                    DirectOrderId = newDirectOrder.Id
                });
                //Add default direct order calculation 
                await _orderService.InsertDirectOrderCalculationAsync(new DirectOrderCalculation
                {
                    CreatedOnUtc = DateTime.UtcNow,
                    DirectOrderId = newDirectOrder.Id,
                    Quantity = request.Quantity,
                    Price = quotation.Price
                });
            }

            return new { success = true, message = await _localizationService.GetResourceAsync("Seller.Bid.Created.Success") };
        }

        public async Task<object> GetAllDirectOrderByRequestForQuotationId(int requestForQuotationId)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                return new { success = false, message = "user not found" };

            var requestForQuotation = await _requestService.GetRequestForQuotationByIdAsync(requestForQuotationId);
            if (requestForQuotation is null)
                return new { success = false, message = "Request for quotation not found" };

            var request = await _requestService.GetRequestByIdAsync(requestForQuotation.RequestId);
            if (request is null)
                return new { success = false, message = "Request not found" };
            var data = new
            {
                directOrder = await (await _orderService.SearchDirectOrdersAsync(rfqId: requestForQuotation.Id)).SelectAwait(async dir =>
                {
                    return new
                    {
                        id = dir.Id,
                        supplierId = dir.SupplierId,
                        supplier = (await _customerService.GetCustomerByIdAsync(dir.SupplierId))?.FullName,
                        quantity = dir.Quantity,
                        countryId = dir.CountryId,
                        cityId = dir.CityId,
                        areaId = dir.AreaId,
                        streetAddress = dir.StreetAddress,
                        interGeography = dir.InterGeography.HasValue ? dir.InterGeography.Value : false,
                        pinlocation = new
                        {
                            latitude = dir.PinLocation_Latitude,
                            longitude = dir.PinLocation_Longitude,
                            location = dir.PinLocation_Location,
                        },
                        expectedShipments = (await _orderService.GetAllDirectOrderDeliveryScheduleAsync(dir.Id)).Select(async d =>
                        {
                            return new
                            {
                                id = d.Id,
                                expectedDeliveryDateUtc = d.ExpectedDeliveryDateUtc.HasValue ? await _dateTimeHelper.ConvertToUserTimeAsync(d.ExpectedDeliveryDateUtc.Value, DateTimeKind.Utc) : (DateTime?)null,
                                expectedShipmentDateUtc = d.ExpectedShipmentDateUtc.HasValue ? await _dateTimeHelper.ConvertToUserTimeAsync(d.ExpectedShipmentDateUtc.Value, DateTimeKind.Utc) : (DateTime?)null,
                                expectedQuantity = d.ExpectedQuantity.HasValue ? d.ExpectedQuantity.ToString() : "",
                                expectedDeliveryCost = d.ExpectedDeliveryCost.HasValue ? d.ExpectedDeliveryCost.ToString() : "",
                                createdOnUtc = d.CreatedOnUtc,
                                directOrderId = d.DirectOrderId
                            };
                        }).Select(t => t.Result).ToList(),
                    };

                }).ToListAsync()
            };

            return new { success = true, data };
        }

        public async Task<object> DirectPurchaseOrderInfo(int directOrderId, DirectOrderApiModel.DirectOrderInfoModel model)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                return new { success = false, message = "user not found" };

            var directOrder = await _orderService.GetDirectOrderByIdAsync(directOrderId);
            if (directOrder is null)
                return new { success = false, message = "Direct order not found" };

            if (model.CountryId.HasValue)
                directOrder.CountryId = model.CountryId.Value;

            if (model.CityId.HasValue)
                directOrder.CityId = model.CityId.Value;

            if (model.AreaId.HasValue)
                directOrder.AreaId = model.AreaId.Value;

            if (model.StreetAddress is not null)
                directOrder.StreetAddress = model.StreetAddress;

            if (model.PinLocation is not null)
            {
                directOrder.PinLocation_Latitude = model.PinLocation.Latitude;
                directOrder.PinLocation_Longitude = model.PinLocation.Longitude;
                directOrder.PinLocation_Location = model.PinLocation.Location;
            }

            if (model.InterGeography.HasValue)
                directOrder.InterGeography = model.InterGeography.Value;

            await _orderService.UpdateDirectOrderAsync(directOrder);

            return new { success = true, message = await _localizationService.GetResourceAsync("DirectOrder.infos.Update") };
        }

        public async Task<object> PurchaseOrderBusinessModelFormJson(int requestForQuotationId)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                return new { success = false, message = "user not found" };

            var requestForQuotation = await _requestService.GetRequestForQuotationByIdAsync(requestForQuotationId);
            if (requestForQuotation is null)
                return new { success = false, message = "Request for quotation not found" };

            var request = await _requestService.GetRequestByIdAsync(requestForQuotation.RequestId);
            if (request is null)
                return new { success = false, message = "Request not found" };

            var directOrders = await _orderService.SearchDirectOrdersAsync(rfqId: requestForQuotation.Id);

            var businessModels = new List<BusinessModelApiModel>();
            foreach (var directOrder in directOrders)
            {
                var model = new BusinessModelApiModel();

                var directOrderSupplierInfo = await _orderService.GetAllDirectOrderCalculationAsync(directOrder.Id);
                if (directOrderSupplierInfo is not null)
                    businessModels.Add(await _utilityService.PurchaseOrder_BusinessModelFormCalculatedJson(directOrder));
            }

            return new { success = true, message = "", data = new { bunsinessModelFields = businessModels, businessModelName = await _localizationService.GetLocalizedEnumAsync(request.BusinessModelEnum) } };
        }

        public async Task<object> PurchaseOrderBusinessModelFormCalculation(BusinessModelApiModel model)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                return new { success = false, message = "user not found" };

            var directOrderCalculation = await _orderService.GetDirectOrderCalculationByIdAsync(model.OrderCalculationId);
            if (directOrderCalculation is null)
                return new { success = false, message = "Order calculation not found" };

            var directOrder = await _orderService.GetDirectOrderByQuotationId(model.QuotationId);
            if (directOrder is null)
                return new { success = false, message = "Direct order not found" };

            #region Standard

            if (directOrderCalculation.BusinessModelId == (int)BusinessModelEnum.Standard)
            {
                var totalPayble = directOrderCalculation.Price * directOrderCalculation.Quantity;
                var totalReceivable = directOrderCalculation.Price * directOrderCalculation.Quantity;

                var includeGstInTotalAmount = model.Payables.Where(x => x.Name == "GSTIncludedInTotalAmount")?.FirstOrDefault()?.Value;

                #region Payable

                foreach (var item in model.Payables)
                {
                    if (item.Name is not null)
                    {
                        if (item.Name == "GrossRate")
                            directOrderCalculation.GrossCommissionRate = totalPayble;

                        else if (item.Name == "CommissionType")
                            directOrderCalculation.GrossCommissionRateType = (string)item.Value;

                        else if (item.Name == "Commission")
                        {
                            //var comissionType = model.Payables.Where(x => x.Name == "CommissionType")?.FirstOrDefault()?.Value;

                            directOrderCalculation.GrossCommissionRate = _utilityService.ConvertToDecimal(item.Value);
                            //directOrderCalculation.Payable_ComissionType_0 = (string)comissionType/*.ToLower() == "true" ? "Percent" : "Value"*/;

                            if (_utilityService.ConvertToDecimal(item.Value) > 0)
                            {
                                if (directOrderCalculation.GrossCommissionRateType == "%")
                                    directOrderCalculation.GrossCommissionAmount = (decimal)((float)totalPayble * (float)directOrderCalculation.GrossCommissionRate / 100f);
                                else
                                    directOrderCalculation.GrossCommissionAmount = directOrderCalculation.GrossCommissionRate;

                                totalPayble -= directOrderCalculation.GrossCommissionRate;
                                directOrderCalculation.NetAmount = totalPayble;

                                if (_shoppingCartSettings.RoundPricesDuringCalculation)
                                {
                                    totalPayble = await _priceCalculationService.RoundPriceAsync(totalPayble);
                                    directOrderCalculation.NetAmount = totalPayble;
                                }
                            }
                            else
                            {
                                directOrderCalculation.GrossCommissionRate = 0;
                                //directOrderCalculation.Payable_ComissionType_0 = "";
                                directOrderCalculation.GrossCommissionAmount = 0;
                                directOrderCalculation.NetAmount = totalPayble;
                            }
                        }

                        //Inclusive of GST
                        else if (item.Name == "GSTIncludedInTotalAmount")
                        {
                            if (((string)item.Value).ToLower() == "yes")
                            {
                                directOrderCalculation.GSTIncludedInTotalAmount = true;
                                directOrderCalculation.NetAmountWithoutGST = (decimal)((float)totalPayble / (float)_calculationSettings.GSTPercentageForStandardQuotation/* 1.17f*/);
                                totalPayble = directOrderCalculation.NetAmountWithoutGST;
                                totalReceivable = (decimal)((float)(directOrderCalculation.Price * directOrderCalculation.Quantity) / (float)_calculationSettings.GSTPercentageForStandardQuotation /*1.17f*/);
                            }
                            else
                            {
                                directOrderCalculation.GSTIncludedInTotalAmount = false;
                                directOrderCalculation.NetAmountWithoutGST = totalPayble;
                                totalPayble = directOrderCalculation.NetAmount;
                                totalReceivable = directOrderCalculation.Price * directOrderCalculation.Quantity;

                            }
                        }
                        else if (item.Name == "SupplierCreditTerms")
                            directOrderCalculation.SupplierCreditTerms = _utilityService.ConvertToDecimal(item.Value);
                        //GST
                        else if (item.Name == "GSTRate")
                        {
                            if (_utilityService.ConvertToDecimal(item.Value) > 0)
                            {
                                directOrderCalculation.GSTRate = _utilityService.ConvertToDecimal(item.Value);
                                directOrderCalculation.GSTAmount = (decimal)((float)directOrderCalculation.NetAmountWithoutGST * (float)_utilityService.ConvertToDecimal(item.Value) / 100f);

                                if (_shoppingCartSettings.RoundPricesDuringCalculation)
                                    directOrderCalculation.GSTAmount = await _priceCalculationService.RoundPriceAsync(directOrderCalculation.GSTAmount);
                            }
                            else
                            {
                                directOrderCalculation.GSTRate = 0;
                                directOrderCalculation.GSTAmount = 0;
                            }
                        }

                        //WHT
                        else if (item.Name == "WHTRate")
                        {
                            if (_utilityService.ConvertToDecimal(item.Value) > 0)
                            {
                                directOrderCalculation.WHTRate = _utilityService.ConvertToDecimal(item.Value);
                                directOrderCalculation.WHTAmount = (decimal)((float)(totalPayble + directOrderCalculation.GSTAmount) * ((float)directOrderCalculation.WHTRate / 100f));

                                if (_shoppingCartSettings.RoundPricesDuringCalculation)
                                    directOrderCalculation.WHTAmount = await _priceCalculationService.RoundPriceAsync(directOrderCalculation.WHTAmount);
                            }
                            else
                            {
                                directOrderCalculation.WHTRate = 0;
                                directOrderCalculation.WHTAmount = 0;
                            }
                            totalPayble = totalPayble + directOrderCalculation.GSTAmount - directOrderCalculation.WHTAmount;
                            directOrderCalculation.SubTotal = totalPayble;
                            directOrderCalculation.OrderTotal = directOrderCalculation.SubTotal;
                        }

                    }
                }

                #endregion

            }

            #endregion

            #region All other business module

            else if (directOrderCalculation.BusinessModelId == (int)BusinessModelEnum.OneOnOne || directOrderCalculation.BusinessModelId == (int)BusinessModelEnum.Agency || directOrderCalculation.BusinessModelId == (int)BusinessModelEnum.FineCounts || directOrderCalculation.BusinessModelId == (int)BusinessModelEnum.ForwardBuying || directOrderCalculation.BusinessModelId == (int)BusinessModelEnum.ForwardSelling)
            {
                #region Payable

                var gstAmount_Payable = 0m;
                var whtAmount_Payable = 0m;
                var wholesaleTax = 0m;
                var payableToMill_Payable = 0m;
                var payableToMillAfterMultiply = 0m;
                var paymentInCash_Payable = 0m;
                var paymentInCash_PayableAfterMultiply = 0m;

                foreach (var item in model.Payables)
                {
                    if (item.Name is not null)
                    {
                        var InvoicedAmount_Payable = _utilityService.ConvertToDecimal(model.Payables.Where(x => x.Name == "InvoicedAmount")?.FirstOrDefault().Value);

                        //GST
                        if (item.Name == "GSTAmount")
                        {
                            var gstRate = _utilityService.ConvertToDecimal(model.Payables.Where(x => x.Name == "GSTRate")?.FirstOrDefault().Value);

                            if (gstRate > 0)
                            {
                                var gstRateCalculationValue = (decimal)((float)InvoicedAmount_Payable * (float)gstRate / 100);
                                gstAmount_Payable = gstRateCalculationValue;

                                directOrderCalculation.GSTIncludedInTotalAmount = true;
                                directOrderCalculation.GSTRate = _utilityService.ConvertToDecimal(gstRate);
                                directOrderCalculation.GSTAmount = gstAmount_Payable;
                            }
                            else
                            {
                                gstAmount_Payable = 0m;
                                directOrderCalculation.GSTIncludedInTotalAmount = false;
                                directOrderCalculation.GSTRate = 0;
                                directOrderCalculation.GSTAmount = gstAmount_Payable;
                            }
                        }

                        //WHT
                        else if (item.Name == "WHTAmount")
                        {
                            var whtRate = _utilityService.ConvertToDecimal(model.Payables.Where(x => x.Name == "WHTRate")?.FirstOrDefault().Value);
                            if (whtRate > 0m)
                            {
                                var whtRateClaculationValue = (decimal)(((float)InvoicedAmount_Payable + (float)gstAmount_Payable) * (float)whtRate / 100);
                                whtAmount_Payable = whtRateClaculationValue;

                                directOrderCalculation.WhtIncluded = true;
                                directOrderCalculation.WHTRate = _utilityService.ConvertToDecimal(whtRate);//((decimal)((float)model.InvoicedAmount_Payable * 1.17f));
                                directOrderCalculation.WHTAmount = whtAmount_Payable;
                            }
                            else
                            {
                                whtAmount_Payable = 0m;
                                directOrderCalculation.WhtIncluded = false;
                                directOrderCalculation.WHTRate = 0;//((decimal)((float)model.InvoicedAmount_Payable * 1.17f));
                                directOrderCalculation.WHTAmount = whtAmount_Payable;
                            }
                        }

                        //Wholesale Tax
                        else if (item.Name == "WholesaleTaxAmount")
                        {
                            var wholesaletaxRate = _utilityService.ConvertToDecimal(model.Payables.Where(x => x.Name == "WholesaleTaxRate")?.FirstOrDefault().Value);
                            if (wholesaletaxRate > 0)
                            {
                                //var wholesaletaxCalculationValue = ((decimal)((float)(InvoicedAmount_Payable + gstAmount_Payable - Math.Abs(whtAmount_Payable)) * 0.001f));
                                var wholesaletaxCalculationValue = (decimal)((float)(InvoicedAmount_Payable + gstAmount_Payable - Math.Abs(whtAmount_Payable)) * (float)wholesaletaxRate / 100);
                                wholesaleTax = wholesaletaxCalculationValue;

                                directOrderCalculation.WholesaletaxIncluded = true;
                                directOrderCalculation.WholesaleTaxRate = _utilityService.ConvertToDecimal(wholesaletaxRate);
                                directOrderCalculation.WholesaleTaxAmount = wholesaleTax;
                            }
                            else
                            {
                                wholesaleTax = 0m;
                                directOrderCalculation.WholesaletaxIncluded = false;
                                directOrderCalculation.WholesaleTaxRate = 0;
                                directOrderCalculation.WholesaleTaxAmount = wholesaleTax;
                            }
                        }

                        else if (item.Name == "Price")
                        {
                            directOrderCalculation.Price = _utilityService.ConvertToDecimal(item.Value);
                            //directOrderCalculation.Receivable_ProductPrice_10 = _utilityService.ConvertToDecimal(item.Value);
                        }

                        else if (item.Name == "InvoicedAmount")
                            directOrderCalculation.InvoicedAmount = InvoicedAmount_Payable;

                        else if (item.Name == "BrokerCash")
                            directOrderCalculation.BrokerCash = directOrderCalculation.Price - directOrderCalculation.InvoicedAmount;

                        else if (item.Name == "BrokerId")
                            directOrderCalculation.BrokerId = Convert.ToInt32(item.Value);

                        else if (item.Name == "PayableToMill")
                        {
                            payableToMill_Payable = InvoicedAmount_Payable + gstAmount_Payable - whtAmount_Payable + wholesaleTax;
                            directOrderCalculation.PayableToMill = payableToMill_Payable;

                            payableToMillAfterMultiply = directOrderCalculation.PayableToMill * directOrderCalculation.Quantity;
                            directOrderCalculation.PayableToMill = payableToMillAfterMultiply;
                        }
                        else if (item.Name == "PaymentInCash")
                        {
                            paymentInCash_Payable = directOrderCalculation.BrokerCash;
                            directOrderCalculation.PaymentInCash = paymentInCash_Payable;

                            paymentInCash_PayableAfterMultiply = directOrderCalculation.PaymentInCash * directOrderCalculation.Quantity;
                            directOrderCalculation.PaymentInCash = paymentInCash_PayableAfterMultiply;
                        }

                        else if (item.Name == "OrderTotal")
                        {
                            //var totalPayable = directOrderCalculation.PayableToMill + directOrderCalculation.PaymentInCash; //(model.ProductPrice_Payable + gstAmount_Payable) - (whtAmount_Payable) + wholesaleTax;
                            directOrderCalculation.TotalPerBag = payableToMill_Payable + paymentInCash_Payable;
                            directOrderCalculation.OrderTotal = payableToMillAfterMultiply + paymentInCash_PayableAfterMultiply;
                            directOrderCalculation.GrossCommissionAmount = directOrderCalculation.OrderTotal;
                            directOrderCalculation.SubTotal = directOrderCalculation.OrderTotal;
                        }
                        else if (item.Name == "SupplierCreditTerms")
                            directOrderCalculation.SupplierCreditTerms = _utilityService.ConvertToDecimal(item.Value);
                        else if (item.Name == "SupplierCommissionBag")
                        {
                            directOrderCalculation.SupplierCommissionBag = _utilityService.ConvertToDecimal(item.Value);
                            directOrderCalculation.SupplierCommissionBag_Summary = _utilityService.ConvertToDecimal(item.Value) * directOrderCalculation.Quantity;
                        }

                        else if (item.Name == "SupplierCommissionPayableUserId")
                            directOrderCalculation.SupplierCommissionPayableUserId = Convert.ToInt32(item.Value);

                        else if (item.Name == "SupplierCommissionReceivableRate")
                        {
                            directOrderCalculation.SupplierCommissionReceivableRate = _utilityService.ConvertToDecimal(item.Value);
                            //var supplierCommissionReceivableRate_Payable = (float)directOrderCalculation.SupplierCommissionReceivableRate / 100f;
                            //directOrderCalculation.SupplierCommissionReceivable_Summary = (decimal)((float)directOrderCalculation.SupplierCommissionReceivableRate * (float)directOrderCalculation.InvoicedAmount) * directOrderCalculation.Quantity;
                            directOrderCalculation.SupplierCommissionReceivable_Summary = directOrderCalculation.SupplierCommissionReceivableRate * directOrderCalculation.Quantity;
                        }

                        else if (item.Name == "SupplierCommissionReceivableUserId")
                            directOrderCalculation.SupplierCommissionReceivableUserId = Convert.ToInt32(item.Value);

                    }
                }

                #endregion
            }

            #endregion

            #region Lending

            else if (directOrderCalculation.BusinessModelId == (int)BusinessModelEnum.Lending)
            {
                #region Payable

                var gstAmount_Payable = 0m;
                var whtAmount_Payable = 0m;
                var wholesaleTax = 0m;
                var payableToMillAfterMultiply = 0m;
                var paymentInCash_Payable = 0m;
                var paymentInCash_PayableAfterMultiply = 0m;

                foreach (var item in model.Payables)
                {
                    var InvoicedAmount_Payable = _utilityService.ConvertToDecimal(model.Payables.Where(x => x.Name == "InvoicedAmount")?.FirstOrDefault().Value);

                    if (item.Name is not null)
                    {
                        var totalFinanceCost_20 = _utilityService.ConvertToDecimal(model.Payables.Where(x => x.Name == "TotalFinanceCost")?.FirstOrDefault().Value);
                        var financeCostPaymentId = Convert.ToInt32(model.Payables.Where(x => x.Name == "FinanceCostPayment").FirstOrDefault().Value);

                        //GST rate
                        if (item.Name == "GSTAmount")
                        {
                            var gstRate = _utilityService.ConvertToDecimal(model.Payables.Where(x => x.Name == "GSTRate")?.FirstOrDefault().Value);

                            if (gstRate > 0)
                            {
                                var gstRateCalculationValue = (decimal)((float)InvoicedAmount_Payable * (float)gstRate / 100);

                                if (financeCostPaymentId == (int)FinanceCostPaymentStatus.DirectlyToMill)
                                    gstRateCalculationValue = (decimal)((float)(InvoicedAmount_Payable + _utilityService.ConvertToDecimal(totalFinanceCost_20)) * (float)gstRate / 100);

                                gstAmount_Payable = gstRateCalculationValue;

                                directOrderCalculation.GSTIncludedInTotalAmount = true;
                                directOrderCalculation.GSTRate = _utilityService.ConvertToDecimal(gstRate);
                                directOrderCalculation.GSTAmount = gstAmount_Payable;
                            }
                            else
                            {
                                gstAmount_Payable = 0m;
                                directOrderCalculation.GSTIncludedInTotalAmount = false;
                                directOrderCalculation.GSTRate = 0;
                                directOrderCalculation.GSTAmount = gstAmount_Payable;
                            }
                        }

                        //WHT rate
                        else if (item.Name == "WHTAmount")
                        {
                            var whtRate = _utilityService.ConvertToDecimal(model.Payables.Where(x => x.Name == "WHTRate")?.FirstOrDefault().Value);
                            if (whtRate > 0)
                            {
                                var whtRateClaculationValue = (decimal)(((float)InvoicedAmount_Payable + (float)gstAmount_Payable) * (float)whtRate / 100);

                                if (financeCostPaymentId == (int)FinanceCostPaymentStatus.DirectlyToMill)
                                    whtRateClaculationValue = (decimal)(((float)(InvoicedAmount_Payable + totalFinanceCost_20) + (float)gstAmount_Payable) * (float)whtRate / 100);

                                whtAmount_Payable = whtRateClaculationValue;

                                directOrderCalculation.WhtIncluded = true;
                                directOrderCalculation.WHTRate = _utilityService.ConvertToDecimal(whtRate);//((decimal)((float)model.InvoicedAmount_Payable * 1.17f));
                                directOrderCalculation.WHTAmount = whtAmount_Payable;
                            }
                            else
                            {
                                whtAmount_Payable = 0m;
                                directOrderCalculation.WhtIncluded = false;
                                directOrderCalculation.WHTRate = 0;//((decimal)((float)model.InvoicedAmount_Payable * 1.17f));
                                directOrderCalculation.WHTAmount = whtAmount_Payable;
                            }
                        }

                        //Wholesale Tax rate
                        else if (item.Name == "WholesaleTaxAmount")
                        {
                            var wholesaletaxRate = _utilityService.ConvertToDecimal(model.Payables.Where(x => x.Name == "WholesaleTaxRate")?.FirstOrDefault().Value);
                            if (wholesaletaxRate > 0)
                            {
                                //var wholesaletaxCalculationValue = ((decimal)((float)(InvoicedAmount_Payable + gstAmount_Payable - Math.Abs(whtAmount_Payable)) * 0.001f));
                                var wholesaletaxCalculationValue = (decimal)((float)(InvoicedAmount_Payable + gstAmount_Payable - Math.Abs(whtAmount_Payable)) * (float)wholesaletaxRate / 100);

                                if (financeCostPaymentId == (int)FinanceCostPaymentStatus.DirectlyToMill)
                                    wholesaletaxCalculationValue = (decimal)((float)(InvoicedAmount_Payable + totalFinanceCost_20 + gstAmount_Payable - Math.Abs(whtAmount_Payable)) * (float)wholesaletaxRate / 100);

                                wholesaleTax = wholesaletaxCalculationValue;

                                directOrderCalculation.WholesaletaxIncluded = true;
                                directOrderCalculation.WholesaleTaxRate = _utilityService.ConvertToDecimal(wholesaletaxRate);
                                directOrderCalculation.WholesaleTaxAmount = wholesaleTax;
                            }
                            else
                            {
                                wholesaleTax = 0m;
                                directOrderCalculation.WholesaletaxIncluded = false;
                                directOrderCalculation.WholesaleTaxRate = 0;
                                directOrderCalculation.WholesaleTaxAmount = wholesaleTax;
                            }
                        }

                        else if (item.Name == "Price")
                        {
                            directOrderCalculation.Price = _utilityService.ConvertToDecimal(item.Value);
                            //directOrderCalculation.Receivable_ProductPrice_20 = _utilityService.ConvertToDecimal(item.Value);
                        }

                        else if (item.Name == "InvoicedAmount")
                            directOrderCalculation.InvoicedAmount = _utilityService.ConvertToDecimal(item.Value);

                        else if (item.Name == "BrokerCash")
                            directOrderCalculation.BrokerCash = directOrderCalculation.Price - directOrderCalculation.InvoicedAmount;

                        //else if (item.Name == "BuyerPaymentTerms")
                        //    directOrderCalculation.BuyerPaymentTerms = _utilityService.ConvertToDecimal(item.Value);

                        else if (item.Name == "TotalFinanceCost")
                            directOrderCalculation.TotalFinanceCost = _utilityService.ConvertToDecimal(item.Value);
                        else if (item.Name == "SupplierCreditTerms")
                            directOrderCalculation.SupplierCreditTerms = _utilityService.ConvertToDecimal(item.Value);

                        else if (item.Name == "FinanceCostPayment")
                            directOrderCalculation.FinanceCostPayment = Convert.ToInt32(item.Value);

                        //else if (item.Name == "SellingPrice_FinanceIncome")
                        //    directOrderCalculation.SellingPrice_FinanceIncome = _utilityService.ConvertToDecimal(item.Value);

                        else if (item.Name == "FinanceCost")
                            directOrderCalculation.FinanceCost = directOrderCalculation.TotalFinanceCost;

                        else if (item.Name == "BrokerId")
                            directOrderCalculation.BrokerId = Convert.ToInt32(item.Value);

                        else if (item.Name == "PayableToMill")
                        {
                            var payableToMill_Payable = InvoicedAmount_Payable + gstAmount_Payable - whtAmount_Payable + wholesaleTax;

                            if (directOrderCalculation.FinanceCostPayment == (int)FinanceCostPaymentStatus.DirectlyToMill)
                            {
                                payableToMill_Payable = InvoicedAmount_Payable + directOrderCalculation.TotalFinanceCost + gstAmount_Payable - whtAmount_Payable + wholesaleTax;
                                //payableToMill_Payable = ((decimal)(InvoicedAmount_Payable + directOrderCalculation.TotalFinanceCost));
                            }

                            payableToMillAfterMultiply = payableToMill_Payable * directOrderCalculation.Quantity;
                            directOrderCalculation.PayableToMill = payableToMillAfterMultiply;
                        }

                        else if (item.Name == "PaymentInCash")
                        {
                            if (directOrderCalculation.FinanceCostPayment == (int)FinanceCostPaymentStatus.Cash)
                                paymentInCash_Payable = directOrderCalculation.BrokerCash + directOrderCalculation.TotalFinanceCost;
                            else
                                paymentInCash_Payable = directOrderCalculation.BrokerCash;

                            directOrderCalculation.PaymentInCash = paymentInCash_Payable;
                            paymentInCash_PayableAfterMultiply = directOrderCalculation.PaymentInCash * directOrderCalculation.Quantity;
                            directOrderCalculation.PaymentInCash = paymentInCash_PayableAfterMultiply;
                        }

                        else if (item.Name == "OrderTotal")
                        {
                            //var totalPayable = directOrderCalculation.Payable_PaymentInCash_20 + directOrderCalculation.Payable_PaymentInCash_20; //(model.ProductPrice_Payable + gstAmount_Payable) - (whtAmount_Payable) + wholesaleTax;
                            directOrderCalculation.TotalPerBag = directOrderCalculation.PayableToMill + paymentInCash_Payable;
                            directOrderCalculation.OrderTotal = payableToMillAfterMultiply + paymentInCash_PayableAfterMultiply;

                            directOrderCalculation.GrossCommissionAmount = directOrderCalculation.OrderTotal;
                            directOrderCalculation.SubTotal = directOrderCalculation.OrderTotal;
                        }

                        else if (item.Name == "SupplierCommissionBag")
                        {
                            directOrderCalculation.SupplierCommissionBag = _utilityService.ConvertToDecimal(item.Value);
                            directOrderCalculation.SupplierCommissionBag_Summary = _utilityService.ConvertToDecimal(item.Value) * directOrderCalculation.Quantity;
                        }

                        else if (item.Name == "SupplierCommissionReceivableRate")
                        {
                            directOrderCalculation.SupplierCommissionReceivableRate = _utilityService.ConvertToDecimal(item.Value);
                            //var supplierCommissionReceivableRate_Payable = directOrderCalculation.SupplierCommissionReceivableRate / 100;
                            //directOrderCalculation.SupplierCommissionReceivable_Summary = (decimal)((float)directOrderCalculation.SupplierCommissionReceivableRate * (float)InvoicedAmount_Payable) * directOrderCalculation.Quantity;
                            directOrderCalculation.SupplierCommissionReceivable_Summary = directOrderCalculation.SupplierCommissionReceivableRate * directOrderCalculation.Quantity;
                        }

                        else if (item.Name == "SupplierCommissionPayableUserId")
                            directOrderCalculation.SupplierCommissionPayableUserId = Convert.ToInt32(item.Value);

                        else if (item.Name == "SupplierCommissionReceivableUserId")
                            directOrderCalculation.SupplierCommissionReceivableUserId = Convert.ToInt32(item.Value);
                    }
                }

                #endregion
            }

            #endregion

            await _orderService.UpdateDirectOrderCalculationAsync(directOrderCalculation);

            return new { success = true, message = "", data = await _utilityService.PurchaseOrder_BusinessModelFormCalculatedJson(directOrder) };
        }

        public async Task<object> DirectPurchaseOrderPlaced(int requestForQuotationId)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                return new { success = false, message = "User not found" };

            var requestForQuotation = await _requestService.GetRequestForQuotationByIdAsync(requestForQuotationId);
            if (requestForQuotation is null)
                return new { success = false, message = "Request for directOrderCalculation not found" };

            var directOrders = (await _orderService.SearchDirectOrdersAsync(rfqId: requestForQuotation.Id)).ToList();

            foreach (var directOrder in directOrders)
            {
                var supplier = await _customerService.GetCustomerByIdAsync(directOrder.SupplierId);
                if (supplier is null)
                    return new { success = false, message = "Supplier not found" };

                var request = await _requestService.GetRequestByIdAsync(directOrder.RequestId);
                if (request is null)
                    return new { success = false, message = "Request not found" };

                var directOrderCalculation = await _orderService.GetDirectOrderCalculationByDirectOrderIdAsync(directOrder.Id);
                if (directOrderCalculation is null)
                    return new { success = false, message = "Direct order calculation not found" };

                if (directOrderCalculation.BusinessModelId != (int)BusinessModelEnum.Standard)
                {
                    if (directOrderCalculation.BrokerCash > 0 && directOrderCalculation.BrokerId == 0)
                        return new { success = false, message = await _localizationService.GetResourceAsync("Admin.Request.Order.SalesOrder.Fields.BrokerId.Required") };

                    if (directOrderCalculation.SupplierCommissionPayableUserId > 0 && directOrderCalculation.SupplierCommissionBag == 0)
                        return new { success = false, message = await _localizationService.GetResourceAsync("Admin.Request.Order.SalesOrder.Fields.SupplierCommissionPayableUserId.Required") };

                    if (directOrderCalculation.SupplierCommissionReceivableUserId > 0 && directOrderCalculation.SupplierCommissionReceivableRate == 0)
                        return new { success = false, message = await _localizationService.GetResourceAsync("Admin.Request.Order.SalesOrder.Fields.SupplierCommissionReceivableUserId.Required") };
                }

                var product = await _productService.GetProductByIdAsync(request.ProductId);
                if (product is null)
                    return new { success = false, message = "Product not found" };

                var quotation = await _quotationService.GetQuotationByIdAsync(directOrder.QuotationId);
                if (quotation is null)
                    return new { success = false, message = "Quotation not found" };

                //Clear Cart
                var cartItems = await _shoppingCartService.GetShoppingCartAsync(supplier, ShoppingCartType.ShoppingCart, storeId: (await _storeContext.GetCurrentStoreAsync()).Id);
                foreach (var cartItem in cartItems)
                    await _shoppingCartService.DeleteShoppingCartItemAsync(cartItem);

                var warning = await _utilityService.PreparePurchaseOrderCalculationAsync(quotation, directOrderCalculation);
                if (warning.Any())
                    return new { success = false, message = warning };

                //now let's try adding product to the cart (now including product attribute validation, etc)
                var addToCartWarnings = await _shoppingCartService.AddToCartAsync(customer: supplier,
                    product: product,
                    shoppingCartType: ShoppingCartType.ShoppingCart,
                    storeId: (await _storeContext.GetCurrentStoreAsync()).Id,
                    attributesXml: request.ProductAttributeXml,
                    quantity: directOrderCalculation.Quantity, overridePrice: directOrderCalculation.Price, brandId: request.BrandId);
                if (addToCartWarnings.Any())
                {
                    return new { success = false, message = addToCartWarnings };
                }

                //save address for supplier 
                await _utilityService.SavePurchaseOrderAddressAsync(supplier);

                //place order
                var processPaymentRequest = new ProcessPaymentRequest
                {
                    StoreId = (await _storeContext.GetCurrentStoreAsync()).Id,
                };

                _paymentService.GenerateOrderGuid(processPaymentRequest);
                processPaymentRequest.CustomerId = directOrder.SupplierId;
                processPaymentRequest.RequestId = request.Id;
                processPaymentRequest.RFQId = directOrder.RequestForQuotationId;
                processPaymentRequest.QuotationId = directOrder.QuotationId;
                processPaymentRequest.OrderTypeId = (int)OrderType.PurchaseOrder;
                processPaymentRequest.OrderTotal = directOrderCalculation.Price * directOrderCalculation.Quantity;
                processPaymentRequest.SalePrice = directOrderCalculation.Price;
                processPaymentRequest.Quotation = quotation;
                processPaymentRequest.Source = "Tijara";

                var placeOrderResult = await _orderProcessingService.PlaceOrderAsync(processPaymentRequest);
                if (placeOrderResult.Success)
                {
                    //Insert delivery schedule
                    foreach (var schedule in await _orderService.GetAllDirectOrderDeliveryScheduleAsync(directOrder.Id))
                    {
                        await _orderService.InsertDeliveryScheduleAsync(new OrderDeliverySchedule
                        {
                            OrderId = placeOrderResult.PlacedOrder.Id,
                            CreatedById = user.Id,
                            ExpectedDeliveryDateUtc = schedule.ExpectedDeliveryDateUtc.Value,
                            ExpectedShipmentDateUtc = schedule.ExpectedShipmentDateUtc.Value,
                            ExpectedQuantity = schedule.ExpectedQuantity.Value,
                            ExpectedDeliveryCost = schedule.ExpectedDeliveryCost.Value,
                            CreatedOnUtc = DateTime.UtcNow
                        });
                    }

                    await _utilityService.SavePurchaseOrderCalculationAsync(placeOrderResult.PlacedOrder, directOrderCalculation);

                    //Update Quotation Status
                    quotation.QuotationStatus = QuotationStatus.QuotationSelected;
                    quotation.IsApproved = true;
                    await _quotationService.UpdateQuotationAsync(quotation);

                    //Update request
                    request.PinLocation_Latitude = directOrder.PinLocation_Latitude;
                    request.PinLocation_Location = directOrder.PinLocation_Location;
                    request.PinLocation_Longitude = directOrder.PinLocation_Longitude;
                    await _requestService.UpdateRequestAsync(request);

                    //Update Rfq status
                    requestForQuotation.RequestForQuotationStatus = RequestForQuotationStatus.Processing;
                    await _requestService.UpdateRequestForQuotationAsync(requestForQuotation);

                    //Remove Temp
                    var directOrderDeliverySchedules = await _orderService.GetAllDirectOrderDeliveryScheduleAsync(directOrder.Id);
                    foreach (var directOrderDeliverySchedule in directOrderDeliverySchedules)
                        await _orderService.DeleteDirectOrderDeliveryScheduleAsync(directOrderDeliverySchedule);

                    //Remove direct order supplier info
                    var directOrderCalculationsExists = await _orderService.GetAllDirectOrderCalculationAsync(directOrder.Id);
                    foreach (var directOrderCalculationsExist in directOrderCalculationsExists)
                        await _orderService.DeleteDirectOrderCalculationAsync(directOrderCalculationsExist);

                    await _orderService.DeleteDirectOrderAsync(directOrder);
                }
                else
                    return new { success = false, message = await _localizationService.GetResourceAsync("Order.Does'nt.Placed.Success") };
            }

            var quotationsUnselected = (await _quotationService.GetAllQuotationAsync(RfqId: requestForQuotation.Id)).Where(x => !x.IsApproved).ToList();
            foreach (var unselectQuotation in quotationsUnselected)
            {
                unselectQuotation.QuotationStatus = QuotationStatus.QuotationUnSelected;
                await _quotationService.UpdateQuotationAsync(unselectQuotation);
            }

            return new { success = true, message = await _localizationService.GetResourceAsync("Request.PurchaseOrder.Genearted.Successfully") };
        }

        public async Task<object> PurchaseOrderDetail(int orderId)
        {
            if (orderId <= 0)
                return new { success = false, message = "Buyer order id is required" };

            var buyer = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(buyer) /*|| !await _customerService.IsBuyerAsync(buyer)*/)
                return new { success = false, message = "Buyer not found" };

            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order is null || order.Deleted)
                return new { success = false, message = "Order not found" };

            var quotation = await _quotationService.GetQuotationByIdAsync(order.QuotationId.Value);
            if (quotation is null)
                return new { success = false, message = "Quotation not found" };

            var requestForQuotation = await _requestService.GetRequestForQuotationByIdAsync(order.RFQId.Value);
            if (requestForQuotation is null)
                return new { success = false, message = "Order request for quotation not found" };

            var buyerRequest = await _requestService.GetRequestByIdAsync(order.RequestId);
            if (buyerRequest is null || buyerRequest.Deleted)
                return new { success = false, message = "Order request not found" };

            var industry = "";
            var productName = "";
            var brand = "";
            var qtyType = "";
            var deliveryDate = "";
            var deliveryAddress = "";
            var quantity = 0m;
            var orderShipped = "";
            var orderDelivered = "";

            industry = (await _industryService.GetIndustryByIdAsync(buyerRequest.IndustryId))?.Name ?? "-";
            quantity = quotation.Quantity;

            var product = await _productService.GetProductByIdAsync(buyerRequest.ProductId);
            if (product is not null)
            {
                productName = await _localizationService.GetLocalizedAsync(product, x => x.Name);
                qtyType = (await _measureService.GetMeasureWeightByIdAsync(product.UnitId))?.Name ?? string.Empty;
            }

            brand = quotation.BrandId > 0 ? await _localizationService.GetLocalizedAsync(await _brandService.GetManufacturerByIdAsync(quotation.BrandId), x => x.Name) : "-";
            deliveryDate = (await _dateTimeHelper.ConvertToUserTimeAsync(buyerRequest.DeliveryDate, DateTimeKind.Utc)).ToString("dd MMM yy");

            var city = await _stateProvinceService.GetStateProvinceByIdAsync(buyerRequest.CityId);
            if (city != null)
                deliveryAddress += city.Name + ", ";

            var area = await _stateProvinceService.GetStateProvinceByIdAsync(buyerRequest.AreaId);
            if (area != null)
                deliveryAddress += area.Name + ", ";

            if (!string.IsNullOrWhiteSpace(buyerRequest.DeliveryAddress))
                deliveryAddress += buyerRequest.DeliveryAddress + ", " + buyerRequest.DeliveryAddress2;

            if (order.ShippingStatusId == (int)ShippingStatus.Delivered || order.ShippingStatusId == (int)ShippingStatus.Shipped)
            {
                var orderShipment = (await _shipmentService.GetAllShipmentsAsync(orderId: order.Id, shipmentTypeId: (int)ShipmentType.PurchaseOrder)).FirstOrDefault();
                if (orderShipment is not null)
                {
                    orderShipped = orderShipment.ShippedDateUtc.HasValue ?
                        (await _dateTimeHelper.ConvertToUserTimeAsync(orderShipment.ShippedDateUtc.Value, DateTimeKind.Utc)).ToString("dd MMM yy - hh:mm tt") : "";

                    orderDelivered = orderShipment.DeliveryDateUtc.HasValue && order.ShippingStatusId == (int)ShippingStatus.Delivered ?
                        (await _dateTimeHelper.ConvertToUserTimeAsync(orderShipment.DeliveryDateUtc.Value, DateTimeKind.Utc)).ToString("dd MMM yy - hh:mm tt") : "";
                }
            }

            //balance quantity working
            decimal shipmentBalanceQuantity = 0;
            foreach (var orderItem in await _orderService.GetOrderItemsAsync(order.Id, isShipEnabled: true)) //we can ship only shippable products
            {
                var totalNumberOfItemsCanBeAddedToShipment = await _orderService.GetTotalNumberOfItemsCanBeAddedToShipmentAsync(orderItem);
                if (totalNumberOfItemsCanBeAddedToShipment <= 0)
                    continue;

                //yes, we have at least one item to create a new shipment
                shipmentBalanceQuantity += await _priceCalculationService.RoundPriceAsync(totalNumberOfItemsCanBeAddedToShipment);
            }

            var supplierContract = await _orderService.GetContractByOrderIdAsync(order.Id);
            if (supplierContract is not null)
                await _httpClient.GetStringAsync(_storeContext.GetCurrentStore().Url + $"download/get-contract/{supplierContract.ContractGuid}");

            var data = new
            {
                OrderInfo = new
                {
                    OrderId = order.Id,
                    order.CustomOrderNumber,
                    order.OrderStatusId,
                    OrderStatus = await _localizationService.GetLocalizedEnumAsync(order.OrderStatus),
                    Industry = industry,
                    Product = productName,
                    Brand = brand,
                    TotalQuantity = quantity,
                    QtyType = qtyType,
                    DeliveryDate = deliveryDate,
                    OrderCreatedDate = (await _dateTimeHelper.ConvertToUserTimeAsync(order.CreatedOnUtc, DateTimeKind.Utc)).ToString("dd MMM yy"),
                    OrderCreatedTime = (await _dateTimeHelper.ConvertToUserTimeAsync(order.CreatedOnUtc, DateTimeKind.Utc)).ToString("hh:mm tt"),
                    BuyerDeliveryAddress = deliveryAddress,
                    quotation.BookerId,
                    BookerName = (await _customerService.GetCustomerByIdAsync(customerId: quotation.BookerId))?.FullName,
                    BuyerRequestId = buyerRequest.Id,
                    SupplierName = (await _customerService.GetCustomerByIdAsync(customerId: order.CustomerId))?.FullName,
                    SupplierEmail = (await _customerService.GetCustomerByIdAsync(customerId: order.CustomerId))?.Email,
                    ProductAttributesInfo = await _utilityService.ParseAttributeXml(buyerRequest.ProductAttributeXml),
                    SupplierContract = new
                    {
                        contractId = supplierContract != null ? supplierContract.Id : 0,
                        downloadUrl = supplierContract != null ? _storeContext.GetCurrentStore().Url + $"download/get-contract/{supplierContract.ContractGuid}" : "",
                        previewUrl = supplierContract != null ? _storeContext.GetCurrentStore().Url + $"files/contracts/{supplierContract.ContractGuid}.pdf" : "",
                        contractSignature = supplierContract != null && supplierContract.SignaturePictureId > 0 ? true : false
                    }
                },
                OrderStatus = new
                {
                    OrderPending = (await _dateTimeHelper.ConvertToUserTimeAsync(order.CreatedOnUtc, DateTimeKind.Utc)).ToString("dd MMM yy - hh:mm tt"),
                    OrderProcessed = order.ProcessingDateUtc.HasValue ? (await _dateTimeHelper.ConvertToUserTimeAsync(order.ProcessingDateUtc.Value, DateTimeKind.Utc)).ToString("dd MMM yy - hh:mm tt") : "",
                    OrderShipped = orderShipped,
                    OrderDelivered = orderDelivered
                },
                ShippmentDetail = new
                {
                    DeliverAddress = buyerRequest.DeliveryAddress,
                    ShippingStatus = await _localizationService.GetLocalizedEnumAsync(order.ShippingStatus),
                    NoOfShipmentNumber = (await _shipmentService.GetShipmentsByOrderIdAsync(order.Id)).Count(),
                    QuantityLeft = shipmentBalanceQuantity,
                    Shippments = (await _shipmentService.GetAllShipmentsAsync(orderId: order.Id, shipmentTypeId: (int)ShipmentType.PurchaseOrder)).Select(s =>
                    {
                        return new
                        {
                            ShippmentId = s.Id,
                            ShipmentStatus = _localizationService.GetLocalizedEnumAsync(s.DeliveryStatus).Result,
                        };
                    }).ToList(),
                    PinLocation = new
                    {
                        Location = buyerRequest.PinLocation_Location,
                        Longitude = buyerRequest.PinLocation_Longitude,
                        Latitude = buyerRequest.PinLocation_Latitude
                    }
                },
                expectedShipments = await _orderService.GetAllDeliveryScheduleAsync(orderId: order.Id).Result.SelectAwait(async b =>
                {
                    return new
                    {
                        expectedDeliveryDateUtc = (await _dateTimeHelper.ConvertToUserTimeAsync(b.ExpectedDeliveryDateUtc, DateTimeKind.Utc)).ToString("dd MMM yy - hh:mm tt"),
                        expectedShipmentDateUtc = (await _dateTimeHelper.ConvertToUserTimeAsync(b.ExpectedShipmentDateUtc, DateTimeKind.Utc)).ToString("dd MMM yy - hh:mm tt"),
                        expectedDeliveryCost = b.ExpectedDeliveryCost,
                        Quantity = b.ExpectedQuantity
                    };
                }).ToListAsync(),
            };
            return new { success = true, message = "", data };
        }

        public async Task<object> PurchaseOrderList(bool showActiveOrders = true, int pageIndex = 0, int pageSize = 10)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                return new { success = false, message = "User not found" };

            List<Order> allOrders = new List<Order>();
            List<OrderManagementApiModel.Orders> activeOrders = new List<OrderManagementApiModel.Orders>();
            List<OrderManagementApiModel.Orders> pastOrders = new List<OrderManagementApiModel.Orders>();
            List<int> orderStatusIds = new List<int>();

            if (showActiveOrders)
                orderStatusIds.AddRange(new List<int> { (int)OrderStatus.Pending, (int)OrderStatus.Processing });
            else
                orderStatusIds.AddRange(new List<int> { (int)OrderStatus.Cancelled, (int)OrderStatus.Complete });

            var allOrdersPageList = await _orderService.SearchOrdersAsync(osIds: orderStatusIds, orderTypeId: (int)OrderType.PurchaseOrder, pageIndex: pageIndex, pageSize: pageSize);

            allOrders = allOrdersPageList.ToList();
            if (allOrders.Any())
            {
                var currentTime = await _dateTimeHelper.ConvertToUserTimeAsync(DateTime.UtcNow, DateTimeKind.Utc);
                //Get Active Request List Filter By Date 
                var activeOrderDates = allOrders.OrderByDescending(x => x.CreatedOnUtc).Where(x => x.OrderStatusId == (int)OrderStatus.Pending || x.OrderStatusId == (int)OrderStatus.Processing).GroupBy(p => _dateTimeHelper.ConvertToUserTimeAsync(p.CreatedOnUtc, DateTimeKind.Utc).Result.ToString("dd/MM/yyyy")).ToList();

                await Task.WhenAll(activeOrderDates.Select(async activeOrderDate =>
                {
                    var model = new OrderManagementApiModel.Orders();
                    var yesterday = currentTime.AddDays(-1).ToString("dd/MM/yyyy");
                    if (currentTime.ToString("dd/MM/yyyy") == activeOrderDate.Key)
                        model.Date = "Today";
                    else if (currentTime.AddDays(-1).ToString("dd/MM/yyyy") == activeOrderDate.Key)
                        model.Date = "Yesterday";
                    else
                        model.Date = activeOrderDate.Key;

                    await Task.WhenAll(activeOrderDate.Select(async order =>
                    {
                        var paymentDueDate = "";
                        var industry = "";

                        if (order.RequestId > 0)
                        {
                            var buyerRequest = await _requestService.GetRequestByIdAsync(order.RequestId);
                            if (buyerRequest is not null)
                            {
                                industry = (await _categoryService.GetCategoryByIdAsync(buyerRequest.IndustryId))?.Name ?? "-";


                                var orderShipments = await _shipmentService.GetAllShipmentsAsync(orderId: order.Id, shipmentTypeId: (int)ShipmentType.PurchaseOrder);
                                if (!orderShipments.Any())
                                {
                                    paymentDueDate = (await _dateTimeHelper.ConvertToUserTimeAsync(order.CreatedOnUtc.AddDays(buyerRequest.PaymentDuration), DateTimeKind.Utc)).ToString("dd MMM yy");
                                }
                                else
                                {
                                    var orderShipment = orderShipments.FirstOrDefault();
                                    paymentDueDate = orderShipment.DeliveryDateUtc.HasValue ?
                                        (await _dateTimeHelper.ConvertToUserTimeAsync(orderShipment.DeliveryDateUtc.Value.AddDays(buyerRequest.PaymentDuration), DateTimeKind.Utc)).ToString("dd MMM yy") :
                                        (await _dateTimeHelper.ConvertToUserTimeAsync(order.CreatedOnUtc.AddDays(buyerRequest.PaymentDuration), DateTimeKind.Utc)).ToString("dd MMM yy");
                                }
                            }

                            var requestForQuotation = await _requestService.GetRequestForQuotationByIdAsync(order.RFQId.Value);

                            var quotation = await _quotationService.GetQuotationByIdAsync(order.QuotationId.Value);

                            var product = await _productService.GetProductByIdAsync((await _orderService.GetOrderItemsAsync(order.Id)).FirstOrDefault().ProductId);

                            model.Data.Add(new OrdersData
                            {
                                OrderId = order.Id,
                                OrderCustomNumber = order.CustomOrderNumber,
                                CreatedOnUtc = (await _dateTimeHelper.ConvertToUserTimeAsync(order.CreatedOnUtc, DateTimeKind.Utc)).ToString("hh:mm tt"),
                                OrderStatus = await _localizationService.GetLocalizedEnumAsync(order.OrderStatus),
                                OrderStatusId = order.OrderStatusId,
                                ProductId = product.Id,
                                ProductName = product.Name,
                                SupplierId = quotation.SupplierId,
                                SupplierName = (await _customerService.GetCustomerByIdAsync(customerId: quotation.SupplierId))?.FullName,
                                BrandId = quotation.BrandId,
                                BrandName = quotation.BrandId > 0 ? await _localizationService.GetLocalizedAsync(await _brandService.GetManufacturerByIdAsync(quotation.BrandId), x => x.Name) : "-",
                                BookerId = quotation.BookerId,
                                BookerName = (await _customerService.GetCustomerByIdAsync(customerId: quotation.BookerId))?.FullName,
                                Quantity = quotation.Quantity,
                                PaymentDueDate = paymentDueDate,
                                Industry = industry,
                            });
                        }
                    }));
                    activeOrders.Add(model);
                }));

                var pastOrderDates = allOrders.OrderByDescending(x => x.CreatedOnUtc).Where(x => x.OrderStatusId == (int)OrderStatus.Cancelled).GroupBy(p => _dateTimeHelper.ConvertToUserTimeAsync(p.CreatedOnUtc, DateTimeKind.Utc).Result.ToString("dd/MM/yyyy")).ToList();

                await Task.WhenAll(pastOrderDates.Select(async pastOrderDate =>
                {
                    var model = new OrderManagementApiModel.Orders();
                    var yesterday = currentTime.AddDays(-1).ToString("dd/MM/yyyy");
                    if (currentTime.ToString("dd/MM/yyyy") == pastOrderDate.Key)
                        model.Date = "Today";
                    else if (currentTime.AddDays(-1).ToString("dd/MM/yyyy") == pastOrderDate.Key)
                        model.Date = "Yesterday";
                    else
                        model.Date = pastOrderDate.Key;

                    foreach (var order in pastOrderDate)
                    {
                        var paymentDueDate = "";
                        var industry = "";

                        if (order.RequestId > 0)
                        {
                            var buyerRequest = await _requestService.GetRequestByIdAsync(order.RequestId);
                            if (buyerRequest is not null)
                            {
                                industry = (await _categoryService.GetCategoryByIdAsync(buyerRequest.IndustryId))?.Name ?? "-";


                                var orderShipments = await _shipmentService.GetAllShipmentsAsync(orderId: order.Id, shipmentTypeId: (int)ShipmentType.PurchaseOrder);
                                if (!orderShipments.Any())
                                {
                                    paymentDueDate = (await _dateTimeHelper.ConvertToUserTimeAsync(order.CreatedOnUtc.AddDays(buyerRequest.PaymentDuration), DateTimeKind.Utc)).ToString("dd MMM yy");
                                }
                                else
                                {
                                    var orderShipment = orderShipments.FirstOrDefault();
                                    paymentDueDate = orderShipment.DeliveryDateUtc.HasValue ?
                                        (await _dateTimeHelper.ConvertToUserTimeAsync(orderShipment.DeliveryDateUtc.Value.AddDays(buyerRequest.PaymentDuration), DateTimeKind.Utc)).ToString("dd MMM yy") :
                                        (await _dateTimeHelper.ConvertToUserTimeAsync(order.CreatedOnUtc.AddDays(buyerRequest.PaymentDuration), DateTimeKind.Utc)).ToString("dd MMM yy");
                                }
                            }

                            var requestForQuotation = await _requestService.GetRequestForQuotationByIdAsync(order.RFQId.Value);
                            if (requestForQuotation is null)
                                continue;

                            var quotation = await _quotationService.GetQuotationByIdAsync(order.QuotationId.Value);
                            if (quotation is null)
                                continue;

                            var product = await _productService.GetProductByIdAsync((await _orderService.GetOrderItemsAsync(order.Id)).FirstOrDefault().ProductId);
                            if (product == null)
                                continue;

                            model.Data.Add(new OrdersData
                            {
                                OrderId = order.Id,
                                OrderCustomNumber = order.CustomOrderNumber,
                                CreatedOnUtc = (await _dateTimeHelper.ConvertToUserTimeAsync(order.CreatedOnUtc, DateTimeKind.Utc)).ToString("hh:mm tt"),
                                OrderStatus = await _localizationService.GetLocalizedEnumAsync(order.OrderStatus),
                                OrderStatusId = order.OrderStatusId,
                                ProductId = product.Id,
                                ProductName = product.Name,
                                SupplierId = quotation.SupplierId,
                                SupplierName = (await _customerService.GetCustomerByIdAsync(customerId: quotation.SupplierId))?.FullName,
                                BrandId = quotation.BrandId,
                                BrandName = quotation.BrandId > 0 ? await _localizationService.GetLocalizedAsync(await _brandService.GetManufacturerByIdAsync(quotation.BrandId), x => x.Name) : "-",
                                BookerId = quotation.BookerId,
                                BookerName = (await _customerService.GetCustomerByIdAsync(customerId: quotation.BookerId))?.FullName,
                                Quantity = quotation.Quantity,
                                PaymentDueDate = paymentDueDate,
                                Industry = industry,
                            });
                        }
                    }
                    pastOrders.Add(model);
                }));

                var data = new List<object>();
                if (activeOrders.Any())
                {
                    data.Add(new
                    {
                        DataType = await _localizationService.GetResourceAsync("Order.ActiveOrder"),
                        Data = activeOrders
                    });
                }
                if (pastOrders.Any())
                {
                    data.Add(new
                    {
                        DataType = await _localizationService.GetResourceAsync("Order.PastOrder"),
                        Data = pastOrders
                    });
                }

                return new { success = true, message = "", data, totalPages = allOrdersPageList.TotalPages, currentPage = allOrdersPageList.PageIndex };

            }
            else
            {
                return new { success = false, message = "No orders found." };
            }
        }

        public async Task<object> SupplierOrderSummary(int orderId)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                return new { success = false, message = "user not found" };

            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order is null || order.Deleted)
                return new { success = false, message = "Order not found" };

            var orderCalculation = await _orderService.GetOrderCalculationByOrderIdAsync(orderId);
            if (orderCalculation is null)
                return new { success = false, message = "Order calculation not found" };

            var totalAmountPaid = await _shipmentService.GetOrderPaidAmount(order);
            var totalAmountBalance = orderCalculation.OrderTotal - totalAmountPaid;

            var data = new
            {
                TotalPaybles = await _priceFormatter.FormatPriceAsync(orderCalculation.OrderTotal),
                TotalPaid = await _priceFormatter.FormatPriceAsync(totalAmountPaid),
                TotalBalance = await _priceFormatter.FormatPriceAsync(totalAmountBalance),
                PaymentStatus = await _localizationService.GetLocalizedEnumAsync(order.PaymentStatus)
            };
            return new { success = true, message = "", data };
        }

        #endregion

        #region Order Delivery Request

        public async Task<object> SaleOrderAcceptShipmentRequest(int shipmentRequestId)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                return new { success = false, message = "user not found" };

            var shipmentRequest = await _orderService.GetOrderDeliveryRequestByIdAsync(shipmentRequestId);
            if (shipmentRequest is null)
                return new { success = false, message = "Shipment request not found" };

            var expectedShipment = await _orderService.GetDeliveryScheduleByIdAsync(shipmentRequest.OrderDeliveryScheduleId);
            if (expectedShipment is null)
                return new { success = false, message = "Expected shipment not found" };

            var order = await _orderService.GetOrderByIdAsync(shipmentRequest.OrderId);
            if (order is null)
                return new { success = false, message = "Order not found" };

            var buyerRequest = await _requestService.GetRequestByIdAsync(order.RequestId);
            if (buyerRequest is null)
                return new { success = false, message = "Buyer Request not found" };

            //Delivery request update 
            shipmentRequest.VerifiedUserId = (await _workContext.GetCurrentCustomerAsync()).Id;
            shipmentRequest.StatusId = (int)OrderDeliveryRequestEnum.Complete;
            await _orderService.UpdateOrderDeliveryRequestAsync(shipmentRequest);

            var orderItems = await _orderService.GetOrderItemsAsync(order.Id, isShipEnabled: true);

            //Add shipment from delivery request
            var shipment = new Shipment
            {
                OrderId = shipmentRequest.OrderId,
                //TrackingNumber = model.TrackingNumber,
                TotalWeight = null,
                //AdminComment = model.AdminComment,
                CreatedOnUtc = DateTime.UtcNow,
                ExpectedDateShipped = expectedShipment.ExpectedShipmentDateUtc,
                ExpectedDateDelivered = expectedShipment.ExpectedDeliveryDateUtc,
                ExpectedDeliveryCost = expectedShipment.ExpectedDeliveryCost,
                ExpectedQuantity = shipmentRequest.Quantity,
                PaymentStatusId = (int)PaymentStatus.Pending,
                DeliveryStatusId = (int)DeliveryStatus.Pending,
                ShipmentType = ShipmentType.SaleOrder,
                DeliveryRequestId = shipmentRequest.Id,
                Source = "Tijara",
            };

            var shipmentItems = new List<ShipmentItem>();

            foreach (var orderItem in orderItems)
            {
                var product = await _productService.GetProductByIdAsync(orderItem.ProductId);

                //create a shipment item
                shipmentItems.Add(new ShipmentItem
                {
                    OrderItemId = orderItem.Id,
                    Quantity = shipmentRequest.Quantity,
                    WarehouseId = 0
                });
            }

            //if we have at least one item in the shipment, then save it
            if (shipmentItems.Any())
            {
                await _shipmentService.InsertShipmentAsync(shipment);

                //generate and set custom shipment number
                shipment.CustomShipmentNumber = _customNumberFormatter.GenerateSOShipmentCustomNumber(shipment);
                await _shipmentService.UpdateShipmentAsync(shipment);

                foreach (var shipmentItem in shipmentItems)
                {
                    shipmentItem.ShipmentId = shipment.Id;
                    await _shipmentService.InsertShipmentItemAsync(shipmentItem);
                }

                //add a note
                await _orderService.InsertOrderNoteAsync(new OrderNote
                {
                    OrderId = order.Id,
                    Note = "A shipment has been added",
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });

                await _eventPublisher.PublishAsync(new ShipmentCreatedEvent(shipment));

                return new { success = true, message = await _localizationService.GetResourceAsync("Shipment.create.Success"), shipmentId = shipment.Id };
            }

            return new { success = false, message = await _localizationService.GetResourceAsync("Shipment.does'nt.create") };
        }

        public async Task<object> PurchaseOrderAcceptShipmentRequest(int shipmentRequestId)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                return new { success = false, message = "user not found" };

            var shipmentRequest = await _orderService.GetOrderDeliveryRequestByIdAsync(shipmentRequestId);
            if (shipmentRequest is null)
                return new { success = false, message = "Shipment request not found" };

            var expectedShipment = await _orderService.GetDeliveryScheduleByIdAsync(shipmentRequest.OrderDeliveryScheduleId);
            if (expectedShipment is null)
                return new { success = false, message = "Expected shipment not found" };

            var order = await _orderService.GetOrderByIdAsync(shipmentRequest.OrderId);
            if (order is null)
                return new { success = false, message = "Order not found" };

            var requestForQuotation = await _requestService.GetRequestForQuotationByIdAsync(order.RFQId.Value);
            if (requestForQuotation is null)
                return new { success = false, message = "Request for quotation not found" };

            var buyerRequest = await _requestService.GetRequestByIdAsync(order.RequestId);
            if (buyerRequest is null)
                return new { success = false, message = "Buyer Request not found" };

            //Delivery request update 
            shipmentRequest.VerifiedUserId = (await _workContext.GetCurrentCustomerAsync()).Id;
            shipmentRequest.StatusId = (int)OrderDeliveryRequestEnum.Complete;
            await _orderService.UpdateOrderDeliveryRequestAsync(shipmentRequest);

            var orderItems = await _orderService.GetOrderItemsAsync(order.Id, isShipEnabled: true);

            //Add shipment from delivery request
            var shipment = new Shipment
            {
                OrderId = shipmentRequest.OrderId,
                //TrackingNumber = model.TrackingNumber,
                TotalWeight = null,
                WarehouseId = shipmentRequest.WarehouseId,
                //AdminComment = model.AdminComment,
                CreatedOnUtc = DateTime.UtcNow,
                ExpectedDateShipped = expectedShipment.ExpectedShipmentDateUtc,
                ExpectedDateDelivered = expectedShipment.ExpectedDeliveryDateUtc,
                ExpectedDeliveryCost = expectedShipment.ExpectedDeliveryCost,
                ExpectedQuantity = shipmentRequest.Quantity,
                PaymentStatusId = (int)PaymentStatus.Pending,
                DeliveryStatusId = (int)DeliveryStatus.Pending,
                ShipmentType = ShipmentType.PurchaseOrder,
                DeliveryRequestId = shipmentRequest.Id,
                Source = "Tijara",
            };

            var shipmentItems = new List<ShipmentItem>();

            foreach (var orderItem in orderItems)
            {
                var product = await _productService.GetProductByIdAsync(orderItem.ProductId);

                //create a shipment item
                shipmentItems.Add(new ShipmentItem
                {
                    OrderItemId = orderItem.Id,
                    Quantity = shipmentRequest.Quantity,
                    WarehouseId = shipment.WarehouseId
                });
            }

            //if we have at least one item in the shipment, then save it
            if (shipmentItems.Any())
            {
                await _shipmentService.InsertShipmentAsync(shipment);

                //generate and set custom shipment number
                shipment.CustomShipmentNumber = _customNumberFormatter.GenerateSOShipmentCustomNumber(shipment);
                await _shipmentService.UpdateShipmentAsync(shipment);

                foreach (var shipmentItem in shipmentItems)
                {
                    shipmentItem.ShipmentId = shipment.Id;
                    await _shipmentService.InsertShipmentItemAsync(shipmentItem);
                }

                //add inventory inbound
                await _inventoryService.AddInventoryInboundAsync(order, requestForQuotation, shipment);

                //add a note
                await _orderService.InsertOrderNoteAsync(new OrderNote
                {
                    OrderId = order.Id,
                    Note = "A shipment has been added",
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });

                await _eventPublisher.PublishAsync(new ShipmentCreatedEvent(shipment));

                return new { success = true, message = await _localizationService.GetResourceAsync("Shipment.create.Success"), shipmentId = shipment.Id };
            }

            return new { success = false, message = await _localizationService.GetResourceAsync("Shipment.does'nt.create") };
        }

        public async Task<object> ShipmentRequestReject(RejectDeliveryRequest model)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                return new { success = false, message = "User not found" };

            if (model.ShipmentRequestId <= 0)
                return new { success = false, message = "Shipment request id is required" };

            var shipmentyRequest = await _orderService.GetOrderDeliveryRequestByIdAsync(model.ShipmentRequestId);
            if (shipmentyRequest is null)
                return new { success = false, message = "Delivery request not found" };

            if (string.IsNullOrWhiteSpace(model.RejectedReason))
                return new { success = false, message = "Rejected reason is required" };

            shipmentyRequest.StatusId = (int)OrderDeliveryRequestEnum.Cancelled;
            await _orderService.UpdateOrderDeliveryRequestAsync(shipmentyRequest);

            return new { success = true, message = await _localizationService.GetResourceAsync("Delivery.Cancelled.Rejected.Success") };
        }

        public async Task<object> ReAssignAgent(ReAssignAgent model)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                return new { success = false, message = "User not found" };

            if (model.Id <= 0)
                return new { success = false, message = "Id is required" };

            if (model.agentId <= 0)
                return new { success = false, message = "AgentId is required" };

            if (string.IsNullOrWhiteSpace(model.type))
                return new { success = false, message = "type is required" };

            var agent = await _customerService.GetCustomerByIdAsync(model.agentId);
            if (agent is null)
                return new { success = false, message = "Agent not found" };

            if (model.type == "ShipmentRequest")
            {
                var deliverRequest = await _orderService.GetOrderDeliveryRequestByIdAsync(model.Id);
                if (deliverRequest is null)
                    return new { success = false, message = "Delivery request not found" };

                var oldAgentId = deliverRequest.AgentId;
                deliverRequest.AgentId = agent.Id;
                await _orderService.UpdateOrderDeliveryRequestAsync(deliverRequest);

                return new { success = true, message = "Delivery request updated succesfuly" };
            }

            return new { success = false, message = "Invalid type" };
        }

        public async Task<object> OrderDeliveryRequestMarkAsIncomplete(int deliveryRequestId)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                return new { success = false, message = "user not found" };

            var delievryRequestForm = await _orderService.GetOrderDeliveryRequestByIdAsync(deliveryRequestId);
            if (delievryRequestForm is null)
                return new { success = false, message = "Delivery order request not found" };

            delievryRequestForm.StatusId = (int)OrderDeliveryRequestEnum.Cancelled;
            delievryRequestForm.VerifiedUserId = user.Id;
            await _orderService.UpdateOrderDeliveryRequestAsync(delievryRequestForm);

            return new { success = true, message = "" };
        }

        #endregion

        #region Payment orders

        [HttpGet("all-tickets")]
        public async Task<object> GetAllTickets(string status = "", string orderby = "")
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user) && !await _customerService.IsOperationsAsync(user) && !await _customerService.IsBusinessHeadAsync(user) &&
                !await _customerService.IsFinanceAsync(user))
                return new { success = false, message = "User not found" };

            List<DrANDSrList> todo = new List<DrANDSrList>();
            //List<DrANDSrList> done = new List<DrANDSrList>();
            List<DRAndSRGenericList> combineList = new List<DRAndSRGenericList>();
            List<int> deliveryRequestsStatusIds = new List<int>();
            List<int> saleReturnRequestsStatusIds = new List<int>();
            List<int> poStatusIds = new List<int>();


            if (status == "Pending")
            {
                deliveryRequestsStatusIds.AddRange(new List<int> { (int)OrderDeliveryRequestEnum.Pending, (int)OrderDeliveryRequestEnum.Expired });
                saleReturnRequestsStatusIds.AddRange(new List<int> { (int)OrderSalesReturnRequestEnum.Pending, (int)OrderSalesReturnRequestEnum.Expired });
            }
            if (status == "Accepted")
            {
                deliveryRequestsStatusIds.AddRange(new List<int> { (int)OrderDeliveryRequestEnum.Complete });
                saleReturnRequestsStatusIds.AddRange(new List<int> { (int)OrderSalesReturnRequestEnum.Complete });
            }
            if (status == "Rejected")
            {
                deliveryRequestsStatusIds.AddRange(new List<int> { (int)OrderDeliveryRequestEnum.Cancelled });
                saleReturnRequestsStatusIds.AddRange(new List<int> { (int)OrderSalesReturnRequestEnum.Cancelled });
            }

            int agentId = 0;
            if (await _customerService.IsOperationsAsync(user) && !await _customerService.IsOpsHeadAsync(user))
                agentId = user.Id;

            if (await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageDeliveryRequestTicket))
            {
                //Get delivery Requests
                var orderDeliveryRequests = (await _orderService.SearchOrderDeliveryRequestsAsync(sIds: deliveryRequestsStatusIds, agentId: agentId)).ToList();
                foreach (var deliveryRequest in orderDeliveryRequests)
                {
                    var order = await _orderService.GetOrderByIdAsync(deliveryRequest.OrderId);
                    if (order is null)
                        continue;

                    var buyerRequest = await _requestService.GetRequestByIdAsync(order.RequestId);
                    if (buyerRequest is null)
                        continue;

                    if (order.OrderTypeId == (int)OrderType.PurchaseOrder)
                    {
                        combineList.Add(new DRAndSRGenericList
                        {
                            Id = await _shipmentService.GetShipmentByDeliveryRequestId(deliveryRequest.Id) == null ? deliveryRequest.Id : (await _shipmentService.GetShipmentByDeliveryRequestId(deliveryRequest.Id)).Id,
                            Type = "PickupRequest",
                            ShipmentType = await _shipmentService.GetShipmentByDeliveryRequestId(deliveryRequest.Id) == null ? "ExpectedShipment" : "Shipment",
                            AgentId = deliveryRequest.AgentId,
                            AgentName = (await _customerService.GetCustomerByIdAsync(deliveryRequest.AgentId))?.FullName,
                            TimeRemaining = deliveryRequest.TicketExpiryDate.HasValue ? deliveryRequest.TicketExpiryDate.Value.Subtract(DateTime.Now).TotalHours > 0 || deliveryRequest.TicketExpiryDate.Value.Subtract(DateTime.Now).TotalMinutes > 0 ? deliveryRequest.TicketExpiryDate.Value.Subtract(DateTime.Now).ToString("hh") + " : hrs " + deliveryRequest.TicketExpiryDate.Value.Subtract(DateTime.Now).ToString("mm") + ": mns" : "0" : "0",
                            Date = (await _dateTimeHelper.ConvertToUserTimeAsync(deliveryRequest.CreatedOnUtc, DateTimeKind.Utc)).ToString("dd/MM/yy"),
                            Priority = await _localizationService.GetLocalizedEnumAsync(deliveryRequest.TicketEnum),
                            Requester = (await _customerService.GetCustomerByIdAsync(deliveryRequest.CreatedById))?.FullName,
                            StatusId = deliveryRequest.StatusId,
                            Status = await _localizationService.GetLocalizedEnumAsync(deliveryRequest.OrderDeliveryRequestEnum),
                            OrderType = await _localizationService.GetLocalizedEnumAsync(order.OrderType),
                            OrderId = order.Id,
                            CustomeOrderNumber = order.CustomOrderNumber,
                            CustomerId = order.CustomerId,
                            FullName = (await _customerService.GetCustomerByIdAsync(order.CustomerId))?.FirstName
                        });
                    }
                    if (order.OrderTypeId == (int)OrderType.SaleOrder)
                    {
                        combineList.Add(new DRAndSRGenericList
                        {
                            Id = await _shipmentService.GetShipmentByDeliveryRequestId(deliveryRequest.Id) == null ? deliveryRequest.Id : (await _shipmentService.GetShipmentByDeliveryRequestId(deliveryRequest.Id)).Id,
                            Type = "ShipmentRequest",
                            ShipmentType = await _shipmentService.GetShipmentByDeliveryRequestId(deliveryRequest.Id) == null ? "ExpectedShipment" : "Shipment",
                            AgentId = deliveryRequest.AgentId,
                            AgentName = (await _customerService.GetCustomerByIdAsync(deliveryRequest.AgentId))?.FullName,
                            TimeRemaining = deliveryRequest.TicketExpiryDate.HasValue ? deliveryRequest.TicketExpiryDate.Value.Subtract(DateTime.Now).TotalHours > 0 || deliveryRequest.TicketExpiryDate.Value.Subtract(DateTime.Now).TotalMinutes > 0 ? deliveryRequest.TicketExpiryDate.Value.Subtract(DateTime.Now).ToString("hh") + " : hrs " + deliveryRequest.TicketExpiryDate.Value.Subtract(DateTime.Now).ToString("mm") + ": mns" : "0" : "0",
                            Date = (await _dateTimeHelper.ConvertToUserTimeAsync(deliveryRequest.CreatedOnUtc, DateTimeKind.Utc)).ToString("dd/MM/yy"),
                            Priority = await _localizationService.GetLocalizedEnumAsync(deliveryRequest.TicketEnum),
                            Requester = (await _customerService.GetCustomerByIdAsync(deliveryRequest.CreatedById))?.FullName,
                            StatusId = deliveryRequest.StatusId,
                            Status = await _localizationService.GetLocalizedEnumAsync(deliveryRequest.OrderDeliveryRequestEnum),
                            OrderType = await _localizationService.GetLocalizedEnumAsync(order.OrderType),
                            OrderId = order.Id,
                            CustomeOrderNumber = order.CustomOrderNumber,
                            CustomerId = order.CustomerId,
                            FullName = (await _customerService.GetCustomerByIdAsync(order.CustomerId))?.FirstName
                        });
                    }
                }
            }

            var model = new DrANDSrList();

            if (combineList.Any())
            {
                var currentTime = await _dateTimeHelper.ConvertToUserTimeAsync(DateTime.UtcNow, DateTimeKind.Utc);
                List<IGrouping<string, DRAndSRGenericList>> todoGroupingLists = null;

                if (orderby == "Asc")
                {
                    todoGroupingLists = combineList.OrderBy(x => x.CreatedOnUtc)
                    .GroupBy(p => _dateTimeHelper.ConvertToUserTimeAsync(p.CreatedOnUtc, DateTimeKind.Utc).Result.ToString("dd/MM/yyyy"))
                    .ToList();

                }
                if (orderby == "Desc")
                {
                    todoGroupingLists = combineList.OrderByDescending(x => x.CreatedOnUtc).GroupBy(p => _dateTimeHelper.ConvertToUserTimeAsync(p.CreatedOnUtc, DateTimeKind.Utc).Result.ToString("dd/MM/yyyy")).ToList();

                }

                foreach (var todoGrouping in todoGroupingLists)
                {
                    var yesterday = currentTime.AddDays(-1).ToString("dd/MM/yyyy");
                    if (currentTime.ToString("dd/MM/yyyy") == todoGrouping.Key)
                        model.Date = "Today";
                    else if (currentTime.AddDays(-1).ToString("dd/MM/yyyy") == todoGrouping.Key)
                        model.Date = "Yesterday";
                    else
                        model.Date = todoGrouping.Key;

                    foreach (var todoGroup in todoGrouping)
                    {
                        model.Data.Add(new DRAndSRData
                        {
                            Id = todoGroup.Id,
                            Type = todoGroup.Type,
                            AgentName = todoGroup.AgentName,
                            TimeRemaining = todoGroup.TimeRemaining,
                            Date = todoGroup.Date,
                            Priority = todoGroup.Priority,
                            Requester = todoGroup.Requester,
                            StatusId = todoGroup.StatusId,
                            Status = todoGroup.Status,
                            ShipmentType = todoGroup.ShipmentType,
                            OrderType = todoGroup.OrderType,
                            OrderId = todoGroup.OrderId,
                            CustomeOrderNumber = (await _orderService.GetOrderByIdAsync(todoGroup.OrderId)).CustomOrderNumber,
                            CustomerId = todoGroup.CustomerId,
                            FullName = (await _customerService.GetCustomerByIdAsync(todoGroup.CustomerId))?.FirstName

                        });
                    }
                    todo.Add(model);
                }

                return new { success = true, data = model };
            }
            else
            {
                return new { success = true, message = "No data found." };
            }
        }

        #endregion

        #region Buyer Contract

        public async Task<object> BuyerContactUploadSignature(BuyerContactUploadSignatureModel model)
        {
            if (model.imgBytes == null)
                return new { success = false, message = "Image is required" };

            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                return new { success = false, message = "user not found" };

            var order = await _orderService.GetOrderByIdAsync(model.OrderId);
            if (order == null)
                return new { success = false, message = "Order not found." };

            var buyerContract = await _orderService.GetContractByIdAsync(model.ContactId);
            if (buyerContract == null)
                return new { success = false, message = "Buyer contract not found." };

            buyerContract.SignaturePictureId = await _utilityService.OrderManagement_UploadPicture(model.imgBytes, $"{buyerContract.ContractGuid}");
            await _orderService.UpdateContractAsync(buyerContract);

            await _httpClient.GetStringAsync(_storeContext.GetCurrentStore().Url + $"download/get-contract/{buyerContract.ContractGuid}");

            return new
            {
                success = true,
                message = "",
                BuyerContract = new
                {
                    contractId = buyerContract.Id,
                    downloadUrl = _storeContext.GetCurrentStore().Url + $"download/get-contract/{buyerContract.ContractGuid}",
                    previewUrl = _storeContext.GetCurrentStore().Url + $"files/contracts/{buyerContract.ContractGuid}.pdf",
                    contractSignature = buyerContract.SignaturePictureId > 0 ? true : false
                }
            };
        }

        public async Task<object> SupplierContactUploadSignature(BuyerContactUploadSignatureModel model)
        {
            if (model.imgBytes == null)
                return new { success = false, message = "Image is required" };

            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                return new { success = false, message = "user not found" };

            var order = await _orderService.GetOrderByIdAsync(model.OrderId);
            if (order == null)
                return new { success = false, message = "Order not found." };

            var supplierContract = await _orderService.GetContractByIdAsync(model.ContactId);
            if (supplierContract == null)
                return new { success = false, message = "Buyer contract not found." };

            supplierContract.SignaturePictureId = await _utilityService.OrderManagement_UploadPicture(model.imgBytes, $"{supplierContract.ContractGuid}");
            await _orderService.UpdateContractAsync(supplierContract);

            await _httpClient.GetStringAsync(_storeContext.GetCurrentStore().Url + $"download/get-contract/{supplierContract.ContractGuid}");

            return new
            {
                success = true,
                message = "",
                SupplierContract = new
                {
                    contractId = supplierContract.Id,
                    downloadUrl = _storeContext.GetCurrentStore().Url + $"download/get-contract/{supplierContract.ContractGuid}",
                    previewUrl = _storeContext.GetCurrentStore().Url + $"files/contracts/{supplierContract.ContractGuid}.pdf",
                    contractSignature = supplierContract.SignaturePictureId > 0 ? true : false
                }
            };
        }

        #endregion

        #region Expected Shipments

        public async Task<object> GetExpectedShipmentsByOrderId(int orderId)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                return new { success = false, message = "user not found" };
            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order is null)
                return new { success = false, message = "Order not found" };

            var orderItem = (await _orderService.GetOrderItemsAsync(orderId)).FirstOrDefault();
            if (orderItem is null)
                return new { success = false, message = "Order item not found" };

            var request = await _requestService.GetRequestByIdAsync(order.RequestId);
            if (request is null)
                return new { success = false, message = "Request not found" };

            var product = await _productService.GetProductByIdAsync(request.ProductId);
            if (product is null)
                return new { success = false, message = "Product not found" };

            var data = new
            {
                orderNo = order.CustomOrderNumber,
                productId = product.Id,
                productName = product.Name,
                productAttributesInfo = await _utilityService.ParseAttributeXml(request.ProductAttributeXml),
                brand = request.BrandId > 0 ? await _localizationService.GetLocalizedAsync(await _brandService.GetManufacturerByIdAsync(request.BrandId), x => x.Name) : "-",
                leftQuantity = orderItem.Quantity - _orderService.GetTotalOrderDeliveryScheduleQuantityByOrderIdAsync(order.Id),
                //leftQuantity = orderItem.Quantity - remaining,
                totalQuantity = orderItem.Quantity,
                expectedShipments = (await _orderService.GetAllDeliveryScheduleAsync(order.Id)).Select(async d =>
                {
                    return new
                    {
                        id = d.Id,
                        expectedDeliveryDateUtc = await _dateTimeHelper.ConvertToUserTimeAsync(d.ExpectedDeliveryDateUtc, DateTimeKind.Utc),
                        expectedShipmentDateUtc = await _dateTimeHelper.ConvertToUserTimeAsync(d.ExpectedShipmentDateUtc, DateTimeKind.Utc),
                        expectedQuantity = d.ExpectedQuantity > 0 ? d.ExpectedQuantity.ToString() : "",
                        expectedDeliveryCost = d.ExpectedDeliveryCost,
                        createdOnUtc = d.CreatedOnUtc,
                        orderId = d.OrderId,
                        requestRemaninigQuantity = orderItem.Quantity - _orderService.GetTotalOrderDeliveryRequestQuantityByOrderIdAsync(order.Id, d.Id),
                        //requestRemaninigQuantity = orderItem.Quantity - remaining,
                        deliveryRequests = (await _orderService.SearchOrderDeliveryRequestsAsync(d.Id)).Select(async dr =>
                        {
                            return new
                            {
                                id = dr.Id,
                                //remianing = orderItem.Quantity - ((await _shipmentService.GetShipmentByDeliveryRequestId(dr.Id)) != null ? /*Shipped condition*/ (await _shipmentService.GetShipmentByDeliveryRequestId(d.Id)).ShippedDateUtc.HasValue ? (await _shipmentService.GetShipmentByDeliveryRequestId(d.Id)).ActualShippedQuantity : /*Delivered condition*/ (await _shipmentService.GetShipmentByDeliveryRequestId(d.Id)).DeliveryDateUtc.HasValue ? (await _shipmentService.GetShipmentByDeliveryRequestId(d.Id)).ActualDeliveredQuantity : _orderService.GetTotalOrderDeliveryRequestQuantityByOrderIdAsync(order.Id, d.Id)),
                                shipmentId = dr.StatusId == (int)OrderDeliveryRequestEnum.Complete ? (await _shipmentService.GetShipmentByDeliveryRequestId(dr.Id))?.Id : 0,
                                statusId = dr.StatusId,
                                status = await _shipmentService.GetShipmentByDeliveryRequestId(dr.Id) != null && (await _shipmentService.GetShipmentByDeliveryRequestId(dr.Id)).DeliveryDateUtc.HasValue ? "Delivered" : await _shipmentService.GetShipmentByDeliveryRequestId(dr.Id) != null && (await _shipmentService.GetShipmentByDeliveryRequestId(dr.Id)).ShippedDateUtc.HasValue ? "Shipped" : await _localizationService.GetLocalizedEnumAsync(dr.OrderDeliveryRequestEnum),
                                quantity = await _shipmentService.GetShipmentByDeliveryRequestId(dr.Id) != null && (await _shipmentService.GetShipmentByDeliveryRequestId(dr.Id)).DeliveryDateUtc.HasValue ? (await _shipmentService.GetShipmentByDeliveryRequestId(dr.Id)).ActualDeliveredQuantity : await _shipmentService.GetShipmentByDeliveryRequestId(dr.Id) != null && (await _shipmentService.GetShipmentByDeliveryRequestId(dr.Id)).ShippedDateUtc.HasValue ? (await _shipmentService.GetShipmentByDeliveryRequestId(dr.Id))?.ActualShippedQuantity : dr.Quantity
                            };
                        }).Select(t => t.Result).ToList(),
                    };
                }).Select(t => t.Result).ToList(),
            };

            return new { success = true, message = "", data };
        }

        #endregion

        #region Sale order Shipments

        public async Task<object> GetAllShipments(int ShipmentTypeId = 0)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                return new { success = false, message = "User not found" };

            var data = await (await _shipmentService.GetAllShipmentsAsync(shipmentTypeId: ShipmentTypeId)).SelectAwait(async sh =>
            {
                return new
                {
                    orderNumber = (await _orderService.GetOrderByIdAsync(sh.OrderId))?.CustomOrderNumber,
                    orderId = (await _orderService.GetOrderByIdAsync(sh.OrderId))?.Id,
                    shipmentNumber = sh.CustomShipmentNumber,
                    shipmentId = sh.Id,
                    shipmentStatus = await _localizationService.GetLocalizedEnumAsync((await _orderService.GetOrderByIdAsync(sh.OrderId)).ShippingStatus),
                    shipmentRequestStatus = await _orderService.GetOrderDeliveryRequestByOrderIdAsync(sh.OrderId) != null ? await _localizationService.GetLocalizedEnumAsync((await _orderService.GetOrderDeliveryRequestByOrderIdAsync(sh.OrderId)).OrderDeliveryRequestEnum) : "",
                    agent = await _orderService.GetOrderDeliveryRequestByOrderIdAsync(sh.OrderId) != null ? (await _customerService.GetCustomerByIdAsync((await _orderService.GetOrderDeliveryRequestByOrderIdAsync(sh.OrderId)).AgentId))?.FullName : "",
                    trackingNumber = sh.TrackingNumber != null ? sh.TrackingNumber : "",
                    adminComment = sh.AdminComment != null ? sh.AdminComment : "",
                    expectedDateShipped = sh.ExpectedDateShipped,
                    expectedDateDelivered = sh.ExpectedDateDelivered,
                    expectedQuantity = sh.ExpectedQuantity,
                    expectedDeliveryCost = sh.ExpectedDeliveryCost,
                    shipmentType = await _localizationService.GetLocalizedEnumAsync(sh.ShipmentType)
                };
            }).ToListAsync();

            return new { success = true, data };
        }

        public async Task<object> SaleOrderMarkAsShipped(MaskAsShippedModel model)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                return new { success = false, message = "user not found" };

            if (model.ShipmentId <= 0)
                return new { success = false, message = "Shipment id is required" };

            var shipment = await _shipmentService.GetShipmentByIdAsync(model.ShipmentId);
            if (shipment is null)
                return new { success = false, message = "Shipment not found" };

            var order = await _orderService.GetOrderByIdAsync(shipment.OrderId);
            if (order is null)
                return new { success = false, message = "Order not found" };

            var request = await _requestService.GetRequestByIdAsync(order.RequestId);
            if (request == null)
                return new { success = false, message = "Request not found" };

            var orderItem = (await _orderService.GetOrderItemsAsync(order.Id)).FirstOrDefault();
            if (orderItem == null)
                return new { success = false, message = "Order item not found" };

            var shipmentItem = (await _shipmentService.GetShipmentItemsByShipmentIdAsync(shipment.Id)).FirstOrDefault(x => x.OrderItemId == orderItem.Id);
            if (shipmentItem == null)
                return new { success = false, message = "Shipment item not found" };

            if (model.TransporterId <= 0)
                return new { success = false, message = "Transporter id is required" };

            if (model.VehicleId <= 0)
                return new { success = false, message = "Vehicle id is required" };

            if (model.VehicleId > 0 && string.IsNullOrWhiteSpace(model.VehicleNumber))
                return new { success = false, message = "Vehicle number is required" };

            if (model.RouteTypeId <= 0)
                return new { success = false, message = "RouteType id is required" };

            //Vallidate inventory outbound
            if (model.Inventories.Count == 0)
                return new { success = false, message = "Inventory is required" };

            if (model.ActualShippedQuantity < shipmentItem.Quantity && string.IsNullOrWhiteSpace(model.ActualShippedQuantityReason))
                return new { success = false, message = "Actual shipped quantity reason is required" };

            var selectedOutboundQuantity = 0m;
            foreach (var inboundInventory in model.Inventories)
            {
                if (inboundInventory.OutboundQuantity > inboundInventory.BalanceQuantity)
                    return new { success = false, message = await _localizationService.GetResourceAsync("Admin.Shipment.InputQuanity.Exceeded.OutboundQuantity") };
                else
                    selectedOutboundQuantity += inboundInventory.OutboundQuantity;
            }

            if (selectedOutboundQuantity != model.ActualShippedQuantity)
                return new { success = false, message = await _localizationService.GetResourceAsync("Admin.Shipment.Inventory.Invalid.Quantity") };

            foreach (var inbound in model.Inventories)
            {
                var totalRemainingQty = inbound.OutboundQuantity;
                if (totalRemainingQty <= 0)
                    continue;

                var inventyoryInbound = await _inventoryService.GetInventoryInboundByIdAsync(inbound.InventoryId);
                if (inventyoryInbound is null)
                    return new { success = false, message = await _localizationService.GetResourceAsync("Admin.Shipment.Inventory.Not.Found") };

                //Add Inventory OutBound
                await _inventoryService.AddInventoryOutboundAsync(inventyoryInbound, order, shipment, orderItem, totalRemainingQty);
            }

            var shipDate = shipment.ShippedDateUtc.HasValue;
            shipment.TransporterId = model.TransporterId;
            shipment.VehicleId = model.VehicleId;
            shipment.VehicleNumber = model.VehicleNumber;
            shipment.RouteTypeId = model.RouteTypeId;
            shipment.DeliveryStatusId = (int)DeliveryStatus.EnRoute;
            shipment.PickupAddress = model.PickupAddress;
            shipment.ShippedDateUtc = DateTime.UtcNow;
            shipment.OnLabourCharges = model.Laborcharges;
            shipment.CostOnZarayeId = model.CostOnZarayeId;
            shipment.ActualShippedQuantity = model.ActualShippedQuantity;
            shipment.ActualShippedQuantityReason = model.ActualShippedQuantityReason;
            await _shipmentService.UpdateShipmentAsync(shipment);

            if (!string.IsNullOrWhiteSpace(model.ActualShippedQuantityReason) && model.ActualShippedQuantity < shipmentItem.Quantity)
            {
                shipmentItem.Quantity = model.ActualShippedQuantity;
                await _shipmentService.UpdateShipmentItemAsync(shipmentItem);

                //update delivery request
                var deliveryRequest = await _orderService.GetOrderDeliveryRequestByIdAsync(shipment.DeliveryRequestId.Value);
                if (deliveryRequest is not null)
                {
                    deliveryRequest.Quantity = model.ActualShippedQuantity;
                    await _orderService.UpdateOrderDeliveryRequestAsync(deliveryRequest);
                }
            }

            var allTransporterLedger = await _customerLedgerService.GetAllTransporterLedgerByShipmentAsync(shipment.Id, new string[] { "Loading charges" });
            if (allTransporterLedger.Any())
            {
                foreach (var ledger in allTransporterLedger)
                    await _customerLedgerService.DeleteCustomerLedgerAsync(ledger);
            }

            if (model.Laborcharges > 0)
            {
                var transpoter = await _customerService.GetCustomerByIdAsync(model.TransporterId);
                if (transpoter.IsZarayeTransporter && model.CostOnZarayeId == (int)CostOnZaraye.Yes)
                {
                    await _customerLedgerService.AddCustomerLedgerAsync(date: DateTime.UtcNow, customerId: _customerSettings.TranspoterZaraye, description: "Loading charges", debit: 0, credit: model.Laborcharges, shipmentId: shipment.Id);
                }
                else if (transpoter.IsZarayeTransporter && model.CostOnZarayeId == (int)CostOnZaraye.No)
                {
                    await _customerLedgerService.AddCustomerLedgerAsync(date: DateTime.UtcNow, customerId: _customerSettings.ThirdPartyCustomer, description: "Loading charges", debit: 0, credit: model.Laborcharges, shipmentId: shipment.Id);
                }
                else if (!transpoter.IsZarayeTransporter && model.CostOnZarayeId == (int)CostOnZaraye.Yes)
                {
                    await _customerLedgerService.AddCustomerLedgerAsync(date: DateTime.UtcNow, customerId: _customerSettings.ThirdPartyCustomer, description: "Loading charges", debit: 0, credit: model.Laborcharges, shipmentId: shipment.Id);
                }
                else if (!transpoter.IsZarayeTransporter && model.CostOnZarayeId == (int)CostOnZaraye.No)
                {
                    await _customerLedgerService.AddCustomerLedgerAsync(date: DateTime.UtcNow, customerId: _customerSettings.ThirdPartyCustomer, description: "Loading charges", debit: 0, credit: model.Laborcharges, shipmentId: shipment.Id);
                }
            }

            //check whether we have more items to ship
            if (await _orderService.HasItemsToAddToShipmentAsync(order) || await _orderService.HasItemsToShipAsync(order))
                order.ShippingStatusId = (int)ShippingStatus.PartiallyShipped;
            else
                order.ShippingStatusId = (int)ShippingStatus.Shipped;
            await _orderService.UpdateOrderAsync(order);


            return new { success = true, message = await _localizationService.GetResourceAsync("Mark.as.shipped.success") };

        }

        public async Task<object> SaleOrderMarkAsDelivered(MaskAsDeliveredModel model)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                return new { success = false, message = "user not found" };

            if (model.ShipmentId <= 0)
                return new { success = false, message = "Shipment id is required" };

            var shipment = await _shipmentService.GetShipmentByIdAsync(model.ShipmentId);
            if (shipment is null)
                return new { success = false, message = "Shipment not found" };

            var saleOrder = await _orderService.GetOrderByIdAsync(shipment.OrderId);
            if (saleOrder == null)
                return new { success = false, message = "Sale order not found" };

            var saleOrderCalculation = await _orderService.GetOrderCalculationByOrderIdAsync(shipment.OrderId);
            if (saleOrderCalculation == null)
                return new { success = false, message = "Sale order calcultaion not found" };

            var shipmentItem = (await _shipmentService.GetShipmentItemsByShipmentIdAsync(shipment.Id)).FirstOrDefault();
            if (shipmentItem == null)
                return new { success = false, message = "Shipment item not found" };

            if (model.ActualDeliveredQuantity < shipmentItem.Quantity && string.IsNullOrWhiteSpace(model.ActualDeliveredQuantityReason))
                return new { success = false, message = "Actual delivered quantity reason is required" };

            if (model.DeliveryTypeId <= 0)
                throw new Exception("Select delivery type");
            if (model.TransporterTypeId <= 0)
                throw new Exception("Select transporter type");
            if (model.DeliveryTimingId == 0)
                throw new Exception("Select delivery time");
            if (model.DeliveryTimingId == (int)DeliveryTiming.Delayed && model.DeliveryDelayedReasonId == 0)
                throw new Exception("Select delivery time reason");
            if (string.IsNullOrWhiteSpace(model.FileName))
                throw new Exception("Picture is required");

            var shipmentItems = await _shipmentService.GetShipmentItemsByShipmentIdAsync(shipment.Id);

            //sum both fileds value then check is > 0
            var deliveryCost = shipment.OnLabourCharges + model.FreightCharges + model.LabourCharges;

            if (model.DeliveryCostTypeId != (int)DeliveryCostType.FreeDelivery && model.DeliveryCostTypeId != (int)DeliveryCostType.BuyerSelfPickup && deliveryCost <= 0)
                throw new Exception("Enter delivery cost");

            if (shipment.ExpectedDeliveryCost > 0 && deliveryCost > shipment.ExpectedDeliveryCost && model.DeliveryCostReasonId == 0)
                throw new Exception("Select delivery cost reason");

            if (model.DeliveryCostTypeId == (int)DeliveryCostType.FreeDelivery || model.DeliveryCostTypeId == (int)DeliveryCostType.BuyerSelfPickup)
            {
                model.FreightCharges = 0;
                model.LabourCharges = 0;
                deliveryCost = 0;
            }

            var deliveredDate = shipment.DeliveryDateUtc.HasValue;
            shipment.PictureId = model.FileName != null ? await _utilityService.OrderManagement_UploadPicture(model.ImageBytes, model.FileName) : 0;
            shipment.DeliveryCostTypeId = model.DeliveryCostTypeId;
            shipment.DeliveryTypeId = model.DeliveryTypeId;
            shipment.TransporterTypeId = model.TransporterTypeId;
            shipment.DeliveryCost = deliveryCost;
            //shipment.WarehouseId = model.WarehouseId;
            shipment.FreightCharges = model.FreightCharges;
            shipment.LabourCharges = model.LabourCharges;
            shipment.DeliveryTimingId = model.DeliveryTimingId;
            shipment.DeliveryDelayedReasonId = model.DeliveryDelayedReasonId;
            shipment.DeliveryCostReasonId = model.DeliveryCostReasonId;
            shipment.LabourTypeId = model.LabourTypeId;
            shipment.ShipmentDeliveryAddress = model.ShipmentDeliveryAddress;
            shipment.ActualDeliveredQuantity = model.ActualDeliveredQuantity;
            shipment.ActualDeliveredQuantityReason = model.ActualDeliveredQuantityReason;
            await _shipmentService.UpdateShipmentAsync(shipment);

            var allTransporterLedger = await _customerLedgerService.GetAllTransporterLedgerByShipmentAsync(shipment.Id, new string[] { "Frieght charges", "UnLoading charges" });
            if (allTransporterLedger.Any())
            {
                foreach (var ledger in allTransporterLedger)
                    await _customerLedgerService.DeleteCustomerLedgerAsync(ledger);
            }

            if (/*shipment.DeliveryCostType != DeliveryCostType.FulfilledBySupplier &&*/ shipment.DeliveryCostType != DeliveryCostType.FreeDelivery)
            {
                var transpoter = await _customerService.GetCustomerByIdAsync(shipment.TransporterId);
                if (transpoter.IsZarayeTransporter && shipment.LabourTypeId == (int)LabourType.Yes)
                {
                    if (shipment.FreightCharges > 0)
                        await _customerLedgerService.AddCustomerLedgerAsync(date: DateTime.UtcNow, customerId: _customerSettings.TranspoterZaraye, description: "Frieght charges", debit: 0, credit: shipment.FreightCharges, shipmentId: shipment.Id);

                    if (shipment.LabourCharges > 0)
                        await _customerLedgerService.AddCustomerLedgerAsync(date: DateTime.UtcNow, customerId: _customerSettings.TranspoterZaraye, description: "UnLoading charges", debit: 0, credit: shipment.LabourCharges, shipmentId: shipment.Id);
                }
                else if (!transpoter.IsZarayeTransporter && shipment.LabourTypeId == (int)LabourType.Yes)
                {
                    if (shipment.FreightCharges > 0)
                        await _customerLedgerService.AddCustomerLedgerAsync(date: DateTime.UtcNow, customerId: transpoter.Id, description: "Frieght charges", debit: 0, credit: shipment.FreightCharges, shipmentId: shipment.Id);
                    //await _customerLedgerService.AddCustomerLedgerAsync(date: DateTime.UtcNow, customerId: _customerSettings.ThirdPartyCustomer, description: "Frieght charges", debit: 0, credit: shipment.FreightCharges, shipmentId: shipment.Id); Align to khurram

                    if (shipment.LabourCharges > 0)
                        await _customerLedgerService.AddCustomerLedgerAsync(date: DateTime.UtcNow, customerId: _customerSettings.TranspoterZaraye, description: "UnLoading charges", debit: 0, credit: shipment.LabourCharges, shipmentId: shipment.Id);
                }
                else if (transpoter.IsZarayeTransporter && shipment.LabourTypeId == (int)LabourType.No)
                {
                    if (shipment.FreightCharges > 0)
                        await _customerLedgerService.AddCustomerLedgerAsync(date: DateTime.UtcNow, customerId: _customerSettings.TranspoterZaraye, description: "Frieght charges", debit: 0, credit: shipment.FreightCharges, shipmentId: shipment.Id);

                    if (shipment.LabourCharges > 0)
                        await _customerLedgerService.AddCustomerLedgerAsync(date: DateTime.UtcNow, customerId: _customerSettings.ThirdPartyCustomer, description: "UnLoading charges", debit: 0, credit: shipment.LabourCharges, shipmentId: shipment.Id);
                }
                else if (!transpoter.IsZarayeTransporter && shipment.LabourTypeId == (int)LabourType.No)
                {
                    if (shipment.FreightCharges > 0)
                        await _customerLedgerService.AddCustomerLedgerAsync(date: DateTime.UtcNow, customerId: _customerSettings.ThirdPartyCustomer, description: "Frieght charges", debit: 0, credit: shipment.FreightCharges, shipmentId: shipment.Id);

                    if (shipment.LabourCharges > 0)
                        await _customerLedgerService.AddCustomerLedgerAsync(date: DateTime.UtcNow, customerId: _customerSettings.ThirdPartyCustomer, description: "UnLoading charges", debit: 0, credit: shipment.LabourCharges, shipmentId: shipment.Id);
                }
            }

            if (!string.IsNullOrWhiteSpace(model.ActualDeliveredQuantityReason) && model.ActualDeliveredQuantity < shipmentItem.Quantity)
            {
                shipmentItem.Quantity = model.ActualDeliveredQuantity;
                await _shipmentService.UpdateShipmentItemAsync(shipmentItem);

                //update delivery request
                var deliveryRequest = await _orderService.GetOrderDeliveryRequestByIdAsync(shipment.DeliveryRequestId.Value);
                if (deliveryRequest is not null)
                {
                    deliveryRequest.Quantity = model.ActualDeliveredQuantity;
                    await _orderService.UpdateOrderDeliveryRequestAsync(deliveryRequest);
                }
            }

            await _orderProcessingService.DeliverAsync(shipment, true);

            if (saleOrderCalculation.BusinessModelId != (int)BusinessModelEnum.Broker)
            {
                var shipmentDeliveredAmount = shipment.DeliveredAmount;

                #region Finance Income

                if (saleOrderCalculation.FinanceIncome > 0)
                {
                    if (saleOrderCalculation.FinanceCostPayment == (int)FinanceCostPaymentStatus.Cash)
                    {
                        var calculatedFinanceIncomeValue = saleOrderCalculation.FinanceIncome * shipmentItem.Quantity;
                        shipmentDeliveredAmount = shipmentDeliveredAmount - calculatedFinanceIncomeValue;
                        await _customerLedgerService.AddCustomerLedgerAsync(date: shipment.DeliveryDateUtc.Value, customerId: saleOrder.CustomerId, description: "Finance Income", debit: calculatedFinanceIncomeValue, credit: 0, shipmentId: shipment.Id);
                    }
                    else
                    {
                        var calculatedFinanceIncome = saleOrderCalculation.FinanceIncome * (saleOrderCalculation.GSTRate / 100 + 1) * (1 - saleOrderCalculation.WHTRate / 100) * (saleOrderCalculation.WholesaleTaxRate / 100 + 1);
                        calculatedFinanceIncome = calculatedFinanceIncome * shipmentItem.Quantity;
                        shipmentDeliveredAmount = shipmentDeliveredAmount - calculatedFinanceIncome;
                        await _customerLedgerService.AddCustomerLedgerAsync(date: shipment.DeliveryDateUtc.Value, customerId: saleOrder.CustomerId, description: "Finance Income", debit: calculatedFinanceIncome, credit: 0, shipmentId: shipment.Id);
                    }
                }

                #endregion

                #region Deliver Ledger

                //Add Customer Ledger for buyer delivery
                await _customerLedgerService.AddCustomerLedgerAsync(date: shipment.DeliveryDateUtc.Value, customerId: saleOrder.CustomerId, description: "Delivery", debit: shipment.DeliveredAmount, credit: 0, shipmentId: shipment.Id);

                #endregion

                #region Broker Cash Ledger

                if (saleOrderCalculation.BrokerId > 0)
                {
                    var calculatedBrokerCash = saleOrderCalculation.BrokerCash * shipmentItem.Quantity;
                    if (saleOrderCalculation.BrokerCash > 0)
                    {
                        //Add Customer Ledger for positive broker cash
                        await _customerLedgerService.AddCustomerLedgerAsync(date: shipment.DeliveryDateUtc.Value, customerId: saleOrderCalculation.BrokerId, description: "Broker Cash", debit: calculatedBrokerCash, credit: 0, shipmentId: shipment.Id);
                    }
                    else if (saleOrderCalculation.BrokerCash < 0)
                    {
                        //Add Customer Ledger for negative broker cash
                        await _customerLedgerService.AddCustomerLedgerAsync(date: shipment.DeliveryDateUtc.Value, customerId: saleOrderCalculation.BrokerId, description: "Broker Cash", debit: 0, credit: Math.Abs(calculatedBrokerCash), shipmentId: shipment.Id);
                    }
                }

                #endregion
            }

            #region Buyer Commission Receivable Ledger

            if (saleOrderCalculation.BuyerCommissionReceivableUserId > 0 && saleOrderCalculation.BuyerCommissionReceivablePerBag > 0)
            {
                var calculatedBuyerCommissionReceivablePerBag = saleOrderCalculation.BuyerCommissionReceivablePerBag * shipmentItem.Quantity;

                //Add Customer Ledger for Buyer Commission Receivable
                await _customerLedgerService.AddCustomerLedgerAsync(date: shipment.DeliveryDateUtc.Value, customerId: saleOrderCalculation.BuyerCommissionReceivableUserId, description: "Buyer Commission Receivable", debit: calculatedBuyerCommissionReceivablePerBag, credit: 0, shipmentId: shipment.Id);
            }

            #endregion

            #region Buyer Commission Payable Ledger

            if (saleOrderCalculation.BuyerCommissionPayableUserId > 0 && saleOrderCalculation.BuyerCommissionPayablePerBag > 0)
            {
                var calculatedBuyerCommissionPayablePerBag = saleOrderCalculation.BuyerCommissionPayablePerBag * shipmentItem.Quantity;

                //Add Customer Ledger for Buyer Commission Payable
                await _customerLedgerService.AddCustomerLedgerAsync(date: shipment.DeliveryDateUtc.Value, customerId: saleOrderCalculation.BuyerCommissionPayableUserId, description: "Buyer Commission Payable", debit: 0, credit: calculatedBuyerCommissionPayablePerBag, shipmentId: shipment.Id);
            }

            #endregion

            #region Margin Ledger

            if (saleOrderCalculation.BusinessModelId == (int)BusinessModelEnum.Broker && saleOrderCalculation.MarginAmount > 0)
            {
                var calculatedMargin = saleOrderCalculation.MarginAmount * shipmentItem.Quantity;

                //Add Customer Ledger for Buyer Margin
                await _customerLedgerService.AddCustomerLedgerAsync(date: shipment.DeliveryDateUtc.Value, customerId: saleOrder.CustomerId, description: "Margin", debit: calculatedMargin, credit: 0, shipmentId: shipment.Id);
            }

            #endregion

            //Add Customer Ledger for supplier delivery
            //await _customerLedgerService.AddCustomerLedgerAsync(date: shipment.DeliveryDateUtc.Value, customerId: saleOrder.CustomerId, description: "Delivery", debit: shipment.DeliveredAmount, credit: 0, shipmentId: shipment.Id);

            //Add customer ledger for delivery cost on buyer
            if (shipment.DeliveryCostType == DeliveryCostType.DeliveryCostOnBuyer && shipment.DeliveryCost > 0)
                await _customerLedgerService.AddCustomerLedgerAsync(date: shipment.DeliveryDateUtc.Value, customerId: saleOrder.CustomerId, description: "Delivery cost on buyer", debit: shipment.DeliveryCost, credit: 0, shipmentId: shipment.Id, updateRecord: true);

            return new { success = true, message = await _localizationService.GetResourceAsync("Mark.as.delivered.success") };
        }

        public async Task<object> PurchaseOrderMarkAsShipped(MaskAsShippedModel model)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                return new { success = false, message = "user not found" };

            if (model.ShipmentId <= 0)
                return new { success = false, message = "Shipment id is required" };

            var shipment = await _shipmentService.GetShipmentByIdAsync(model.ShipmentId);
            if (shipment is null)
                return new { success = false, message = "Shipment not found" };

            var order = await _orderService.GetOrderByIdAsync(shipment.OrderId);
            if (order is null)
                return new { success = false, message = "Order not found" };

            var request = await _requestService.GetRequestByIdAsync(order.RequestId);
            if (request == null)
                return new { success = false, message = "Request not found" };

            var orderItem = (await _orderService.GetOrderItemsAsync(order.Id)).FirstOrDefault();
            if (orderItem == null)
                return new { success = false, message = "Order item not found" };

            var shipmentItem = (await _shipmentService.GetShipmentItemsByShipmentIdAsync(shipment.Id)).FirstOrDefault(x => x.OrderItemId == orderItem.Id);
            if (shipmentItem == null)
                return new { success = false, message = "Shipment item not found" };

            if (model.TransporterId <= 0)
                return new { success = false, message = "Transporter id is required" };

            if (model.VehicleId <= 0)
                return new { success = false, message = "Vehicle id is required" };

            if (model.VehicleId > 0 && string.IsNullOrWhiteSpace(model.VehicleNumber))
                return new { success = false, message = "Vehicle number is required" };

            if (model.RouteTypeId <= 0)
                return new { success = false, message = "RouteType id is required" };

            if (model.ActualShippedQuantity < shipmentItem.Quantity && string.IsNullOrWhiteSpace(model.ActualShippedQuantityReason))
                return new { success = false, message = "Actual shipped quantity reason is required" };

            shipment.TransporterId = model.TransporterId;
            shipment.WarehouseId = model.WarehouseId;
            shipment.VehicleId = model.VehicleId;
            shipment.VehicleNumber = model.VehicleNumber;
            shipment.RouteTypeId = model.RouteTypeId;
            shipment.DeliveryStatusId = (int)DeliveryStatus.EnRoute;
            shipment.PickupAddress = model.PickupAddress;
            shipment.OnLabourCharges = model.Laborcharges;
            shipment.ShippedDateUtc = DateTime.UtcNow;
            shipment.CostOnZarayeId = model.CostOnZarayeId;
            shipment.ActualShippedQuantity = model.ActualShippedQuantity;
            shipment.ActualShippedQuantityReason = model.ActualShippedQuantityReason;
            await _shipmentService.UpdateShipmentAsync(shipment);

            if (!string.IsNullOrWhiteSpace(model.ActualShippedQuantityReason) && model.ActualShippedQuantity < shipmentItem.Quantity)
            {
                shipmentItem.Quantity = model.ActualShippedQuantity;
                await _shipmentService.UpdateShipmentItemAsync(shipmentItem);

                //update delivery request
                var deliveryRequest = await _orderService.GetOrderDeliveryRequestByIdAsync(shipment.DeliveryRequestId.Value);
                if (deliveryRequest is not null)
                {
                    deliveryRequest.Quantity = model.ActualShippedQuantity;
                    await _orderService.UpdateOrderDeliveryRequestAsync(deliveryRequest);
                }
            }
            //Update inventory inbound to 'InTransit'
            var inventoryInbound = _inventoryService.GetInventoryInboundByShipmentId(shipment.Id);
            if (inventoryInbound != null)
            {
                inventoryInbound.InventoryInboundStatusEnum = InventoryInboundStatusEnum.InTransit;
                await _inventoryService.UpdateInventoryInboundAsync(inventoryInbound);
            }

            var allTransporterLedger = await _customerLedgerService.GetAllTransporterLedgerByShipmentAsync(shipment.Id, new string[] { "Loading charges" });
            if (allTransporterLedger.Any())
            {
                foreach (var ledger in allTransporterLedger)
                    await _customerLedgerService.DeleteCustomerLedgerAsync(ledger);
            }

            if (model.Laborcharges > 0)
            {
                var transpoter = await _customerService.GetCustomerByIdAsync(model.TransporterId);
                if (transpoter.IsZarayeTransporter && model.CostOnZarayeId == (int)CostOnZaraye.Yes)
                {
                    await _customerLedgerService.AddCustomerLedgerAsync(date: DateTime.UtcNow, customerId: _customerSettings.TranspoterZaraye, description: "Loading charges", debit: 0, credit: model.Laborcharges, shipmentId: shipment.Id);
                }
                else if (transpoter.IsZarayeTransporter && model.CostOnZarayeId == (int)CostOnZaraye.No)
                {
                    await _customerLedgerService.AddCustomerLedgerAsync(date: DateTime.UtcNow, customerId: _customerSettings.ThirdPartyCustomer, description: "Loading charges", debit: 0, credit: model.Laborcharges, shipmentId: shipment.Id);
                }
                else if (!transpoter.IsZarayeTransporter && model.CostOnZarayeId == (int)CostOnZaraye.Yes)
                {
                    await _customerLedgerService.AddCustomerLedgerAsync(date: DateTime.UtcNow, customerId: _customerSettings.ThirdPartyCustomer, description: "Loading charges", debit: 0, credit: model.Laborcharges, shipmentId: shipment.Id);
                }
                else if (!transpoter.IsZarayeTransporter && model.CostOnZarayeId == (int)CostOnZaraye.No)
                {
                    await _customerLedgerService.AddCustomerLedgerAsync(date: DateTime.UtcNow, customerId: _customerSettings.ThirdPartyCustomer, description: "Loading charges", debit: 0, credit: model.Laborcharges, shipmentId: shipment.Id);
                }
            }

            //check whether we have more items to ship
            if (await _orderService.HasItemsToAddToShipmentAsync(order) || await _orderService.HasItemsToShipAsync(order))
                order.ShippingStatusId = (int)ShippingStatus.PartiallyShipped;
            else
                order.ShippingStatusId = (int)ShippingStatus.Shipped;
            await _orderService.UpdateOrderAsync(order);


            return new { success = true, message = await _localizationService.GetResourceAsync("Mark.as.shipped.success") };
        }

        public async Task<object> PurchaseOrderMarkAsDelivered(MaskAsDeliveredModel model)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                return new { success = false, message = "user not found" };

            if (model.ShipmentId <= 0)
                return new { success = false, message = "Shipment id is required" };

            var shipment = await _shipmentService.GetShipmentByIdAsync(model.ShipmentId);
            if (shipment is null)
                return new { success = false, message = "Shipment not found" };

            var purchaseOrder = await _orderService.GetOrderByIdAsync(shipment.OrderId);
            if (purchaseOrder == null)
                return new { success = false, message = "Purchase order not found" };

            var purchaseOrderCalculation = await _orderService.GetOrderCalculationByOrderIdAsync(shipment.OrderId);
            if (purchaseOrderCalculation == null)
                return new { success = false, message = "Purchase order calculation not found" };

            var quotation = await _quotationService.GetQuotationByIdAsync(purchaseOrder.QuotationId.Value);
            if (quotation == null)
                return new { success = false, message = "Quotation not found" };

            var shipmentItem = (await _shipmentService.GetShipmentItemsByShipmentIdAsync(shipment.Id)).FirstOrDefault();
            if (shipmentItem == null)
                return new { success = false, message = "Shipment item not found" };

            if (model.ActualDeliveredQuantity < shipmentItem.Quantity && string.IsNullOrWhiteSpace(model.ActualDeliveredQuantityReason))
                return new { success = false, message = "Actual delivered quantity reason is required" };

            if (model.DeliveryTypeId <= 0)
                throw new Exception("Select delivery type");
            if (model.TransporterTypeId <= 0)
                throw new Exception("Select transporter type");
            if (model.DeliveryTimingId == 0)
                throw new Exception("Select delivery time");
            if (model.DeliveryTimingId == (int)DeliveryTiming.Delayed && model.DeliveryDelayedReasonId == 0)
                throw new Exception("Select delivery time reason");
            if (string.IsNullOrWhiteSpace(model.FileName))
                throw new Exception("Picture is required");

            var shipmentItems = await _shipmentService.GetShipmentItemsByShipmentIdAsync(shipment.Id);

            //sum both fileds value then check is > 0
            var deliveryCost = shipment.OnLabourCharges + model.FreightCharges + model.LabourCharges;

            if (model.DeliveryCostTypeId != (int)DeliveryCostType.FreeDelivery && model.DeliveryCostTypeId != (int)DeliveryCostType.BuyerSelfPickup && deliveryCost <= 0)
                throw new Exception("Enter delivery cost");

            if (shipment.ExpectedDeliveryCost > 0 && deliveryCost > shipment.ExpectedDeliveryCost && model.DeliveryCostReasonId == 0)
                throw new Exception("Select delivery cost reason");

            if (model.DeliveryCostTypeId == (int)DeliveryCostType.FreeDelivery || model.DeliveryCostTypeId == (int)DeliveryCostType.BuyerSelfPickup)
            {
                model.FreightCharges = 0;
                model.LabourCharges = 0;
                deliveryCost = 0;
            }

            shipment.PictureId = model.FileName != null ? await _utilityService.OrderManagement_UploadPicture(model.ImageBytes, model.FileName) : 0;
            shipment.DeliveryCostTypeId = model.DeliveryCostTypeId;
            shipment.DeliveryTypeId = model.DeliveryTypeId;
            shipment.TransporterTypeId = model.TransporterTypeId;
            shipment.DeliveryCost = deliveryCost;
            shipment.FreightCharges = model.FreightCharges;
            shipment.LabourCharges = model.LabourCharges;
            shipment.DeliveryTimingId = model.DeliveryTimingId;
            shipment.DeliveryDelayedReasonId = model.DeliveryDelayedReasonId;
            shipment.DeliveryCostReasonId = model.DeliveryCostReasonId;
            shipment.ShipmentDeliveryAddress = model.ShipmentDeliveryAddress;
            shipment.LabourTypeId = model.LabourTypeId;
            shipment.WarehouseId = model.WarehouseId;
            shipment.ActualDeliveredQuantity = model.ActualDeliveredQuantity;
            shipment.ActualDeliveredQuantityReason = model.ActualDeliveredQuantityReason;
            await _shipmentService.UpdateShipmentAsync(shipment);

            var allTransporterLedger = await _customerLedgerService.GetAllTransporterLedgerByShipmentAsync(shipment.Id, new string[] { "Frieght charges", "UnLoading charges" });
            if (allTransporterLedger.Any())
            {
                foreach (var ledger in allTransporterLedger)
                    await _customerLedgerService.DeleteCustomerLedgerAsync(ledger);
            }

            if (/*shipment.DeliveryCostType != DeliveryCostType.FulfilledBySupplier &&*/ shipment.DeliveryCostType != DeliveryCostType.FreeDelivery)
            {
                var transpoter = await _customerService.GetCustomerByIdAsync(shipment.TransporterId);
                if (transpoter.IsZarayeTransporter && shipment.LabourTypeId == (int)LabourType.Yes)
                {
                    if (shipment.FreightCharges > 0)
                        await _customerLedgerService.AddCustomerLedgerAsync(date: DateTime.UtcNow, customerId: _customerSettings.TranspoterZaraye, description: "Frieght charges", debit: 0, credit: shipment.FreightCharges, shipmentId: shipment.Id);

                    if (shipment.LabourCharges > 0)
                        await _customerLedgerService.AddCustomerLedgerAsync(date: DateTime.UtcNow, customerId: _customerSettings.TranspoterZaraye, description: "UnLoading charges", debit: 0, credit: shipment.LabourCharges, shipmentId: shipment.Id);
                }
                else if (!transpoter.IsZarayeTransporter && shipment.LabourTypeId == (int)LabourType.Yes)
                {
                    if (shipment.FreightCharges > 0)
                        await _customerLedgerService.AddCustomerLedgerAsync(date: DateTime.UtcNow, customerId: transpoter.Id, description: "Frieght charges", debit: 0, credit: shipment.FreightCharges, shipmentId: shipment.Id);
                    //await _customerLedgerService.AddCustomerLedgerAsync(date: DateTime.UtcNow, customerId: _customerSettings.ThirdPartyCustomer, description: "Frieght charges", debit: 0, credit: shipment.FreightCharges, shipmentId: shipment.Id); Align to khurram


                    if (shipment.LabourCharges > 0)
                        await _customerLedgerService.AddCustomerLedgerAsync(date: DateTime.UtcNow, customerId: _customerSettings.TranspoterZaraye, description: "UnLoading charges", debit: 0, credit: shipment.LabourCharges, shipmentId: shipment.Id);
                }
                else if (transpoter.IsZarayeTransporter && shipment.LabourTypeId == (int)LabourType.No)
                {
                    if (shipment.FreightCharges > 0)
                        await _customerLedgerService.AddCustomerLedgerAsync(date: DateTime.UtcNow, customerId: _customerSettings.TranspoterZaraye, description: "Frieght charges", debit: 0, credit: shipment.FreightCharges, shipmentId: shipment.Id);

                    if (shipment.LabourCharges > 0)
                        await _customerLedgerService.AddCustomerLedgerAsync(date: DateTime.UtcNow, customerId: _customerSettings.ThirdPartyCustomer, description: "UnLoading charges", debit: 0, credit: shipment.LabourCharges, shipmentId: shipment.Id);
                }
                else if (!transpoter.IsZarayeTransporter && shipment.LabourTypeId == (int)LabourType.No)
                {
                    if (shipment.FreightCharges > 0)
                        await _customerLedgerService.AddCustomerLedgerAsync(date: DateTime.UtcNow, customerId: _customerSettings.ThirdPartyCustomer, description: "Frieght charges", debit: 0, credit: shipment.FreightCharges, shipmentId: shipment.Id);

                    if (shipment.LabourCharges > 0)
                        await _customerLedgerService.AddCustomerLedgerAsync(date: DateTime.UtcNow, customerId: _customerSettings.ThirdPartyCustomer, description: "UnLoading charges", debit: 0, credit: shipment.LabourCharges, shipmentId: shipment.Id);
                }
            }

            //Update inventoru inbound to 'Physical'
            var inventoryInbound = _inventoryService.GetInventoryInboundByShipmentId(shipment.Id);
            if (inventoryInbound != null)
            {
                inventoryInbound.InventoryInboundStatusEnum = InventoryInboundStatusEnum.Physical;
                await _inventoryService.UpdateInventoryInboundAsync(inventoryInbound);
            }

            if (!string.IsNullOrWhiteSpace(model.ActualDeliveredQuantityReason) && model.ActualDeliveredQuantity < shipmentItem.Quantity)
            {
                shipmentItem.Quantity = model.ActualDeliveredQuantity;
                await _shipmentService.UpdateShipmentItemAsync(shipmentItem);

                //update delivery request
                var deliveryRequest = await _orderService.GetOrderDeliveryRequestByIdAsync(shipment.DeliveryRequestId.Value);
                if (deliveryRequest is not null)
                {
                    deliveryRequest.Quantity = model.ActualDeliveredQuantity;
                    await _orderService.UpdateOrderDeliveryRequestAsync(deliveryRequest);
                }
            }

            await _orderProcessingService.DeliverAsync(shipment, true);

            #region Deliver Ledger

            //Add Customer Ledger for supplier delivery
            await _customerLedgerService.AddCustomerLedgerAsync(date: shipment.DeliveryDateUtc.Value, customerId: purchaseOrder.CustomerId, description: "Delivery", debit: 0, credit: shipment.DeliveredAmount, shipmentId: shipment.Id);

            #endregion

            var calculatedPayableToMiLL = purchaseOrderCalculation.PayableToMill / quotation.Quantity * shipmentItem.Quantity;

            #region Finance Cost

            if (purchaseOrderCalculation.FinanceCost > 0)
            {
                if (purchaseOrderCalculation.FinanceCostPayment == (int)FinanceCostPaymentStatus.Cash)
                {
                    var calculatedFinanceCostValue = purchaseOrderCalculation.FinanceCost / quotation.Quantity * shipmentItem.Quantity;
                    calculatedPayableToMiLL = calculatedPayableToMiLL - calculatedFinanceCostValue;
                    await _customerLedgerService.AddCustomerLedgerAsync(date: shipment.DeliveryDateUtc.Value, customerId: purchaseOrder.CustomerId, description: "Finance Cost", debit: 0, credit: calculatedFinanceCostValue, shipmentId: shipment.Id);
                }
                else
                {
                    var calculatedFinanceCost = purchaseOrderCalculation.FinanceCost / quotation.Quantity * (purchaseOrderCalculation.GSTRate / 100 + 1) * (1 - purchaseOrderCalculation.WHTRate / 100) * (purchaseOrderCalculation.WholesaleTaxRate / 100 + 1);
                    calculatedFinanceCost = calculatedFinanceCost * shipmentItem.Quantity;
                    calculatedPayableToMiLL = calculatedPayableToMiLL - calculatedFinanceCost;
                    await _customerLedgerService.AddCustomerLedgerAsync(date: shipment.DeliveryDateUtc.Value, customerId: purchaseOrder.CustomerId, description: "Finance Cost", debit: 0, credit: calculatedFinanceCost, shipmentId: shipment.Id);
                }
            }

            #endregion

            if (purchaseOrderCalculation.BusinessModelId != (int)BusinessModelEnum.Standard)
            {
                #region Broker Cash Ledger

                if (purchaseOrderCalculation.BrokerId > 0)
                {
                    //var calculatedPayableToMiLL = (decimal)(purchaseOrderCalculation.PayableToMill / quotation.Quantity) * shipmentItem.Quantity;
                    var calculatedPaymentInCash = purchaseOrderCalculation.PaymentInCash / quotation.Quantity * shipmentItem.Quantity;
                    //New Condition
                    if (purchaseOrderCalculation.BrokerId == purchaseOrder.CustomerId)
                    {
                        //Supplier record
                        await _customerLedgerService.AddCustomerLedgerAsync(date: shipment.DeliveryDateUtc.Value, customerId: purchaseOrder.CustomerId, description: "Payable to mill", debit: 0, credit: calculatedPayableToMiLL, shipmentId: shipment.Id);
                        await _customerLedgerService.AddCustomerLedgerAsync(date: shipment.DeliveryDateUtc.Value, customerId: purchaseOrder.CustomerId, description: "Payment in cash", debit: 0, credit: calculatedPaymentInCash, shipmentId: shipment.Id);
                    }
                    else if (purchaseOrderCalculation.BrokerId != purchaseOrder.CustomerId)
                    {
                        //Supplier record
                        await _customerLedgerService.AddCustomerLedgerAsync(date: shipment.DeliveryDateUtc.Value, customerId: purchaseOrder.CustomerId, description: "Payable to mill", debit: 0, credit: calculatedPayableToMiLL, shipmentId: shipment.Id);

                        //Broker record
                        await _customerLedgerService.AddCustomerLedgerAsync(date: shipment.DeliveryDateUtc.Value, customerId: purchaseOrderCalculation.BrokerId, description: "Payment in cash", debit: 0, credit: calculatedPaymentInCash, shipmentId: shipment.Id);
                    }

                    var calculatedBrokerCash = purchaseOrderCalculation.BrokerCash * shipmentItem.Quantity;
                    if (purchaseOrderCalculation.BrokerCash > 0)
                    {
                        //Add Customer Ledger for positive broker cash
                        await _customerLedgerService.AddCustomerLedgerAsync(date: shipment.DeliveryDateUtc.Value, customerId: purchaseOrderCalculation.BrokerId, description: "Broker Cash", debit: 0, credit: calculatedBrokerCash, shipmentId: shipment.Id);
                    }
                    else if (purchaseOrderCalculation.BrokerCash < 0)
                    {
                        //Add Customer Ledger for negative broker cash
                        await _customerLedgerService.AddCustomerLedgerAsync(date: shipment.DeliveryDateUtc.Value, customerId: purchaseOrderCalculation.BrokerId, description: "Broker Cash", debit: Math.Abs(calculatedBrokerCash), credit: 0, shipmentId: shipment.Id);
                    }
                }
                else if (purchaseOrderCalculation.BrokerId == 0 && purchaseOrder.CustomerId > 0)
                {
                    //Supplier record
                    await _customerLedgerService.AddCustomerLedgerAsync(date: shipment.DeliveryDateUtc.Value, customerId: purchaseOrder.CustomerId, description: "Payable to mill", debit: 0, credit: calculatedPayableToMiLL, shipmentId: shipment.Id);
                }
                #endregion
            }
            #region Supplier Commission Receivable Ledger

            if (purchaseOrderCalculation.SupplierCommissionReceivableUserId > 0 && purchaseOrderCalculation.SupplierCommissionReceivableAmount > 0)
            {
                var calculatedSupplierCommissionReceivablePerBag = purchaseOrderCalculation.SupplierCommissionReceivableAmount * shipmentItem.Quantity;

                //Add Customer Ledger for Supplier Commission Receivable
                await _customerLedgerService.AddCustomerLedgerAsync(date: shipment.DeliveryDateUtc.Value, customerId: purchaseOrderCalculation.SupplierCommissionReceivableUserId, description: "Supplier Commission Receivable", debit: calculatedSupplierCommissionReceivablePerBag, credit: 0, shipmentId: shipment.Id);
            }

            #endregion

            #region Supplier Commission Payable Ledger

            if (purchaseOrderCalculation.SupplierCommissionPayableUserId > 0 && purchaseOrderCalculation.SupplierCommissionBag > 0)
            {
                var calculatedSupplierCommissionPayablePerBag = purchaseOrderCalculation.SupplierCommissionBag * shipmentItem.Quantity;

                //Add Customer Ledger for Supplier Commission Payable
                await _customerLedgerService.AddCustomerLedgerAsync(date: shipment.DeliveryDateUtc.Value, customerId: purchaseOrderCalculation.SupplierCommissionPayableUserId, description: "Supplier Commission Payable", debit: 0, credit: calculatedSupplierCommissionPayablePerBag, shipmentId: shipment.Id);
            }

            #endregion

            //Add Customer Ledger for supplier delivery
            //await _customerLedgerService.AddCustomerLedgerAsync(date: shipment.DeliveryDateUtc.Value, customerId: purchaseOrder.CustomerId, description: "Delivery", debit: 0, credit: shipment.DeliveredAmount, shipmentId: shipment.Id);

            //Add customer ledger for delivery cost on supplier
            if (shipment.DeliveryCostType == DeliveryCostType.FulfilledBySupplier && shipment.DeliveryCost > 0)
                await _customerLedgerService.AddCustomerLedgerAsync(date: shipment.DeliveryDateUtc.Value, customerId: purchaseOrder.CustomerId, description: "Delivery cost on supplier", debit: 0, credit: shipment.DeliveryCost, shipmentId: shipment.Id, updateRecord: true);

            return new { success = true, message = await _localizationService.GetResourceAsync("Mark.as.delivered.success") };
        }

        public async Task<object> GetAllTransporters()
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                return new { success = false, message = "User not found" };

            var transporters = await _customerService.GetAllCustomersAsync(customerRoleIds: new int[] { (await _customerService.GetCustomerRoleBySystemNameAsync(ZarayeCustomerDefaults.TransporterRoleName)).Id });

            var result = new List<SelectListItem>();
            //clone the list to ensure that "selected" property is not set
            foreach (var item in transporters)
            {
                result.Add(new SelectListItem
                {
                    Text = item.FullName,
                    Value = item.Id.ToString()
                });
            }

            return new { success = true, data = result };
        }

        public async Task<object> GetVehiclesByTransporterId(int transporterId)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                return new { success = false, message = "User not found" };

            var transporter = await _customerService.GetCustomerByIdAsync(transporterId);
            if (transporter is null)
                return new { success = false, message = "Transporter not found" };

            var result = new List<SelectListItem>();
            var TransportVehiclelist = (await _customerService.GetTransporterVehicleByTransporterIdAsync(Convert.ToInt32(transporter.Id))).ToList();

            //clone the list to ensure that "selected" property is not set
            foreach (var item in TransportVehiclelist)
            {
                result.Add(new SelectListItem
                {
                    Text = $"{_customerService.GetVehiclePortfolioByIdAsync(item.VehicleId).Result.Name}",
                    Value = item.Id.ToString()
                });
            }
            return new { success = true, data = result };
        }
        #endregion
    }
}
