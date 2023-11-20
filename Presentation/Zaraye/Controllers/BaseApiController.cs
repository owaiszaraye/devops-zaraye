using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Zaraye.Controllers
{
    public partial class BaseApiController : Controller
    {
        protected object GetModelErrors(ModelStateDictionary modelState)
        {
            var errorList = modelState.Values.SelectMany(m => m.Errors)
                                  .Select(e => e.ErrorMessage)
                                  .ToList();
            return errorList;
        }
    }
}