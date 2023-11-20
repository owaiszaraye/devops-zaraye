using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zaraye.Core.Domain.CustomerLedgers
{
    public partial class PaymentJson:BaseEntity
    {
        public DateTime PaymentDate { get; set; }
        public int SupplierId { get; set; }
        public string SupplierName { get; set; }
        public int BuyerId { get; set; }
        public string BuyerName { get; set; }
        public int OrderId { get; set; }
        public string OrderNumber { get; set; }
        public string AmountFormatted { get; set; }
        public decimal Amount { get; set; }
        public string strPaymentStatus { get; set; }
        public int PaymentStatusId { get; set; }
        public int CreatedById { get; set; }
        public string CreatedByName { get; set; }

        public ReceivableAndPayablePaymentStatus PaymentStatus
        {
            get => (ReceivableAndPayablePaymentStatus)PaymentStatusId;
            set => PaymentStatusId = (int)value;
        }
    }
}
