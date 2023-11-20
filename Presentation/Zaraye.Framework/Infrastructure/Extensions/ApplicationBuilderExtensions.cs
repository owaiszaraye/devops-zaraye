using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Hosting;
using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.ExceptionServices;
using WebMarkupMin.AspNetCore7;
using Zaraye.Core;
using Zaraye.Core.Configuration;
using Zaraye.Core.Domain.Common;
using Zaraye.Core.Http;
using Zaraye.Core.Infrastructure;
using Zaraye.Data;
using Zaraye.Data.Migrations;
using Zaraye.Framework.Mvc.Routing;
using Zaraye.Framework.Websocket;
using Zaraye.Services.Authentication;
using Zaraye.Services.Common;
using Zaraye.Services.Localization;
using Zaraye.Services.Logging;

namespace Zaraye.Framework.Infrastructure.Extensions
{
    /// <summary>
    /// Represents extensions of IApplicationBuilder
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Configure the application HTTP request pipeline
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void ConfigureRequestPipeline(this IApplicationBuilder application)
        {
            EngineContext.Current.ConfigureRequestPipeline(application);
        }

        public static void StartEngine(this IApplicationBuilder application)
        {
            var engine = EngineContext.Current;

            //further actions are performed only when the database is installed
            if (DataSettingsManager.IsDatabaseInstalled())
            {
                //log application start
                //engine.Resolve<ILogger>().Information("Application started");

                //install and update plugins
                //var pluginService = engine.Resolve<IPluginService>();
                //pluginService.InstallPluginsAsync().Wait();
                //pluginService.UpdatePluginsAsync().Wait();

                //update nopCommerce core and db
                var migrationManager = engine.Resolve<IMigrationManager>();
                var assembly = Assembly.GetAssembly(typeof(ApplicationBuilderExtensions));
                migrationManager.ApplyUpMigrations(assembly, MigrationProcessType.Update);
                assembly = Assembly.GetAssembly(typeof(IMigrationManager));
                migrationManager.ApplyUpMigrations(assembly, MigrationProcessType.Update);

               // var taskScheduler = engine.Resolve<ITaskScheduler>();
                //taskScheduler.InitializeAsync().Wait();
                //taskScheduler.StartScheduler();
            }
        }

        /// <summary>
        /// Add exception handling
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UseZarayeExceptionHandler(this IApplicationBuilder application)
        {
            var appSettings = EngineContext.Current.Resolve<AppSettings>();
            var webHostEnvironment = EngineContext.Current.Resolve<IWebHostEnvironment>();
            var useDetailedExceptionPage = appSettings.Get<CommonConfig>().DisplayFullErrorStack || webHostEnvironment.IsDevelopment();
            if (useDetailedExceptionPage)
            {
                //get detailed exceptions for developing and testing purposes
                application.UseDeveloperExceptionPage();
            }
            else
            {
                //or use special exception handler
                application.UseExceptionHandler("/Error/Error");
            }

            //log errors
            application.UseExceptionHandler(handler =>
            {
                handler.Run(async context =>
                {
                    var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
                    if (exception == null)
                        return;

                    try
                    {
                        //check whether database is installed
                        if (DataSettingsManager.IsDatabaseInstalled())
                        {
                            //get current customer
                            var currentCustomer = await EngineContext.Current.Resolve<IWorkContext>().GetCurrentCustomerAsync();

                            //log error
                           await EngineContext.Current.Resolve<ILogger>().ErrorAsync(exception.Message, exception, currentCustomer);
                        }
                    }
                    finally
                    {
                        //rethrow the exception to show the error page
                        ExceptionDispatchInfo.Throw(exception);
                    }
                });
            });
        }

        /// <summary>
        /// Adds a special handler that checks for responses with the 400 status code (bad request)
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UseBadRequestResult(this IApplicationBuilder application)
        {
            application.UseStatusCodePages(async context =>
            {
                //handle 404 (Bad request)
                if (context.HttpContext.Response.StatusCode == StatusCodes.Status400BadRequest)
                {
                    var logger = EngineContext.Current.Resolve<ILogger>();
                    var workContext = EngineContext.Current.Resolve<IWorkContext>();
                    await logger.ErrorAsync("Error 400. Bad request", null, customer: await workContext.GetCurrentCustomerAsync());
                }
            });
        }

