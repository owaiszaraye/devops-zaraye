using Zaraye.Core;
using Zaraye.Services.Customers;

namespace Zaraye.Middleware
{
    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class JwtMiddlewareExtensions
    {
        public static IApplicationBuilder UseJwtMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<JwtMiddleware>();
        }
    }

    public class JwtMiddleware
    {
        #region Fields

        private readonly RequestDelegate _next;
        private readonly IConfiguration _config;

        #endregion

        #region Ctor

        public JwtMiddleware(RequestDelegate next, IConfiguration config)
        {
            _next = next;
            this._config = config;
        }

        #endregion

        #region Utility

        public async Task<string> ExtractApiVersionAsync(HttpContext context)
        {
            string url = $"{context.Request.Scheme}://{context.Request.Host}{context.Request.Path}";
            // Find the position of "v" in the URL
            int versionIndex = url.IndexOf("/v", StringComparison.OrdinalIgnoreCase);

            if (versionIndex != -1)
            {
                // Extract the version substring
                string versionSubstring = url.Substring(versionIndex + 2);

                // Find the next slash after the version substring
                int nextSlashIndex = versionSubstring.IndexOf('/');

                if (nextSlashIndex != -1)
                {
                    // Extract the version
                    string version = versionSubstring.Substring(0, nextSlashIndex);
                    return version;
                }
            }

            // Default to an empty string if version extraction fails
            return string.Empty;
        }

        #endregion

        public async Task Invoke(HttpContext context, ICustomerService userService, IWorkContext workContext, IWebHelper webHelper, JwtService jwtUtils)
        {
            if (context.WebSockets.IsWebSocketRequest)
                return;

            var url = await ExtractApiVersionAsync(context);
            var apiUrl = $"{webHelper.GetStoreLocation()}v{url}/";
            if (webHelper.GetThisPageUrl(false).StartsWith(apiUrl, StringComparison.InvariantCultureIgnoreCase))
            {
                try
                {
                    var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                    var userId = jwtUtils.ValidateJwtToken(token);
                    if (userId != null)
                    {
                        // attach user to context on successful jwt validation
                        context.Items["User"] = await userService.GetCustomerByIdAsync(userId.Value);
                        await workContext.SetCurrentCustomerAsync(await userService.GetCustomerByIdAsync(userId.Value));
                    }
                }
                catch (Exception ex)
                {
                   
                }
            }

            await _next(context);
        }

    }
}
