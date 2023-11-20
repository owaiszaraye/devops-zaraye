using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Zaraye.Core;
using Zaraye.Core.Domain.Catalog;
using Zaraye.Core.Domain.Common;
using Zaraye.Core.Domain.Customers;
using Zaraye.Core.Domain.Discounts;
using Zaraye.Core.Domain.Orders;
using Zaraye.Core.Domain.Shipping;
using Zaraye.Core.Domain.Tax;
using Zaraye.Services.Catalog;
using Zaraye.Services.Common;
using Zaraye.Services.Customers;
using Zaraye.Services.Discounts;
using Zaraye.Services.Payments;
using Zaraye.Services.Shipping;

namespace Zaraye.Services.Orders
{
    /// <summary>
    /// Order service
    /// </summary>
    public partial class OrderTotalCalculationService : IOrderTotalCalculationService
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly IAddressService _addressService;
        private readonly ICustomerService _customerService;
        private readonly IDiscountService _discountService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IOrderService _orderService;
        private readonly IPaymentService _paymentService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IProductService _productService;
        private readonly IRewardPointService _rewardPointService;
        private readonly IShippingService _shippingService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly ShippingSettings _shippingSettings;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly TaxSettings _taxSettings;

        #endregion

        #region Ctor

        public OrderTotalCalculationService(CatalogSettings catalogSettings,
            IAddressService addressService,
            ICustomerService customerService,
            IDiscountService discountService,
            IGenericAttributeService genericAttributeService,
            IOrderService orderService,
            IPaymentService paymentService,
            IPriceCalculationService priceCalculationService,
            IProductService productService,
            IRewardPointService rewardPointService,
            IShippingService shippingService,
            IShoppingCartService shoppingCartService,
            IStoreContext storeContext,
            IWorkContext workContext,
            RewardPointsSettings rewardPointsSettings,
            ShippingSettings shippingSettings,
            ShoppingCartSettings shoppingCartSettings,
            TaxSettings taxSettings)
        {
            _catalogSettings = catalogSettings;
            _addressService = addressService;
            _customerService = customerService;
            _discountService = discountService;
            _genericAttributeService = genericAttributeService;
            _orderService = orderService;
            _paymentService = paymentService;
            _priceCalculationService = priceCalculationService;
            _productService = productService;
            _rewardPointService = rewardPointService;
            _shippingService = shippingService;
            _shoppingCartService = shoppingCartService;
            _storeContext = storeContext;
            _workContext = workContext;
            _rewardPointsSettings = rewardPointsSettings;
            _shippingSettings = shippingSettings;
            _shoppingCartSettings = shoppingCartSettings;
            _taxSettings = taxSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Gets an order discount (applied to order subtotal)
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="orderSubTotal">Order subtotal</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the order discount, Applied discounts
        /// </returns>
        protected virtual async Task<(decimal orderDiscount, List<Discount> appliedDiscounts)> GetOrderSubtotalDiscountAsync(Customer customer,
            decimal orderSubTotal)
        {
            var appliedDiscounts = new List<Discount>();
            var discountAmount = decimal.Zero;
            if (_catalogSettings.IgnoreDiscounts)
                return (discountAmount, appliedDiscounts);

            var allDiscounts = await _discountService.GetAllDiscountsAsync(DiscountType.AssignedToOrderSubTotal);
            var allowedDiscounts = new List<Discount>();
            if (allDiscounts != null)
            {
                foreach (var discount in allDiscounts)
                    if (!_discountService.ContainsDiscount(allowedDiscounts, discount) &&
                        (await _discountService.ValidateDiscountAsync(discount, customer)).IsValid)
                    {
                        allowedDiscounts.Add(discount);
                    }
            }

            appliedDiscounts = _discountService.GetPreferredDiscount(allowedDiscounts, orderSubTotal, out discountAmount);

            if (discountAmount < decimal.Zero)
                discountAmount = decimal.Zero;

            return (discountAmount, appliedDiscounts);
        }

        /// <summary>
        /// Gets a shipping discount
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="shippingTotal">Shipping total</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the shipping discount. Applied discounts
        /// </returns>
        protected virtual async Task<(decimal shippingDiscount, List<Discount> appliedDiscounts)> GetShippingDiscountAsync(Customer customer, decimal shippingTotal)
        {
            var appliedDiscounts = new List<Discount>();
            var shippingDiscountAmount = decimal.Zero;
            if (_catalogSettings.IgnoreDiscounts)
                return (shippingDiscountAmount, appliedDiscounts);

            var allDiscounts = await _discountService.GetAllDiscountsAsync(DiscountType.AssignedToShipping);
            var allowedDiscounts = new List<Discount>();
            if (allDiscounts != null)
                foreach (var discount in allDiscounts)
                    if (!_discountService.ContainsDiscount(allowedDiscounts, discount) &&
                        (await _discountService.ValidateDiscountAsync(discount, customer)).IsValid)
                    {
                        allowedDiscounts.Add(discount);
                    }

            appliedDiscounts = _discountService.GetPreferredDiscount(allowedDiscounts, shippingTotal, out shippingDiscountAmount);

            if (shippingDiscountAmount < decimal.Zero)
                shippingDiscountAmount = decimal.Zero;

            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                shippingDiscountAmount = await _priceCalculationService.RoundPriceAsync(shippingDiscountAmount);

            return (shippingDiscountAmount, appliedDiscounts);
        }

        /// <summary>
        /// Gets an order discount (applied to order total)
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="orderTotal">Order total</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the order discount. Applied discounts
        /// </returns>
        protected virtual async Task<(decimal orderDiscount, List<Discount> appliedDiscounts)> GetOrderTotalDiscountAsync(Customer customer, decimal orderTotal)
        {
            var appliedDiscounts = new List<Discount>();
            var discountAmount = decimal.Zero;
            if (_catalogSettings.IgnoreDiscounts)
                return (discountAmount, appliedDiscounts);

            var allDiscounts = await _discountService.GetAllDiscountsAsync(DiscountType.AssignedToOrderTotal);
            var allowedDiscounts = new List<Discount>();
            if (allDiscounts != null)
                foreach (var discount in allDiscounts)
                    if (!_discountService.ContainsDiscount(allowedDiscounts, discount) &&
                        (await _discountService.ValidateDiscountAsync(discount, customer)).IsValid)
                    {
                        allowedDiscounts.Add(discount);
                    }

            appliedDiscounts = _discountService.GetPreferredDiscount(allowedDiscounts, orderTotal, out discountAmount);

            if (discountAmount < decimal.Zero)
                discountAmount = decimal.Zero;

            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                discountAmount = await _priceCalculationService.RoundPriceAsync(discountAmount);

            return (discountAmount, appliedDiscounts);
        }

        /// <summary>
        /// Update order total
        /// </summary>
        /// <param name="updateOrderParameters">UpdateOrderParameters</param>
        /// <param name="subTotalExclTax">Subtotal (excl tax)</param>
        /// <param name="discountAmountExclTax">Discount amount (excl tax)</param>
        /// <param name="shippingTotalExclTax">Shipping (excl tax)</param>
        /// <param name="taxTotal">Tax</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task UpdateTotalAsync(UpdateOrderParameters updateOrderParameters, decimal subTotalExclTax,
            decimal discountAmountExclTax, decimal shippingTotalExclTax, decimal taxTotal)
        {
            var updatedOrder = updateOrderParameters.UpdatedOrder;
            var customer = await _customerService.GetCustomerByIdAsync(updatedOrder.CustomerId);

            var total = subTotalExclTax - discountAmountExclTax + shippingTotalExclTax + updatedOrder.PaymentMethodAdditionalFeeExclTax + taxTotal;

            //get discounts for the order total
            var (discountAmountTotal, orderAppliedDiscounts) = await GetOrderTotalDiscountAsync(customer, total);
            if (total < discountAmountTotal)
                discountAmountTotal = total;
            total -= discountAmountTotal;

            //reward points
            var rewardPointsOfOrder = await _rewardPointService.GetRewardPointsHistoryEntryByIdAsync(updatedOrder.RedeemedRewardPointsEntryId ?? 0);
            if (rewardPointsOfOrder != null)
            {
                var rewardPoints = -rewardPointsOfOrder.Points;
                var rewardPointsAmount = await ConvertRewardPointsToAmountAsync(rewardPoints);
                if (total < rewardPointsAmount)
                {
                    rewardPoints = ConvertAmountToRewardPoints(total);
                    rewardPointsAmount = total;
                }

                if (total > decimal.Zero)
                    total -= rewardPointsAmount;

                //uncomment here for the return unused reward points if new order total less redeemed reward points amount
                //if (rewardPoints < -rewardPointsOfOrder.Points)
                //    _rewardPointService.AddRewardPointsHistoryEntry(customer, -rewardPointsOfOrder.Points - rewardPoints, store.Id, "Return unused reward points");

                if (rewardPointsAmount != rewardPointsOfOrder.UsedAmount)
                {
                    rewardPointsOfOrder.UsedAmount = rewardPointsAmount;
                    rewardPointsOfOrder.Points = -rewardPoints;
                    await _rewardPointService.UpdateRewardPointsHistoryEntryAsync(rewardPointsOfOrder);
                }
            }

            //rounding
            if (total < decimal.Zero)
                total = decimal.Zero;
            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                total = await _priceCalculationService.RoundPriceAsync(total);

            updatedOrder.OrderDiscount = discountAmountTotal;
            updatedOrder.OrderTotal = total;

            foreach (var discount in orderAppliedDiscounts)
                if (!_discountService.ContainsDiscount(updateOrderParameters.AppliedDiscounts, discount))
                    updateOrderParameters.AppliedDiscounts.Add(discount);
        }

