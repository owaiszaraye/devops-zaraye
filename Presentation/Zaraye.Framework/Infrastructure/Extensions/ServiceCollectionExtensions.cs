using Azure.Identity;
using Azure.Storage.Blobs;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using StackExchange.Profiling.Storage;
using System;
using System.Linq;
using System.Net;
using WebMarkupMin.AspNetCore7;
using WebMarkupMin.Core;
using WebMarkupMin.NUglify;
using Zaraye.Core;
using Zaraye.Core.Configuration;
using Zaraye.Core.Domain.Common;
using Zaraye.Core.Http;
using Zaraye.Core.Infrastructure;
using Zaraye.Core.Security;
using Zaraye.Data;
using Zaraye.Services.Authentication;
using Zaraye.Services.Common;
using Zaraye.Services.Security;

namespace Zaraye.Framework.Infrastructure.Extensions
{
    /// <summary>
    /// Represents extensions of IServiceCollection
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Configure base application settings
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="builder">A builder for web applications and services</param>
        public static void ConfigureApplicationSettings(this IServiceCollection services,
            WebApplicationBuilder builder)
        {
            //let the operating system decide what TLS protocol version to use
            //see https://docs.microsoft.com/dotnet/framework/network-programming/tls
            ServicePointManager.SecurityProtocol = SecurityProtocolType.SystemDefault;

            //create default file provider
            CommonHelper.DefaultFileProvider = new ZarayeFileProvider(builder.Environment);

            //register type finder
            var typeFinder = new WebAppTypeFinder();
            Singleton<ITypeFinder>.Instance = typeFinder;
            services.AddSingleton<ITypeFinder>(typeFinder);

            //add configuration parameters
            var configurations = typeFinder
                .FindClassesOfType<IConfig>()
                .Select(configType => (IConfig)Activator.CreateInstance(configType))
                .ToList();

            foreach (var config in configurations)
                builder.Configuration.GetSection(config.Name).Bind(config, options => options.BindNonPublicProperties = true);

            var appSettings = AppSettingsHelper.SaveAppSettings(configurations, CommonHelper.DefaultFileProvider, false);
            services.AddSingleton(appSettings);
        }

        /// <summary>
        /// Add services to the application and configure service provider
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="builder">A builder for web applications and services</param>
        public static void ConfigureApplicationServices(this IServiceCollection services,
            WebApplicationBuilder builder)
        {
            //add accessor to HttpContext
            services.AddHttpContextAccessor();

            //initialize plugins
            var mvcCoreBuilder = services.AddMvcCore();
            // var pluginConfig = new PluginConfig();
            // builder.Configuration.GetSection(nameof(PluginConfig)).Bind(pluginConfig, options => options.BindNonPublicProperties = true);
            // mvcCoreBuilder.PartManager.InitializePlugins(pluginConfig);

            //create engine and configure service provider
            var engine = EngineContext.Create();

            engine.ConfigureServices(services, builder.Configuration);
        }

