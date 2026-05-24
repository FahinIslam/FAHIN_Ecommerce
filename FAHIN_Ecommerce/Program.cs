using System.Text;
using FAHIN_Ecommerce.Context;
using FAHIN_Ecommerce.Models;
using FAHIN_Ecommerce.Data.Entity;
using FAHIN_Ecommerce.Services;
using FAHIN_Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

#region Database Context
builder.Services.AddDbContext<dbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHttpContextAccessor();
#endregion

#region Identity Setup
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
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
})
.AddEntityFrameworkStores<dbContext>()
.AddDefaultTokenProviders();
#endregion

#region JWT Settings
builder.Services.AddSingleton<IJwtFactoryService, JwtFactoryService>();
var jwtAppsettingsOptions = builder.Configuration.GetSection(nameof(JwtIssuerOptions));

SymmetricSecurityKey _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtAppsettingsOptions["SecreatKey"] ?? "SuperSecretKeyForFahinEcommerceMillionsOfUsers"));

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
builder.Services.AddAuthorization(option => {
    option.AddPolicy("UserAccessPolicy", policy => policy.Requirements.Add(new UserAccessPageRequirement(null, null)));
});

builder.Services.AddScoped<IAuthorizationHandler, UserAccessPageHandler>();
builder.Services.AddSingleton<IGlobalDataService, GlobalDataService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

builder.Services.AddAuthentication()
    .AddJwtBearer(configureOptions =>
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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