        /// <summary>
        /// Update tax rates
        /// </summary>
        /// <param name="subTotalTaxRates">Subtotal tax rates</param>
        /// <param name="shippingTotalInclTax">Shipping (incl tax)</param>
        /// <param name="shippingTotalExclTax">Shipping (excl tax)</param>
        /// <param name="shippingTaxRate">Shipping tax rates</param>
        /// <param name="updatedOrder">Order</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the ax total
        /// </returns>
        protected virtual async Task<decimal> UpdateTaxRatesAsync(SortedDictionary<decimal, decimal> subTotalTaxRates, decimal shippingTotalInclTax,
            decimal shippingTotalExclTax, decimal shippingTaxRate, Order updatedOrder)
        {
            var taxRates = new SortedDictionary<decimal, decimal>();

            //order subtotal taxes
            var subTotalTax = decimal.Zero;
            foreach (var kvp in subTotalTaxRates)
            {
                subTotalTax += kvp.Value;
                if (kvp.Key <= decimal.Zero || kvp.Value <= decimal.Zero)
                    continue;

                if (!taxRates.ContainsKey(kvp.Key))
                    taxRates.Add(kvp.Key, kvp.Value);
                else
                    taxRates[kvp.Key] = taxRates[kvp.Key] + kvp.Value;
            }

            //shipping taxes
            var shippingTax = decimal.Zero;
            if (_taxSettings.ShippingIsTaxable)
            {
                shippingTax = shippingTotalInclTax - shippingTotalExclTax;
                if (shippingTax < decimal.Zero)
                    shippingTax = decimal.Zero;

                if (shippingTaxRate > decimal.Zero && shippingTax > decimal.Zero)
                {
                    if (!taxRates.ContainsKey(shippingTaxRate))
                        taxRates.Add(shippingTaxRate, shippingTax);
                    else
                        taxRates[shippingTaxRate] = taxRates[shippingTaxRate] + shippingTax;
                }
            }

            //payment method additional fee tax
            var paymentMethodAdditionalFeeTax = decimal.Zero;
            if (_taxSettings.PaymentMethodAdditionalFeeIsTaxable)
            {
                paymentMethodAdditionalFeeTax = updatedOrder.PaymentMethodAdditionalFeeInclTax - updatedOrder.PaymentMethodAdditionalFeeExclTax;
                if (paymentMethodAdditionalFeeTax < decimal.Zero)
                    paymentMethodAdditionalFeeTax = decimal.Zero;

                if (updatedOrder.PaymentMethodAdditionalFeeExclTax > decimal.Zero)
                {
                    var paymentTaxRate = Math.Round(100 * paymentMethodAdditionalFeeTax / updatedOrder.PaymentMethodAdditionalFeeExclTax, 3);
                    if (paymentTaxRate > decimal.Zero && paymentMethodAdditionalFeeTax > decimal.Zero)
                    {
                        if (!taxRates.ContainsKey(paymentTaxRate))
                            taxRates.Add(paymentTaxRate, paymentMethodAdditionalFeeTax);
                        else
                            taxRates[paymentTaxRate] = taxRates[paymentTaxRate] + paymentMethodAdditionalFeeTax;
                    }
                }
            }

            //add at least one tax rate (0%)
            if (!taxRates.Any())
                taxRates.Add(decimal.Zero, decimal.Zero);

            //summarize taxes
            var taxTotal = subTotalTax + shippingTax + paymentMethodAdditionalFeeTax;
            if (taxTotal < decimal.Zero)
                taxTotal = decimal.Zero;

            //round tax
            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                taxTotal = await _priceCalculationService.RoundPriceAsync(taxTotal);

            updatedOrder.OrderTax = taxTotal;
            updatedOrder.TaxRates = taxRates.Aggregate(string.Empty, (current, next) =>
                $"{current}{next.Key.ToString(CultureInfo.InvariantCulture)}:{next.Value.ToString(CultureInfo.InvariantCulture)};   ");
            
            return taxTotal;
        }