        /// <summary>
        /// Register HttpContextAccessor
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public static void AddHttpContextAccessor(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        /// <summary>
        /// Adds services required for anti-forgery support
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public static void AddAntiForgery(this IServiceCollection services)
        {
            //override cookie name
            services.AddAntiforgery(options =>
            {
                options.Cookie.Name = $"{ZarayeCookieDefaults.Prefix}{ZarayeCookieDefaults.AntiforgeryCookie}";
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            });
        }

        /// <summary>
        /// Adds services required for application session state
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public static void AddHttpSession(this IServiceCollection services)
        {
            services.AddSession(options =>
            {
                options.Cookie.Name = $"{ZarayeCookieDefaults.Prefix}{ZarayeCookieDefaults.SessionCookie}";
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            });
        }

        /// <summary>
        /// Adds services required for distributed cache
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public static void AddDistributedCache(this IServiceCollection services)
        {
            var appSettings = Singleton<AppSettings>.Instance;
            var distributedCacheConfig = appSettings.Get<DistributedCacheConfig>();

            if (!distributedCacheConfig.Enabled)
                return;

            switch (distributedCacheConfig.DistributedCacheType)
            {
                case DistributedCacheType.Memory:
                    services.AddDistributedMemoryCache();
                    break;

                case DistributedCacheType.SqlServer:
                    services.AddDistributedSqlServerCache(options =>
                    {
                        options.ConnectionString = distributedCacheConfig.ConnectionString;
                        options.SchemaName = distributedCacheConfig.SchemaName;
                        options.TableName = distributedCacheConfig.TableName;
                    });
                    break;

                case DistributedCacheType.Redis:
                    services.AddStackExchangeRedisCache(options =>
                    {
                        options.Configuration = distributedCacheConfig.ConnectionString;
                    });
                    break;
            }
        }

        /// <summary>
        /// Adds data protection services
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public static void AddZarayeDataProtection(this IServiceCollection services)
        {
            var appSettings = Singleton<AppSettings>.Instance;
            if (appSettings.Get<AzureBlobConfig>().Enabled && appSettings.Get<AzureBlobConfig>().StoreDataProtectionKeys)
            {
                var blobServiceClient = new BlobServiceClient(appSettings.Get<AzureBlobConfig>().ConnectionString);
                var blobContainerClient = blobServiceClient.GetBlobContainerClient(appSettings.Get<AzureBlobConfig>().DataProtectionKeysContainerName);
                var blobClient = blobContainerClient.GetBlobClient(ZarayeDataProtectionDefaults.AzureDataProtectionKeyFile);

                var dataProtectionBuilder = services.AddDataProtection().PersistKeysToAzureBlobStorage(blobClient);

                if (!appSettings.Get<AzureBlobConfig>().DataProtectionKeysEncryptWithVault)
                    return;

                var keyIdentifier = appSettings.Get<AzureBlobConfig>().DataProtectionKeysVaultId;
                var credentialOptions = new DefaultAzureCredentialOptions();
                var tokenCredential = new DefaultAzureCredential(credentialOptions);

                dataProtectionBuilder.ProtectKeysWithAzureKeyVault(new Uri(keyIdentifier), tokenCredential);
            }
            else
            {
                var dataProtectionKeysPath = CommonHelper.DefaultFileProvider.MapPath(ZarayeDataProtectionDefaults.DataProtectionKeysPath);
                var dataProtectionKeysFolder = new System.IO.DirectoryInfo(dataProtectionKeysPath);

                //configure the data protection system to persist keys to the specified directory
                services.AddDataProtection().PersistKeysToFileSystem(dataProtectionKeysFolder);
            }
        }

        /// <summary>
        /// Adds authentication service
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public static void AddZarayeAuthentication(this IServiceCollection services)
        {
            //set default authentication schemes
            var authenticationBuilder = services.AddAuthentication(options =>
            {
                options.DefaultChallengeScheme = ZarayeAuthenticationDefaults.AuthenticationScheme;
                options.DefaultScheme = ZarayeAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = ZarayeAuthenticationDefaults.ExternalAuthenticationScheme;
            });

            //add main cookie authentication
            authenticationBuilder.AddCookie(ZarayeAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.Cookie.Name = $"{ZarayeCookieDefaults.Prefix}{ZarayeCookieDefaults.AuthenticationCookie}";
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                options.LoginPath = ZarayeAuthenticationDefaults.LoginPath;
                options.AccessDeniedPath = ZarayeAuthenticationDefaults.AccessDeniedPath;
            });

            //add external authentication
            authenticationBuilder.AddCookie(ZarayeAuthenticationDefaults.ExternalAuthenticationScheme, options =>
            {
                options.Cookie.Name = $"{ZarayeCookieDefaults.Prefix}{ZarayeCookieDefaults.ExternalAuthenticationCookie}";
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                options.LoginPath = ZarayeAuthenticationDefaults.LoginPath;
                options.AccessDeniedPath = ZarayeAuthenticationDefaults.AccessDeniedPath;
            });

            //register and configure external authentication plugins now
            var typeFinder = Singleton<ITypeFinder>.Instance;
            //var externalAuthConfigurations = typeFinder.FindClassesOfType<IExternalAuthenticationRegistrar>();
            //var externalAuthInstances = externalAuthConfigurations
            //    .Select(x => (IExternalAuthenticationRegistrar)Activator.CreateInstance(x));

            //foreach (var instance in externalAuthInstances)
            //    instance.Configure(authenticationBuilder);
        }