        /// <summary>
        /// Configure middleware for dynamically compressing HTTP responses
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UseZarayeResponseCompression(this IApplicationBuilder application)
        {
            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            //whether to use compression (gzip by default)
            if (EngineContext.Current.Resolve<CommonSettings>().UseResponseCompression)
                application.UseResponseCompression();
        }

        /// <summary>
        /// Configure middleware checking whether requested page is keep alive page
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UseKeepAlive(this IApplicationBuilder application)
        {
            application.UseMiddleware<KeepAliveMiddleware>();
        }

        /// <summary>
        /// Adds the authentication middleware, which enables authentication capabilities.
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UseZarayeAuthentication(this IApplicationBuilder application)
        {
            //check whether database is installed
            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            application.UseMiddleware<AuthenticationMiddleware>();
        }

        public static void UseWebSocketServerForZaraye(this IApplicationBuilder application)
        {
            //check whether database is installed
            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            application.UseMiddleware<WebSocketServerMiddleware>();
        }

        /// <summary>
        /// Configure the request localization feature
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UseZarayeRequestLocalization(this IApplicationBuilder application)
        {
            application.UseRequestLocalization(async options =>
            {
                if (!DataSettingsManager.IsDatabaseInstalled())
                    return;

                //prepare supported cultures
                var cultures = (await EngineContext.Current.Resolve<ILanguageService>().GetAllLanguagesAsync())
                    .OrderBy(language => language.DisplayOrder)
                    .Select(language => new CultureInfo(language.LanguageCulture)).ToList();
                options.SupportedCultures = cultures;
                options.SupportedUICultures = cultures;
                options.DefaultRequestCulture = new RequestCulture(cultures.FirstOrDefault());
                options.ApplyCurrentCultureToResponseHeaders = true;

                //configure culture providers
                //options.AddInitialRequestCultureProvider(new NopSeoUrlCultureProvider());
                var cookieRequestCultureProvider = options.RequestCultureProviders.OfType<CookieRequestCultureProvider>().FirstOrDefault();
                if (cookieRequestCultureProvider is not null)
                    cookieRequestCultureProvider.CookieName = $"{ZarayeCookieDefaults.Prefix}{ZarayeCookieDefaults.CultureCookie}";
            });
        }

        /// <summary>
        /// Configure Endpoints routing
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UseZarayeEndpoints(this IApplicationBuilder application)
        {
            //Execute the endpoint selected by the routing middleware
            application.UseEndpoints(endpoints =>
            {
                //register all routes
                EngineContext.Current.Resolve<IRoutePublisher>().RegisterRoutes(endpoints);
            });
        }

        /// <summary>
        /// Configure applying forwarded headers to their matching fields on the current request.
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UseZarayeProxy(this IApplicationBuilder application)
        {
            var appSettings = EngineContext.Current.Resolve<AppSettings>();

            if (appSettings.Get<HostingConfig>().UseProxy)
            {
                var options = new ForwardedHeadersOptions
                {
                    ForwardedHeaders = ForwardedHeaders.All,
                    // IIS already serves as a reverse proxy and will add X-Forwarded headers to all requests,
                    // so we need to increase this limit, otherwise, passed forwarding headers will be ignored.
                    ForwardLimit = 2
                };

                if (!string.IsNullOrEmpty(appSettings.Get<HostingConfig>().ForwardedForHeaderName))
                    options.ForwardedForHeaderName = appSettings.Get<HostingConfig>().ForwardedForHeaderName;

                if (!string.IsNullOrEmpty(appSettings.Get<HostingConfig>().ForwardedProtoHeaderName))
                    options.ForwardedProtoHeaderName = appSettings.Get<HostingConfig>().ForwardedProtoHeaderName;

                if (!string.IsNullOrEmpty(appSettings.Get<HostingConfig>().KnownProxies))
                {
                    foreach (var strIp in appSettings.Get<HostingConfig>().KnownProxies.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList())
                    {
                        if (IPAddress.TryParse(strIp, out var ip))
                            options.KnownProxies.Add(ip);
                    }

                    if (options.KnownProxies.Count > 1)
                        options.ForwardLimit = null; //disable the limit, because KnownProxies is configured
                }

                //configure forwarding
                application.UseForwardedHeaders(options);
            }
        }

        /// <summary>
        /// Configure WebMarkupMin
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UseZarayeWebMarkupMin(this IApplicationBuilder application)
        {
            //check whether database is installed
            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            application.UseWebMarkupMin();
        }
    }
}
