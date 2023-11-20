namespace Zaraye.Core.Domain.CustomerLedgers
{
    public partial class BuyerLedgerList : BaseEntity
    {
        public int BuyerId { get; set; }
        public string BuyerName { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal Balance { get; set; }
    }
}
