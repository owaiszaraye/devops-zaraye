using Zaraye.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zaraye.Core.Domain.Customers
{
    public partial class TransporterCost :BaseEntity,StoreEntity,ISoftDeletedEntity
    {
        public int TransporterId { get; set; }
        public int VehicleTransporterMappingId { get; set; }
        public int CountryId { get; set; }
        public int CityId { get; set; }
        public decimal VehicleCost { get; set; }
        public decimal LabourCharges { get; set; }
        public decimal MonthlyFixedCost { get; set; }
        public int WorkingDays { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public bool Deleted { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
        public int StoreId { get; set; }
    }
}
