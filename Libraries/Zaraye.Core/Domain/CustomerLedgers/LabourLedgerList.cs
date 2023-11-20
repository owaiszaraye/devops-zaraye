namespace Zaraye.Core.Domain.CustomerLedgers
{
    public partial class LabourLedgerList : BaseEntity
    {
        public int LabourId { get; set; }
        public string LabourName { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal Balance { get; set; }
    }
}
