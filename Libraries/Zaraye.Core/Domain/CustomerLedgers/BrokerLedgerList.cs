namespace Zaraye.Core.Domain.CustomerLedgers
{
    public partial class BrokerLedgerList : BaseEntity
    {
        public int BrokerId { get; set; }
        public string BrokerName { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal Balance { get; set; }
    }
}
