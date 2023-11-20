namespace Zaraye.Core.Domain.Customers
{
    public partial class CustomerIndustryMapping : BaseEntity
    {
        public int CustomerId { get; set; }
        public int IndustryId { get; set; }
    }
}