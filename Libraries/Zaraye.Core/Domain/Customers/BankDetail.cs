using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zaraye.Core.Domain.Customers
{
    public partial class BankDetail : BaseEntity
    {
        public int CustomerId { get; set; }
        public string BankName { get; set; }
        public string AccountTitle { get; set; }
        public string AccountNumber { get; set; }
        public bool Published { get; set; }
        public bool Deleted { get; set; }
        public DateTime CreatedOnUtc { get; set; }
    }
}
