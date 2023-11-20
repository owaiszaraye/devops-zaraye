using Zaraye.Core;
using Zaraye.Core.Domain.CustomerLedgers;
using Zaraye.Core.Domain.Orders;
using Zaraye.Core.Domain.Shipping;
using Zaraye.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Zaraye.Services.CustomerLedgers
{
    public partial interface ICustomerLedgerService
    {
        #region Payment Methods

        Task<IPagedList<Payment>> GetAllPaymentsAsync(string paymentType, int paymentId = 0, int customerId = 0, string fullname = "", int modeOfPaymentId = 0,
                    DateTime? dateFrom = null, DateTime? dateTo = null, List<int> psIds = null,
                    int pageIndex = 0, int pageSize = int.MaxValue, bool showAll = false, bool checkRemainingAmount = false, string username = "", bool IsTransporter = false, bool IsLabour = false,bool isTransporterPayment = false);

        Task<Payment> GetPaymentByIdAsync(int Id);
        
        Task InsertPaymentAsync(Payment payment);
        
        Task UpdatePaymentAsync(Payment payment);
        
        Task DeletePaymentAsync(Payment payment);

        #region Payment Sheduler

        Task<IPagedList<PaymentJson>> GetAllPaymentForJsonAsync(string paymentType = "", int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false);

        #endregion


        #endregion

        #region CustomerLedger

        Task<CustomerLedger> GetCustomerLedgerByIdAsync(int Id);

        Task InsertCustomerLedgerAsync(CustomerLedger customerLedger);

        Task<CustomerLedger> GetCustomerLedgerByShipmentIdAndDescription(int customerId, string description, int? shipmentId, int? paymentId = null, int? inventoryInboundId = null);

        Task AddCustomerLedgerAsync(DateTime date, int customerId, string description, decimal debit, decimal credit, int? shipmentId = null, int? paymentId = null, int? inventoryInboundId = null, bool updateRecord = false);

        Task UpdateCustomerLedgerAsync(CustomerLedger customerLedger);

        Task DeleteCustomerLedgerAsync(CustomerLedger customerLedger);

        Task<IList<CustomerLedger>> GetAllTransporterLedgerByShipmentAsync(int shipmentId, string[] strArray);

        Task<IList<CustomerLedger>> GetAllLedgerByCustomerIdAsync(int customerId, int shipmentId = 0);

        Task<IPagedList<BuyerLedgerList>> GetAllBuyerLedgerAsync(string buyerName = "", DateTime? startDate = null, DateTime? endDate = null, int pageIndex = 0, int pageSize = int.MaxValue);

        Task<IPagedList<BrokerLedgerList>> GetAllBrokerLedgerAsync(string brokerName = "", DateTime? startDate = null, DateTime? endDate = null, int pageIndex = 0, int pageSize = int.MaxValue);

        Task<IPagedList<BrokerLedgerDetails>> GetBrokerLedgerDetailByIdAsync(int brokerId = 0, DateTime? startDate = null, DateTime? endDate = null, int pageIndex = 0, int pageSize = int.MaxValue);

        Task<IPagedList<TransporterLedgerList>> GetAllTransporterLedgerAsync(string transporterName = "", DateTime? startDate = null, DateTime? endDate = null, int pageIndex = 0, int pageSize = int.MaxValue);

        Task<IPagedList<TransporterLedgerDetails>> GetTransporterLedgerDetailByIdAsync(int transporterId = 0, DateTime? startDate = null, DateTime? endDate = null, int pageIndex = 0, int pageSize = int.MaxValue);

        Task<IPagedList<BuyerLedgerDetails>> GetCustomerLedgerDetailByIdAsync(int buyerId = 0, DateTime? startDate = null, DateTime? endDate = null, int pageIndex = 0, int pageSize = int.MaxValue);

        Task<IPagedList<SupplierLedgerList>> GetAllSupplierLedgerAsync(int supplierId = 0, string supplierName = "", DateTime? startDate = null, DateTime? endDate = null, int pageIndex = 0, int pageSize = int.MaxValue);

        Task<IPagedList<SupplierLedgerDetails>> GetCustomerLedgerDetailBySupplierIdAsync(int supplierId = 0, DateTime? startDate = null, DateTime? endDate = null, int pageIndex = 0, int pageSize = int.MaxValue);

        Task<IPagedList<LabourLedgerList>> GetAllLabourLedgerAsync(string labourName = "", DateTime? startDate = null, DateTime? endDate = null, int pageIndex = 0, int pageSize = int.MaxValue);

        Task<IPagedList<LabourLedgerDetails>> GetLabourLedgerDetailByIdAsync(int labourId = 0, DateTime? startDate = null, DateTime? endDate = null, int pageIndex = 0, int pageSize = int.MaxValue);
        




        decimal GetShipmentPaidAmount(int shipmentId, bool IsDeliveryCost = false);

        decimal GetShipmentRemainingAmount(Shipment shipment, bool IsDeliveryCost = false);

        #endregion

        #region Shipment Payment Mapping

        Task<IPagedList<ShipmentPaymentMapping>> GetAllShipmentPaymentMappingsAsync(int shipmentId = 0, int paymentId = 0,
            DateTime? dateFrom = null, DateTime? dateTo = null,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false, bool? IsDeliveryCost = null);
       
        Task<ShipmentPaymentMapping> GetShipmentPaymentMappingByIdAsync(int Id);

        Task InsertShipmentPaymentMappingAsync(ShipmentPaymentMapping shipmentPaymentMapping);

        Task UpdateShipmentPaymentMappingAsync(ShipmentPaymentMapping shipmentPaymentMapping);

        Task DeleteShipmentPaymentMappingAsync(ShipmentPaymentMapping shipmentPaymentMapping);

        Task CheckAndUpdateOrderPaymentStatusAsync(Order order);

        Task CheckAndUpdateShipmentPaymentStatusAsync(Shipment shipment);

        #endregion
    }
}
