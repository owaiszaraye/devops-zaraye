using Zaraye.Core.Domain.Common;
using System;

namespace Zaraye.Core.Domain.Catalog
{
    public partial class TransporterVehicleMapping : BaseEntity , ISoftDeletedEntity
    {
        public int TransporterId { get; set; }
        public int VehicleId { get; set; }
        public string VehicleNumber { get; set; }
        public bool Published { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
        public bool Deleted { get; set; }
    }
}