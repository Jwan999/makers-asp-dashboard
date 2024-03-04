using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;
using Makers.Database.Contexts;
using Makers.Security;
using Makers.Utilities;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.RollingFileAlternate;
using System.Text;
using Constants = Makers.Utilities.Constants;

var builder = WebApplication.CreateBuilder(args);

var tskUsers = string.Empty;
var tskServicesApi = string.Empty;

if (builder.Environment.IsDevelopment())
{
    tskUsers = "HvPyogPqGolixz6e7nbb6M4whT5wP6szDYb56x16C20XmMSqhxtapCg0ZdLYYs8w";
}
else
{
    tskUsers = SecurityHelper.GenerateRandMakerstring(68);
}

IConfigurationRoot environmentVariables = new ConfigurationBuilder().AddEnvironmentVariables().Build();

IConfigurationRoot Configuration = new ConfigurationBuilder()
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Warning()
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Fatal)
    .WriteTo.RollingFileAlternate(Path.Combine(Environment.CurrentDirectory, "Logs"),
        LogEventLevel.Warning,
        "{Timestamp:HH:mm:ss.fff}  - [{RequestId}] {Message}{NewLine}{Exception}",
        fileSizeLimitBytes: 50 * 1024 * 1024
    ).CreateLogger();

builder.Services.AddDbContext<Db>(options => options.UseSqlServer(SecurityHelper.GetSecret(Configuration.GetValue(typeof(string), "ConnectionString").ToString())));

builder.Services.Configure<Configurations>(Configuration.GetSection("ApplicationConfiguration"));

builder.Services.AddSingleton(Configuration);

builder.Services.AddScoped<IJwt, Jwt>();
builder.Services.AddScoped<IFileManager, FileManager>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSignalR();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(Constants.AuthSchemeServicesApi, options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,
        ValidIssuer = "Makers",
        ValidAudience = Constants.JwtAudienceServicesClients,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
                tskServicesApi))
    };
})
.AddJwtBearer(Constants.AuthSchemeUsers, options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,
        ValidIssuer = "Makers",
        ValidAudience = Constants.JwtAudienceWebApplication,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
                tskUsers))
    };
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];

            if (!string.IsNullOrEmpty(accessToken) &&
                context.HttpContext.Request.Path.StartsWithSegments(WebSocketHub.Route))
            {
                context.Token = accessToken;
            }

            return Task.CompletedTask;
        }
    };
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "CorsPolicy",
        policy =>
        {
            policy.AllowAnyOrigin();
            policy.AllowAnyHeader();
            policy.AllowAnyMethod();
        });
});

builder.Services.AddMvc(options =>
{
    options.Filters.Add<ExceptionHandler>();
}).AddNewtonsoftJson(option =>
{
    option.SerializerSettings.DateFormatString = "yyyy/MM/dd HH:mm:ss";
    option.SerializerSettings.ContractResolver = new DefaultContractResolver { NamingStrategy = new DefaultNamingStrategy { } };
});

builder.Services.AddSpaStaticFiles(configuration => { configuration.RootPath = "ClientApp/build"; });

builder.Services.Configure<FormOptions>(options => {
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartBodyLengthLimit = 500000000;
});

builder.Services.Configure<KestrelServerOptions>(options => {
    options.Limits.MaxRequestBodySize = 500000000;
});

var app = builder.Build();

app.Services.GetService<ILoggerFactory>().AddSerilog();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSpaStaticFiles();
app.UseStatusCodePages();
app.UseCors("CorsPolicy");
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute("default", "api/{controller}/{action=Index}/{id?}");
});

app.UseSpa(spa =>
{
    spa.Options.SourcePath = "ClientApp";

    if (app.Environment.IsDevelopment())
    {
        spa.UseReactDevelopmentServer(npmScript: "start");
    }

    // Staging and Production environment should not use the ReactDevelopmentServer.
});

app.MapHub<WebSocketHub>(WebSocketHub.Route);


using (var scope = app.Services.CreateScope())
{
    var cache = app.Services.GetService<IMemoryCache>();
    var db = scope.ServiceProvider.GetRequiredService<Db>();
    var config = scope.ServiceProvider.GetRequiredService(typeof(IOptions<Configurations>)) as IOptions<Configurations>;
    var jwtOptions = scope.ServiceProvider.GetRequiredService(typeof(IOptionsMonitor<JwtBearerOptions>)) as IOptionsMonitor<JwtBearerOptions>;

    Configurations.Reinit(db, cache, tskUsers);

    tskServicesApi = SecurityHelper.BuildServicesApiTsk(db, config.Value);

    // We are recreating this scheme to use the full key which requires database access that is not available untill this part.
    var servicesApiShceme = jwtOptions.Get(Constants.AuthSchemeServicesApi);
    servicesApiShceme.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,
        ValidIssuer = "Makers",
        ValidAudience = Constants.JwtAudienceServicesClients,
        IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    tskServicesApi))
    };
}

app.Run();