        /// <summary>
        /// Update shipping
        /// </summary>
        /// <param name="updateOrderParameters">UpdateOrderParameters</param>
        /// <param name="restoredCart">Cart</param>
        /// <param name="subTotalInclTax">Subtotal (incl tax)</param>
        /// <param name="subTotalExclTax">Subtotal (excl tax)</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the shipping total. Shipping (incl tax). Shipping tax rate
        /// </returns>
        //protected virtual async Task<(decimal shippingTotal, decimal shippingTotalInclTax, decimal shippingTaxRate)> UpdateShippingAsync(UpdateOrderParameters updateOrderParameters, IList<ShoppingCartItem> restoredCart,
        //    decimal subTotalInclTax, decimal subTotalExclTax)
        //{
        //    var shippingTotalExclTax = decimal.Zero;
        //    var shippingTotalInclTax = decimal.Zero;
        //    var shippingTaxRate = decimal.Zero;

        //    var updatedOrder = updateOrderParameters.UpdatedOrder;
        //    var customer = await _customerService.GetCustomerByIdAsync(updatedOrder.CustomerId);
        //    var currentCustomer = await _workContext.GetCurrentCustomerAsync();
        //    var store = await _storeContext.GetCurrentStoreAsync();

        //    if (await _shoppingCartService.ShoppingCartRequiresShippingAsync(restoredCart))
        //    {
        //        if (!await IsFreeShippingAsync(restoredCart, _shippingSettings.FreeShippingOverXIncludingTax ? subTotalInclTax : subTotalExclTax))
        //        {
        //            var shippingTotal = decimal.Zero;
        //            if (!string.IsNullOrEmpty(updatedOrder.ShippingRateComputationMethodSystemName))
        //            {
        //                //in the updated order were shipping items
        //                if (updatedOrder.PickupInStore)
        //                {
        //                    //customer chose pickup in store method, try to get chosen pickup point
        //                    if (_shippingSettings.AllowPickupInStore)
        //                    {
        //                        var address = await _addressService.GetAddressByIdAsync(updatedOrder.BillingAddressId);
        //                        var pickupPointsResponse = await _shippingService.GetPickupPointsAsync(restoredCart, address,
        //                            customer, updatedOrder.ShippingRateComputationMethodSystemName, store.Id);
        //                        if (pickupPointsResponse.Success)
        //                        {
        //                            var selectedPickupPoint =
        //                                pickupPointsResponse.PickupPoints.FirstOrDefault(point =>
        //                                    updatedOrder.ShippingMethod.Contains(point.Name));
        //                            if (selectedPickupPoint != null)
        //                                shippingTotal = selectedPickupPoint.PickupFee;
        //                            else
        //                                updateOrderParameters.Warnings.Add(
        //                                    $"Shipping method {updatedOrder.ShippingMethod} could not be loaded");
        //                        }
        //                        else
        //                            updateOrderParameters.Warnings.AddRange(pickupPointsResponse.Errors);
        //                    }
        //                    else
        //                        updateOrderParameters.Warnings.Add("Pick up in store is not available");
        //                }
        //                else
        //                {
        //                    //customer chose shipping to address, try to get chosen shipping option
        //                    var shippingAddress = await _addressService.GetAddressByIdAsync(updatedOrder.ShippingAddressId ?? 0);
        //                    var shippingOptionsResponse = await _shippingService.GetShippingOptionsAsync(restoredCart, shippingAddress, customer, updatedOrder.ShippingRateComputationMethodSystemName, store.Id);
        //                    if (shippingOptionsResponse.Success)
        //                    {
        //                        var shippingOption = shippingOptionsResponse.ShippingOptions.FirstOrDefault(option =>
        //                            updatedOrder.ShippingMethod.Contains(option.Name));
        //                        if (shippingOption != null)
        //                            shippingTotal = shippingOption.Rate;
        //                        else
        //                            updateOrderParameters.Warnings.Add(
        //                                $"Shipping method {updatedOrder.ShippingMethod} could not be loaded");
        //                    }
        //                    else
        //                        updateOrderParameters.Warnings.AddRange(shippingOptionsResponse.Errors);
        //                }
        //            }
        //            else
        //            {
        //                //before updating order was without shipping
        //                if (_shippingSettings.AllowPickupInStore)
        //                {
        //                    //try to get the cheapest pickup point
        //                    var address = await _addressService.GetAddressByIdAsync(updatedOrder.BillingAddressId);
        //                    var pickupPointsResponse = await _shippingService.GetPickupPointsAsync(restoredCart, address,
        //                        currentCustomer, storeId: store.Id);
        //                    if (pickupPointsResponse.Success)
        //                    {
        //                        updateOrderParameters.PickupPoint = pickupPointsResponse.PickupPoints
        //                            .OrderBy(point => point.PickupFee).First();
        //                        shippingTotal = updateOrderParameters.PickupPoint.PickupFee;
        //                    }
        //                    else
        //                        updateOrderParameters.Warnings.AddRange(pickupPointsResponse.Errors);
        //                }
        //                else
        //                    updateOrderParameters.Warnings.Add("Pick up in store is not available");

