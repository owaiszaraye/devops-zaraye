namespace Zaraye.Core.Domain.Directory
{
    public partial class CityCombinationList : BaseEntity
    {
        public int City1Id { get; set; }
        public string City1Name { get; set; }
        public int City2Id { get; set; }
        public string City2Name { get; set; }
    }
}