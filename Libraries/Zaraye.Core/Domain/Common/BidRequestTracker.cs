using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zaraye.Core.Domain.Common
{
    public class BidRequestTracker : BaseEntity
    {
        public int TargetId { get; set; }
        public int RequestId { get; set; }
        public int QuotationId { get; set; }
        public int OrderId { get; set; }
        public string TrackerType { get; set; }
        public int StatusId { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedOnUtc { get; set; }
    }
}