        //                if (updateOrderParameters.PickupPoint == null)
        //                {
        //                    //or try to get the cheapest shipping option for the shipping to the customer address 
        //                    var shippingRateComputationMethods = await _shippingPluginManager.LoadActivePluginsAsync();
        //                    if (shippingRateComputationMethods.Any())
        //                    {
        //                        var customerShippingAddress = await _customerService.GetCustomerShippingAddressAsync(customer);

        //                        var shippingOptionsResponse = await _shippingService.GetShippingOptionsAsync(restoredCart, customerShippingAddress, currentCustomer, storeId: store.Id);
        //                        if (shippingOptionsResponse.Success)
        //                        {
        //                            var shippingOption = shippingOptionsResponse.ShippingOptions.OrderBy(option => option.Rate)
        //                                .First();
        //                            updatedOrder.ShippingRateComputationMethodSystemName =
        //                                shippingOption.ShippingRateComputationMethodSystemName;
        //                            updatedOrder.ShippingMethod = shippingOption.Name;

        //                            var updatedShippingAddress = _addressService.CloneAddress(customerShippingAddress);
        //                            await _addressService.InsertAddressAsync(updatedShippingAddress);
        //                            updatedOrder.ShippingAddressId = updatedShippingAddress.Id;

        //                            shippingTotal = shippingOption.Rate;
        //                        }
        //                        else
        //                            updateOrderParameters.Warnings.AddRange(shippingOptionsResponse.Errors);
        //                    }
        //                    else
        //                        updateOrderParameters.Warnings.Add("Shipping rate computation method could not be loaded");
        //                }
        //            }

