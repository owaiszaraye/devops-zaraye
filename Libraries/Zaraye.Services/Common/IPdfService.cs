using Zaraye.Core.Domain.Catalog;
using Zaraye.Core.Domain.Localization;
using Zaraye.Core.Domain.Orders;
using Zaraye.Core.Domain.Shipping;
using Zaraye.Core.Domain.Stores;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Zaraye.Services.Common
{
    /// <summary>
    /// Customer service interface
    /// </summary>
    public partial interface IPdfService
    {
        /// <summary>
        /// Write PDF invoice to the specified stream
        /// </summary>
        /// <param name="stream">Stream to save PDF</param>
        /// <param name="order">Order</param>
        /// <param name="language">Language; null to use a language used when placing an order</param>
        /// <param name="store">Store</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// </returns>
        Task PrintOrderToPdfAsync(Stream stream, Order order, Language language = null, Store store = null);

        /// <summary>
        /// Write ZIP archive with invoices to the specified stream
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="orders">Orders</param>
        /// <param name="language">Language; null to use a language used when placing an order</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task PrintOrdersToPdfAsync(Stream stream, IList<Order> orders, Language language = null);

        /// <summary>
        /// Write packaging slip to the specified stream
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="shipment">Shipment</param>
        /// <param name="language">Language; null to use a language used when placing an order</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task PrintPackagingSlipToPdfAsync(Stream stream, Shipment shipment, Language language = null);

        /// <summary>
        /// Write ZIP archive with packaging slips to the specified stream
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="shipments">Shipments</param>
        /// <param name="language">Language; null to use a language used when placing an order</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task PrintPackagingSlipsToPdfAsync(Stream stream, IList<Shipment> shipments, Language language = null);

        /// <summary>
        /// Write PDF catalog to the specified stream
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="products">Products</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task PrintProductsToPdfAsync(Stream stream, IList<Product> products);

        /// <summary>
        /// Export an order to PDF and save to disk
        /// </summary>
        /// <param name="order">Order</param>
        /// <param name="language">Language identifier; null to use a language used when placing an order</param>
        /// <returns>
        /// The task result contains a path of generated file
        /// </returns>
        Task<string> SaveOrderPdfToDiskAsync(Order order, Language language = null);
    }
}