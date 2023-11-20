using Amazon.S3;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Zaraye;
using Zaraye.Core.Configuration;
using Zaraye.Core.Infrastructure;
using Zaraye.Framework.Infrastructure.Extensions;
using Zaraye.Middleware;

var builder = WebApplication.CreateBuilder(args);


builder.Configuration.AddJsonFile(ZarayeConfigurationDefaults.AppSettingsFilePath, true, true);

//var jwt = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();
var JwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();

if (!string.IsNullOrEmpty(builder.Environment?.EnvironmentName))
{
    var path = string.Format(ZarayeConfigurationDefaults.AppSettingsEnvironmentFilePath, builder.Environment.EnvironmentName);
    builder.Configuration.AddJsonFile(path, true, true);
}
builder.Configuration.AddEnvironmentVariables();

//load application settings
builder.Services.ConfigureApplicationSettings(builder);
builder.Services.AddAWSService<IAmazonS3>();
Environment.SetEnvironmentVariable("AWS_ACCESS_KEY_ID", builder.Configuration.GetSection("AWS").GetValue<string>("AWS_ACCESS_KEY_ID"));
Environment.SetEnvironmentVariable("AWS_SECRET_ACCESS_KEY", builder.Configuration.GetSection("AWS").GetValue<string>("AWS_SECRET_ACCESS_KEY"));

var appSettings = Singleton<AppSettings>.Instance;
var useAutofac = appSettings.Get<CommonConfig>().UseAutofac;
//var allowedOrigins = appSettings.Get<CorsConfig>().allowedOrigins;


if (useAutofac)
    builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
else
    builder.Host.UseDefaultServiceProvider(options =>
    {
        //we don't validate the scopes, since at the app start and the initial configuration we need 
        //to resolve some services (registered as "scoped") through the root container
        options.ValidateScopes = false;
        options.ValidateOnBuild = true;
    });

//add services to the application and configure service provider
builder.Services.ConfigureApplicationServices(builder);


builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(4, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});
builder.Services.AddVersionedApiExplorer(options =>
{
    // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service  
    // note: the specified format code will format the version as "'v'major[.minor][-status]"  
    options.GroupNameFormat = "'v'VVV";

    // note: this option is only necessary when versioning by url segment. the SubstitutionFormat  
    // can also be used to control the format of the API version in route templates  
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddSwaggerGen(swagger =>
{
    //This is to generate the Default UI of Swagger Documentation  
    swagger.SwaggerDoc("v4", new OpenApiInfo
    {
        Version = "V4",
        Title = "V4",
        Description = ".NET 7.0 Web API"
    });
    swagger.SwaggerDoc("v5", new OpenApiInfo
    {
        Version = "V5",
        Title = "V5",
        Description = ".NET 7.0 Web API"
    });
    // To Enable authorization using Swagger (JWT)  
    swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
    });
    swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = JwtSettings.Issuer,
        ValidAudience = JwtSettings.Issuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSettings.Key)),
    };
});

builder.Services.AddScoped<JwtService>();

builder.Services.AddCors(options =>
{
    var origins = builder.Configuration.GetSection("CorsConfig").Get<string[]>();
    options.AddPolicy("AllowAllOrigins", builder =>
    {
        builder.WithOrigins(origins)
               .AllowAnyHeader()
               .AllowCredentials()
               .AllowAnyMethod();
    });


});
// response compression
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
});

var app = builder.Build();

// response compression
app.UseResponseCompression();

app.UseDeveloperExceptionPage();

// Use Swagger UI only if in development environment
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v4/swagger.json", "v4");
    c.SwaggerEndpoint("/swagger/v5/swagger.json", "v5");
    c.DefaultModelsExpandDepth(-1);
    c.RoutePrefix = "swagger";
    c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
});

// Configure for production environment
// Example: Add security headers, enable HTTPS, set up logging, etc.
app.UseExceptionHandler("/page-not-found");

// Enable HTTPS redirection
app.UseHttpsRedirection();

// Set security headers
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("Content-Security-Policy", "...");
    await next();
});

// Other production-specific middleware and configuration

// Configure error handling
app.UseExceptionHandler("/page-not-found");

app.UseCors("AllowAllOrigins");

//app.UseMiddleware<JwtMiddleware>();
app.UseJwtMiddleware();
app.UseAuthentication();
app.UseAuthorization();

//WebSocket middleware
app.UseWebSockets();
app.UseWebSocketServerForZaraye();

//configure the application HTTP request pipeline
app.ConfigureRequestPipeline();
app.StartEngine();

app.Run();