        //            //additional shipping charge
        //            shippingTotal += await GetShoppingCartAdditionalShippingChargeAsync(restoredCart);

        //            //shipping discounts
        //            var (shippingDiscount, shippingTotalDiscounts) = await GetShippingDiscountAsync(customer, shippingTotal);
        //            shippingTotal -= shippingDiscount;
        //            if (shippingTotal < decimal.Zero)
        //                shippingTotal = decimal.Zero;

        //            shippingTotalExclTax = (await _taxService.GetShippingPriceAsync(shippingTotal, false, customer)).price;
        //            (shippingTotalInclTax, shippingTaxRate) = await _taxService.GetShippingPriceAsync(shippingTotal, true, customer);

        //            //rounding
        //            if (_shoppingCartSettings.RoundPricesDuringCalculation)
        //            {
        //                shippingTotalExclTax = await _priceCalculationService.RoundPriceAsync(shippingTotalExclTax);
        //                shippingTotalInclTax = await _priceCalculationService.RoundPriceAsync(shippingTotalInclTax);
        //            }

        //            //change shipping status
        //            if (updatedOrder.ShippingStatus == ShippingStatus.ShippingNotRequired ||
        //                updatedOrder.ShippingStatus == ShippingStatus.NotYetShipped)
        //                updatedOrder.ShippingStatus = ShippingStatus.NotYetShipped;
        //            else
        //                updatedOrder.ShippingStatus = ShippingStatus.PartiallyShipped;

        //            foreach (var discount in shippingTotalDiscounts)
        //                if (!_discountService.ContainsDiscount(updateOrderParameters.AppliedDiscounts, discount))
        //                    updateOrderParameters.AppliedDiscounts.Add(discount);
        //        }
        //    }
        //    else
        //        updatedOrder.ShippingStatus = ShippingStatus.ShippingNotRequired;

        //    updatedOrder.OrderShippingExclTax = shippingTotalExclTax;
        //    updatedOrder.OrderShippingInclTax = shippingTotalInclTax;

        //    return (shippingTotalExclTax, shippingTotalInclTax, shippingTaxRate);
        //}

