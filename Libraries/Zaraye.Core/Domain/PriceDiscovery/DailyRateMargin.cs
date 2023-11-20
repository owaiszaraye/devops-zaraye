namespace Zaraye.Core.Domain.PriceDiscovery
{
    public partial class DailyRateMargin :BaseEntity
    { 
        public int DailyRateId { get; set; }
        public int CityId { get; set; }
        public decimal? MarginRate { get; set; }
    }
}
