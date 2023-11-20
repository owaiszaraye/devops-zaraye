using Zaraye.Core;
using Zaraye.Core.Caching;
using Zaraye.Core.Domain.Blogs;
using Zaraye.Core.Domain.Catalog;
using Zaraye.Core.Domain.Common;
using Zaraye.Core.Domain.Customers;
using Zaraye.Core.Domain.Forums;
using Zaraye.Core.Domain.News;
using Zaraye.Core.Domain.Orders;
using Zaraye.Core.Domain.Shipping;
using Zaraye.Core.Domain.Tax;
using Zaraye.Core.Infrastructure;
using Zaraye.Data;
using Zaraye.Services.Common;
using Zaraye.Services.Localization;
using StackExchange.Profiling.Internal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace Zaraye.Services.Customers
{
    /// <summary>
    /// Customer service
    /// </summary>
    public partial class CustomerService : ICustomerService
    {
        #region Fields

        private readonly CustomerSettings _customerSettings;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IZarayeDataProvider _dataProvider;
        private readonly IRepository<Address> _customerAddressRepository;
        private readonly IRepository<BlogComment> _blogCommentRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<CustomerAddressMapping> _customerAddressMappingRepository;
        private readonly IRepository<CustomerCustomerRoleMapping> _customerCustomerRoleMappingRepository;
        private readonly IRepository<CustomerPassword> _customerPasswordRepository;
        private readonly IRepository<CustomerRole> _customerRoleRepository;
        private readonly IRepository<ForumPost> _forumPostRepository;
        private readonly IRepository<ForumTopic> _forumTopicRepository;
        private readonly IRepository<GenericAttribute> _gaRepository;
        private readonly IRepository<NewsComment> _newsCommentRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<ProductReview> _productReviewRepository;
        private readonly IRepository<ProductReviewHelpfulness> _productReviewHelpfulnessRepository;
        private readonly IRepository<ShoppingCartItem> _shoppingCartRepository;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IStoreContext _storeContext;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly IRepository<UserType> _userTypeRepository;
        private readonly IRepository<VehiclePortfolio> _vehiclePortfolioRepository;
        private readonly IRepository<CustomerIndustryMapping> _customerIndustryMappingRepository;
        private readonly IRepository<Industry> _industryRepository;
        private readonly IRepository<TransporterVehicleMapping> _transporterVehicleMappingRepository;
        private readonly IRepository<TransporterCost> _transpoterCostRepository;
        private readonly IRepository<BankDetail> _bankDetailRepository;
        private readonly IRepository<OnlineLead> _onlineLeadRepository;
        private readonly IRepository<AppliedCreditCustomer> _appliedCreditCustomerRepository;
        private readonly IRepository<OnlineLeadRejectReason> _rejectedReasonRepository;
        private readonly IRepository<BankStatement> _bankStatementRepository;
        #endregion

        #region Ctor

        public CustomerService(CustomerSettings customerSettings,
            IGenericAttributeService genericAttributeService,
            IZarayeDataProvider dataProvider,
            IRepository<Address> customerAddressRepository,
            IRepository<BlogComment> blogCommentRepository,
            IRepository<Customer> customerRepository,
            IRepository<CustomerAddressMapping> customerAddressMappingRepository,
            IRepository<CustomerCustomerRoleMapping> customerCustomerRoleMappingRepository,
            IRepository<CustomerPassword> customerPasswordRepository,
            IRepository<CustomerRole> customerRoleRepository,
            IRepository<ForumPost> forumPostRepository,
            IRepository<ForumTopic> forumTopicRepository,
            IRepository<GenericAttribute> gaRepository,
            IRepository<NewsComment> newsCommentRepository,
            IRepository<Order> orderRepository,
            IRepository<ProductReview> productReviewRepository,
            IRepository<ProductReviewHelpfulness> productReviewHelpfulnessRepository,
            IRepository<ShoppingCartItem> shoppingCartRepository,
            IStaticCacheManager staticCacheManager,
            IStoreContext storeContext,
            ShoppingCartSettings shoppingCartSettings,
            IRepository<UserType> userTypeRepository,
            IRepository<VehiclePortfolio> vehiclePortfolioRepository,
            IRepository<CustomerIndustryMapping> customerIndustryMappingRepository,
            IRepository<Industry> industryRepository,
            IRepository<TransporterVehicleMapping> transporterVehicleMappingRepository,
            IRepository<TransporterCost> transpoterCostRepository,
            IRepository<BankDetail> bankDetailRepository,
            IRepository<OnlineLead> onlineLeadRepository,
            IRepository<AppliedCreditCustomer> appliedCreditCustomerRepository,
            IRepository<OnlineLeadRejectReason> rejectedReasonRepository, IRepository<BankStatement> bankStatementRepository)
        {
            _customerSettings = customerSettings;
            _genericAttributeService = genericAttributeService;
            _dataProvider = dataProvider;
            _customerAddressRepository = customerAddressRepository;
            _blogCommentRepository = blogCommentRepository;
            _customerRepository = customerRepository;
            _customerAddressMappingRepository = customerAddressMappingRepository;
            _customerCustomerRoleMappingRepository = customerCustomerRoleMappingRepository;
            _customerPasswordRepository = customerPasswordRepository;
            _customerRoleRepository = customerRoleRepository;
            _forumPostRepository = forumPostRepository;
            _forumTopicRepository = forumTopicRepository;
            _gaRepository = gaRepository;
            _newsCommentRepository = newsCommentRepository;
            _orderRepository = orderRepository;
            _productReviewRepository = productReviewRepository;
            _productReviewHelpfulnessRepository = productReviewHelpfulnessRepository;
            _shoppingCartRepository = shoppingCartRepository;
            _staticCacheManager = staticCacheManager;
            _storeContext = storeContext;
            _shoppingCartSettings = shoppingCartSettings;
            _userTypeRepository = userTypeRepository;
            _vehiclePortfolioRepository = vehiclePortfolioRepository;
            _customerIndustryMappingRepository = customerIndustryMappingRepository;
            _industryRepository = industryRepository;
            _transporterVehicleMappingRepository = transporterVehicleMappingRepository;
            _transpoterCostRepository = transpoterCostRepository;
            _bankDetailRepository = bankDetailRepository;
            _onlineLeadRepository = onlineLeadRepository;
            _appliedCreditCustomerRepository = appliedCreditCustomerRepository;
            _rejectedReasonRepository = rejectedReasonRepository;
            _bankStatementRepository = bankStatementRepository;
        }

        #endregion

        #region Methods

        #region Customers

        /// <summary>
        /// Gets all customers
        /// </summary>
        /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
        /// <param name="lastActivityFromUtc">Last activity date from (UTC); null to load all records</param>
        /// <param name="lastActivityToUtc">Last activity date to (UTC); null to load all records</param>
        /// <param name="affiliateId">Affiliate identifier</param>
        /// <param name="customerRoleIds">A list of customer role identifiers to filter by (at least one match); pass null or empty list in order to load all customers; </param>
        /// <param name="email">Email; null to load all customers</param>
        /// <param name="username">Username; null to load all customers</param>
        /// <param name="firstName">First name; null to load all customers</param>
        /// <param name="lastName">Last name; null to load all customers</param>
        /// <param name="dayOfBirth">Day of birth; 0 to load all customers</param>
        /// <param name="monthOfBirth">Month of birth; 0 to load all customers</param>
        /// <param name="company">Company; null to load all customers</param>
        /// <param name="phone">Phone; null to load all customers</param>
        /// <param name="zipPostalCode">Phone; null to load all customers</param>
        /// <param name="ipAddress">IP address; null to load all customers</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="getOnlyTotalCount">A value in indicating whether you want to load only total number of records. Set to "true" if you don't want to load data from database</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customers
        /// </returns>
        //public virtual async Task<IPagedList<Customer>> GetAllCustomersAsync(DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
        //    DateTime? lastActivityFromUtc = null, DateTime? lastActivityToUtc = null, int[] industryIds = null, int[] vehiclePortfolioIds = null,
        //    int affiliateId = 0, int[] customerRoleIds = null, int buyerTypeId = 0, int supplierTypeId = 0, 
        //    string email = null, string username = null, string firstName = null, string lastName = null, string fullName = null,
        //    int dayOfBirth = 0, int monthOfBirth = 0, int supportAgentId = 0, int createdBy = 0, string source = null,
        //    string company = null, string phone = null, string zipPostalCode = null, string ipAddress = null,
        //    int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false)
        //{
        //    var customers = await _customerRepository.GetAllPagedAsync(query =>
        //    {
        //        if (createdFromUtc.HasValue)
        //            query = query.Where(c => createdFromUtc.Value <= c.CreatedOnUtc);
        //        if (createdToUtc.HasValue)
        //            query = query.Where(c => createdToUtc.Value >= c.CreatedOnUtc);
        //        if (lastActivityFromUtc.HasValue)
        //            query = query.Where(c => lastActivityFromUtc.Value <= c.LastActivityDateUtc);
        //        if (lastActivityToUtc.HasValue)
        //            query = query.Where(c => lastActivityToUtc.Value >= c.LastActivityDateUtc);
        //        if (affiliateId > 0)
        //            query = query.Where(c => affiliateId == c.AffiliateId);
        //        if (supportAgentId > 0)
        //            query = query.Where(c => supportAgentId == c.SupportAgentId);

        //        query = query.Where(c => !c.Deleted);

        //        if (customerRoleIds != null && customerRoleIds.Length > 0)
        //        {
        //            query = query.Join(_customerCustomerRoleMappingRepository.Table, x => x.Id, y => y.CustomerId,
        //                    (x, y) => new { Customer = x, Mapping = y })
        //                .Where(z => customerRoleIds.Contains(z.Mapping.CustomerRoleId))
        //                .Select(z => z.Customer)
        //                .Distinct();
        //        }

        //        if (!string.IsNullOrWhiteSpace(email))
        //            query = query.Where(c => c.Email.Contains(email));
        //        if (!string.IsNullOrWhiteSpace(username))
        //            query = query.Where(c => c.Username.Contains(username));
        //        if (!string.IsNullOrWhiteSpace(firstName))
        //            query = query.Where(c => c.FirstName.Contains(firstName));
        //        if (!string.IsNullOrWhiteSpace(lastName))
        //            query = query.Where(c => c.LastName.Contains(lastName));
        //        if (!string.IsNullOrWhiteSpace(fullName))
        //            query = query.Where(c => c.FullName.Contains(fullName));
        //        if (!string.IsNullOrWhiteSpace(company))
        //            query = query.Where(c => c.Company.Contains(company));
        //        if (!string.IsNullOrWhiteSpace(phone))
        //            query = query.Where(c => c.Phone.Contains(phone));
        //        if (!string.IsNullOrWhiteSpace(zipPostalCode))
        //            query = query.Where(c => c.ZipPostalCode.Contains(zipPostalCode));

        //        if (dayOfBirth > 0 && monthOfBirth > 0)
        //            query = query.Where(c => c.DateOfBirth.HasValue && c.DateOfBirth.Value.Day == dayOfBirth &&
        //                c.DateOfBirth.Value.Month == monthOfBirth);
        //        else if (dayOfBirth > 0)
        //            query = query.Where(c => c.DateOfBirth.HasValue && c.DateOfBirth.Value.Day == dayOfBirth);
        //        else if (monthOfBirth > 0)
        //            query = query.Where(c => c.DateOfBirth.HasValue && c.DateOfBirth.Value.Month == monthOfBirth);

        //        //serach by industry
        //        if (industryIds != null && industryIds.Length > 0 && !industryIds.Any(x => x == 0))
        //        {
        //            query = query.Join(_customerIndustryMappingRepository.Table, x => x.Id, y => y.CustomerId,
        //                    (x, y) => new { Customer = x, Mapping = y })
        //                .Where(z => industryIds.Contains(z.Mapping.IndustryId))
        //                .Select(z => z.Customer)
        //                .Distinct();
        //        }

        //        //serach by vehiclePortfolio
        //        if (vehiclePortfolioIds != null && vehiclePortfolioIds.Length > 0 && !vehiclePortfolioIds.Any(x => x == 0))
        //        {
        //            query = query.Join(_transporterVehicleMappingRepository.Table, x => x.Id, y => y.TransporterId,
        //                    (x, y) => new { Customer = x, Mapping = y })
        //                .Where(z => vehiclePortfolioIds.Contains(z.Mapping.VehicleId))
        //                .Select(z => z.Customer)
        //                .Distinct();
        //        }

        //        //search by IpAddress
        //        if (!string.IsNullOrWhiteSpace(ipAddress) && CommonHelper.IsValidIpAddress(ipAddress))
        //        {
        //            query = query.Where(w => w.LastIpAddress == ipAddress);
        //        }

        //        query = query.OrderByDescending(c => c.CreatedOnUtc);

        //        return query;
        //    }, pageIndex, pageSize, getOnlyTotalCount);

        //    return customers;
        //}

        public virtual async Task<IPagedList<Customer>> GetAllCustomersAsync(DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
           int affiliateId = 0, int[] customerRoleIds = null, int[] industryIds = null, int[] vehiclePortfolioIds = null,
           string email = null, string username = null, string firstName = null, string lastName = null,
           int dayOfBirth = 0, int monthOfBirth = 0, string transporterType = null,
           string company = null, string phone = null, string zipPostalCode = null, string ipAddress = null,
           int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false, string fullName = null,
           int productId = 0, int createdBy = 0, string source = null, int buyerIndustryId = 0, int supplierIndustryId = 0,
           int countryId = 0, int cityId = 0, int buyerTypeId = 0, int supplierTypeId = 0, int customerId = 0,
           bool? isActive = null, bool? getOnlyBuyerOrders = null, bool? getOnlySupplierOrders = null,bool? getIszarayeTranspoter = null, int supportAgentId = 0,
           int parentId = 0, int? filterByCompanyOrEmployee = null,bool isPoc=false, string keywords = null)
        {
            var customers = await _customerRepository.GetAllPagedAsync(query =>
            {
                query = query.Where(c => !c.Deleted);

                //if (getOnlyBuyerOrders.HasValue && getOnlyBuyerOrders.Value)
                //{
                //    query = (from c in query
                //             join
                //             o in _orderRepository.Table on c.Id equals o.CustomerId
                //             join br in _buyerRequestRepository.Table on o.BuyerRequestId equals br.Id
                //             where o.Deleted == false && o.OrderStatusId != (int)OrderStatus.Cancelled && o.PaymentStatusId != (int)PaymentStatus.Paid
                //             select c);
                //}

                //if (getOnlySupplierOrders.HasValue && getOnlySupplierOrders.Value)
                //{
                //    query = (from c in query
                //             join rq in _requestQuotationApprovedMappingRepository.Table on c.Id equals rq.SupplierId
                //             join o in _orderRepository.Table on rq.OrderId equals o.Id
                //             join q in _supplierBidrepository.Table on rq.QuotationId equals q.Id
                //             where o.Deleted == false && q.Deleted == false && o.OrderStatusId != (int)OrderStatus.Cancelled && o.SupplierPaymentStatusId != (int)SupplierPaymentStatus.Paid
                //             select c);
                //}

                if (createdFromUtc.HasValue)
                    query = query.Where(c => createdFromUtc.Value <= c.CreatedOnUtc);
                if (createdToUtc.HasValue)
                    query = query.Where(c => createdToUtc.Value >= c.CreatedOnUtc);
                if (affiliateId > 0)
                    query = query.Where(c => affiliateId == c.AffiliateId);
                if (createdBy > 0)
                    query = query.Where(c => createdBy == c.CreatedBy);
                if (!string.IsNullOrWhiteSpace(source))
                    query = query.Where(c => source == c.Source);
                if (isActive.HasValue)
                    query = query.Where(c => c.Active == isActive.Value);
                if (customerId > 0)
                    query = query.Where(c => customerId == c.Id);
                if (supportAgentId > 0)
                    query = query.Where(c => supportAgentId == c.SupportAgentId);
                if (parentId > 0)
                    query = query.Where(c => parentId == c.ParentId);

                if (isPoc)
                    query = query.Where(c => isPoc == c.IsPoc);

                if (customerRoleIds != null && customerRoleIds.Length > 0)
                {
                    query = query.Join(_customerCustomerRoleMappingRepository.Table, x => x.Id, y => y.CustomerId,
                            (x, y) => new { Customer = x, Mapping = y })
                        .Where(z => customerRoleIds.Contains(z.Mapping.CustomerRoleId))
                        .Select(z => z.Customer)
                        .Distinct();
                }

                if (filterByCompanyOrEmployee.HasValue)
                {
                    if (filterByCompanyOrEmployee.Value == 1)
                        query = query.Where(c => c.ParentId == 0);
                    else
                        query = query.Where(c => c.ParentId > 0);
                }
                
                if (getIszarayeTranspoter.HasValue)
                {
                   query = query.Where(c => c.IsZarayeTransporter == getIszarayeTranspoter.Value);
                }

                if (industryIds != null && industryIds.Length > 0 && !industryIds.Any(x => x == 0))
                {
                    query = query.Join(_customerIndustryMappingRepository.Table, x => x.Id, y => y.CustomerId,
                            (x, y) => new { Customer = x, Mapping = y })
                        .Where(z => industryIds.Contains(z.Mapping.IndustryId))
                        .Select(z => z.Customer)
                        .Distinct();
                }

                if (!string.IsNullOrWhiteSpace(fullName))
                {
                    query = query.Where(c => c.FullName.Contains(fullName) || c.Username.Contains(fullName));

                    //query = query
                    //    .Join(_gaRepository.Table, x => x.Id, y => y.EntityId,
                    //        (x, y) => new { Customer = x, Attribute = y })
                    //    .Where(z => z.Attribute.KeyGroup == nameof(Customer) &&
                    //                z.Attribute.Key == NopCustomerDefaults.PhoneAttribute &&
                    //                z.Attribute.Value.Contains(fullName) || z.Customer.FullName.Contains(fullName))
                    //    .Select(z => z.Customer).Distinct();
                }
                if (!string.IsNullOrWhiteSpace(email))
                    query = query.Where(c => c.Email.Contains(email));
                if (!string.IsNullOrWhiteSpace(username))
                    query = query.Where(c => c.Username.Contains(username));

                if (!string.IsNullOrWhiteSpace(firstName))
                {
                    query = query.Where(x => x.FirstName.Contains(firstName));
                }

                //Filter111
                if (!string.IsNullOrWhiteSpace(lastName))
                {
                    query = query.Where(x => x.LastName.Contains(lastName));
                }
                if (!string.IsNullOrWhiteSpace(transporterType))
                {
                    query = query
                        .Join(_gaRepository.Table, x => x.Id, y => y.EntityId,
                            (x, y) => new { Customer = x, Attribute = y })
                        .Where(z => z.Attribute.KeyGroup == nameof(Customer) &&
                                    z.Attribute.Key == ZarayeCustomerDefaults.TransporterTypeAttribute &&
                                    z.Attribute.Value.Contains(transporterType))
                        .Select(z => z.Customer);
                }

                //date of birth is stored as a string into database.
                //we also know that date of birth is stored in the following format YYYY-MM-DD (for example, 1983-02-18).
                //so let's search it as a string
                if (dayOfBirth > 0 && monthOfBirth > 0)
                {
                    //both are specified
                    var dateOfBirthStr = monthOfBirth.ToString("00", CultureInfo.InvariantCulture) + "-" +
                                         dayOfBirth.ToString("00", CultureInfo.InvariantCulture);

                    //z.Attribute.Value.Length - dateOfBirthStr.Length = 5
                    //dateOfBirthStr.Length = 5
                    query = query
                        .Join(_gaRepository.Table, x => x.Id, y => y.EntityId,
                            (x, y) => new { Customer = x, Attribute = y })
                        .Where(z => z.Attribute.KeyGroup == nameof(Customer) &&
                                    z.Attribute.Key == ZarayeCustomerDefaults.DateOfBirthAttribute &&
                                    z.Attribute.Value.Substring(5, 5) == dateOfBirthStr)
                        .Select(z => z.Customer);
                }
                else if (dayOfBirth > 0)
                {
                    //only day is specified
                    var dateOfBirthStr = dayOfBirth.ToString("00", CultureInfo.InvariantCulture);

                    //z.Attribute.Value.Length - dateOfBirthStr.Length = 8
                    //dateOfBirthStr.Length = 2
                    query = query
                        .Join(_gaRepository.Table, x => x.Id, y => y.EntityId,
                            (x, y) => new { Customer = x, Attribute = y })
                        .Where(z => z.Attribute.KeyGroup == nameof(Customer) &&
                                    z.Attribute.Key == ZarayeCustomerDefaults.DateOfBirthAttribute &&
                                    z.Attribute.Value.Substring(8, 2) == dateOfBirthStr)
                        .Select(z => z.Customer);
                }
                else if (monthOfBirth > 0)
                {
                    //only month is specified
                    var dateOfBirthStr = "-" + monthOfBirth.ToString("00", CultureInfo.InvariantCulture) + "-";
                    query = query
                        .Join(_gaRepository.Table, x => x.Id, y => y.EntityId,
                            (x, y) => new { Customer = x, Attribute = y })
                        .Where(z => z.Attribute.KeyGroup == nameof(Customer) &&
                                    z.Attribute.Key == ZarayeCustomerDefaults.DateOfBirthAttribute &&
                                    z.Attribute.Value.Contains(dateOfBirthStr))
                        .Select(z => z.Customer);
                }

                //search by company
                if (!string.IsNullOrWhiteSpace(company))
                {
                    query = query.Where(x => x.Company.Contains(company));

                }

                //search by phone
                if (!string.IsNullOrWhiteSpace(phone))
                {
                    query = query
                        .Join(_gaRepository.Table, x => x.Id, y => y.EntityId,
                            (x, y) => new { Customer = x, Attribute = y })
                        .Where(z => z.Attribute.KeyGroup == nameof(Customer) &&
                                    z.Attribute.Key == ZarayeCustomerDefaults.PhoneAttribute &&
                                    z.Attribute.Value.Contains(phone))
                        .Select(z => z.Customer);
                }

                //search by zip
                if (!string.IsNullOrWhiteSpace(zipPostalCode))
                {
                    query = query
                        .Join(_gaRepository.Table, x => x.Id, y => y.EntityId,
                            (x, y) => new { Customer = x, Attribute = y })
                        .Where(z => z.Attribute.KeyGroup == nameof(Customer) &&
                                    z.Attribute.Key == ZarayeCustomerDefaults.ZipPostalCodeAttribute &&
                                    z.Attribute.Value.Contains(zipPostalCode))
                        .Select(z => z.Customer);
                }

                //search by product id
                //if (productId > 0)
                //{
                //    query = from request in query
                //            join sp in _supplierProductRepository.Table on request.Id equals sp.SupplierId
                //            where sp.ProductId == productId && sp.Published
                //            select request;
                //}

                //Filter111
                //search by supplier industry id
                if (supplierIndustryId > 0)
                {
                    query = query.Where(x => x.IndustryId == supplierIndustryId);
                }

                //Filter111
                //search by buyer industry id
                if (buyerIndustryId > 0)
                {
                    query = query.Where(x => x.IndustryId == buyerIndustryId);
                }

                //Filter111
                //search by country id
                if (countryId > 0)
                {
                    query = query.Where(x => x.CountryId == countryId);
                }

                //Filter111
                //search by city id
                if (cityId > 0)
                {
                    query = query
                        .Join(_gaRepository.Table, x => x.Id, y => y.EntityId,
                            (x, y) => new { Customer = x, Attribute = y })
                        .Where(z => z.Attribute.KeyGroup == nameof(Customer) &&
                                    z.Attribute.Key == ZarayeCustomerDefaults.StateProvinceIdAttribute &&
                                    z.Attribute.Value == cityId.ToString())
                        .Select(z => z.Customer);
                }

                //Filter111
                // search by buyer type  id
                if (buyerTypeId > 0)
                {
                    query = query.Where(x => x.UserTypeId == buyerTypeId);
                }

                //Filter111
                // search by buyer type  id
                if (supplierTypeId > 0)
                {
                    query = query.Where(x => x.UserTypeId == supplierTypeId);
                }

                if (!string.IsNullOrWhiteSpace(transporterType))
                {
                    query = query
                        .Join(_gaRepository.Table, x => x.Id, y => y.EntityId,
                            (x, y) => new { Customer = x, Attribute = y })
                        .Where(z => z.Attribute.KeyGroup == nameof(Customer) &&
                                    z.Attribute.Key == ZarayeCustomerDefaults.TransporterTypeAttribute &&
                                    z.Attribute.Value.Contains(transporterType))
                        .Select(z => z.Customer);
                }

                //search by IpAddress
                if (!string.IsNullOrWhiteSpace(ipAddress) && CommonHelper.IsValidIpAddress(ipAddress))
                {
                    query = query.Where(w => w.LastIpAddress == ipAddress);
                }

                if (!string.IsNullOrEmpty(keywords))
                {
                    IQueryable<int> usersByKeywords;

                    usersByKeywords =
                            from u in query
                            where u.FullName.Contains(keywords)
                            select u.Id;

                    query =
                    from p in query
                    from pbk in LinqToDB.LinqExtensions.InnerJoin(usersByKeywords, pbk => pbk == p.Id)
                    select p;
                }


                query = query.OrderByDescending(c => c.CreatedOnUtc);

                return query;
            }, pageIndex, pageSize, getOnlyTotalCount);

            return customers;
        }

        /// <summary>
        /// Gets online customers
        /// </summary>
        /// <param name="lastActivityFromUtc">Customer last activity date (from)</param>
        /// <param name="customerRoleIds">A list of customer role identifiers to filter by (at least one match); pass null or empty list in order to load all customers; </param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customers
        /// </returns>
        public virtual async Task<IPagedList<Customer>> GetOnlineCustomersAsync(DateTime lastActivityFromUtc,
            int[] customerRoleIds, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _customerRepository.Table;
            query = query.Where(c => lastActivityFromUtc <= c.LastActivityDateUtc);
            query = query.Where(c => !c.Deleted);

            if (customerRoleIds != null && customerRoleIds.Length > 0)
                query = query.Where(c => _customerCustomerRoleMappingRepository.Table.Any(ccrm => ccrm.CustomerId == c.Id && customerRoleIds.Contains(ccrm.CustomerRoleId)));

            query = query.OrderByDescending(c => c.LastActivityDateUtc);
            var customers = await query.ToPagedListAsync(pageIndex, pageSize);

            return customers;
        }

        /// <summary>
        /// Gets customers with shopping carts
        /// </summary>
        /// <param name="shoppingCartType">Shopping cart type; pass null to load all records</param>
        /// <param name="storeId">Store identifier; pass 0 to load all records</param>
        /// <param name="productId">Product identifier; pass null to load all records</param>
        /// <param name="createdFromUtc">Created date from (UTC); pass null to load all records</param>
        /// <param name="createdToUtc">Created date to (UTC); pass null to load all records</param>
        /// <param name="countryId">Billing country identifier; pass null to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customers
        /// </returns>
        public virtual async Task<IPagedList<Customer>> GetCustomersWithShoppingCartsAsync(ShoppingCartType? shoppingCartType = null,
            int storeId = 0, int? productId = null,
            DateTime? createdFromUtc = null, DateTime? createdToUtc = null, int? countryId = null,
            int pageIndex = 0, int pageSize = int.MaxValue)
        {
            //get all shopping cart items
            var items = _shoppingCartRepository.Table;

            //filter by type
            if (shoppingCartType.HasValue)
                items = items.Where(item => item.ShoppingCartTypeId == (int)shoppingCartType.Value);

            //filter shopping cart items by store
            if (storeId > 0 && !_shoppingCartSettings.CartsSharedBetweenStores)
                items = items.Where(item => item.StoreId == storeId);

            //filter shopping cart items by product
            if (productId > 0)
                items = items.Where(item => item.ProductId == productId);

            //filter shopping cart items by date
            if (createdFromUtc.HasValue)
                items = items.Where(item => createdFromUtc.Value <= item.CreatedOnUtc);
            if (createdToUtc.HasValue)
                items = items.Where(item => createdToUtc.Value >= item.CreatedOnUtc);

            //get all active customers
            var customers = _customerRepository.Table.Where(customer => customer.Active && !customer.Deleted);

            //filter customers by billing country
            if (countryId > 0)
                customers = from c in customers
                            join a in _customerAddressRepository.Table on c.BillingAddressId equals a.Id
                            where a.CountryId == countryId
                            select c;

            var customersWithCarts = from c in customers
                                     join item in items on c.Id equals item.CustomerId
                                     //we change ordering for the MySQL engine to avoid problems with the ONLY_FULL_GROUP_BY server property that is set by default since the 5.7.5 version
                                     orderby _dataProvider.ConfigurationName == "MySql" ? c.CreatedOnUtc : item.CreatedOnUtc descending
                                     select c;

            return await customersWithCarts.Distinct().ToPagedListAsync(pageIndex, pageSize);
        }

        /// <summary>
        /// Gets customer for shopping cart
        /// </summary>
        /// <param name="shoppingCart">Shopping cart</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public virtual async Task<Customer> GetShoppingCartCustomerAsync(IList<ShoppingCartItem> shoppingCart)
        {
            var customerId = shoppingCart.FirstOrDefault()?.CustomerId;

            return customerId.HasValue && customerId != 0 ? await GetCustomerByIdAsync(customerId.Value) : null;
        }

        /// <summary>
        /// Delete a customer
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteCustomerAsync(Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            if (customer.IsSystemAccount)
                throw new ZarayeException($"System customer account ({customer.SystemName}) could not be deleted");

            customer.Deleted = true;

            if (_customerSettings.SuffixDeletedCustomers)
            {
                if (!string.IsNullOrEmpty(customer.Email))
                    customer.Email += "-DELETED";
                if (!string.IsNullOrEmpty(customer.Username))
                    customer.Username += "-DELETED";
            }

            await _customerRepository.UpdateAsync(customer, false);
            await _customerRepository.DeleteAsync(customer);
        }

        /// <summary>
        /// Gets a customer
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains a customer
        /// </returns>
        public virtual async Task<Customer> GetCustomerByIdAsync(int customerId)
        {
            return await _customerRepository.GetByIdAsync(customerId,
                cache => cache.PrepareKeyForShortTermCache(ZarayeEntityCacheDefaults<Customer>.ByIdCacheKey, customerId));
        }

        /// <summary>
        /// Get customers by identifiers
        /// </summary>
        /// <param name="customerIds">Customer identifiers</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customers
        /// </returns>
        public virtual async Task<IList<Customer>> GetCustomersByIdsAsync(int[] customerIds)
        {
            return await _customerRepository.GetByIdsAsync(customerIds, includeDeleted: false);
        }

        /// <summary>
        /// Get customers by guids
        /// </summary>
        /// <param name="customerGuids">Customer guids</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customers
        /// </returns>
        public virtual async Task<IList<Customer>> GetCustomersByGuidsAsync(Guid[] customerGuids)
        {
            if (customerGuids == null)
                return null;

            var query = from c in _customerRepository.Table
                        where customerGuids.Contains(c.CustomerGuid)
                        select c;
            var customers = await query.ToListAsync();

            return customers;
        }

        /// <summary>
        /// Gets a customer by GUID
        /// </summary>
        /// <param name="customerGuid">Customer GUID</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains a customer
        /// </returns>
        public virtual async Task<Customer> GetCustomerByGuidAsync(Guid customerGuid)
        {
            if (customerGuid == Guid.Empty)
                return null;

            var query = from c in _customerRepository.Table
                        where c.CustomerGuid == customerGuid
                        orderby c.Id
                        select c;

            var key = _staticCacheManager.PrepareKeyForShortTermCache(ZarayeCustomerServicesDefaults.CustomerByGuidCacheKey, customerGuid);

            return await _staticCacheManager.GetAsync(key, async () => await query.FirstOrDefaultAsync());
        }

        /// <summary>
        /// Get customer by email
        /// </summary>
        /// <param name="email">Email</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer
        /// </returns>
        public virtual async Task<Customer> GetCustomerByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            var query = from c in _customerRepository.Table
                        orderby c.Id
                        where c.Email == email
                        select c;
            var customer = await query.FirstOrDefaultAsync();

            return customer;
        }

        /// <summary>
        /// Get customer by system name
        /// </summary>
        /// <param name="systemName">System name</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer
        /// </returns>
        public virtual async Task<Customer> GetCustomerBySystemNameAsync(string systemName)
        {
            if (string.IsNullOrWhiteSpace(systemName))
                return null;

            var key = _staticCacheManager.PrepareKeyForDefaultCache(ZarayeCustomerServicesDefaults.CustomerBySystemNameCacheKey, systemName);

            var query = from c in _customerRepository.Table
                        orderby c.Id
                        where c.SystemName == systemName
                        select c;

            var customer = await _staticCacheManager.GetAsync(key, async () => await query.FirstOrDefaultAsync());

            return customer;
        }

        /// <summary>
        /// Gets built-in system record used for background tasks
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains a customer object
        /// </returns>
        public virtual async Task<Customer> GetOrCreateBackgroundTaskUserAsync()
        {
            var backgroundTaskUser = await GetCustomerBySystemNameAsync(ZarayeCustomerDefaults.BackgroundTaskCustomerName);

            if (backgroundTaskUser is null)
            {
                var store = await _storeContext.GetCurrentStoreAsync();
                //If for any reason the system user isn't in the database, then we add it
                backgroundTaskUser = new Customer
                {
                    Email = "builtin@background-task-record.com",
                    CustomerGuid = Guid.NewGuid(),
                    AdminComment = "Built-in system record used for background tasks.",
                    Active = true,
                    IsSystemAccount = true,
                    SystemName = ZarayeCustomerDefaults.BackgroundTaskCustomerName,
                    CreatedOnUtc = DateTime.UtcNow,
                    LastActivityDateUtc = DateTime.UtcNow,
                    RegisteredInStoreId = store.Id
                };

                await InsertCustomerAsync(backgroundTaskUser);

                var guestRole = await GetCustomerRoleBySystemNameAsync(ZarayeCustomerDefaults.GuestsRoleName);

                if (guestRole is null)
                    throw new ZarayeException("'Guests' role could not be loaded");

                await AddCustomerRoleMappingAsync(new CustomerCustomerRoleMapping { CustomerRoleId = guestRole.Id, CustomerId = backgroundTaskUser.Id });
            }

            return backgroundTaskUser;
        }

        /// <summary>
        /// Gets built-in system guest record used for requests from search engines
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains a customer object
        /// </returns>
        public virtual async Task<Customer> GetOrCreateSearchEngineUserAsync()
        {
            var searchEngineUser = await GetCustomerBySystemNameAsync(ZarayeCustomerDefaults.SearchEngineCustomerName);

            if (searchEngineUser is null)
            {
                var store = await _storeContext.GetCurrentStoreAsync();
                //If for any reason the system user isn't in the database, then we add it
                searchEngineUser = new Customer
                {
                    Email = "builtin@search_engine_record.com",
                    CustomerGuid = Guid.NewGuid(),
                    AdminComment = "Built-in system guest record used for requests from search engines.",
                    Active = true,
                    IsSystemAccount = true,
                    SystemName = ZarayeCustomerDefaults.SearchEngineCustomerName,
                    CreatedOnUtc = DateTime.UtcNow,
                    LastActivityDateUtc = DateTime.UtcNow,
                    RegisteredInStoreId = store.Id
                };

                await InsertCustomerAsync(searchEngineUser);

                var guestRole = await GetCustomerRoleBySystemNameAsync(ZarayeCustomerDefaults.GuestsRoleName);

                if (guestRole is null)
                    throw new ZarayeException("'Guests' role could not be loaded");

                await AddCustomerRoleMappingAsync(new CustomerCustomerRoleMapping { CustomerRoleId = guestRole.Id, CustomerId = searchEngineUser.Id });
            }

            return searchEngineUser;
        }

        /// <summary>
        /// Get customer by username
        /// </summary>
        /// <param name="username">Username</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer
        /// </returns>
        public virtual async Task<Customer> GetCustomerByUsernameAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return null;

            var query = from c in _customerRepository.Table
                        orderby c.Id
                        where c.Username == username
                        select c;
            var customer = await query.FirstOrDefaultAsync();

            return customer;
        }

        /// <summary>
        /// Insert a guest customer
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer
        /// </returns>
        public virtual async Task<Customer> InsertGuestCustomerAsync()
        {
            var customer = new Customer
            {
                CustomerGuid = Guid.NewGuid(),
                Active = true,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
                IsGuestCustomer = true
            };

            //add to 'Guests' role
            var guestRole = await GetCustomerRoleBySystemNameAsync(ZarayeCustomerDefaults.GuestsRoleName);
            if (guestRole == null)
                throw new ZarayeException("'Guests' role could not be loaded");

            await _customerRepository.InsertAsync(customer);

            await AddCustomerRoleMappingAsync(new CustomerCustomerRoleMapping { CustomerId = customer.Id, CustomerRoleId = guestRole.Id });

            return customer;
        }

        /// <summary>
        /// Insert a customer
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertCustomerAsync(Customer customer)
        {
            var firstName = customer.FirstName;
            var lastName = customer.LastName;

            var fullName = string.Empty;
            if (!string.IsNullOrWhiteSpace(firstName) && !string.IsNullOrWhiteSpace(lastName))
                fullName = $"{firstName} {lastName}";
            else
            {
                if (!string.IsNullOrWhiteSpace(firstName))
                    fullName = firstName;

                if (!string.IsNullOrWhiteSpace(lastName))
                    fullName = fullName + " " + lastName;
            }
            customer.FullName = fullName;
            customer.UpdatedOnUtc = DateTime.UtcNow;
            await _customerRepository.InsertAsync(customer);
        }

        /// <summary>
        /// Updates the customer
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateCustomerAsync(Customer customer)
        {
            var firstName = customer.FirstName;
            var lastName = customer.LastName;

            var fullName = string.Empty;
            if (!string.IsNullOrWhiteSpace(firstName) && !string.IsNullOrWhiteSpace(lastName))
                fullName = $"{firstName} {lastName}";
            else
            {
                if (!string.IsNullOrWhiteSpace(firstName))
                    fullName = firstName;

                if (!string.IsNullOrWhiteSpace(lastName))
                    fullName = fullName + " " + lastName;
            }
            customer.FullName = fullName;
            customer.UpdatedOnUtc = DateTime.UtcNow;
            await _customerRepository.UpdateAsync(customer);
        }

        /// <summary>
        /// Reset data required for checkout
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="clearCouponCodes">A value indicating whether to clear coupon code</param>
        /// <param name="clearCheckoutAttributes">A value indicating whether to clear selected checkout attributes</param>
        /// <param name="clearRewardPoints">A value indicating whether to clear "Use reward points" flag</param>
        /// <param name="clearShippingMethod">A value indicating whether to clear selected shipping method</param>
        /// <param name="clearPaymentMethod">A value indicating whether to clear selected payment method</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task ResetCheckoutDataAsync(Customer customer, int storeId,
            bool clearCouponCodes = false, bool clearCheckoutAttributes = false,
            bool clearRewardPoints = true, bool clearShippingMethod = true,
            bool clearPaymentMethod = true)
        {
            if (customer == null)
                throw new ArgumentNullException();

            //clear entered coupon codes
            if (clearCouponCodes)
            {
                await _genericAttributeService.SaveAttributeAsync<string>(customer, ZarayeCustomerDefaults.DiscountCouponCodeAttribute, null);
                await _genericAttributeService.SaveAttributeAsync<string>(customer, ZarayeCustomerDefaults.GiftCardCouponCodesAttribute, null);
            }

            //clear reward points flag
            if (clearRewardPoints)
                await _genericAttributeService.SaveAttributeAsync(customer, ZarayeCustomerDefaults.UseRewardPointsDuringCheckoutAttribute, false, storeId);

            //clear selected shipping method
            if (clearShippingMethod)
            {
                await _genericAttributeService.SaveAttributeAsync<ShippingOption>(customer, ZarayeCustomerDefaults.SelectedShippingOptionAttribute, null, storeId);
                await _genericAttributeService.SaveAttributeAsync<ShippingOption>(customer, ZarayeCustomerDefaults.OfferedShippingOptionsAttribute, null, storeId);
                await _genericAttributeService.SaveAttributeAsync<PickupPoint>(customer, ZarayeCustomerDefaults.SelectedPickupPointAttribute, null, storeId);
            }

            //clear selected payment method
            if (clearPaymentMethod)
                await _genericAttributeService.SaveAttributeAsync<string>(customer, ZarayeCustomerDefaults.SelectedPaymentMethodAttribute, null, storeId);

            await UpdateCustomerAsync(customer);
        }

        /// <summary>
        /// Delete guest customer records
        /// </summary>
        /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
        /// <param name="onlyWithoutShoppingCart">A value indicating whether to delete customers only without shopping cart</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the number of deleted customers
        /// </returns>
        public virtual async Task<int> DeleteGuestCustomersAsync(DateTime? createdFromUtc, DateTime? createdToUtc, bool onlyWithoutShoppingCart)
        {
            var guestRole = await GetCustomerRoleBySystemNameAsync(ZarayeCustomerDefaults.GuestsRoleName);

            var allGuestCustomers = from guest in _customerRepository.Table
                                    join ccm in _customerCustomerRoleMappingRepository.Table on guest.Id equals ccm.CustomerId
                                    where ccm.CustomerRoleId == guestRole.Id
                                    select guest;

            var guestsToDelete = from guest in _customerRepository.Table
                                 join g in allGuestCustomers on guest.Id equals g.Id
                                 from leads in _onlineLeadRepository.Table.Where(sci => sci.CreatedById == guest.Id).DefaultIfEmpty()
                                 from sCart in _shoppingCartRepository.Table.Where(sci => sci.CustomerId == guest.Id).DefaultIfEmpty()
                                 from order in _orderRepository.Table.Where(o => o.CustomerId == guest.Id).DefaultIfEmpty()
                                 from blogComment in _blogCommentRepository.Table.Where(o => o.CustomerId == guest.Id).DefaultIfEmpty()
                                 from newsComment in _newsCommentRepository.Table.Where(o => o.CustomerId == guest.Id).DefaultIfEmpty()
                                 from productReview in _productReviewRepository.Table.Where(o => o.CustomerId == guest.Id).DefaultIfEmpty()
                                 from productReviewHelpfulness in _productReviewHelpfulnessRepository.Table.Where(o => o.CustomerId == guest.Id).DefaultIfEmpty()
                                 from forumTopic in _forumTopicRepository.Table.Where(o => o.CustomerId == guest.Id).DefaultIfEmpty()
                                 from forumPost in _forumPostRepository.Table.Where(o => o.CustomerId == guest.Id).DefaultIfEmpty()
                                 where (!onlyWithoutShoppingCart || sCart == null) &&
                                     order == null && leads == null && blogComment == null && newsComment == null && productReview == null && productReviewHelpfulness == null &&
                                     forumTopic == null && forumPost == null &&
                                     !guest.IsSystemAccount &&
                                     (createdFromUtc == null || guest.CreatedOnUtc > createdFromUtc) &&
                                     (createdToUtc == null || guest.CreatedOnUtc < createdToUtc)
                                 select new { CustomerId = guest.Id };

            await using var tmpGuests = await _dataProvider.CreateTempDataStorageAsync("tmp_guestsToDelete", guestsToDelete);
            await using var tmpAddresses = await _dataProvider.CreateTempDataStorageAsync("tmp_guestsAddressesToDelete",
                _customerAddressMappingRepository.Table
                    .Where(ca => tmpGuests.Any(c => c.CustomerId == ca.CustomerId))
                    .Select(ca => new { AddressId = ca.AddressId }));

            //delete guests
            var totalRecordsDeleted = await _customerRepository.DeleteAsync(c => tmpGuests.Any(tmp => tmp.CustomerId == c.Id));

            //delete attributes
            await _gaRepository.DeleteAsync(ga => tmpGuests.Any(c => c.CustomerId == ga.EntityId) && ga.KeyGroup == nameof(Customer));

            //delete m -> m addresses
            await _customerAddressRepository.DeleteAsync(a => tmpAddresses.Any(tmp => tmp.AddressId == a.Id));

            return totalRecordsDeleted;
        }

        /// <summary>
        /// Gets a default tax display type (if configured)
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public virtual async Task<TaxDisplayType?> GetCustomerDefaultTaxDisplayTypeAsync(Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            var roleWithOverriddenTaxType = (await GetCustomerRolesAsync(customer)).FirstOrDefault(cr => cr.Active && cr.OverrideTaxDisplayType);
            if (roleWithOverriddenTaxType == null)
                return null;

            return (TaxDisplayType)roleWithOverriddenTaxType.DefaultTaxDisplayTypeId;
        }

        /// <summary>
        /// Get full name
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer full name
        /// </returns>
        public virtual Task<string> GetCustomerFullNameAsync(Customer customer)
        {
            var fullName = string.Empty;

            if (customer == null)
                return Task.FromResult(fullName);

            var firstName = customer.FirstName;
            var lastName = customer.LastName;

            if (!string.IsNullOrWhiteSpace(firstName) && !string.IsNullOrWhiteSpace(lastName))
                fullName = $"{firstName} {lastName}";
            else
            {
                if (!string.IsNullOrWhiteSpace(firstName))
                    fullName = firstName;

                if (!string.IsNullOrWhiteSpace(lastName))
                    fullName = lastName;
            }

            return Task.FromResult(fullName);
        }

        public virtual async Task<string> GetCustomerFullNameAsync(int customerId)
        {
            return await GetCustomerFullNameAsync(await GetCustomerByIdAsync(customerId));
        }

        /// <summary>
        /// Formats the customer name
        /// </summary>
        /// <param name="customer">Source</param>
        /// <param name="stripTooLong">Strip too long customer name</param>
        /// <param name="maxLength">Maximum customer name length</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the formatted text
        /// </returns>
        public virtual async Task<string> FormatUsernameAsync(Customer customer, bool stripTooLong = false, int maxLength = 0)
        {
            if (customer == null)
                return string.Empty;

            if (await IsGuestAsync(customer))
                //do not inject ILocalizationService via constructor because it'll cause circular references
                return await EngineContext.Current.Resolve<ILocalizationService>().GetResourceAsync("Customer.Guest");

            var result = string.Empty;
            switch (_customerSettings.CustomerNameFormat)
            {
                case CustomerNameFormat.ShowEmails:
                    result = customer.Email;
                    break;
                case CustomerNameFormat.ShowUsernames:
                    result = customer.Username;
                    break;
                case CustomerNameFormat.ShowFullNames:
                    result = await GetCustomerFullNameAsync(customer);
                    break;
                case CustomerNameFormat.ShowFirstName:
                    result = customer.FirstName;
                    break;
                default:
                    break;
            }

            if (stripTooLong && maxLength > 0)
                result = CommonHelper.EnsureMaximumLength(result, maxLength);

            return result;
        }

        /// <summary>
        /// Gets coupon codes
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the coupon codes
        /// </returns>
        public virtual async Task<string[]> ParseAppliedDiscountCouponCodesAsync(Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            var existingCouponCodes = await _genericAttributeService.GetAttributeAsync<string>(customer, ZarayeCustomerDefaults.DiscountCouponCodeAttribute);

            var couponCodes = new List<string>();
            if (string.IsNullOrEmpty(existingCouponCodes))
                return couponCodes.ToArray();

            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(existingCouponCodes);

                var nodeList1 = xmlDoc.SelectNodes(@"//DiscountCouponCodes/CouponCode");
                foreach (XmlNode node1 in nodeList1)
                {
                    if (node1.Attributes?["Code"] == null)
                        continue;
                    var code = node1.Attributes["Code"].InnerText.Trim();
                    couponCodes.Add(code);
                }
            }
            catch
            {
                // ignored
            }

            return couponCodes.ToArray();
        }

        /// <summary>
        /// Adds a coupon code
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="couponCode">Coupon code</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the new coupon codes document
        /// </returns>
        public virtual async Task ApplyDiscountCouponCodeAsync(Customer customer, string couponCode)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            var result = string.Empty;
            try
            {
                var existingCouponCodes = await _genericAttributeService.GetAttributeAsync<string>(customer, ZarayeCustomerDefaults.DiscountCouponCodeAttribute);

                couponCode = couponCode.Trim().ToLowerInvariant();

                var xmlDoc = new XmlDocument();
                if (string.IsNullOrEmpty(existingCouponCodes))
                {
                    var element1 = xmlDoc.CreateElement("DiscountCouponCodes");
                    xmlDoc.AppendChild(element1);
                }
                else
                    xmlDoc.LoadXml(existingCouponCodes);

                var rootElement = (XmlElement)xmlDoc.SelectSingleNode(@"//DiscountCouponCodes");

                XmlElement gcElement = null;
                //find existing
                var nodeList1 = xmlDoc.SelectNodes(@"//DiscountCouponCodes/CouponCode");
                foreach (XmlNode node1 in nodeList1)
                {
                    if (node1.Attributes?["Code"] == null)
                        continue;

                    var couponCodeAttribute = node1.Attributes["Code"].InnerText.Trim();

                    if (couponCodeAttribute.ToLowerInvariant() != couponCode.ToLowerInvariant())
                        continue;

                    gcElement = (XmlElement)node1;
                    break;
                }

                //create new one if not found
                if (gcElement == null)
                {
                    gcElement = xmlDoc.CreateElement("CouponCode");
                    gcElement.SetAttribute("Code", couponCode);
                    rootElement.AppendChild(gcElement);
                }

                result = xmlDoc.OuterXml;
            }
            catch
            {
                // ignored
            }

            //apply new value
            await _genericAttributeService.SaveAttributeAsync(customer, ZarayeCustomerDefaults.DiscountCouponCodeAttribute, result);
        }

        /// <summary>
        /// Removes a coupon code
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="couponCode">Coupon code to remove</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the new coupon codes document
        /// </returns>
        public virtual async Task RemoveDiscountCouponCodeAsync(Customer customer, string couponCode)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            //get applied coupon codes
            var existingCouponCodes = await ParseAppliedDiscountCouponCodesAsync(customer);

            //clear them
            await _genericAttributeService.SaveAttributeAsync<string>(customer, ZarayeCustomerDefaults.DiscountCouponCodeAttribute, null);

            //save again except removed one
            foreach (var existingCouponCode in existingCouponCodes)
                if (!existingCouponCode.Equals(couponCode, StringComparison.InvariantCultureIgnoreCase))
                    await ApplyDiscountCouponCodeAsync(customer, existingCouponCode);
        }

        /// <summary>
        /// Gets coupon codes
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the coupon codes
        /// </returns>
        public virtual async Task<string[]> ParseAppliedGiftCardCouponCodesAsync(Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            var existingCouponCodes = await _genericAttributeService.GetAttributeAsync<string>(customer, ZarayeCustomerDefaults.GiftCardCouponCodesAttribute);

            var couponCodes = new List<string>();
            if (string.IsNullOrEmpty(existingCouponCodes))
                return couponCodes.ToArray();

            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(existingCouponCodes);

                var nodeList1 = xmlDoc.SelectNodes(@"//GiftCardCouponCodes/CouponCode");
                foreach (XmlNode node1 in nodeList1)
                {
                    if (node1.Attributes?["Code"] == null)
                        continue;

                    var code = node1.Attributes["Code"].InnerText.Trim();
                    couponCodes.Add(code);
                }
            }
            catch
            {
                // ignored
            }

            return couponCodes.ToArray();
        }

        /// <summary>
        /// Adds a coupon code
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="couponCode">Coupon code</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the new coupon codes document
        /// </returns>
        public virtual async Task ApplyGiftCardCouponCodeAsync(Customer customer, string couponCode)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            var result = string.Empty;
            try
            {
                var existingCouponCodes = await _genericAttributeService.GetAttributeAsync<string>(customer, ZarayeCustomerDefaults.GiftCardCouponCodesAttribute);

                couponCode = couponCode.Trim().ToLowerInvariant();

                var xmlDoc = new XmlDocument();
                if (string.IsNullOrEmpty(existingCouponCodes))
                {
                    var element1 = xmlDoc.CreateElement("GiftCardCouponCodes");
                    xmlDoc.AppendChild(element1);
                }
                else
                    xmlDoc.LoadXml(existingCouponCodes);

                var rootElement = (XmlElement)xmlDoc.SelectSingleNode(@"//GiftCardCouponCodes");

                XmlElement gcElement = null;
                //find existing
                var nodeList1 = xmlDoc.SelectNodes(@"//GiftCardCouponCodes/CouponCode");
                foreach (XmlNode node1 in nodeList1)
                {
                    if (node1.Attributes?["Code"] == null)
                        continue;

                    var couponCodeAttribute = node1.Attributes["Code"].InnerText.Trim();
                    if (couponCodeAttribute.ToLowerInvariant() != couponCode.ToLowerInvariant())
                        continue;

                    gcElement = (XmlElement)node1;
                    break;
                }

                //create new one if not found
                if (gcElement == null)
                {
                    gcElement = xmlDoc.CreateElement("CouponCode");
                    gcElement.SetAttribute("Code", couponCode);
                    rootElement.AppendChild(gcElement);
                }

                result = xmlDoc.OuterXml;
            }
            catch
            {
                // ignored
            }

            //apply new value
            await _genericAttributeService.SaveAttributeAsync(customer, ZarayeCustomerDefaults.GiftCardCouponCodesAttribute, result);
        }

        /// <summary>
        /// Removes a coupon code
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="couponCode">Coupon code to remove</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the new coupon codes document
        /// </returns>
        public virtual async Task RemoveGiftCardCouponCodeAsync(Customer customer, string couponCode)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            //get applied coupon codes
            var existingCouponCodes = await ParseAppliedGiftCardCouponCodesAsync(customer);

            //clear them
            await _genericAttributeService.SaveAttributeAsync<string>(customer, ZarayeCustomerDefaults.GiftCardCouponCodesAttribute, null);

            //save again except removed one
            foreach (var existingCouponCode in existingCouponCodes)
                if (!existingCouponCode.Equals(couponCode, StringComparison.InvariantCultureIgnoreCase))
                    await ApplyGiftCardCouponCodeAsync(customer, existingCouponCode);
        }

        /// <summary>
        /// Returns a list of guids of not existing customers
        /// </summary>
        /// <param name="guids">The guids of the customers to check</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of guids not existing customers
        /// </returns>
        public virtual async Task<Guid[]> GetNotExistingCustomersAsync(Guid[] guids)
        {
            if (guids == null)
                throw new ArgumentNullException(nameof(guids));

            var query = _customerRepository.Table;
            var queryFilter = guids.Distinct().ToArray();
            //filtering by guid
            var filter = await query.Select(c => c.CustomerGuid)
                .Where(c => queryFilter.Contains(c))
                .ToListAsync();

            return queryFilter.Except(filter).ToArray();
        }

        #endregion

        #region Customer roles

        /// <summary>
        /// Add a customer-customer role mapping
        /// </summary>
        /// <param name="roleMapping">Customer-customer role mapping</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task AddCustomerRoleMappingAsync(CustomerCustomerRoleMapping roleMapping)
        {
            await _customerCustomerRoleMappingRepository.InsertAsync(roleMapping);
        }

        /// <summary>
        /// Remove a customer-customer role mapping
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="role">Customer role</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task RemoveCustomerRoleMappingAsync(Customer customer, CustomerRole role)
        {
            if (customer is null)
                throw new ArgumentNullException(nameof(customer));

            if (role is null)
                throw new ArgumentNullException(nameof(role));

            var mapping = await _customerCustomerRoleMappingRepository.Table
                .SingleOrDefaultAsync(ccrm => ccrm.CustomerId == customer.Id && ccrm.CustomerRoleId == role.Id);

            if (mapping != null)
                await _customerCustomerRoleMappingRepository.DeleteAsync(mapping);
        }

        /// <summary>
        /// Delete a customer role
        /// </summary>
        /// <param name="customerRole">Customer role</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteCustomerRoleAsync(CustomerRole customerRole)
        {
            if (customerRole == null)
                throw new ArgumentNullException(nameof(customerRole));

            if (customerRole.IsSystemRole)
                throw new ZarayeException("System role could not be deleted");

            await _customerRoleRepository.DeleteAsync(customerRole);
        }

        /// <summary>
        /// Gets a customer role
        /// </summary>
        /// <param name="customerRoleId">Customer role identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer role
        /// </returns>
        public virtual async Task<CustomerRole> GetCustomerRoleByIdAsync(int customerRoleId)
        {
            return await _customerRoleRepository.GetByIdAsync(customerRoleId, cache => default);
        }

        /// <summary>
        /// Gets a customer role
        /// </summary>
        /// <param name="systemName">Customer role system name</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer role
        /// </returns>
        public virtual async Task<CustomerRole> GetCustomerRoleBySystemNameAsync(string systemName)
        {
            if (string.IsNullOrWhiteSpace(systemName))
                return null;

            var key = _staticCacheManager.PrepareKeyForDefaultCache(ZarayeCustomerServicesDefaults.CustomerRolesBySystemNameCacheKey, systemName);

            var query = from cr in _customerRoleRepository.Table
                        orderby cr.Id
                        where cr.SystemName == systemName
                        select cr;

            var customerRole = await _staticCacheManager.GetAsync(key, async () => await query.FirstOrDefaultAsync());

            return customerRole;
        }

        /// <summary>
        /// Get customer role identifiers
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="showHidden">A value indicating whether to load hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer role identifiers
        /// </returns>
        public virtual async Task<int[]> GetCustomerRoleIdsAsync(Customer customer, bool showHidden = false)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            var query = from cr in _customerRoleRepository.Table
                        join crm in _customerCustomerRoleMappingRepository.Table on cr.Id equals crm.CustomerRoleId
                        where crm.CustomerId == customer.Id &&
                        (showHidden || cr.Active)
                        select cr.Id;

            var key = _staticCacheManager.PrepareKeyForShortTermCache(ZarayeCustomerServicesDefaults.CustomerRoleIdsCacheKey, customer, showHidden);

            return await _staticCacheManager.GetAsync(key, () => query.ToArray());
        }

        /// <summary>
        /// Gets list of customer roles
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="showHidden">A value indicating whether to load hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public virtual async Task<IList<CustomerRole>> GetCustomerRolesAsync(Customer customer, bool showHidden = false)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            return await _customerRoleRepository.GetAllAsync(query =>
            {
                return from cr in query
                       join crm in _customerCustomerRoleMappingRepository.Table on cr.Id equals crm.CustomerRoleId
                       where crm.CustomerId == customer.Id &&
                             (showHidden || cr.Active)
                       select cr;
            }, cache => cache.PrepareKeyForShortTermCache(ZarayeCustomerServicesDefaults.CustomerRolesCacheKey, customer, showHidden));

        }

        /// <summary>
        /// Gets all customer roles
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer roles
        /// </returns>
        public virtual async Task<IList<CustomerRole>> GetAllCustomerRolesAsync(bool showHidden = false)
        {
            var storeId = (await _storeContext.GetCurrentStoreAsync()).Id;
            var key = _staticCacheManager.PrepareKeyForDefaultCache(ZarayeCustomerServicesDefaults.CustomerRolesAllCacheKey, showHidden, storeId);

            var query = from cr in _customerRoleRepository.Table
                        orderby cr.Name
                        where showHidden || cr.Active
                        select cr;

            var customerRoles = await _staticCacheManager.GetAsync(key, async () => await query.ToListAsync());

            return customerRoles;
        }

        /// <summary>
        /// Inserts a customer role
        /// </summary>
        /// <param name="customerRole">Customer role</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertCustomerRoleAsync(CustomerRole customerRole)
        {
            await _customerRoleRepository.InsertAsync(customerRole);
        }

        /// <summary>
        /// Gets a value indicating whether customer is in a certain customer role
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="customerRoleSystemName">Customer role system name</param>
        /// <param name="onlyActiveCustomerRoles">A value indicating whether we should look only in active customer roles</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public virtual async Task<bool> IsInCustomerRoleAsync(Customer customer,
            string customerRoleSystemName, bool onlyActiveCustomerRoles = true)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            if (string.IsNullOrEmpty(customerRoleSystemName))
                throw new ArgumentNullException(nameof(customerRoleSystemName));

            var customerRoles = await GetCustomerRolesAsync(customer, !onlyActiveCustomerRoles);

            return customerRoles?.Any(cr => cr.SystemName == customerRoleSystemName) ?? false;
        }

        /// <summary>
        /// Gets a value indicating whether customer is administrator
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="onlyActiveCustomerRoles">A value indicating whether we should look only in active customer roles</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public virtual async Task<bool> IsAdminAsync(Customer customer, bool onlyActiveCustomerRoles = true)
        {
            return await IsInCustomerRoleAsync(customer, ZarayeCustomerDefaults.AdministratorsRoleName, onlyActiveCustomerRoles);
        }

        /// <summary>
        /// Gets a value indicating whether customer is a forum moderator
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="onlyActiveCustomerRoles">A value indicating whether we should look only in active customer roles</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public virtual async Task<bool> IsForumModeratorAsync(Customer customer, bool onlyActiveCustomerRoles = true)
        {
            return await IsInCustomerRoleAsync(customer, ZarayeCustomerDefaults.ForumModeratorsRoleName, onlyActiveCustomerRoles);
        }

        /// <summary>
        /// Gets a value indicating whether customer is registered
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="onlyActiveCustomerRoles">A value indicating whether we should look only in active customer roles</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public virtual async Task<bool> IsRegisteredAsync(Customer customer, bool onlyActiveCustomerRoles = true)
        {
            return await IsInCustomerRoleAsync(customer, ZarayeCustomerDefaults.RegisteredRoleName, onlyActiveCustomerRoles);
        }

        public virtual async Task<bool> IsBookerAsync(Customer customer, bool onlyActiveCustomerRoles = true)
        {
            return await IsInCustomerRoleAsync(customer, ZarayeCustomerDefaults.BookerRoleName, onlyActiveCustomerRoles);
        }
        public virtual async Task<bool> IsBrokerAsync(Customer customer, bool onlyActiveCustomerRoles = true)
        {
            return await IsInCustomerRoleAsync(customer, ZarayeCustomerDefaults.BrokerRoleName, onlyActiveCustomerRoles);
        }
        public virtual async Task<bool> IsDemandAsync(Customer customer, bool onlyActiveCustomerRoles = true)
        {
            return await IsInCustomerRoleAsync(customer, ZarayeCustomerDefaults.DemandRoleName, onlyActiveCustomerRoles);
        }

        public virtual async Task<bool> IsSupplyAsync(Customer customer, bool onlyActiveCustomerRoles = true)
        {
            return await IsInCustomerRoleAsync(customer, ZarayeCustomerDefaults.SupplyRoleName, onlyActiveCustomerRoles);
        }

        public virtual async Task<bool> IsOperationsAsync(Customer customer, bool onlyActiveCustomerRoles = true)
        {
            return await IsInCustomerRoleAsync(customer, ZarayeCustomerDefaults.OperationsRoleName, onlyActiveCustomerRoles);
        }

        public virtual async Task<bool> IsOrderBookerDemandAsync(Customer customer, bool onlyActiveCustomerRoles = true)
        {
            return await IsInCustomerRoleAsync(customer, ZarayeCustomerDefaults.OrderBookerDemandRoleName, onlyActiveCustomerRoles);
        }
        public virtual async Task<bool> IsDemandAssociateAsync(Customer customer, bool onlyActiveCustomerRoles = true)
        {
            return await IsInCustomerRoleAsync(customer, ZarayeCustomerDefaults.DemandAssociateRoleName, onlyActiveCustomerRoles);
        }

        public virtual async Task<bool> IsSupplyAssociateAsync(Customer customer, bool onlyActiveCustomerRoles = true)
        {
            return await IsInCustomerRoleAsync(customer, ZarayeCustomerDefaults.SupplyAssociateRoleName, onlyActiveCustomerRoles);
        }

        public virtual async Task<bool> IsGroundOperationsAsync(Customer customer, bool onlyActiveCustomerRoles = true)
        {
            return await IsInCustomerRoleAsync(customer, ZarayeCustomerDefaults.GroundOperationsRoleName, onlyActiveCustomerRoles);
        }

        public virtual async Task<bool> IsOPerationAssociateAsync(Customer customer, bool onlyActiveCustomerRoles = true)
        {
            return await IsInCustomerRoleAsync(customer, ZarayeCustomerDefaults.OPerationAssociateRoleName, onlyActiveCustomerRoles);
        }

        public virtual async Task<bool> IsOPerationLeadAsync(Customer customer, bool onlyActiveCustomerRoles = true)
        {
            return await IsInCustomerRoleAsync(customer, ZarayeCustomerDefaults.OPerationLeadRoleName, onlyActiveCustomerRoles);
        }

        public virtual async Task<bool> IsBusinessLeadAsync(Customer customer, bool onlyActiveCustomerRoles = true)
        {
            return await IsInCustomerRoleAsync(customer, ZarayeCustomerDefaults.BusinessLeadRoleName, onlyActiveCustomerRoles);
        }

        public virtual async Task<bool> IsControlTowerAsync(Customer customer, bool onlyActiveCustomerRoles = true)
        {
            return await IsInCustomerRoleAsync(customer, ZarayeCustomerDefaults.ControlTowerRoleName, onlyActiveCustomerRoles);
        }

        /// <summary>
        /// Gets a value indicating whether customer is guest
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="onlyActiveCustomerRoles">A value indicating whether we should look only in active customer roles</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public virtual async Task<bool> IsGuestAsync(Customer customer, bool onlyActiveCustomerRoles = true)
        {
            return await IsInCustomerRoleAsync(customer, ZarayeCustomerDefaults.GuestsRoleName, onlyActiveCustomerRoles);
        }

        public virtual async Task<bool> IsLiveOpsAsync(Customer customer, bool onlyActiveCustomerRoles = true)
        {
            return await IsInCustomerRoleAsync(customer, ZarayeCustomerDefaults.LiveOpsRoleName, onlyActiveCustomerRoles);
        }
        /// <summary>
        /// Updates the customer role
        /// </summary>
        /// <param name="customerRole">Customer role</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateCustomerRoleAsync(CustomerRole customerRole)
        {
            await _customerRoleRepository.UpdateAsync(customerRole);
        }

        #endregion

        #region Customer passwords

        /// <summary>
        /// Gets customer passwords
        /// </summary>
        /// <param name="customerId">Customer identifier; pass null to load all records</param>
        /// <param name="passwordFormat">Password format; pass null to load all records</param>
        /// <param name="passwordsToReturn">Number of returning passwords; pass null to load all records</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of customer passwords
        /// </returns>
        public virtual async Task<IList<CustomerPassword>> GetCustomerPasswordsAsync(int? customerId = null,
            PasswordFormat? passwordFormat = null, int? passwordsToReturn = null)
        {
            var query = _customerPasswordRepository.Table;

            //filter by customer
            if (customerId.HasValue)
                query = query.Where(password => password.CustomerId == customerId.Value);

            //filter by password format
            if (passwordFormat.HasValue)
                query = query.Where(password => password.PasswordFormatId == (int)passwordFormat.Value);

            //get the latest passwords
            if (passwordsToReturn.HasValue)
                query = query.OrderByDescending(password => password.CreatedOnUtc).Take(passwordsToReturn.Value);

            return await query.ToListAsync();
        }

        /// <summary>
        /// Get current customer password
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer password
        /// </returns>
        public virtual async Task<CustomerPassword> GetCurrentPasswordAsync(int customerId)
        {
            if (customerId == 0)
                return null;

            //return the latest password
            return (await GetCustomerPasswordsAsync(customerId, passwordsToReturn: 1)).FirstOrDefault();
        }

        /// <summary>
        /// Insert a customer password
        /// </summary>
        /// <param name="customerPassword">Customer password</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertCustomerPasswordAsync(CustomerPassword customerPassword)
        {
            await _customerPasswordRepository.InsertAsync(customerPassword);
        }

        /// <summary>
        /// Update a customer password
        /// </summary>
        /// <param name="customerPassword">Customer password</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateCustomerPasswordAsync(CustomerPassword customerPassword)
        {
            await _customerPasswordRepository.UpdateAsync(customerPassword);
        }

        /// <summary>
        /// Check whether password recovery token is valid
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="token">Token to validate</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public virtual async Task<bool> IsPasswordRecoveryTokenValidAsync(Customer customer, string token)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            var cPrt = await _genericAttributeService.GetAttributeAsync<string>(customer, ZarayeCustomerDefaults.PasswordRecoveryTokenAttribute);
            if (string.IsNullOrEmpty(cPrt))
                return false;

            if (!cPrt.Equals(token, StringComparison.InvariantCultureIgnoreCase))
                return false;

            return true;
        }

        /// <summary>
        /// Check whether password recovery link is expired
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public virtual async Task<bool> IsPasswordRecoveryLinkExpiredAsync(Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            if (_customerSettings.PasswordRecoveryLinkDaysValid == 0)
                return false;

            var generatedDate = await _genericAttributeService.GetAttributeAsync<DateTime?>(customer, ZarayeCustomerDefaults.PasswordRecoveryTokenDateGeneratedAttribute);
            if (!generatedDate.HasValue)
                return false;

            var daysPassed = (DateTime.UtcNow - generatedDate.Value).TotalDays;
            if (daysPassed > _customerSettings.PasswordRecoveryLinkDaysValid)
                return true;

            return false;
        }

        /// <summary>
        /// Check whether customer password is expired 
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the rue if password is expired; otherwise false
        /// </returns>
        public virtual async Task<bool> IsPasswordExpiredAsync(Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            //the guests don't have a password
            if (await IsGuestAsync(customer))
                return false;

            //password lifetime is disabled for user
            if (!(await GetCustomerRolesAsync(customer)).Any(role => role.Active && role.EnablePasswordLifetime))
                return false;

            //setting disabled for all
            if (_customerSettings.PasswordLifetime == 0)
                return false;

            var cacheKey = _staticCacheManager.PrepareKeyForShortTermCache(ZarayeCustomerServicesDefaults.CustomerPasswordLifetimeCacheKey, customer);

            //get current password usage time
            var currentLifetime = await _staticCacheManager.GetAsync(cacheKey, async () =>
            {
                var customerPassword = await GetCurrentPasswordAsync(customer.Id);
                //password is not found, so return max value to force customer to change password
                if (customerPassword == null)
                    return int.MaxValue;

                return (DateTime.UtcNow - customerPassword.CreatedOnUtc).Days;
            });

            return currentLifetime >= _customerSettings.PasswordLifetime;
        }

        #endregion

            #region Customer address mapping

        /// <summary>
        /// Remove a customer-address mapping record
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="address">Address</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task RemoveCustomerAddressAsync(Customer customer, Address address)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            if (await _customerAddressMappingRepository.Table
                .FirstOrDefaultAsync(m => m.AddressId == address.Id && m.CustomerId == customer.Id)
                is CustomerAddressMapping mapping)
            {
                if (customer.BillingAddressId == address.Id)
                    customer.BillingAddressId = null;
                if (customer.ShippingAddressId == address.Id)
                    customer.ShippingAddressId = null;

                await _customerAddressMappingRepository.DeleteAsync(mapping);
            }
        }

        /// <summary>
        /// Inserts a customer-address mapping record
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="address">Address</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertCustomerAddressAsync(Customer customer, Address address)
        {
            if (customer is null)
                throw new ArgumentNullException(nameof(customer));

            if (address is null)
                throw new ArgumentNullException(nameof(address));

            if (await _customerAddressMappingRepository.Table
                .FirstOrDefaultAsync(m => m.AddressId == address.Id && m.CustomerId == customer.Id)
                is null)
            {
                var mapping = new CustomerAddressMapping
                {
                    AddressId = address.Id,
                    CustomerId = customer.Id
                };

                await _customerAddressMappingRepository.InsertAsync(mapping);
            }
        }

        /// <summary>
        /// Gets a list of addresses mapped to customer
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public virtual async Task<IList<Address>> GetAddressesByCustomerIdAsync(int customerId)
        {
            var query = from address in _customerAddressRepository.Table
                        join cam in _customerAddressMappingRepository.Table on address.Id equals cam.AddressId
                        where cam.CustomerId == customerId
                        select address;

            var key = _staticCacheManager.PrepareKeyForShortTermCache(ZarayeCustomerServicesDefaults.CustomerAddressesCacheKey, customerId);

            return await _staticCacheManager.GetAsync(key, async () => await query.ToListAsync());
        }

        /// <summary>
        /// Gets a address mapped to customer
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="addressId">Address identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public virtual async Task<Address> GetCustomerAddressAsync(int customerId, int addressId)
        {
            if (customerId == 0 || addressId == 0)
                return null;

            var query = from address in _customerAddressRepository.Table
                        join cam in _customerAddressMappingRepository.Table on address.Id equals cam.AddressId
                        where cam.CustomerId == customerId && address.Id == addressId
                        select address;

            var key = _staticCacheManager.PrepareKeyForShortTermCache(ZarayeCustomerServicesDefaults.CustomerAddressCacheKey, customerId, addressId);

            return await _staticCacheManager.GetAsync(key, async () => await query.FirstOrDefaultAsync());
        }

        /// <summary>
        /// Gets a customer billing address
        /// </summary>
        /// <param name="customer">Customer identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public virtual async Task<Address> GetCustomerBillingAddressAsync(Customer customer)
        {
            if (customer is null)
                throw new ArgumentNullException(nameof(customer));

            return await GetCustomerAddressAsync(customer.Id, customer.BillingAddressId ?? 0);
        }

        /// <summary>
        /// Gets a customer shipping address
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public virtual async Task<Address> GetCustomerShippingAddressAsync(Customer customer)
        {
            if (customer is null)
                throw new ArgumentNullException(nameof(customer));

            return await GetCustomerAddressAsync(customer.Id, customer.ShippingAddressId ?? 0);
        }

        #endregion

        #region Vehicle portfolio

        public virtual async Task DeleteVehiclePortfolioAsync(VehiclePortfolio vehiclePortfolio)
        {
            await _vehiclePortfolioRepository.DeleteAsync(vehiclePortfolio, false);
        }

        public virtual async Task<VehiclePortfolio> GetVehiclePortfolioByIdAsync(int vehiclePortfolioId)
        {
            return await _vehiclePortfolioRepository.GetByIdAsync(vehiclePortfolioId);
        }

        public virtual async Task InsertVehiclePortfolioAsync(VehiclePortfolio vehiclePortfolio)
        {
            vehiclePortfolio.UpdatedOnUtc = DateTime.UtcNow;
            await _vehiclePortfolioRepository.InsertAsync(vehiclePortfolio);
        }
        public virtual async Task UpdateVehiclePortfolioAsync(VehiclePortfolio vehiclePortfolio)
        {
            vehiclePortfolio.UpdatedOnUtc = DateTime.UtcNow;
            await _vehiclePortfolioRepository.UpdateAsync(vehiclePortfolio);
        }

        public virtual async Task<IPagedList<VehiclePortfolio>> GetAllVehiclePortfolioAsync(string name = "",
          int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false, bool? overridePublished = null)
        {

            var query = _vehiclePortfolioRepository.Table;

            if (!showHidden)
                query = query.Where(c => c.Published);
            else if (overridePublished.HasValue)
                query = query.Where(c => c.Published == overridePublished.Value);

            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(c => c.Name.Contains(name));

            query = query.Where(c => !c.Deleted);

            var vehicles = await query.ToListAsync();

            //paging
            return new PagedList<VehiclePortfolio>(vehicles, pageIndex, pageSize);
        }


        #endregion

        #endregion

        #region User Type

        public virtual async Task DeleteUserTypeAsync(UserType userType)
        {
            await _userTypeRepository.DeleteAsync(userType, false);
        }

        public virtual async Task<UserType> GetUserTypeByIdAsync(int userTypeId)
        {
            return await _userTypeRepository.GetByIdAsync(userTypeId);
        }

        public virtual async Task InsertUserTypeAsync(UserType userType)
        {
            userType.UpdatedOnUtc = DateTime.UtcNow;
            await _userTypeRepository.InsertAsync(userType);
        }

        public virtual async Task UpdateUserTypeAsync(UserType userType)
        {
            userType.UpdatedOnUtc = DateTime.UtcNow;
            await _userTypeRepository.UpdateAsync(userType);
        }

        public virtual async Task<IPagedList<UserType>> GetAllUserTypesAsync(string type = "", int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            var query = from ut in _userTypeRepository.Table
                        where ut.Type == type
                        && !ut.Deleted
                        select ut;

            if (!showHidden)
                query = query.Where(c => c.Published);

            var userTypes = await query.ToListAsync();

            //paging
            return new PagedList<UserType>(userTypes, pageIndex, pageSize);
        }

        public virtual async Task<UserType> GetUserTypeByNameAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            var query = _userTypeRepository.Table.Where(b => b.Name == name);
            return await query.FirstOrDefaultAsync();
        }

        #endregion

        #region Transporter Vehicle Mapping

        public virtual async Task InsertTransporterVehicleMappingAsync(TransporterVehicleMapping TransporterVehicle)
        {
            TransporterVehicle.CreatedOnUtc = DateTime.UtcNow;
            TransporterVehicle.UpdatedOnUtc = DateTime.UtcNow;
            await _transporterVehicleMappingRepository.InsertAsync(TransporterVehicle);
        }

        public virtual async Task UpdateTransporterVehicleMappingAsync(TransporterVehicleMapping TransporterVehicle)
        {
            TransporterVehicle.UpdatedOnUtc = DateTime.UtcNow;
            await _transporterVehicleMappingRepository.UpdateAsync(TransporterVehicle);
        }

        public virtual async Task<TransporterVehicleMapping> GetTransporterVehicleMappingByIdAsync(int TransporterVehicleId)
        {
            return await _transporterVehicleMappingRepository.GetByIdAsync(TransporterVehicleId);
        }

        public virtual async Task<IPagedList<TransporterVehicleMapping>> GetTransporterVehicleByTransporterIdAsync(int TransporterId, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = from t in _transporterVehicleMappingRepository.Table
                        where t.TransporterId == TransporterId && !t.Deleted
                        select t;

            var TransporterVehicle = await query.ToPagedListAsync(pageIndex, pageSize);
            return TransporterVehicle;
        }

        public virtual async Task DeleteTransporterVehicleMappingAsync(TransporterVehicleMapping TransporterVehicle)
        {
            await _transporterVehicleMappingRepository.DeleteAsync(TransporterVehicle);
        }

        public virtual async Task<TransporterVehicleMapping> GetTransporterVehicleByVehicleNumberAsync(string vehiclenumber)
        {
            var query = from t in _transporterVehicleMappingRepository.Table
                        where t.VehicleNumber == vehiclenumber && !t.Deleted
                        select t;

            return await query.FirstOrDefaultAsync();
        }

        public virtual async Task<TransporterVehicleMapping> GetTransporterVehiclesByVehicleIdAsync(int vehicleId, int transporterId)
        {
            var query = from t in _transporterVehicleMappingRepository.Table
                        where t.VehicleId == vehicleId && t.TransporterId == transporterId && !t.Deleted
                        select t;

            return await query.FirstOrDefaultAsync();
        }

        #endregion

        #region TranspoterCost

        public virtual async Task DeleteTranspoterCostAsync(TransporterCost transporterCost)
        {
            await _transpoterCostRepository.DeleteAsync(transporterCost);
        }

        public virtual async Task<TransporterCost> GetTranspoterCostByIdAsync(int transporterCostId)
        {
            return await _transpoterCostRepository.GetByIdAsync(transporterCostId);
        }

        public virtual async Task<TransporterCost> GetTranspoterCostByTransporterIdAsync(int transporterId)
        {
            return await _transpoterCostRepository.GetByIdAsync(transporterId);
        }

        public virtual decimal GetTotalMonthlyCostByTransporterId(int transporterId)
        {
            if (transporterId == 0)
                return 0m;

            var query = _transpoterCostRepository.Table.Where(x => x.TransporterId == transporterId && x.Deleted == false);
            return query.Sum(x => x.MonthlyFixedCost);
        }

        public virtual async Task InsertTranspoterCostAsync(TransporterCost transporterCost)
        {
            await _transpoterCostRepository.InsertAsync(transporterCost);
        }

        public virtual async Task UpdateTranspoterCostAsync(TransporterCost transporterCost)
        {
            await _transpoterCostRepository.UpdateAsync(transporterCost);
        }

        public virtual async Task<IPagedList<TransporterCost>> GetTransporterCostByTransporterIdAsync(int TransporterId, int pageIndex = 0, int pageSize = int.MaxValue)
        {

            var query = from t in _transpoterCostRepository.Table
                        where t.TransporterId == TransporterId && t.Deleted != true
                        select t;

            var TransporterCost = await query.ToPagedListAsync(pageIndex, pageSize);
            return TransporterCost;
        }

        public virtual async Task<VehiclePortfolio> GetVehiclePortfolioByTransporterVehicleMappingIdAsync(int transporterId, int transporterVehicleMappingId)
        {
            var query = from vpm in _transporterVehicleMappingRepository.Table
                        join vp in _vehiclePortfolioRepository.Table on vpm.VehicleId equals vp.Id
                        where vpm.TransporterId == transporterId
                        select vp;

            var vehiclePortfolio = await query.FirstOrDefaultAsync();
            return vehiclePortfolio;
        }

        #endregion

        #region Customer Industry Mappings

        public async Task AddCustomerIndustryMappingAsync(CustomerIndustryMapping customerIndustryMapping)
        {
            await _customerIndustryMappingRepository.InsertAsync(customerIndustryMapping);
        }

        public async Task RemoveCustomerIndustryMappingAsync(Customer customer, Category industry)
        {
            if (customer is null)
                throw new ArgumentNullException(nameof(customer));

            if (industry is null)
                throw new ArgumentNullException(nameof(industry));

            var mapping = await _customerIndustryMappingRepository.Table
                .SingleOrDefaultAsync(ccrm => ccrm.CustomerId == customer.Id && ccrm.IndustryId == industry.Id);

            if (mapping != null)
                await _customerIndustryMappingRepository.DeleteAsync(mapping);
        }

        public virtual async Task<int[]> GetCustomerIndustryIdsAsync(Customer customer, bool showHidden = false)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            var query = from cr in _industryRepository.Table
                        join crm in _customerIndustryMappingRepository.Table on cr.Id equals crm.IndustryId
                        where crm.CustomerId == customer.Id
                        select cr.Id;

            var list = await query.ToListAsync();
            return list.ToArray();
        }

        public virtual async Task<IList<CustomerIndustryMapping>> GetCustomerIndustriesAsync(Customer customer, bool showHidden = false)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            var query = from cr in _customerIndustryMappingRepository.Table
                        where cr.CustomerId == customer.Id
                        select cr;

            return await query.ToListAsync();
        }
        public virtual async Task<IPagedList<CustomerIndustryMapping>> GetCustomerIndustryMappingsByIndustryIdAsync(int industryId,
           int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            if (industryId == 0)
                return new PagedList<CustomerIndustryMapping>(new List<CustomerIndustryMapping>(), pageIndex, pageSize);

            var query = from pc in _customerIndustryMappingRepository.Table
                        join p in _industryRepository.Table on pc.IndustryId equals p.Id
                        where pc.IndustryId == industryId && !p.Deleted
                        //orderby pc.Id
                        select pc;

            return await query.ToPagedListAsync(pageIndex, pageSize);
        }

        public virtual CustomerIndustryMapping FindCustomerIndustryMapping(IList<CustomerIndustryMapping> source, int industryId, int customerId)
        {
            foreach (var customerIndustryMapping in source)
                if (customerIndustryMapping.IndustryId == industryId && customerIndustryMapping.CustomerId == customerId)
                    return customerIndustryMapping;

            return null;
        }

        public virtual async Task DeleteCustomerIndustryMappingAsync(CustomerIndustryMapping customerIndustryMapping)
        {
            await _customerIndustryMappingRepository.DeleteAsync(customerIndustryMapping);
        }

        #endregion

        #region Bank Details
        public virtual async Task InsertBankDetailAsync(BankDetail bankDetail)
        {
            bankDetail.CreatedOnUtc = DateTime.UtcNow;
            await _bankDetailRepository.InsertAsync(bankDetail);
        }
        public virtual async Task UpdateBankDetailAsync(BankDetail bankDetail)
        {
            await _bankDetailRepository.UpdateAsync(bankDetail);
        }
        public virtual async Task DeleteBankDetailAsync(BankDetail bankDetail)
        {
            bankDetail.Deleted = true;
            await _bankDetailRepository.UpdateAsync(bankDetail);
        }
        public virtual async Task<BankDetail> GetBankDetailByIdAsync(int id)
        {
            return await _bankDetailRepository.GetByIdAsync(id);
        }
        public virtual async Task<IPagedList<BankDetail>> GetAllBankDetailAsync(int customerId = 0, bool showHidden = false, int pageIndex = 0,
            int pageSize = int.MaxValue, bool getOnlyTotalCount = false)
        {
            var model = await _bankDetailRepository.GetAllPagedAsync(query =>
            {
                query = query.Where(c => !c.Deleted);

                if (customerId > 0)
                    query = query.Where(c => c.CustomerId == customerId);

                if (!showHidden)
                    query = query.Where(t => t.Published);

                return query;
            }, pageIndex, pageSize, getOnlyTotalCount);
            return model;
        }

        public virtual async Task<BankDetail> GetBankDetailByBAAAsync(string bankname, string accountNumber, string accountTitle)
        {
            if (string.IsNullOrEmpty(bankname))
                return null;

            if (string.IsNullOrEmpty(accountNumber))
                return null;

            if (string.IsNullOrEmpty(accountTitle))
                return null;

            bankname = bankname.Trim();
            accountNumber = accountNumber.Trim();
            accountTitle = accountTitle.Trim();

            var query = from p in _bankDetailRepository.Table
                        orderby p.Id
                        where !p.Deleted &&
                        p.BankName == bankname && p.AccountNumber == accountNumber && p.AccountTitle == accountTitle
                        select p;
            var bankDetail = await query.FirstOrDefaultAsync();

            return bankDetail;
        }

        #endregion

        #region Online Reject Reason

        public virtual async Task InsertRejectedReasonAsync(OnlineLeadRejectReason rejectedReason)
        {
            await _rejectedReasonRepository.InsertAsync(rejectedReason);
        }

        public virtual async Task UpdateRejectedReasonAsync(OnlineLeadRejectReason rejectedReason)
        {
            await _rejectedReasonRepository.UpdateAsync(rejectedReason);
        }

        public virtual async Task<OnlineLeadRejectReason> GetRejectedReasonByIdAsync(int faqId)
        {
            return await _rejectedReasonRepository.GetByIdAsync(faqId);
        }

        public virtual async Task DeleteRejectedReasonAsync(OnlineLeadRejectReason rejectedReason)
        {  
            await _rejectedReasonRepository.DeleteAsync(rejectedReason);
        }

        public virtual async Task<IPagedList<OnlineLeadRejectReason>> GetAllRejectedReasonAsync(string name = "", int type = 0, bool? overridePublished = null,
            bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = from f in _rejectedReasonRepository.Table
                        where !f.Deleted
                        select f;

            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(x => x.Name.Contains(name));


            if (!showHidden)
                query = query.Where(br => br.Published);
            else if (overridePublished.HasValue)
                query = query.Where(c => c.Published == overridePublished.Value);

            var faqs = await query.ToPagedListAsync(pageIndex, pageSize);
            return faqs;
        }

        #endregion


        public virtual async Task<bool> IsBuyerAsync(Customer customer, bool onlyActiveCustomerRoles = true)
        {
            return await IsInCustomerRoleAsync(customer, ZarayeCustomerDefaults.BuyerRoleName, onlyActiveCustomerRoles);
        }

        public virtual async Task<bool> IsSupplierAsync(Customer customer, bool onlyActiveCustomerRoles = true)
        {
            return await IsInCustomerRoleAsync(customer, ZarayeCustomerDefaults.SupplierRoleName, onlyActiveCustomerRoles);
        }

        public virtual async Task<bool> IsFinanceAsync(Customer customer, bool onlyActiveCustomerRoles = true)
        {
            return await IsInCustomerRoleAsync(customer, ZarayeCustomerDefaults.FinanceRoleName, onlyActiveCustomerRoles);
        }
        public virtual async Task<bool> IsBusinessHeadAsync(Customer customer, bool onlyActiveCustomerRoles = true)
        {
            return await IsInCustomerRoleAsync(customer, ZarayeCustomerDefaults.BusinessHeadRoleName, onlyActiveCustomerRoles);
        }

        public virtual async Task<bool> IsFinanceHeadAsync(Customer customer, bool onlyActiveCustomerRoles = true)
        {
            return await IsInCustomerRoleAsync(customer, ZarayeCustomerDefaults.FinanceHeadRoleName, onlyActiveCustomerRoles);
        }

        public virtual async Task<bool> IsOpsHeadAsync(Customer customer, bool onlyActiveCustomerRoles = true)
        {
            return await IsInCustomerRoleAsync(customer, ZarayeCustomerDefaults.OpsHeadRoleName, onlyActiveCustomerRoles);
        }

        public virtual async Task<bool> IsTrasnporterAsync(Customer customer, bool onlyActiveCustomerRoles = true)
        {
            return await IsInCustomerRoleAsync(customer, ZarayeCustomerDefaults.TransporterRoleName, onlyActiveCustomerRoles);
        }
        public virtual async Task<bool> IsThirdPartyLedgerAsync(Customer customer, bool onlyActiveCustomerRoles = true)
        {
            return await IsInCustomerRoleAsync(customer, ZarayeCustomerDefaults.ThirdPartyLedgerCustomerName, onlyActiveCustomerRoles);
        }


        #region Applied Credit Customer
        
        public virtual async Task InsertAppliedCreditCustomerAsync(AppliedCreditCustomer appliedCredit)
        {
            await _appliedCreditCustomerRepository.InsertAsync(appliedCredit);
        }
        public virtual async Task UpdateAppliedCreditCustomerAsync(AppliedCreditCustomer appliedCredit)
        {
            await _appliedCreditCustomerRepository.UpdateAsync(appliedCredit);
        }
        public virtual async Task DeleteAppliedCreditCustomerAsync(AppliedCreditCustomer appliedCredit)
        {  
            await _appliedCreditCustomerRepository.UpdateAsync(appliedCredit);
        }
        public virtual async Task<AppliedCreditCustomer> GetAppliedCreditCustomerByIdAsync(int id)
        {
            return await _appliedCreditCustomerRepository.GetByIdAsync(id);
        }
        public virtual async Task<AppliedCreditCustomer> GetAppliedCreditCustomerByCustomerIdAsync(int customerId)
        {
                var query = from t in _appliedCreditCustomerRepository.Table
                            where t.CustomerId==customerId && !t.Deleted orderby t.Id descending
                            select t ;
            return await query.FirstOrDefaultAsync();

        }
        public virtual async Task<IPagedList<AppliedCreditCustomer>> GetAllAppliedCreditCustomerAsync(int customerId = 0,string fullname="",
            string cnic="",string phone="",bool showHidden = false, int pageIndex = 0,
            int pageSize = int.MaxValue, bool getOnlyTotalCount = false)
        {
            var model = await _appliedCreditCustomerRepository.GetAllPagedAsync(query =>
            {
                query = query.Where(c => !c.Deleted);

                if (customerId > 0)
                    query = query.Where(c => c.Id == customerId);

                //if (!string.IsNullOrEmpty(fullname))
                //    query = query.Where(c => c.FullName.Contains(fullname));

                //if (!string.IsNullOrEmpty(cnic))
                //    query = query.Where(c => c.Cnic.Contains(cnic));

                //if (!string.IsNullOrEmpty(phone))
                //    query = query.Where(c => c.RegisteredPhoneNumber.Contains(phone));


                return query;
            }, pageIndex, pageSize, getOnlyTotalCount);
            return model;
        }
        public virtual async Task InsertBankStatementAsync(BankStatement bankStatement)
        {
            await _bankStatementRepository.InsertAsync(bankStatement);
        }
        #endregion
    }
}