using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zaraye.Core.Domain.Common
{
    public partial class BankStatement : BaseEntity
    {
        public string BankName { get; set; }
        public int FileId { get; set; }
        public int AppliedCreditCustomerId { get; set; }
        public DateTime CreatedOnUtc { get; set; }
    }
}
