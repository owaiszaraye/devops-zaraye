using LinqToDB.Data;
using Zaraye.Core;
using Zaraye.Core.Domain.Catalog;
using Zaraye.Core.Domain.CustomerLedgers;
using Zaraye.Core.Domain.Customers;
using Zaraye.Core.Domain.Orders;
using Zaraye.Core.Domain.Payments;
using Zaraye.Core.Domain.Shipping;
using Zaraye.Data;
using Zaraye.Services.Customers;
using Zaraye.Services.Orders;
using Zaraye.Services.Shipping;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Zaraye.Services.CustomerLedgers
{
    public partial class CustomerLedgerService : ICustomerLedgerService
    {
        #region Fields

        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<Payment> _paymentRepository;
        private readonly IRepository<CustomerLedger> _customerLedgerRepository;
        private readonly IRepository<ShipmentPaymentMapping> _shipmentPaymentMappingRepository;
        private readonly IRepository<Shipment> _shipmentRepository;
        private readonly IZarayeDataProvider _zarayeDataProvider;
        private readonly IWorkContext _workContext;
        private readonly ICustomerService _customerService;
        private readonly IShipmentService _shipmentService;
        private readonly IOrderService _orderService;

        #endregion

        #region Ctor

        public CustomerLedgerService(
            IRepository<Customer> customerRepository,
            IRepository<Payment> paymentRepository,
            IRepository<CustomerLedger> customerLedgerRepository,
            IRepository<ShipmentPaymentMapping> shipmentPaymentMappingRepository,
            IRepository<Shipment> shipmentRepository,
            IZarayeDataProvider zarayeDataProvider,
            IWorkContext workContext,
            ICustomerService customerService,
            IShipmentService shipmentService,
            IOrderService orderService
            )
        {
            _customerRepository = customerRepository;
            _paymentRepository = paymentRepository;
            _customerLedgerRepository = customerLedgerRepository;
            _shipmentPaymentMappingRepository = shipmentPaymentMappingRepository;
            _shipmentRepository = shipmentRepository;
            _zarayeDataProvider = zarayeDataProvider;
            _workContext = workContext;
            _customerService = customerService;
            _shipmentService = shipmentService;
            _orderService = orderService;
        }

        #endregion

        #region Methods

        #region Payment Methods

        public virtual async Task<IPagedList<Payment>> GetAllPaymentsAsync(string paymentType, int paymentId = 0, int customerId = 0, string fullname = "", int modeOfPaymentId = 0,
            DateTime? dateFrom = null, DateTime? dateTo = null, List<int> psIds = null,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showAll = false, bool checkRemainingAmount = false, string username = "", bool IsTransporter = false, bool IsLabour = false, bool isTransporterPayment = false)
        {
            var currentCustomer = await _workContext.GetCurrentCustomerAsync();
            var isBusinessHead = false;

            if (isTransporterPayment)
            {
                if (await _customerService.IsOpsHeadAsync(currentCustomer))
                    isBusinessHead = true;
                else if (await _customerService.IsOPerationLeadAsync(currentCustomer))
                    isBusinessHead = true;
                else
                    isBusinessHead = false;
            }
            else
                await _customerService.IsBusinessHeadAsync(currentCustomer);

            var isFinance = await _customerService.IsFinanceAsync(currentCustomer);

            var payments = await _paymentRepository.GetAllPagedAsync(query =>
            {
                query = query.Where(c => c.PaymentType == paymentType);
                query = query.Where(c => !c.Deleted);

                if (dateFrom.HasValue)
                    query = query.Where(c => dateFrom.Value <= c.CreatedOnUtc);
                if (dateTo.HasValue)
                    query = query.Where(c => dateTo.Value >= c.CreatedOnUtc);

                if (paymentId > 0)
                    query = query.Where(c => paymentId == c.Id);


                if (modeOfPaymentId > 0)
                    query = query.Where(c => modeOfPaymentId == c.ModeOfPaymentId);

                if (!string.IsNullOrEmpty(fullname))
                    query = from o in query
                            join c in _customerRepository.Table on o.CustomerId equals c.Id
                            where c.FullName.Contains(fullname)
                            select o;
                query = query.Distinct();

                if (!string.IsNullOrEmpty(username))
                    query = from o in query
                            join c in _customerRepository.Table on o.CustomerId equals c.Id
                            where c.Username.Contains(username)
                            select o;
                query = query.Distinct();

                if (!showAll)
                {
                    if (customerId > 0)
                        query = query.Where(c => c.CustomerId == customerId);

                    if (isBusinessHead || isFinance)
                    {
                        // show all
                    }
                    else
                    {
                        query = query.Where(x => x.CreatedById == currentCustomer.Id);
                    }
                }
                else
                {
                    query = query.Where(c => c.CustomerId == customerId);
                }

                if (psIds != null && psIds.Any())
                    query = query.Where(p => psIds.Contains(p.PaymentStatusId));

                if (checkRemainingAmount)
                    query = query.Where(x => (x.Amount - x.AdjustAmount) > 0);

                if (IsTransporter)
                    query = query.Where(x => x.IsTransporter == IsTransporter);
                else if (IsLabour)
                    query = query.Where(x => x.IsLabour == IsLabour);
                else
                    query = query.Where(x => x.IsLabour == false && x.IsTransporter == false);



                query = query.OrderByDescending(c => c.Id);

                return query;
            }, pageIndex, pageSize);

            return payments;
        }

        public virtual async Task<Payment> GetPaymentByIdAsync(int Id)
        {
            return await _paymentRepository.GetByIdAsync(Id);
        }

        public virtual async Task InsertPaymentAsync(Payment payment)
        {
            await _paymentRepository.InsertAsync(payment);
        }

        public virtual async Task UpdatePaymentAsync(Payment payment)
        {
            await _paymentRepository.UpdateAsync(payment);
        }

        public virtual async Task DeletePaymentAsync(Payment payment)
        {
            await _paymentRepository.DeleteAsync(payment);
        }

        #endregion

        #region Payment Sheduler

        public virtual async Task<IPagedList<PaymentJson>> GetAllPaymentForJsonAsync(string paymentType = "", int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false)
        {
            var query = (from p in _paymentRepository.Table
                         join c in _customerRepository.Table on p.CustomerId equals c.Id
                         join cc in _customerRepository.Table on p.CreatedById equals cc.Id
                         where p.PaymentType == paymentType
                         orderby p.PaymentDateUtc descending
                         select new PaymentJson()
                         {
                             PaymentDate = p.PaymentDateUtc,
                             Amount = p.Amount,
                             BuyerId = p.CustomerId,
                             BuyerName = c.FullName,
                             SupplierId = p.CustomerId,
                             SupplierName = c.FullName,
                             PaymentStatusId = p.PaymentStatusId,
                             CreatedById = p.CreatedById,
                             CreatedByName = cc.FullName,
                             Id = p.Id
                         });
            return await query.ToPagedListAsync(pageIndex, pageSize, getOnlyTotalCount);
        }

        #endregion

        #region Customer Ledger

        public virtual async Task<CustomerLedger> GetCustomerLedgerByIdAsync(int Id)
        {
            return await _customerLedgerRepository.GetByIdAsync(Id);
        }

        public virtual async Task InsertCustomerLedgerAsync(CustomerLedger customerLedger)
        {
            await _customerLedgerRepository.InsertAsync(customerLedger);
        }

        public virtual async Task<CustomerLedger> GetCustomerLedgerByShipmentIdAndDescription(int customerId, string description, int? shipmentId, int? paymentId = null, int? inventoryInboundId = null)
        {
            var query = from cl in _customerLedgerRepository.Table
                        where cl.CustomerId == customerId && cl.Description == description && cl.ShipmentId == shipmentId && cl.PaymentId == paymentId && cl.InventoryId == inventoryInboundId
                        select cl;

            return await query.FirstOrDefaultAsync();
        }

        public virtual async Task AddCustomerLedgerAsync(DateTime date, int customerId, string description, decimal debit, decimal credit, int? shipmentId = null, int? paymentId = null, int? inventoryInboundId = null, bool updateRecord = false)
        {
            var ledger = await GetCustomerLedgerByShipmentIdAndDescription(customerId, description, shipmentId, paymentId, inventoryInboundId);
            if (ledger is null)
            {
                await InsertCustomerLedgerAsync(new CustomerLedger
                {
                    CustomerId = customerId,
                    ShipmentId = shipmentId,
                    PaymentId = paymentId,
                    Description = description,
                    Debit = debit,
                    Credit = credit,
                    Date = date,
                    InventoryId = inventoryInboundId,
                });
            }
            else
            {
                if (updateRecord)
                {
                    ledger.Debit = debit;
                    ledger.Credit = credit;

                    await UpdateCustomerLedgerAsync(ledger);
                }
            }
        }

        public virtual async Task UpdateCustomerLedgerAsync(CustomerLedger customerLedger)
        {
            await _customerLedgerRepository.UpdateAsync(customerLedger);
        }

        public virtual async Task DeleteCustomerLedgerAsync(CustomerLedger customerLedger)
        {
            await _customerLedgerRepository.DeleteAsync(customerLedger);
        }

        public virtual async Task<IList<CustomerLedger>> GetAllTransporterLedgerByShipmentAsync(int shipmentId, string[] strArray)
        {
            if (shipmentId <= 0)
                return new List<CustomerLedger>();

            var query = from cl in _customerLedgerRepository.Table
                        where strArray.Contains(cl.Description) && cl.ShipmentId == shipmentId && cl.ParentId == 0
                        orderby cl.Id
                        select cl;
            return await query.ToListAsync();
        }

        public virtual async Task<IList<CustomerLedger>> GetAllLedgerByCustomerIdAsync(int customerId, int shipmentId = 0)
        {
            if (customerId <= 0)
                return new List<CustomerLedger>();

            var query = from cl in _customerLedgerRepository.Table
                        where cl.CustomerId == customerId
                        select cl;
            if (shipmentId > 0)
                query = query.Where(x => x.ShipmentId == shipmentId);
            return await query.ToListAsync();
        }

        public virtual async Task<IPagedList<BuyerLedgerList>> GetAllBuyerLedgerAsync(string buyerName = "", DateTime? startDate = null, DateTime? endDate = null, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            DataParameter totalRecordsParam = new DataParameter("@total_records", LinqToDB.DataType.Int32);
            totalRecordsParam.Direction = ParameterDirection.Output;

            var ledger = await _zarayeDataProvider.QueryProcAsync<BuyerLedgerList>("get_buyer_ledgerlist",
                new DataParameter("@p_BuyerName", buyerName, LinqToDB.DataType.VarChar),
                new DataParameter("@p_FromDate", startDate.HasValue ? startDate.Value.ToString("yyyy-MM-dd") : null, LinqToDB.DataType.DateTime),
                new DataParameter("@p_ToDate", endDate.HasValue ? endDate.Value.ToString("yyyy-MM-dd") : null, LinqToDB.DataType.DateTime),
                new DataParameter("@p_page_number", pageIndex > 0 ? pageIndex.ToString() : "1", LinqToDB.DataType.Int32),
                new DataParameter("@p_page_size", pageSize.ToString(), LinqToDB.DataType.Int32),
                totalRecordsParam);

            return await ledger.AsQueryable().ToPagedListAsync(pageIndex, pageSize);
        }

        public virtual async Task<IPagedList<BrokerLedgerList>> GetAllBrokerLedgerAsync(string brokerName = "", DateTime? startDate = null, DateTime? endDate = null, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            DataParameter totalRecordsParam = new DataParameter("@total_records", LinqToDB.DataType.Int32);
            totalRecordsParam.Direction = ParameterDirection.Output;

            var ledger = await _zarayeDataProvider.QueryProcAsync<BrokerLedgerList>("get_broker_ledgerlist",
                new DataParameter("@p_BrokerName", brokerName, LinqToDB.DataType.VarChar),
                new DataParameter("@p_FromDate", startDate.HasValue ? startDate.Value.ToString("yyyy-MM-dd") : null, LinqToDB.DataType.DateTime),
                new DataParameter("@p_ToDate", endDate.HasValue ? endDate.Value.ToString("yyyy-MM-dd") : null, LinqToDB.DataType.DateTime),
                new DataParameter("@p_page_number", pageIndex > 0 ? pageIndex.ToString() : "1", LinqToDB.DataType.Int32),
                new DataParameter("@p_page_size", pageSize.ToString(), LinqToDB.DataType.Int32),
                totalRecordsParam);

            return await ledger.AsQueryable().ToPagedListAsync(pageIndex, pageSize);
        }

        public virtual async Task<IPagedList<BrokerLedgerDetails>> GetBrokerLedgerDetailByIdAsync(int brokerId = 0, DateTime? startDate = null, DateTime? endDate = null, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            DataParameter totalRecordsParam = new DataParameter("@total_records", LinqToDB.DataType.Int32);
            totalRecordsParam.Direction = ParameterDirection.Output;

            var buyerLedgerDetails = await _zarayeDataProvider.QueryProcAsync<BrokerLedgerDetails>("get_broker_ledgerdetail",
                new DataParameter("@p_BrokerId", brokerId, LinqToDB.DataType.Int32),
                new DataParameter("@p_FromDate", startDate.HasValue ? startDate.Value.ToString("yyyy-MM-dd") : null, LinqToDB.DataType.DateTime),
                new DataParameter("@p_ToDate", endDate.HasValue ? endDate.Value.ToString("yyyy-MM-dd") : null, LinqToDB.DataType.DateTime),
                new DataParameter("@p_page_number", pageIndex > 0 ? pageIndex.ToString() : "1", LinqToDB.DataType.Int32),
                new DataParameter("@p_page_size", pageSize.ToString(), LinqToDB.DataType.Int32),
                totalRecordsParam);

            return await buyerLedgerDetails.AsQueryable().ToPagedListAsync(pageIndex, pageSize);
        }

        public virtual async Task<IPagedList<LabourLedgerList>> GetAllLabourLedgerAsync(string labourName = "", DateTime? startDate = null, DateTime? endDate = null, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            DataParameter totalRecordsParam = new DataParameter("@total_records", LinqToDB.DataType.Int32);
            totalRecordsParam.Direction = ParameterDirection.Output;

            var ledger = await _zarayeDataProvider.QueryProcAsync<LabourLedgerList>("get_labour_ledgerlist",
                new DataParameter("@p_LabourName", labourName, LinqToDB.DataType.VarChar),
                new DataParameter("@p_FromDate", startDate.HasValue ? startDate.Value.ToString("yyyy-MM-dd") : null, LinqToDB.DataType.DateTime),
                new DataParameter("@p_ToDate", endDate.HasValue ? endDate.Value.ToString("yyyy-MM-dd") : null, LinqToDB.DataType.DateTime),
                new DataParameter("@p_page_number", pageIndex > 0 ? pageIndex.ToString() : "1", LinqToDB.DataType.Int32),
                new DataParameter("@p_page_size", pageSize.ToString(), LinqToDB.DataType.Int32),
                totalRecordsParam);

            return await ledger.AsQueryable().ToPagedListAsync(pageIndex, pageSize);
        }

        public virtual async Task<IPagedList<LabourLedgerDetails>> GetLabourLedgerDetailByIdAsync(int labourId = 0, DateTime? startDate = null, DateTime? endDate = null, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            DataParameter totalRecordsParam = new DataParameter("@total_records", LinqToDB.DataType.Int32);
            totalRecordsParam.Direction = ParameterDirection.Output;

            var buyerLedgerDetails = await _zarayeDataProvider.QueryProcAsync<LabourLedgerDetails>("get_labour_ledgerdetail",
                new DataParameter("@p_LabourId", labourId, LinqToDB.DataType.Int32),
                new DataParameter("@p_FromDate", startDate.HasValue ? startDate.Value.ToString("yyyy-MM-dd") : null, LinqToDB.DataType.DateTime),
                new DataParameter("@p_ToDate", endDate.HasValue ? endDate.Value.ToString("yyyy-MM-dd") : null, LinqToDB.DataType.DateTime),
                new DataParameter("@p_page_number", pageIndex > 0 ? pageIndex.ToString() : "1", LinqToDB.DataType.Int32),
                new DataParameter("@p_page_size", pageSize.ToString(), LinqToDB.DataType.Int32),
                totalRecordsParam);

            return await buyerLedgerDetails.AsQueryable().ToPagedListAsync(pageIndex, pageSize);
        }

        public virtual async Task<IPagedList<TransporterLedgerList>> GetAllTransporterLedgerAsync(string transporterName = "", DateTime? startDate = null, DateTime? endDate = null, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            DataParameter totalRecordsParam = new DataParameter("@total_records", LinqToDB.DataType.Int32);
            totalRecordsParam.Direction = ParameterDirection.Output;

            var ledger = await _zarayeDataProvider.QueryProcAsync<TransporterLedgerList>("get_transporter_ledgerlist",
                new DataParameter("@p_TransporterName", transporterName, LinqToDB.DataType.VarChar),
                new DataParameter("@p_FromDate", startDate.HasValue ? startDate.Value.ToString("yyyy-MM-dd") : null, LinqToDB.DataType.DateTime),
                new DataParameter("@p_ToDate", endDate.HasValue ? endDate.Value.ToString("yyyy-MM-dd") : null, LinqToDB.DataType.DateTime),
                new DataParameter("@p_page_number", pageIndex > 0 ? pageIndex.ToString() : "1", LinqToDB.DataType.Int32),
                new DataParameter("@p_page_size", pageSize.ToString(), LinqToDB.DataType.Int32),
                totalRecordsParam);

            return await ledger.AsQueryable().ToPagedListAsync(pageIndex, pageSize);
        }

        public virtual async Task<IPagedList<TransporterLedgerDetails>> GetTransporterLedgerDetailByIdAsync(int transporterId = 0, DateTime? startDate = null, DateTime? endDate = null, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            DataParameter totalRecordsParam = new DataParameter("@total_records", LinqToDB.DataType.Int32);
            totalRecordsParam.Direction = ParameterDirection.Output;

            var buyerLedgerDetails = await _zarayeDataProvider.QueryProcAsync<TransporterLedgerDetails>("get_transporter_ledgerdetail",
                new DataParameter("@p_TransporterId", transporterId, LinqToDB.DataType.Int32),
                new DataParameter("@p_FromDate", startDate.HasValue ? startDate.Value.ToString("yyyy-MM-dd") : null, LinqToDB.DataType.DateTime),
                new DataParameter("@p_ToDate", endDate.HasValue ? endDate.Value.ToString("yyyy-MM-dd") : null, LinqToDB.DataType.DateTime),
                new DataParameter("@p_page_number", pageIndex > 0 ? pageIndex.ToString() : "1", LinqToDB.DataType.Int32),
                new DataParameter("@p_page_size", pageSize.ToString(), LinqToDB.DataType.Int32),
                totalRecordsParam);

            return await buyerLedgerDetails.AsQueryable().ToPagedListAsync(pageIndex, pageSize);
        }

        public virtual async Task<IPagedList<BuyerLedgerDetails>> GetCustomerLedgerDetailByIdAsync(int buyerId = 0, DateTime? startDate = null, DateTime? endDate = null, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            DataParameter totalRecordsParam = new DataParameter("@total_records", LinqToDB.DataType.Int32);
            totalRecordsParam.Direction = ParameterDirection.Output;

            var buyerLedgerDetails = await _zarayeDataProvider.QueryProcAsync<BuyerLedgerDetails>("get_buyer_ledgerdetail",
                new DataParameter("@p_BuyerId", buyerId, LinqToDB.DataType.Int32),
                new DataParameter("@p_FromDate", startDate.HasValue ? startDate.Value.ToString("yyyy-MM-dd") : null, LinqToDB.DataType.DateTime),
                new DataParameter("@p_ToDate", endDate.HasValue ? endDate.Value.ToString("yyyy-MM-dd") : null, LinqToDB.DataType.DateTime),
                new DataParameter("@p_page_number", pageIndex > 0 ? pageIndex.ToString() : "1", LinqToDB.DataType.Int32),
                new DataParameter("@p_page_size", pageSize.ToString(), LinqToDB.DataType.Int32),
                totalRecordsParam);

            return await buyerLedgerDetails.AsQueryable().ToPagedListAsync(pageIndex, pageSize);
        }

        public virtual async Task<IPagedList<SupplierLedgerList>> GetAllSupplierLedgerAsync(int supplierId = 0, string supplierName = "", DateTime? startDate = null, DateTime? endDate = null, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            DataParameter totalRecordsParam = new DataParameter("@total_records", LinqToDB.DataType.Int32);
            totalRecordsParam.Direction = ParameterDirection.Output;

            var ledger = await _zarayeDataProvider.QueryProcAsync<SupplierLedgerList>("get_supplier_ledgerlist",
                new DataParameter("@p_SupplierId", supplierId > 0 ? supplierId : null, LinqToDB.DataType.Int32),
                new DataParameter("@p_SupplierName", supplierName, LinqToDB.DataType.VarChar),
                new DataParameter("@p_FromDate", startDate.HasValue ? startDate.Value.ToString("yyyy-MM-dd") : null, LinqToDB.DataType.DateTime),
                new DataParameter("@p_ToDate", endDate.HasValue ? endDate.Value.ToString("yyyy-MM-dd") : null, LinqToDB.DataType.DateTime),
                new DataParameter("@p_page_number", pageIndex > 0 ? pageIndex.ToString() : "1", LinqToDB.DataType.Int32),
                new DataParameter("@p_page_size", pageSize.ToString(), LinqToDB.DataType.Int32),
                totalRecordsParam);

            return await ledger.AsQueryable().ToPagedListAsync(pageIndex, pageSize);
        }

        public virtual async Task<IPagedList<SupplierLedgerDetails>> GetCustomerLedgerDetailBySupplierIdAsync(int supplierId = 0, DateTime? startDate = null, DateTime? endDate = null, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            DataParameter totalRecordsParam = new DataParameter("@total_records", LinqToDB.DataType.Int32);
            totalRecordsParam.Direction = ParameterDirection.Output;

            var supplierLedgerDetails = await _zarayeDataProvider.QueryProcAsync<SupplierLedgerDetails>("get_supplier_ledgerdetail",
                new DataParameter("@p_SupplierId", supplierId, LinqToDB.DataType.Int32),
                new DataParameter("@p_FromDate", startDate.HasValue ? startDate.Value.ToString("yyyy-MM-dd") : null, LinqToDB.DataType.DateTime),
                new DataParameter("@p_ToDate", endDate.HasValue ? endDate.Value.ToString("yyyy-MM-dd") : null, LinqToDB.DataType.DateTime),
                new DataParameter("@p_page_number", pageIndex > 0 ? pageIndex.ToString() : "1", LinqToDB.DataType.Int32),
                new DataParameter("@p_page_size", pageSize.ToString(), LinqToDB.DataType.Int32),
                totalRecordsParam);

            return await supplierLedgerDetails.AsQueryable().ToPagedListAsync(pageIndex, pageSize);
        }

        public virtual decimal GetShipmentPaidAmount(int shipmentId, bool IsDeliveryCost = false)
        {
            if (!IsDeliveryCost)
                return (from spm in _shipmentPaymentMappingRepository.Table
                        where spm.ShipmentId == shipmentId && spm.IsDeliveryCost == false
                        select spm).Sum(x => x.Amount);
            else
                return (from spm in _shipmentPaymentMappingRepository.Table
                        where spm.ShipmentId == shipmentId && spm.IsDeliveryCost == true
                        select spm).Sum(x => x.Amount);
        }

        public virtual decimal GetShipmentRemainingAmount(Shipment shipment, bool IsDeliveryCost = false)
        {
            if (shipment is null)
                throw new ArgumentNullException(nameof(shipment) + " is null ");

            if (!IsDeliveryCost)
            {
                var paidAmound = (from spm in _shipmentPaymentMappingRepository.Table
                                  where spm.ShipmentId == shipment.Id && spm.IsDeliveryCost == false
                                  select spm).Sum(x => x.Amount);

                return shipment.DeliveredAmount - paidAmound;
            }
            else
            {
                var paidAmound = (from spm in _shipmentPaymentMappingRepository.Table
                                  where spm.ShipmentId == shipment.Id && spm.IsDeliveryCost == true
                                  select spm).Sum(x => x.Amount);

                return shipment.DeliveryCost - paidAmound;
            }
        }

        #endregion

        #region Shipment Payment Mapping

        public virtual async Task<IPagedList<ShipmentPaymentMapping>> GetAllShipmentPaymentMappingsAsync(int shipmentId = 0, int paymentId = 0,
            DateTime? dateFrom = null, DateTime? dateTo = null,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false, bool? IsDeliveryCost = null)
        {
            var customerLedgers = await _shipmentPaymentMappingRepository.GetAllPagedAsync(query =>
            {
                if (dateFrom.HasValue)
                    query = query.Where(c => dateFrom.Value <= c.CreatedOnUtc);

                if (dateTo.HasValue)
                    query = query.Where(c => dateTo.Value >= c.CreatedOnUtc);

                if (paymentId > 0)
                    query = query.Where(c => paymentId == c.PaymentId);

                if (shipmentId > 0)
                    query = query.Where(c => shipmentId == c.ShipmentId);

                if (IsDeliveryCost.HasValue)
                {
                    if (IsDeliveryCost.Value)
                        query = query.Where(c => c.IsDeliveryCost == true);
                    else
                        query = query.Where(c => c.IsDeliveryCost == false);
                }

                query = query.OrderBy(c => c.Id);

                return query;
            }, pageIndex, pageSize);

            return customerLedgers;
        }

        public virtual async Task<ShipmentPaymentMapping> GetShipmentPaymentMappingByIdAsync(int Id)
        {
            return await _shipmentPaymentMappingRepository.GetByIdAsync(Id);
        }

        public virtual async Task InsertShipmentPaymentMappingAsync(ShipmentPaymentMapping shipmentPaymentMapping)
        {
            await _shipmentPaymentMappingRepository.InsertAsync(shipmentPaymentMapping);
        }

        public virtual async Task UpdateShipmentPaymentMappingAsync(ShipmentPaymentMapping shipmentPaymentMapping)
        {
            await _shipmentPaymentMappingRepository.UpdateAsync(shipmentPaymentMapping);
        }

        public virtual async Task DeleteShipmentPaymentMappingAsync(ShipmentPaymentMapping shipmentPaymentMapping)
        {
            await _shipmentPaymentMappingRepository.DeleteAsync(shipmentPaymentMapping);
        }

        public virtual async Task CheckAndUpdateOrderPaymentStatusAsync(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            var orderCalculation = await _orderService.GetOrderCalculationByOrderIdAsync(order.Id);
            if (orderCalculation == null)
                throw new ArgumentNullException(nameof(order));

            decimal orderPaidAmount = await _shipmentService.GetOrderPaidAmount(order);
            if (orderPaidAmount >= orderCalculation.OrderTotal)
            {
                order.PaymentStatusId = (int)PaymentStatus.Paid;
                await _orderService.UpdateOrderAsync(order);
            }
            else
            {
                if (orderPaidAmount > 0)
                {
                    order.PaymentStatusId = (int)PaymentStatus.PartiallyPaid;
                    await _orderService.UpdateOrderAsync(order);
                }
                else
                {
                    order.PaymentStatusId = (int)PaymentStatus.Pending;
                    await _orderService.UpdateOrderAsync(order);
                }
            }
        }

        public virtual async Task CheckAndUpdateShipmentPaymentStatusAsync(Shipment shipment)
        {
            if (shipment == null)
                throw new ArgumentNullException(nameof(shipment));

            decimal shipmentPaidAmount = GetShipmentPaidAmount(shipment.Id);
            if (shipmentPaidAmount >= shipment.DeliveredAmount)
            {
                shipment.PaymentStatusId = (int)PaymentStatus.Paid;
                await _shipmentRepository.UpdateAsync(shipment);
            }
            else
            {
                if (shipmentPaidAmount > 0)
                {
                    shipment.PaymentStatusId = (int)PaymentStatus.PartiallyPaid;
                    await _shipmentRepository.UpdateAsync(shipment);
                }
                else
                {
                    shipment.PaymentStatusId = (int)PaymentStatus.Pending;
                    await _shipmentRepository.UpdateAsync(shipment);
                }
            }
        }

        #endregion

        #endregion
    }
}
