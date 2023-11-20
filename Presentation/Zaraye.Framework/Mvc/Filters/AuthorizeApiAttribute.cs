using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Zaraye.Core.Domain.Customers;
using System;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeApiAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = (Customer)context.HttpContext.Items["User"];
        if (user == null)
        {
            // not logged in
            context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
        }
    }
}