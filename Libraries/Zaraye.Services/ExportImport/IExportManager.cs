using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Zaraye.Core.Domain.Catalog;
using Zaraye.Core.Domain.CustomerLedgers;
using Zaraye.Core.Domain.Customers;
using Zaraye.Core.Domain.Directory;
using Zaraye.Core.Domain.Messages;
using Zaraye.Core.Domain.Orders;
using Zaraye.Services.ExportImport.Help;

namespace Zaraye.Services.ExportImport
{
    /// <summary>
    /// Export manager interface
    /// </summary>
    public partial interface IExportManager
    {
        /// <summary>
        /// Export manufacturer list to XML
        /// </summary>
        /// <param name="manufacturers">Manufacturers</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result in XML format
        /// </returns>
        Task<string> ExportManufacturersToXmlAsync(IList<Manufacturer> manufacturers);

        /// <summary>
        /// Export manufacturers to XLSX
        /// </summary>
        /// <param name="manufacturers">Manufactures</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task<byte[]> ExportManufacturersToXlsxAsync(IEnumerable<Manufacturer> manufacturers);

        /// <summary>
        /// Export category list to XML
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result in XML format
        /// </returns>
        Task<string> ExportCategoriesToXmlAsync();

        /// <summary>
        /// Export categories to XLSX
        /// </summary>
        /// <param name="categories">Categories</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task<byte[]> ExportCategoriesToXlsxAsync(IList<Category> categories);

        Task<byte[]> ExportIndustriesToXlsxAsync(IList<Industry> industries);

        /// <summary>
        /// Export product list to XML
        /// </summary>
        /// <param name="products">Products</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result in XML format
        /// </returns>
        Task<string> ExportProductsToXmlAsync(IList<Product> products);

        /// <summary>
        /// Export products to XLSX
        /// </summary>
        /// <param name="products">Products</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task<byte[]> ExportProductsToXlsxAsync(IEnumerable<Product> products);

        /// <summary>
        /// Export order list to XML
        /// </summary>
        /// <param name="orders">Orders</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result in XML format
        /// </returns>
        Task<string> ExportOrdersToXmlAsync(IList<Order> orders);

        /// <summary>
        /// Export orders to XLSX
        /// </summary>
        /// <param name="orders">Orders</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task<byte[]> ExportOrdersToXlsxAsync(IList<Order> orders);

        Task<byte[]> ExportCampaignEmailsToXlsxAsync(IList<CampaignEmail> campaignEmails);

        /// <summary>
        /// Export customer list to XLSX
        /// </summary>
        /// <param name="customers">Customers</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task<byte[]> ExportCustomersToXlsxAsync(IList<Customer> customers);

        /// <summary>
        /// Export customer list to XML
        /// </summary>
        /// <param name="customers">Customers</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result in XML format
        /// </returns>
        Task<string> ExportCustomersToXmlAsync(IList<Customer> customers);

        /// <summary>
        /// Export newsletter subscribers to TXT
        /// </summary>
        /// <param name="subscriptions">Subscriptions</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result in TXT (string) format
        /// </returns>
        Task<string> ExportNewsletterSubscribersToTxtAsync(IList<NewsLetterSubscription> subscriptions);

        /// <summary>
        /// Export states to TXT
        /// </summary>
        /// <param name="states">States</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result in TXT (string) format
        /// </returns>
        Task<string> ExportStatesToTxtAsync(IList<StateProvince> states);

        /// <summary>
        /// Export customer info (GDPR request) to XLSX 
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="storeId">Store identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer GDPR info
        /// </returns>
        Task<byte[]> ExportCustomerGdprInfoToXlsxAsync(Customer customer, int storeId);

        Task<byte[]> ExportBookersToXlsxAsync(IList<Customer> customers);


        Task<byte[]> ExportTransportersToXlsxAsync(IList<Customer> customers);

        Task<byte[]> ExportSupportAgentsToXlsxAsync(IList<Customer> customers);

        Task<byte[]> ExportBrokersToXlsxAsync(IList<Customer> customers);

        Task<byte[]> ExportBuyersToXlsxAsync(IList<Customer> customers);
        
        Task<byte[]> ExportSuppliersToXlsxAsync(IList<Customer> customers);

        Task<byte[]> ExportOnlineLeadsToXlsxAsync(IList<OnlineLead> onlineLeads);

        Task<byte[]> ExportBuyerLedgerRecordsToXlsxAsync(IList<BuyerLedgerDetails> buyerLedgers);

        Task<byte[]> ExportSupplierLedgerRecordsToXlsxAsync(IList<SupplierLedgerDetails> supplierLedgers);

        Task<byte[]> ExportBrokerLedgerRecordsToXlsxAsync(IList<BrokerLedgerDetails> brokerLedgers);

        Task<byte[]> ExportTransporterLedgerRecordsToXlsxAsync(IList<TransporterLedgerDetails> transporterLedgers);

        Task<byte[]> ExportLabourLedgerRecordsToXlsxAsync(IList<LabourLedgerDetails> labourLedgers);



    }
}