        /// <summary>
        /// Update order parameters
        /// </summary>
        /// <param name="updateOrderParameters">UpdateOrderParameters</param>
        /// <param name="restoredCart">Cart</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the subtotal. Subtotal (incl tax). Subtotal tax rates. Discount amount (excl tax)
        /// </returns>
        protected virtual async Task<(decimal subtotal, decimal subTotalInclTax, SortedDictionary<decimal, decimal> subTotalTaxRates, decimal discountAmountExclTax)> UpdateSubTotalAsync(UpdateOrderParameters updateOrderParameters, IList<ShoppingCartItem> restoredCart)
        {
            var subTotalExclTax = decimal.Zero;
            var subTotalInclTax = decimal.Zero;
            var subTotalTaxRates = new SortedDictionary<decimal, decimal>();

            var updatedOrder = updateOrderParameters.UpdatedOrder;
            var updatedOrderItem = updateOrderParameters.UpdatedOrderItem;

            foreach (var shoppingCartItem in restoredCart)
            {
                decimal itemSubTotalExclTax;
                decimal itemSubTotalInclTax;
                decimal taxRate;

                //calculate subtotal for the updated order item
                if (shoppingCartItem.Id == updatedOrderItem.Id)
                {
                    //update order item 
                    updatedOrderItem.UnitPriceExclTax = updateOrderParameters.PriceExclTax;
                    updatedOrderItem.UnitPriceInclTax = updateOrderParameters.PriceInclTax;
                    updatedOrderItem.DiscountAmountExclTax = updateOrderParameters.DiscountAmountExclTax;
                    updatedOrderItem.DiscountAmountInclTax = updateOrderParameters.DiscountAmountInclTax;
                    updatedOrderItem.PriceExclTax = itemSubTotalExclTax = updateOrderParameters.SubTotalExclTax;
                    updatedOrderItem.PriceInclTax = itemSubTotalInclTax = updateOrderParameters.SubTotalInclTax;
                    updatedOrderItem.Quantity = shoppingCartItem.Quantity;

                    taxRate = itemSubTotalExclTax > 0 ? Math.Round(100 * (itemSubTotalInclTax - itemSubTotalExclTax) / itemSubTotalExclTax, 3) : 0M;
                }
                else
                {
                    //get the already calculated subtotal from the order item
                    var order = await _orderService.GetOrderItemByIdAsync(shoppingCartItem.Id);
                    itemSubTotalExclTax = order.PriceExclTax;
                    itemSubTotalInclTax = order.PriceInclTax;

                    taxRate = itemSubTotalExclTax > 0 ? Math.Round(100 * (itemSubTotalInclTax - itemSubTotalExclTax) / itemSubTotalExclTax, 3) : 0M;
                }

                subTotalExclTax += itemSubTotalExclTax;
                subTotalInclTax += itemSubTotalInclTax;

                //tax rates
                var itemTaxValue = itemSubTotalInclTax - itemSubTotalExclTax;
                if (taxRate <= decimal.Zero || itemTaxValue <= decimal.Zero)
                    continue;

                if (!subTotalTaxRates.ContainsKey(taxRate))
                    subTotalTaxRates.Add(taxRate, itemTaxValue);
                else
                    subTotalTaxRates[taxRate] = subTotalTaxRates[taxRate] + itemTaxValue;
            }

            if (subTotalExclTax < decimal.Zero)
                subTotalExclTax = decimal.Zero;

            if (subTotalInclTax < decimal.Zero)
                subTotalInclTax = decimal.Zero;

            //We calculate discount amount on order subtotal excl tax (discount first)
            //calculate discount amount ('Applied to order subtotal' discount)
            var customer = await _customerService.GetCustomerByIdAsync(updatedOrder.CustomerId);
            var (discountAmountExclTax, subTotalDiscounts) = await GetOrderSubtotalDiscountAsync(customer, subTotalExclTax);
            if (subTotalExclTax < discountAmountExclTax)
                discountAmountExclTax = subTotalExclTax;
            var discountAmountInclTax = discountAmountExclTax;

            //add tax for shopping items
            var tempTaxRates = new Dictionary<decimal, decimal>(subTotalTaxRates);
            foreach (var kvp in tempTaxRates)
            {
                if (kvp.Value == decimal.Zero || subTotalExclTax <= decimal.Zero)
                    continue;

                var discountTaxValue = kvp.Value * (discountAmountExclTax / subTotalExclTax);
                discountAmountInclTax += discountTaxValue;
                subTotalTaxRates[kvp.Key] = kvp.Value - discountTaxValue;
            }

            //rounding
            if (_shoppingCartSettings.RoundPricesDuringCalculation)
            {
                subTotalExclTax = await _priceCalculationService.RoundPriceAsync(subTotalExclTax);
                subTotalInclTax = await _priceCalculationService.RoundPriceAsync(subTotalInclTax);
                discountAmountExclTax = await _priceCalculationService.RoundPriceAsync(discountAmountExclTax);
                discountAmountInclTax = await _priceCalculationService.RoundPriceAsync(discountAmountInclTax);
            }

            updatedOrder.OrderSubtotalExclTax = subTotalExclTax;
            updatedOrder.OrderSubtotalInclTax = subTotalInclTax;
            updatedOrder.OrderSubTotalDiscountExclTax = discountAmountExclTax;
            updatedOrder.OrderSubTotalDiscountInclTax = discountAmountInclTax;

            foreach (var discount in subTotalDiscounts)
                if (!_discountService.ContainsDiscount(updateOrderParameters.AppliedDiscounts, discount))
                    updateOrderParameters.AppliedDiscounts.Add(discount);

            return (subTotalExclTax, subTotalInclTax, subTotalTaxRates, discountAmountExclTax);
        }

