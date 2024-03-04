using Makers.Database.Contexts;
using Makers.Security;
using Makers.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Makers.Controllers;

[AuthorizeClaim]
[Authorize(AuthenticationSchemes = Constants.AuthSchemeUsers)]
public partial class DashboardController : Controller
{
    private readonly Db db;
    private readonly IJwt jwt;
    private readonly ILogger logger;
    private readonly IMemoryCache cache;
    private readonly Configurations config;
    private readonly IFileManager fileManager;
    private readonly string BaseURL;
    private readonly IHttpContextAccessor httpContextAccessor;

    public DashboardController(Db db,
        IJwt jwt,
        IMemoryCache cache,
        ILogger<DashboardController> logger,
        IOptions<Configurations> config,
        IFileManager fileManager,
        IHttpContextAccessor httpContextAccessor)
    {
        this.db = db;
        this.jwt = jwt;
        this.cache = cache;
        this.logger = logger;
        this.config = config.Value;
        this.fileManager = fileManager;
        this.httpContextAccessor = httpContextAccessor;
        this.BaseURL = $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}{httpContextAccessor.HttpContext.Request.PathBase}";
    }
}