        /// <summary>
        /// Add and configure MVC for the application
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <returns>A builder for configuring MVC services</returns>
        public static IMvcBuilder AddZarayeMvc(this IServiceCollection services)
        {
            //add basic MVC feature
            var mvcBuilder = services.AddControllersWithViews();

            mvcBuilder.AddRazorRuntimeCompilation();

            var appSettings = Singleton<AppSettings>.Instance;
            if (appSettings.Get<CommonConfig>().UseSessionStateTempDataProvider)
            {
                //use session-based temp data provider
                mvcBuilder.AddSessionStateTempDataProvider();
            }
            else
            {
                //use cookie-based temp data provider
                mvcBuilder.AddCookieTempDataProvider(options =>
                {
                    options.Cookie.Name = $"{ZarayeCookieDefaults.Prefix}{ZarayeCookieDefaults.TempDataCookie}";
                    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                });
            }

            services.AddRazorPages();

            //MVC now serializes JSON with camel case names by default, use this code to avoid it
            mvcBuilder.AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());

            //set some options
            mvcBuilder.AddMvcOptions(options =>
            {
                //we'll use this until https://github.com/dotnet/aspnetcore/issues/6566 is solved 
                //options.ModelBinderProviders.Insert(0, new InvariantNumberModelBinderProvider());
                //options.ModelBinderProviders.Insert(1, new CustomPropertiesModelBinderProvider());
                //add custom display metadata provider 
                // options.ModelMetadataDetailsProviders.Add(new ZarayeMetadataProvider());

                //in .NET model binding for a non-nullable property may fail with an error message "The value '' is invalid"
                //here we set the locale name as the message, we'll replace it with the actual one later when not-null validation failed
                //options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(_ => NopValidationDefaults.NotNullValidationLocaleName);
            });

            //add fluent validation
            services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();

            //register all available validators from Nop assemblies
            var assemblies = mvcBuilder.PartManager.ApplicationParts
                .OfType<AssemblyPart>()
                .Where(part => part.Name.StartsWith("Zaraye", StringComparison.InvariantCultureIgnoreCase))
                .Select(part => part.Assembly);
            services.AddValidatorsFromAssemblies(assemblies);

            //register controllers as services, it'll allow to override them
            mvcBuilder.AddControllersAsServices();

            return mvcBuilder;
        }

        /// <summary>
        /// Add and configure MiniProfiler service
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public static void AddZarayeMiniProfiler(this IServiceCollection services)
        {
            //whether database is already installed
            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            var appSettings = Singleton<AppSettings>.Instance;
            if (appSettings.Get<CommonConfig>().MiniProfilerEnabled)
            {
                services.AddMiniProfiler(miniProfilerOptions =>
                {
                    //use memory cache provider for storing each result
                    ((MemoryCacheStorage)miniProfilerOptions.Storage).CacheDuration = TimeSpan.FromMinutes(appSettings.Get<CacheConfig>().DefaultCacheTime);

                    //determine who can access the MiniProfiler results
                    miniProfilerOptions.ResultsAuthorize = request => EngineContext.Current.Resolve<IPermissionService>().AuthorizeAsync(StandardPermissionProvider.AccessProfiling).Result;
                });
            }
        }

        /// <summary>
        /// Add and configure WebMarkupMin service
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public static void AddZarayeWebMarkupMin(this IServiceCollection services)
        {
            //check whether database is installed
            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            services
                .AddWebMarkupMin(options =>
                {
                    options.AllowMinificationInDevelopmentEnvironment = true;
                    options.AllowCompressionInDevelopmentEnvironment = true;
                    options.DisableMinification = !EngineContext.Current.Resolve<CommonSettings>().EnableHtmlMinification;
                    options.DisableCompression = true;
                    options.DisablePoweredByHttpHeaders = true;
                })
                .AddHtmlMinification(options =>
                {
                    options.MinificationSettings.AttributeQuotesRemovalMode = HtmlAttributeQuotesRemovalMode.KeepQuotes;

                    options.CssMinifierFactory = new NUglifyCssMinifierFactory();
                    options.JsMinifierFactory = new NUglifyJsMinifierFactory();
                })
                .AddXmlMinification(options =>
                {
                    var settings = options.MinificationSettings;
                    settings.RenderEmptyTagsWithSpace = true;
                    settings.CollapseTagsWithoutContent = true;
                });
        }

        /// <summary>
        /// Add and configure default HTTP clients
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public static void AddZarayeHttpClients(this IServiceCollection services)
        {
            //default client
            services.AddHttpClient(ZarayeHttpDefaults.DefaultHttpClient).WithProxy();

            //client to request current store
            services.AddHttpClient<StoreHttpClient>();

            //client to request nopCommerce official site
            services.AddHttpClient<ZarayeHttpClient>().WithProxy();

            //client to request reCAPTCHA service
            //services.AddHttpClient<CaptchaHttpClient>().WithProxy();
        }
    }
}