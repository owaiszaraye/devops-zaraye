using Newtonsoft.Json;
using System;

namespace Zaraye.Core.Domain.Orders
{
    public class DirectOrderCalculation : BaseEntity
    {
        [JsonProperty("directOrderId")]
        public int DirectOrderId { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public int BusinessModelId { get; set; }
        public decimal GrossCommissionRate { get; set; }
        public string GrossCommissionRateType { get; set; }
        public decimal GrossCommissionAmount { get; set; }
        public bool GSTIncludedInTotalAmount { get; set; }
        public bool WhtIncluded { get; set; }
        public bool WholesaletaxIncluded { get; set; }
        public decimal NetAmount { get; set; }
        public decimal NetAmountWithoutGST { get; set; }
        public decimal GSTRate { get; set; }
        public decimal GSTAmount { get; set; }
        public decimal WHTRate { get; set; }
        public decimal WHTAmount { get; set; }
        public decimal WholesaleTaxRate { get; set; }
        public decimal WholesaleTaxAmount { get; set; }
        public decimal MarginRate { get; set; }
        public string MarginRateType { get; set; }
        public decimal MarginAmount { get; set; }
        public decimal DiscountRate { get; set; }
        public string DiscountRateType { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal NetRateWithMargin { get; set; }
        public decimal SubTotal { get; set; }
        public decimal OrderTotal { get; set; }
        public decimal GrossAmount { get; set; }
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
        public decimal TotalPerBag { get; set; }

        #region Receivable

        #region Lending, Broker, All other model

        public int BrokerId { get; set; }
        public int BuyerCommissionReceivableUserId { get; set; }
        public int BuyerCommissionPayableUserId { get; set; }
        public int SupplierCommissionReceivableUserId { get; set; }
        public int SupplierCommissionPayableUserId { get; set; }
        public decimal SellingPriceOfProduct { get; set; } // lending, Brooker
        public decimal BuyerCommissionReceivablePerBag { get; set; }
        public decimal FinanceIncome { get; set; } // lending, Brooker
        public decimal BuyerCommissionReceivable_Summary { get; set; }
        public decimal BuyerCommissionPayablePerBag { get; set; }
        public decimal BuyerCommissionPayable_Summary { get; set; }
        public decimal SellingPrice_FinanceIncome { get; set; }
        public decimal TotalReceivableBuyer { get; set; }
        public decimal TotalReceivableFromBuyerDirectlyToSupplier { get; set; }
        public decimal TotalCommissionReceivableFromBuyerToZaraye { get; set; }
        public decimal BuyingPrice { get; set; }

        #endregion


        #endregion

        #region Payable

        #region Lending

        public decimal InvoicedAmount { get; set; }
        public decimal BrokerCash { get; set; }
        public decimal SupplierCommissionBag { get; set; }
        public decimal SupplierCommissionBag_Summary { get; set; }
        public decimal SupplierCommissionReceivableRate { get; set; }
        public decimal SupplierCommissionReceivableAmount { get; set; }
        public string SupplierCommissionReceivableType { get; set; }
        public decimal BuyerPaymentTerms { get; set; }
        public decimal TotalFinanceCost { get; set; }
        public decimal SupplierCreditTerms { get; set; }
        public int FinanceCostPayment { get; set; }
        public decimal FinanceCost { get; set; }
        public decimal InterestAccrued { get; set; }
        public decimal InterestAccrued_Summary { get; set; }
        public decimal PayableToMill { get; set; }
        public decimal PaymentInCash { get; set; }
        public decimal BuyerCommission { get; set; }
        public decimal SupplierCommissionReceivable_Summary { get; set; }

        #endregion

        #endregion

        public BusinessModelEnum BusinessModelEnum
        {
            get => (BusinessModelEnum)BusinessModelId;
            set => BusinessModelId = (int)value;
        }
    }
}
