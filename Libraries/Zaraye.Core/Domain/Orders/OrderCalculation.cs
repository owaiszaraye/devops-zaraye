namespace Zaraye.Core.Domain.Orders
{
    public partial class OrderCalculation : BaseEntity
    {
        public int OrderId { get; set; }
        public int BusinessModelId { get; set; }
        public decimal GrossCommissionRate { get; set; }
        public decimal GrossAmount { get; set; }
        public string GrossCommissionRateType { get; set; }
        public decimal GrossCommissionAmount { get; set; }
        public bool GSTIncludedInTotalAmount { get; set; }
        public decimal NetAmount { get; set; }
        public decimal NetAmountWithoutGST { get; set; }
        public decimal NetRateWithMargin { get; set; }
        public decimal SubTotal { get; set; }
        public decimal OrderTotal { get; set; }
        public decimal TotalPerBag { get; set; }
        public decimal DeliveryCost { get; set; }

        #region Global fields

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

        #endregion


        #region Receivable

        #region Lending, Broker, All other model

        public decimal CalculatedSellingPriceOfProduct { get; set; } // lending, Brooker
        public decimal ProductPrice { get; set; } // lending, Brooker
        public decimal SellingPriceOfProduct { get; set; } // lending, Brooker
        public decimal FinanceIncome { get; set; } // lending, Brooker
        public decimal BuyerCommissionReceivablePerBag { get; set; }
        public decimal BuyerCommissionPayablePerBag { get; set; }
        public decimal SellingPrice_FinanceIncome { get; set; }
        public decimal TotalReceivableBuyer { get; set; }

        //Only in used in app
        public decimal TotalReceivableFromBuyerDirectlyToSupplier { get; set; }

        #endregion
        

        #endregion

        #region Payable

        #region Lending

        public int BrokerId { get; set; }
        public int BuyerCommissionReceivableUserId { get; set; }
        public int BuyerCommissionPayableUserId { get; set; }
        public int SupplierCommissionReceivableUserId { get; set; }
        public int SupplierCommissionPayableUserId { get; set; }
        public decimal InvoicedAmount { get; set; }
        public decimal BrokerCash { get; set; }
        public decimal SupplierCommissionBag { get; set; }
        public decimal SupplierCommissionReceivableRate { get; set; }
        public decimal SupplierCommissionReceivableAmount { get; set; }
        //Only used in import
        public string SupplierCommissionReceivableType { get; set; }
        public decimal BuyerPaymentTerms { get; set; }
        public decimal TotalFinanceCost { get; set; }
        public decimal SupplierCreditTerms { get; set; }
        public int FinanceCostPayment { get; set; }
        public decimal FinanceCost { get; set; }
        //This is not used in business model
        public decimal InterestAccrued { get; set; }
        public decimal PayableToMill { get; set; }
        public decimal PaymentInCash { get; set; }
        public decimal BuyerCommission { get; set; }

        //This field is useless
        public decimal SupplierCommissionReceivable_Summary { get; set; }
        
        #endregion

        #endregion

        public BusinessModelEnum BusinessModelEnum
        {
            get => (BusinessModelEnum)BusinessModelId;
            set => BusinessModelId = (int)value;
        }
        
        public FinanceCostPaymentStatus FinanceCostPaymentStatus
        {
            get => (FinanceCostPaymentStatus)FinanceCostPayment;
            set => FinanceCostPayment = (int)value;
        }
    }
}