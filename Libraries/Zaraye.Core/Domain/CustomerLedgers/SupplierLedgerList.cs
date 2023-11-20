using System;

namespace Zaraye.Core.Domain.CustomerLedgers
{
    public partial class SupplierLedgerList : BaseEntity
    {
        public int SupplierId { get; set; }
        public string SupplierName { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal Balance { get; set; }
    }
}
