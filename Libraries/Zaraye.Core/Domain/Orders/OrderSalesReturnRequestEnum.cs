using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zaraye.Core.Domain.Orders
{
    public enum OrderSalesReturnRequestEnum
    {
        Pending = 10,
        Complete = 20,
        Cancelled = 30,
        Expired = 40,
    }
}
