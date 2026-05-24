 public class dbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
 {
		private readonly IHttpContextAccessor _httpContextAccessor;

     public dbContext()
     {

     }

     public dbContext(DbContextOptions<dbContext> options, IHttpContextAccessor _httpContextAccessor) : base(options)
     {
         this._httpContextAccessor = _httpContextAccessor;
         Database.SetCommandTimeout(2500000);
     }



     builder.Services.AddSingleton<IJwtFactoryService, JwtFactoryService>();
var jwtAppsettingsOptions = builder.Configuration.GetSection(nameof(JwtIssuerOptions));

SymmetricSecurityKey _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtAppsettingsOptions["SecreatKey"]));

builder.Services.Configure<JwtIssuerOptions>(Options =>
{
    Options.Issuer = jwtAppsettingsOptions[nameof(JwtIssuerOptions.Issuer)];
    Options.Audience = jwtAppsettingsOptions[nameof(JwtIssuerOptions.Audience)];
    Options.SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
});

var tokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuer = true,
    ValidIssuer = jwtAppsettingsOptions[nameof(JwtIssuerOptions.Issuer)],

    ValidateAudience = true,
    ValidAudience = jwtAppsettingsOptions[nameof(JwtIssuerOptions.Audience)],

    ValidateIssuerSigningKey = true,
    IssuerSigningKey = _signingKey,

    RequireExpirationTime = false,
    ValidateLifetime = true,
    ClockSkew = TimeSpan.Zero
};
#endregion

#region Auth Related Settings
builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings.
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 4;
    options.Password.RequiredUniqueChars = 1;

    // Lockout settings.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings.
    options.User.AllowedUserNameCharacters =
    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = false;
});

#region User_Access_Policy
builder.Services.AddAuthorization(option => {
    option.AddPolicy("UserAccessPolicy", policy => policy.Requirements.Add(new UserAccessPageRequirement(null, null)));
});

builder.Services.AddScoped<IAuthorizationHandler, UserAccessPageHandler>();
builder.Services.AddSingleton<IGlobalDataService, GlobalDataService>();
#endregion

builder.Services.AddAuthentication().AddJwtBearer(configureOptions =>
{
    configureOptions.ClaimsIssuer = jwtAppsettingsOptions[nameof(JwtIssuerOptions.Issuer)];
    configureOptions.TokenValidationParameters = tokenValidationParameters;
    configureOptions.SaveToken = true;
});


builder.Services.ConfigureApplicationCookie(options =>
{
    // Cookie settings
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromHours(1);

    options.LoginPath = "/Auth/Account/Login";
    options.AccessDeniedPath = "/Auth/Account/AccessDenied";
    options.SlidingExpiration = true;
});
#endregion




using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UMOJA_ZAMBIA.Data.Entity;
using UMOJA_ZAMBIA.Data.Entity.Auth;
using UMOJA_ZAMBIA.Data.Entity.Client;
using UMOJA_ZAMBIA.Data.Entity.Employees;
using UMOJA_ZAMBIA.Data.Entity.Group;
using UMOJA_ZAMBIA.Data.Entity.Loan;
using UMOJA_ZAMBIA.Data.Entity.Navbar;
using UMOJA_ZAMBIA.Data.Entity.Meeting;
using UMOJA_ZAMBIA.Areas.Meeting.Model;
using UMOJA_ZAMBIA.Areas.Auth.Models;
using UMOJA_ZAMBIA.Models.Auth;
using UMOJA_ZAMBIA.Areas.Groups.Models;
using UMOJA_ZAMBIA.Accounting.Data.Entity.AccountingSettings;
using UMOJA_ZAMBIA.Accounting.Data.Entity.Voucher;
using UMOJA_ZAMBIA.Accounting.Data.Entity.NonPoTransaction;
using UMOJA_ZAMBIA.Accounting.Data.Entity.FDR;
using UMOJA_ZAMBIA.Areas.Accounting.Models;
using UMOJA_ZAMBIA.Accounting.Data.Entity.BankReconciliation;
using UMOJA_ZAMBIA.Accounting.Data.Entity.MasterData;
using UMOJA_ZAMBIA.Data.Entity.MasterData;
using UMOJA_ZAMBIA.Areas.Loans.Models;
using UMOJA_ZAMBIA.Models.Dashboard;
using UMOJA_ZAMBIA.Areas.Clients.Models;
using UMOJA_ZAMBIA.Areas.MasterData.Models;
using UMOJA_ZAMBIA.Areas.Employees.Models;
using UMOJA_ZAMBIA.Areas.API.Models;
using UMOJA_ZAMBIA.Models;
using UMOJA_ZAMBIA.Data.Entity.ShadowHistory;
using UMOJA_ZAMBIA.Areas.AuditLog.Models;
using UMOJA_ZAMBIA.Data.Entity.AccountingSettings;
using UMOJA_ZAMBIA.Accounting.Data.Entity;
using UMOJA_ZAMBIA.Areas.Loans.Views.Loan;
using UMOJA_ZAMBIA.Data.Entity.ReleaseInfo;
using UMOJA_ZAMBIA.Data.Entity.Release;
using UMOJA_ZAMBIA.Data.Entity.IncentiveTracker;
using UMOJA_ZAMBIA.Data.Entity.Logging;

namespace UMOJA_ZAMBIA.Context
{
    public class dbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
		private readonly IHttpContextAccessor _httpContextAccessor;

        public dbContext()
        {

        }

        public dbContext(DbContextOptions<dbContext> options, IHttpContextAccessor _httpContextAccessor) : base(options)
        {
            this._httpContextAccessor = _httpContextAccessor;
            Database.SetCommandTimeout(2500000);
        }




here i want to make a e commerce site , where complete authentication and authorization will be applicable , here admin will have different layout
and for general user will have different layout, check the above code, for dbcontext and authentication purpose, make different controller 
for different purpose and use already created service for for all the interface , here you check the purchaseLog table PurchaseLog, here in the all thable 
you have to inhehit the base entity class, now make all the required classes for e commerce site and and make a new file bussiness.md file 
where you will have to explain the bussiness of this project, 
let me explain one thing that this we are targetting millions of user for this site , so make sure you code and logic do not crashes the system,
make everything , here the server will be sql server and front end will be razor view, it will be a Monolith architecture