        /// <summary>
        /// Set reward points
        /// </summary>
        /// <param name="redeemedRewardPoints">Redeemed reward points</param>
        /// <param name="redeemedRewardPointsAmount">Redeemed reward points amount</param>
        /// <param name="useRewardPoints">A value indicating whether to use reward points</param>
        /// <param name="customer">Customer</param>
        /// <param name="orderTotal">Order total</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task<(int redeemedRewardPoints, decimal redeemedRewardPointsAmount)> SetRewardPointsAsync(int redeemedRewardPoints, decimal redeemedRewardPointsAmount,
            bool? useRewardPoints, Customer customer, decimal orderTotal)
        {
            if (!_rewardPointsSettings.Enabled)
                return (redeemedRewardPoints, redeemedRewardPointsAmount);

            var store = await _storeContext.GetCurrentStoreAsync();
            if (!useRewardPoints.HasValue)
                useRewardPoints = await _genericAttributeService.GetAttributeAsync<bool>(customer, ZarayeCustomerDefaults.UseRewardPointsDuringCheckoutAttribute, store.Id);

            if (!useRewardPoints.Value)
                return (redeemedRewardPoints, redeemedRewardPointsAmount);

            if (orderTotal <= decimal.Zero)
                return (redeemedRewardPoints, redeemedRewardPointsAmount);

            var rewardPointsBalance = await _rewardPointService.GetRewardPointsBalanceAsync(customer.Id, store.Id);

            if (_rewardPointsSettings.MaximumRewardPointsToUsePerOrder > 0 && rewardPointsBalance > _rewardPointsSettings.MaximumRewardPointsToUsePerOrder)
                rewardPointsBalance = _rewardPointsSettings.MaximumRewardPointsToUsePerOrder;

            var rewardPointsBalanceAmount = await ConvertRewardPointsToAmountAsync(rewardPointsBalance);

            if (_rewardPointsSettings.MaximumRedeemedRate > 0 && _rewardPointsSettings.MaximumRedeemedRate < rewardPointsBalanceAmount / orderTotal)
            {
                rewardPointsBalance = ConvertAmountToRewardPoints(orderTotal * _rewardPointsSettings.MaximumRedeemedRate);
                rewardPointsBalanceAmount = await ConvertRewardPointsToAmountAsync(rewardPointsBalance);
            }

            if (!CheckMinimumRewardPointsToUseRequirement(rewardPointsBalance))
                return (redeemedRewardPoints, redeemedRewardPointsAmount);

            if (orderTotal > rewardPointsBalanceAmount)
            {
                redeemedRewardPoints = rewardPointsBalance;
                redeemedRewardPointsAmount = rewardPointsBalanceAmount;
            }
            else
            {
                redeemedRewardPointsAmount = orderTotal;
                redeemedRewardPoints = ConvertAmountToRewardPoints(redeemedRewardPointsAmount);
            }

            return (redeemedRewardPoints, redeemedRewardPointsAmount);
        }

        /// <summary>
        /// Gets shopping cart additional shipping charge
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the additional shipping charge
        /// </returns>
        protected virtual async Task<decimal> GetShoppingCartAdditionalShippingChargeAsync(IList<ShoppingCartItem> cart)
        {
            return await cart.SumAwaitAsync(async shoppingCartItem => await _shippingService.GetAdditionalShippingChargeAsync(shoppingCartItem));
        }

        /// <summary>
        /// Converts an amount to reward points
        /// </summary>
        /// <param name="amount">Amount</param>
        /// <returns>Converted value</returns>
        protected virtual int ConvertAmountToRewardPoints(decimal amount)
        {
            var result = 0;
            if (amount <= 0)
                return 0;

            if (_rewardPointsSettings.ExchangeRate > 0)
                result = (int)Math.Ceiling(amount / _rewardPointsSettings.ExchangeRate);
            return result;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adjust shipping rate (free shipping, additional charges, discounts)
        /// </summary>
        /// <param name="shippingRate">Shipping rate to adjust</param>
        /// <param name="cart">Cart</param>
        /// <param name="applyToPickupInStore">Adjust shipping rate to pickup in store shipping option rate</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the adjusted shipping rate. Applied discounts
        /// </returns>
        public virtual async Task<(decimal adjustedShippingRate, List<Discount> appliedDiscounts)> AdjustShippingRateAsync(decimal shippingRate, IList<ShoppingCartItem> cart, 
            bool applyToPickupInStore = false)
        {
            //free shipping
            //if (await IsFreeShippingAsync(cart))
            //    return (decimal.Zero, new List<Discount>());

            var customer = await _customerService.GetShoppingCartCustomerAsync(cart);
            var store = await _storeContext.GetCurrentStoreAsync();

            //with additional shipping charges
            var pickupPoint = await _genericAttributeService.GetAttributeAsync<PickupPoint>(customer,
                    ZarayeCustomerDefaults.SelectedPickupPointAttribute, store.Id);

            var adjustedRate = shippingRate;

            if (!(applyToPickupInStore && _shippingSettings.AllowPickupInStore && pickupPoint != null && _shippingSettings.IgnoreAdditionalShippingChargeForPickupInStore))
            {
                adjustedRate += await GetShoppingCartAdditionalShippingChargeAsync(cart);
            }

            //discount
            var (discountAmount, appliedDiscounts) = await GetShippingDiscountAsync(customer, adjustedRate);
            adjustedRate -= discountAmount;

            adjustedRate = Math.Max(adjustedRate, decimal.Zero);
            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                adjustedRate = await _priceCalculationService.RoundPriceAsync(adjustedRate);

            return (adjustedRate, appliedDiscounts);
        }

       
        /// <summary>
        /// Converts existing reward points to amount
        /// </summary>
        /// <param name="rewardPoints">Reward points</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the converted value
        /// </returns>
        public virtual async Task<decimal> ConvertRewardPointsToAmountAsync(int rewardPoints)
        {
            if (rewardPoints <= 0)
                return decimal.Zero;

            var result = rewardPoints * _rewardPointsSettings.ExchangeRate;
            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                result = await _priceCalculationService.RoundPriceAsync(result);
            return result;
        }

        /// <summary>
        /// Gets a value indicating whether a customer has minimum amount of reward points to use (if enabled)
        /// </summary>
        /// <param name="rewardPoints">Reward points to check</param>
        /// <returns>true - reward points could use; false - cannot be used.</returns>
        public virtual bool CheckMinimumRewardPointsToUseRequirement(int rewardPoints)
        {
            if (_rewardPointsSettings.MinimumRewardPointsToUse <= 0)
                return true;

            return rewardPoints >= _rewardPointsSettings.MinimumRewardPointsToUse;
        }

        /// <summary>
        /// Calculate how order total (maximum amount) for which reward points could be earned/reduced
        /// </summary>
        /// <param name="orderShippingInclTax">Order shipping (including tax)</param>
        /// <param name="orderTotal">Order total</param>
        /// <returns>Applicable order total</returns>
        public virtual decimal CalculateApplicableOrderTotalForRewardPoints(decimal orderShippingInclTax, decimal orderTotal)
        {
            //do you give reward points for order total? or do you exclude shipping?
            //since shipping costs vary some of store owners don't give reward points based on shipping total
            //you can put your custom logic here
            var totalForRewardPoints = orderTotal - orderShippingInclTax;

            //check the minimum total to award points
            if (totalForRewardPoints < _rewardPointsSettings.MinOrderTotalToAwardPoints)
                return decimal.Zero;

            return totalForRewardPoints;
        }

        /// <summary>
        /// Calculate how much reward points will be earned/reduced based on certain amount spent
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="amount">Amount (in primary store currency)</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the number of reward points
        /// </returns>
        public virtual async Task<int> CalculateRewardPointsAsync(Customer customer, decimal amount)
        {
            if (!_rewardPointsSettings.Enabled)
                return 0;

            if (_rewardPointsSettings.PointsForPurchases_Amount <= decimal.Zero)
                return 0;

            //ensure that reward points are applied only to registered users
            if (customer == null || await _customerService.IsGuestAsync(customer))
                return 0;

            var points = (int)Math.Truncate(amount / _rewardPointsSettings.PointsForPurchases_Amount) * _rewardPointsSettings.PointsForPurchases_Points;
            return points;
        }

        #endregion
    }
}