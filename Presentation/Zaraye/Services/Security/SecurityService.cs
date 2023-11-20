using Zaraye.Core.Domain.Customers;
using Zaraye.Core.Domain.Localization;
using Zaraye.Core.Domain;
using Zaraye.Core.Events;
using Zaraye.Core;
using Zaraye.Services.Catalog;
using Zaraye.Services.Common;
using Zaraye.Services.Customers;
using Zaraye.Services.Helpers;
using Zaraye.Services.Localization;
using Zaraye.Services.Logging;
using Zaraye.Services.Messages;
using Zaraye.Services.Authentication;
using Zaraye.Core.Domain.Common;
using Zaraye.Models.Api.V4.Security;
using Zaraye.Models.Api.V4.Buyer;

namespace Zaraye.Services.Security
{
    public class SecurityService : ISecurityService
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
        private readonly IConfiguration _config;
        private readonly IAuthenticationService _authenticationService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IAddressService _addressService;
        private readonly LocalizationSettings _localizationSettings;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IPriceFormatter _priceFormatter;
        private readonly StoreInformationSettings _storeInformationSettings;
        private readonly IPermissionService _permissionService;

        #endregion

        #region Ctor

        public SecurityService(ICustomerRegistrationService customerRegistrationService,
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
            StoreInformationSettings storeInformationSettings,
            IPermissionService permissionService)
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
            this._config = config;
            _authenticationService = authenticationService;
            _eventPublisher = eventPublisher;
            _addressService = addressService;
            _localizationSettings = localizationSettings;
            _productService = productService;
            _categoryService = categoryService;
            _dateTimeHelper = dateTimeHelper;
            _priceFormatter = priceFormatter;
            _storeInformationSettings = storeInformationSettings;
            _permissionService = permissionService;
        }

        #endregion
        #region Phone Authentication / Login / Register

        public async Task<string> PhoneNumberAuthenticationOtp(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ApplicationException("Phone number is required");

            var user = await _customerService.GetCustomerByUsernameAsync(phoneNumber);
            if (user != null)
                throw new ApplicationException("Phone number already exist");

            var customer = await _workContext.GetCurrentCustomerAsync();

            //save 4 digit otp in customer generic attribute
            var otp = "0000";//CommonHelper.GenerateRandomDigitCode(4);
            await _genericAttributeService.SaveAttributeAsync(customer, ZarayeCustomerDefaults.CustomerPhoneNumberOtp, otp);

            return await _localizationService.GetResourceAsync("Account.Customer.Otpgenerated.successfully") + "," + otp;
        }

        public async Task<object> Login(AccountApiModel.LoginApiModel model)
        {
            var usernameOrEmail = string.IsNullOrWhiteSpace(model.Username) ? model.Email : model.Username;
            var loginResult = await _customerRegistrationService.ValidateCustomerAsync(usernameOrEmail, model.Password);
            switch (loginResult)
            {
                case CustomerLoginResults.Successful:
                    {
                        var customer = await _customerService.GetCustomerByEmailAsync(usernameOrEmail);
                        if (customer == null)
                            customer = await _customerService.GetCustomerByUsernameAsync(usernameOrEmail);

                        if (!await _customerService.IsBuyerAsync(customer))
                            throw new ApplicationException("Access denied");

                        var jwt = new JwtService(_config);
                        var token = jwt.GenerateSecurityToken(customer.Email, customer.Id);

                        await _customerRegistrationService.SignInCustomerAsync(customer, "", model.RememberMe);
                        return new
                        {
                            success = true,
                            message = "",
                            userId = customer.Id,
                            userType = (await _customerService.IsBuyerAsync(customer)) ? "Buyer" : "Supplier",
                            token = token
                        };
                       
                    }
                case CustomerLoginResults.CustomerNotExist:
                    throw new ApplicationException(await _localizationService.GetResourceAsync("Account.Login.WrongCredentials.CustomerNotExist"));
                case CustomerLoginResults.Deleted:
                    throw new ApplicationException(await _localizationService.GetResourceAsync("Account.Login.WrongCredentials.Deleted"));
                case CustomerLoginResults.NotActive:
                    throw new ApplicationException(await _localizationService.GetResourceAsync("Account.Login.WrongCredentials.NotActive"));
                case CustomerLoginResults.NotRegistered:
                    throw new ApplicationException(await _localizationService.GetResourceAsync("Account.Login.WrongCredentials.NotRegistered"));
                case CustomerLoginResults.LockedOut:
                    throw new ApplicationException(await _localizationService.GetResourceAsync("Account.Login.WrongCredentials.LockedOut"));
                case CustomerLoginResults.WrongPassword:
                default:
                    throw new ApplicationException(await _localizationService.GetResourceAsync("Account.Login.WrongCredentials"));
            }
        }

        public async Task<object> Tijara_Register(AccountApiModel.RegisterApiModel model)
        {
            //check whether registration is allowed
            if (_customerSettings.UserRegistrationType == UserRegistrationType.Disabled)
                throw new ApplicationException(await _localizationService.GetResourceAsync("Account.Register.Result.Disabled"));

            if (!CommonHelper.IsValidEmail(model.Email))
                throw new ApplicationException(await _localizationService.GetResourceAsync("Common.WrongEmail"));

            if (string.IsNullOrWhiteSpace(model.Phone))
                throw new ApplicationException(await _localizationService.GetResourceAsync("Account.Fields.Username.Required"));

            var customer = new Customer
            {
                Active = false,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
                RegisteredInStoreId = (await _storeContext.GetCurrentStoreAsync()).Id,
                Source = "APP"
            };
            await _customerService.InsertCustomerAsync(customer);

            var customerEmail = model.Email?.Trim();
            var customerUserName = model.Phone?.Trim();
            model.Company = "Zaraye";

            //var isApproved = _customerSettings.UserRegistrationType == UserRegistrationType.Standard;
            var registrationRequest = new CustomerRegistrationRequest(customer,
                customerEmail,
                customerUserName,
                model.Password,
                _customerSettings.DefaultPasswordFormat,
                (await _storeContext.GetCurrentStoreAsync()).Id, false);

            var registrationResult = await _customerRegistrationService.RegisterCustomerAsync(registrationRequest);
            if (registrationResult.Success)
            {
                //form fields
                if (_customerSettings.FirstNameEnabled)
                    customer.FirstName = model.FirstName;
                if (_customerSettings.LastNameEnabled)
                    customer.LastName = model.LastName;
                customer.Company = model.Company;
                if (!string.IsNullOrWhiteSpace(model.Address))
                    customer.StreetAddress = model.Address;
                if (_customerSettings.CountryEnabled)
                    customer.CountryId = model.CountryId;
                if (_customerSettings.CountryEnabled && _customerSettings.StateProvinceEnabled)
                    customer.StateProvinceId = model.StateId;
                if (_customerSettings.PhoneEnabled)
                    customer.Phone = model.Phone;
                customer.Username = model.Phone;

                //insert default address (if possible)
                var defaultAddress = new Address
                {
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    Email = customer.Email,
                    Company = customer.Company,
                    CountryId = customer.CountryId > 0
                                           ? (int?)customer.CountryId
                                           : null,
                    StateProvinceId = customer.StateProvinceId > 0
                                           ? (int?)customer.StateProvinceId
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

                //Add customer roles
                foreach (var role in model.Roles)
                {
                    var customerRole = await _customerService.GetCustomerRoleBySystemNameAsync(role);
                    if (customerRole != null)
                        await _customerService.AddCustomerRoleMappingAsync(new CustomerCustomerRoleMapping { CustomerId = customer.Id, CustomerRoleId = customerRole.Id });
                }

                //notifications
                if (_customerSettings.NotifyNewCustomerRegistration)
                    await _workflowMessageService.SendBuyerRegisteredNotificationMessageAsync(customer,
                        _localizationSettings.DefaultAdminLanguageId);

                //raise event       
                await _eventPublisher.PublishAsync(new CustomerRegisteredEvent(customer));

                switch (_customerSettings.UserRegistrationType)
                {
                    case UserRegistrationType.EmailValidation:
                        //email validation message
                        await _genericAttributeService.SaveAttributeAsync(customer, ZarayeCustomerDefaults.AccountActivationTokenAttribute, Guid.NewGuid().ToString());
                        await _workflowMessageService.SendCustomerEmailValidationMessageAsync(customer, (await _workContext.GetWorkingLanguageAsync()).Id);

                        //result
                        return await _localizationService.GetResourceAsync("Account.Register.Result.EmailValidation");

                    case UserRegistrationType.AdminApproval:
                        return await _localizationService.GetResourceAsync("Account.Register.Result.AdminApproval");

                    case UserRegistrationType.Standard:
                        //send customer welcome message
                        await _workflowMessageService.SendCustomerWelcomeMessageAsync(customer, (await _workContext.GetWorkingLanguageAsync()).Id);

                        //raise event       
                        await _eventPublisher.PublishAsync(new CustomerActivatedEvent(customer));

                        await _customerRegistrationService.SignInCustomerAsync(customer, null, true);

                        return await _localizationService.GetResourceAsync("Account.Register.Result.Standard");

                    default:
                        throw new ApplicationException("");
                }
            }

            throw new ApplicationException(string.Join(",", registrationResult.Errors));
        }

        public async Task<string> Consumer_Register(AccountApiModel.RegisterApiModel model)
        {
            //check whether registration is allowed
            if (_customerSettings.UserRegistrationType == UserRegistrationType.Disabled)
                throw new ApplicationException(await _localizationService.GetResourceAsync("Account.Register.Result.Disabled"));

            if (!CommonHelper.IsValidEmail(model.Email))
                throw new ApplicationException(await _localizationService.GetResourceAsync("Common.WrongEmail"));

            if (string.IsNullOrWhiteSpace(model.Phone))
                throw new ApplicationException(await _localizationService.GetResourceAsync("Account.Fields.Username.Required"));

            var customer = new Customer
            {
                Active = true,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
                RegisteredInStoreId = (await _storeContext.GetCurrentStoreAsync()).Id,
                Source = "APP"
            };
            await _customerService.InsertCustomerAsync(customer);

            var customerEmail = model.Email?.Trim();
            var customerUserName = model.Phone?.Trim();

            //var isApproved = _customerSettings.UserRegistrationType == UserRegistrationType.Standard;
            var registrationRequest = new CustomerRegistrationRequest(customer,
                customerEmail,
                customerUserName,
                model.Password,
                _customerSettings.DefaultPasswordFormat,
                (await _storeContext.GetCurrentStoreAsync()).Id);

            var registrationResult = await _customerRegistrationService.RegisterCustomerAsync(registrationRequest);
            if (registrationResult.Success)
            {
                //form fields
                if (_customerSettings.FirstNameEnabled)
                    customer.FirstName = model.FirstName;
                if (_customerSettings.LastNameEnabled)
                    customer.LastName = model.LastName;
                customer.Company = model.Company;
                if (!string.IsNullOrWhiteSpace(model.Address))
                    customer.StreetAddress = model.Address;
                if (_customerSettings.CountryEnabled)
                    customer.CountryId = model.CountryId;
                if (_customerSettings.CountryEnabled && _customerSettings.StateProvinceEnabled)
                    customer.StateProvinceId = model.StateId;
                if (_customerSettings.PhoneEnabled)
                    customer.Phone = model.Phone;
                customer.Username = model.Phone;

                //insert default address (if possible)
                var defaultAddress = new Address
                {
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    Email = customer.Email,
                    Company = customer.Company,
                    CountryId = customer.CountryId > 0
                                           ? (int?)customer.CountryId
                                           : null,
                    StateProvinceId = customer.StateProvinceId > 0
                                           ? (int?)customer.StateProvinceId
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

                await _customerService.UpdateCustomerAsync(customer);

                //Save pin location
                if (model.BuyerPinLocation != null)
                {
                    await _genericAttributeService.SaveAttributeAsync(customer, ZarayeCustomerDefaults.RegisteredPinLocationAttribute, $"{model.BuyerPinLocation.Latitude},{model.BuyerPinLocation.Longitude}");
                    await _genericAttributeService.SaveAttributeAsync(customer, ZarayeCustomerDefaults.RegisteredPinLocationAddressAttribute, model.BuyerPinLocation.Location);
                }


                ////Add customer roles
                var customerRole = await _customerService.GetCustomerRoleBySystemNameAsync(ZarayeCustomerDefaults.BuyerRoleName);
                if (customerRole == null)
                    registrationResult.AddError("'Supplier' role could not be loaded");

                await _customerService.AddCustomerRoleMappingAsync(new CustomerCustomerRoleMapping { CustomerId = customer.Id, CustomerRoleId = customerRole.Id });

                //notifications
                if (_customerSettings.NotifyNewCustomerRegistration)
                    await _workflowMessageService.SendBuyerRegisteredNotificationMessageAsync(customer,
                        _localizationSettings.DefaultAdminLanguageId);

                //raise event       
                await _eventPublisher.PublishAsync(new CustomerRegisteredEvent(customer));

                switch (_customerSettings.UserRegistrationType)
                {
                    case UserRegistrationType.EmailValidation:
                        //email validation message
                        await _genericAttributeService.SaveAttributeAsync(customer, ZarayeCustomerDefaults.AccountActivationTokenAttribute, Guid.NewGuid().ToString());
                        await _workflowMessageService.SendCustomerEmailValidationMessageAsync(customer, (await _workContext.GetWorkingLanguageAsync()).Id);

                        //result
                        return await _localizationService.GetResourceAsync("Account.Register.Result.EmailValidation");

                    case UserRegistrationType.AdminApproval:
                        return await _localizationService.GetResourceAsync("Account.Register.Result.AdminApproval");

                    case UserRegistrationType.Standard:
                        //send customer welcome message
                        await _workflowMessageService.SendCustomerWelcomeMessageAsync(customer, (await _workContext.GetWorkingLanguageAsync()).Id);

                        //raise event       
                        await _eventPublisher.PublishAsync(new CustomerActivatedEvent(customer));

                        await _customerRegistrationService.SignInCustomerAsync(customer, null, true);

                        return await _localizationService.GetResourceAsync("Account.Register.Result.Standard");

                    default:
                        throw new ApplicationException("");
                }
            }

            throw new ApplicationException(string.Join(",", registrationResult.Errors));
        }

        #endregion

        #region Other Methods

        public async Task<string> ChangePassword(AccountApiModel.ChangePasswordApiModel model)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                throw new ApplicationException("Not not registered");

            var changePasswordRequest = new ChangePasswordRequest(user.Email,
                    true, _customerSettings.DefaultPasswordFormat, model.NewPassword, model.OldPassword);
            var changePasswordResult = await _customerRegistrationService.ChangePasswordAsync(changePasswordRequest);
            if (changePasswordResult.Success)
                return await _localizationService.GetResourceAsync("Account.ChangePassword.Success");

            throw new ApplicationException(string.Join(",", changePasswordResult.Errors));
        }

        public async Task<string> PasswordRecoverySend(AccountApiModel.RequestPasswordApiModel model)
        {
            var customer = await _customerService.GetCustomerByEmailAsync(model.Email.Trim());
            if (customer != null && customer.Active && !customer.Deleted)
            {
                //save token and current date
                var passwordRecoveryToken = Guid.NewGuid();
                await _genericAttributeService.SaveAttributeAsync(customer, ZarayeCustomerDefaults.PasswordRecoveryTokenAttribute,
                    passwordRecoveryToken.ToString());
                DateTime? generatedDateTime = DateTime.UtcNow;
                await _genericAttributeService.SaveAttributeAsync(customer,
                    ZarayeCustomerDefaults.PasswordRecoveryTokenDateGeneratedAttribute, generatedDateTime);

                //send email
                await _workflowMessageService.SendCustomerPasswordRecoveryMessageAsync(customer, (await _workContext.GetWorkingLanguageAsync()).Id);

                return await _localizationService.GetResourceAsync("Account.PasswordRecovery.EmailHasBeenSent");
            }
            else
            {
                throw new ApplicationException(await _localizationService.GetResourceAsync("Account.PasswordRecovery.EmailNotFound"));
            }
        }

        public async Task<string> AccountDeactivate(string comment)
        {
            var user = await _workContext.GetCurrentCustomerAsync();
            if (!await _customerService.IsRegisteredAsync(user))
                throw new ApplicationException("Not not registered");
            user.Active = false;
            await _genericAttributeService.SaveAttributeAsync(user, ZarayeCustomerDefaults.AccountDeActivateCommentAttribute, comment);
            await _customerService.UpdateCustomerAsync(user);

            return await _localizationService.GetResourceAsync("Account.Customer.Deactivated.Success");
        }

        #endregion

        #region Booker Login

        public async Task<object> BookerLogin(AccountApiModel.LoginApiModel model)
        {
            //var transactions = _dbContext.QueryFromSql<PrePaidWillTransactionDetail>($"Select wt.Id,wt.OrderId,wt.CustomerId as ClientId,wt.JointCustomerId as JointClientId,wt.AdviserId,CONCAT(o.RandomNumber, '/' + CONVERT(VARCHAR(50),o.Id)) as OrderNumber,c.ClientFullName as Client,jc.ClientFullName as JointClient,ad.ClientFullName as Adviser, wt.CreatedOnUtc,iw.AdviserPrePaidLinkSendDate as LinkSendDate From PrePaidWillTransaction wt LEFT JOIN [Order] o ON wt.OrderId = o.Id LEFT JOIN [Customer] c ON wt.CustomerId = c.Id LEFT JOIN [Customer] jc ON wt.JointCustomerId = jc.Id LEFT JOIN [Customer] ad ON wt.AdviserId = ad.Id LEFT JOIN [IntroducerWill] iw ON wt.Id = iw.TransactionId where wt.ParentId ={transaction.Id} union all select top (select debit-(select count(*) from PrePaidWillTransaction where ParentId={transaction.Id}) from PrePaidWillTransaction where id={transaction.Id}) 0 as Id,0 as OrderId,0 as ClientId,0 as JointClientId,0 as AdviserId,'' as OrderNumber,'' as Client,'' as JointClient,'' as Adviser,null as CreatedOnUtc,null as LinkSendDate From sys.columns AS a CROSS JOIN sys.columns AS b").ToList();
            var usernameOrEmail = string.IsNullOrWhiteSpace(model.Username) ? model.Email : model.Username;
            var loginResult = await _customerRegistrationService.ValidateCustomerAsync(usernameOrEmail, model.Password);
            switch (loginResult)
            {
                case CustomerLoginResults.Successful:
                    {
                        var customer = await _customerService.GetCustomerByEmailAsync(usernameOrEmail);
                        if (customer == null)
                            customer = await _customerService.GetCustomerByUsernameAsync(usernameOrEmail);

                        if (!await _customerService.IsBookerAsync(customer))
                            throw new ApplicationException(await _localizationService.GetResourceAsync("Account.Login.WrongCredentials.CustomerNotExist"));

                        var jwt = new JwtService(_config);
                        var token = jwt.GenerateSecurityToken(customer.Email, customer.Id);

                        await _customerRegistrationService.SignInCustomerAsync(customer, "", model.RememberMe);
                        return new
                        {
                            success = true,
                            message = "",
                            buyerId = customer.Id,
                            token = token
                        };
                    }
                case CustomerLoginResults.CustomerNotExist:
                    throw new ApplicationException(await _localizationService.GetResourceAsync("Account.Login.WrongCredentials.CustomerNotExist"));
                case CustomerLoginResults.Deleted:
                    throw new ApplicationException(await _localizationService.GetResourceAsync("Account.Login.WrongCredentials.Deleted"));
                case CustomerLoginResults.NotActive:
                    throw new ApplicationException(await _localizationService.GetResourceAsync("Account.Login.WrongCredentials.NotActive"));
                case CustomerLoginResults.NotRegistered:
                    throw new ApplicationException(await _localizationService.GetResourceAsync("Account.Login.WrongCredentials.NotRegistered"));
                case CustomerLoginResults.LockedOut:
                    throw new ApplicationException(await _localizationService.GetResourceAsync("Account.Login.WrongCredentials.LockedOut"));
                case CustomerLoginResults.WrongPassword:
                default:
                    throw new ApplicationException(await _localizationService.GetResourceAsync("Account.Login.WrongCredentials"));
            }
        }

        #endregion

        #region Tijara Login

        public async Task<object> TijaraLogin(AccountApiModel.LoginApiModel model)
        {
            //var transactions = _dbContext.QueryFromSql<PrePaidWillTransactionDetail>($"Select wt.Id,wt.OrderId,wt.CustomerId as ClientId,wt.JointCustomerId as JointClientId,wt.AdviserId,CONCAT(o.RandomNumber, '/' + CONVERT(VARCHAR(50),o.Id)) as OrderNumber,c.ClientFullName as Client,jc.ClientFullName as JointClient,ad.ClientFullName as Adviser, wt.CreatedOnUtc,iw.AdviserPrePaidLinkSendDate as LinkSendDate From PrePaidWillTransaction wt LEFT JOIN [Order] o ON wt.OrderId = o.Id LEFT JOIN [Customer] c ON wt.CustomerId = c.Id LEFT JOIN [Customer] jc ON wt.JointCustomerId = jc.Id LEFT JOIN [Customer] ad ON wt.AdviserId = ad.Id LEFT JOIN [IntroducerWill] iw ON wt.Id = iw.TransactionId where wt.ParentId ={transaction.Id} union all select top (select debit-(select count(*) from PrePaidWillTransaction where ParentId={transaction.Id}) from PrePaidWillTransaction where id={transaction.Id}) 0 as Id,0 as OrderId,0 as ClientId,0 as JointClientId,0 as AdviserId,'' as OrderNumber,'' as Client,'' as JointClient,'' as Adviser,null as CreatedOnUtc,null as LinkSendDate From sys.columns AS a CROSS JOIN sys.columns AS b").ToList();

            var usernameOrEmail = string.IsNullOrWhiteSpace(model.Username) ? model.Email : model.Username;
            var loginResult = await _customerRegistrationService.ValidateCustomerAsync(usernameOrEmail, model.Password);
            switch (loginResult)
            {
                case CustomerLoginResults.Successful:
                    {
                        var user = await _customerService.GetCustomerByEmailAsync(usernameOrEmail);
                        if (user == null)
                            user = await _customerService.GetCustomerByUsernameAsync(usernameOrEmail);

                        //if (!await _customerService.IsDemandAsync(user)
                        //    && !await _customerService.IsSupplyAsync(user)
                        //    && !await _customerService.IsOperationsAsync(user)
                        //    && !await _customerService.IsFinanceAsync(user)
                        //    && !await _customerService.IsBusinessHeadAsync(user)
                        //    && !await _customerService.IsFinanceHeadAsync(user)
                        //    && !await _customerService.IsOpsHeadAsync(user))
                        //    return Ok(new { success = false, message = await _localizationService.GetResourceAsync("Account.Login.Tijara.NotAllowedToAccess") });

                        if (!await _customerService.IsDemandAsync(user)
                            && !await _customerService.IsDemandAssociateAsync(user)
                            && !await _customerService.IsSupplyAsync(user)
                            && !await _customerService.IsSupplyAssociateAsync(user)
                            && !await _customerService.IsGroundOperationsAsync(user)
                            && !await _customerService.IsOPerationAssociateAsync(user)
                            && !await _customerService.IsOPerationLeadAsync(user)
                            && !await _customerService.IsBusinessLeadAsync(user)
                            && !await _customerService.IsLiveOpsAsync(user)
                            && !await _customerService.IsControlTowerAsync(user))
                            throw new ApplicationException(await _localizationService.GetResourceAsync("Account.Login.Tijara.NotAllowedToAccess"));

                        //if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTijara, user))
                        //    return Ok(new { success = false, message = await _localizationService.GetResourceAsync("Account.Login.Tijara.NotAllowedToAccess") });

                        var jwt = new JwtService(_config);
                        var token = jwt.GenerateSecurityToken(user.Email, user.Id);

                        await _customerRegistrationService.SignInCustomerAsync(user, "", model.RememberMe);
                        return new
                        {
                            success = true,
                            message = "",
                            userId = user.Id,
                            token = token,
                            info = new
                            {
                                Id = user.Id,
                                Email = user.Email,
                                Phone = user.Username,
                                FirstName = user.FirstName,
                                LastName = user.LastName,
                                CompanyName = user.Company,
                                CountryId = user.CountryId,
                                StateId = user.StateProvinceId,
                                Address = user.StreetAddress,
                                City = user.City,
                                BookerType = user.BookerType,
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

                                    ManagePlaceRequest = await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlaceRequest),
                                    ManagePlaceOrder = await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlaceOrder),
                                    ManagePlaceDeliveryRequest = await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlaceDeliveryRequest),
                                    ManagePlaceSalesReturnRequest = await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlaceSalesReturnRequest),

                                    //Supply

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
                    }
                case CustomerLoginResults.CustomerNotExist:
                    throw new ApplicationException(await _localizationService.GetResourceAsync("Account.Login.WrongCredentials.CustomerNotExist"));
                case CustomerLoginResults.Deleted:
                    throw new ApplicationException(await _localizationService.GetResourceAsync("Account.Login.WrongCredentials.Deleted"));
                case CustomerLoginResults.NotActive:
                    throw new ApplicationException(await _localizationService.GetResourceAsync("Account.Login.WrongCredentials.NotActive"));
                case CustomerLoginResults.NotRegistered:
                    throw new ApplicationException(await _localizationService.GetResourceAsync("Account.Login.WrongCredentials.NotRegistered"));
                case CustomerLoginResults.LockedOut:
                    throw new ApplicationException(await _localizationService.GetResourceAsync("Account.Login.WrongCredentials.LockedOut"));
                case CustomerLoginResults.WrongPassword:
                default:
                    throw new ApplicationException(await _localizationService.GetResourceAsync("Account.Login.WrongCredentials"));
            }
        }

        #endregion

        #region Buyer Contract Authorization

        public async Task<string> BuyerContractSendOTP(string email)
        {

            var buyer = await _customerService.GetCustomerByEmailAsync(email);
            if (buyer is null || !await _customerService.IsRegisteredAsync(buyer))
                throw new ApplicationException("User not found");

            if (!await _customerService.IsBuyerAsync(buyer))
                throw new ApplicationException("Buyer not found");

            var generateOtp = CommonHelper.GenerateRandomDigitCode(6);
            await _workflowMessageService.SendBuyerOTPNotificationMessageAsync(buyer, (await _workContext.GetWorkingLanguageAsync()).Id, generateOtp);

            return generateOtp;
        }

        public async Task<string> BuyerContractVerifyOTP(string email, string otp)
        {
            var buyer = await _customerService.GetCustomerByEmailAsync(email);
            if (buyer is null || !await _customerService.IsRegisteredAsync(buyer))
                throw new ApplicationException("User not found");

            if (!await _customerService.IsBuyerAsync(buyer))
                throw new ApplicationException("Buyer not found");

            var savedOtp = await _genericAttributeService.GetAttributeAsync<string>(buyer, ZarayeCustomerDefaults.BuyerOtpVerification);
            if (string.IsNullOrWhiteSpace(otp) || otp != savedOtp)
                throw new ApplicationException("Invalid OTP");

            return "Valid Otp";
        }

        public async Task<string> ChangeBuyerPassword(ChangeBuyerPasswordApiModel model)
        {
            var buyer = await _customerService.GetCustomerByEmailAsync(model.Email);
            if (buyer is null || !await _customerService.IsRegisteredAsync(buyer))
                throw new ApplicationException("User not found");

            if (!await _customerService.IsBuyerAsync(buyer))
                throw new ApplicationException("Buyer not found");

            if (string.IsNullOrWhiteSpace(model.NewPassword) || string.IsNullOrWhiteSpace(model.ConfirmPassword))
                throw new ApplicationException("Passwords not mathed");

            if (model.NewPassword != model.ConfirmPassword)
                throw new ApplicationException("Buyer not found");

            var response = await _customerRegistrationService.ChangePasswordAsync(new ChangePasswordRequest(buyer.Email, false, _customerSettings.DefaultPasswordFormat, model.NewPassword));
            if (response.Success)
                return await _localizationService.GetResourceAsync("Account.ChangePassword.Success");

            throw new ApplicationException(string.Join(",", response.Errors));
        }

        #endregion

        #region Supplier
        public async Task<string> SupplierRegisterationAsync(RegisterSupplierRequest registerSupplierRequest)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            //check whether registration is allowed
            if (_customerSettings.UserRegistrationType == UserRegistrationType.Disabled)
                throw new ApplicationException(await _localizationService.GetResourceAsync("Account.Register.Result.Disabled"));
            var existingCustomer = await _customerService.GetCustomerByEmailAsync(registerSupplierRequest.Email);
            if (existingCustomer != null)
                throw new ApplicationException("Email already exist.");

            var randomGeneratedPassword = CommonHelper.GenerateRandomDigitCode(8);
            var customerEmail = registerSupplierRequest.Email?.Trim();
            var customerUserName = registerSupplierRequest.PhoneNumber?.Trim();
            var registrationRequest = new CustomerRegistrationRequest(customer,
            customerEmail,
                customerUserName,
                randomGeneratedPassword,
                _customerSettings.DefaultPasswordFormat,
                (await _storeContext.GetCurrentStoreAsync()).Id,
                true,
                isSupplier: true);

            var registrationResult = await _customerRegistrationService.RegisterCustomerAsync(registrationRequest);
            if (!registrationResult.Success)
                throw new ApplicationException(string.Join(",", registrationResult.Errors));
            else
            {

                customer.Phone = registerSupplierRequest.PhoneNumber;
                customer.Company = registerSupplierRequest.CompanyName;
                customer.CountryId = registerSupplierRequest.CountryId;
                customer.StateProvinceId = registerSupplierRequest.CityId;
                //custom working for name
                var spaceExist = registerSupplierRequest.Name.Any(x => Char.IsWhiteSpace(x));
                if (spaceExist)
                {
                    var splittedValue = registerSupplierRequest.Name.Split(' ', 2);
                    customer.FirstName = splittedValue[0];
                    customer.LastName = splittedValue[1];
                }
                else
                {
                    customer.FirstName = registerSupplierRequest.Name;
                    customer.LastName = "";
                }
                await _customerService.UpdateCustomerAsync(customer);
            }

            return "Supplier register successfully";

        }
        #endregion
        public async Task<bool> Logout()
        {
            //activity log
            await _customerActivityService.InsertActivityAsync(await _workContext.GetCurrentCustomerAsync(), "PublicStore.Logout",
                await _localizationService.GetResourceAsync("ActivityLog.PublicStore.Logout"), await _workContext.GetCurrentCustomerAsync());

            //standard logout 
            await _authenticationService.SignOutAsync();

            //raise logged out event       
            await _eventPublisher.PublishAsync(new CustomerLoggedOutEvent(await _workContext.GetCurrentCustomerAsync()));

            return true;
        }
    }
}
