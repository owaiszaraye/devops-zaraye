using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zaraye.Services.Logging
{
    public partial interface IAppLoggerService
    {
        Task<object> WriteLogs(Exception exception);
    }
}
