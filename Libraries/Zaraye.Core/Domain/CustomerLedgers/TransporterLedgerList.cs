namespace Zaraye.Core.Domain.CustomerLedgers
{
    public partial class TransporterLedgerList : BaseEntity
    {
        public int TransporterId { get; set; }
        public string TransporterName { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal Balance { get; set; }
    }
}
