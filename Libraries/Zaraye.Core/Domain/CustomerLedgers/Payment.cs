using Zaraye.Core.Domain.Common;
using System;

namespace Zaraye.Core.Domain.CustomerLedgers
{
    public partial class Payment : BaseEntity,StoreEntity, DefaultColumns, ISoftDeletedEntity
    {
        public string CustomPaymentNumber { get; set; }
        public int CustomerId { get; set; }
        public string PaymentType { get; set; }
        public DateTime PaymentDateUtc { get; set; }
        public decimal Amount { get; set; }
        public decimal OpsAmount { get; set; }
        public decimal FinanceAmount { get; set; }
        public decimal AdjustAmount { get; set; }
        public int ModeOfPaymentId { get; set; }
        public string CompanyAccountTitle { get; set; }
        public int? BankDetailId { get; set; }
        public string VerificationNumber { get; set; }
        public string Comments { get; set; }
        public bool IsBusienssApproved { get; set; }
        public int BusinessId { get; set; }
        public string BusinessComment { get; set; }
        public bool IsFinanceApproved { get; set; }
        public int FinanceId { get; set; }
        public string FinanceComment { get; set; }
        public int PaymentStatusId { get; set; }
        public DateTime BusinessActionDateUtc { get; set; }
        public DateTime FinanceActionDateUtc { get; set; }
        public int CompanyBankAccountId { get; set; }
        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
        public bool Deleted { get; set; }
        public int DeletedById { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
        public DateTime? ChequeClearingDate { get; set; }
        public int ShipmentId { get; set; }

        public bool IsTransporter { get; set; }
        public bool IsLabour { get; set; }

        public int StoreId { get; set; }


        public ModeOfPayment ModeOfPayment
        {
            get => (ModeOfPayment)ModeOfPaymentId;
            set => ModeOfPaymentId = (int)value;
        }

        public ReceivableAndPayablePaymentStatus PaymentStatus
        {
            get => (ReceivableAndPayablePaymentStatus)PaymentStatusId;
            set => PaymentStatusId = (int)value;
        }
    }
}
