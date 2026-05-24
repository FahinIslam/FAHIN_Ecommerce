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

#region SQL Server Scripts for Identity Tables
/*
-- AspNetRoles Table
CREATE TABLE [dbo].[AspNetRoles] (
    [Id]               NVARCHAR (450) NOT NULL,
    [Name]             NVARCHAR (256) NULL,
    [NormalizedName]   NVARCHAR (256) NULL,
    [ConcurrencyStamp] NVARCHAR (MAX) NULL,
    [description]      NVARCHAR (MAX) NULL, -- Custom property
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO
CREATE UNIQUE NONCLUSTERED INDEX [RoleNameIndex] ON [dbo].[AspNetRoles]([NormalizedName] ASC) WHERE ([NormalizedName] IS NOT NULL);
GO

-- AspNetUsers Table
CREATE TABLE [dbo].[AspNetUsers] (
    [Id]                   NVARCHAR (450)     NOT NULL,
    [UserName]             NVARCHAR (256)     NULL,
    [NormalizedUserName]   NVARCHAR (256)     NULL,
    [Email]                NVARCHAR (256)     NULL,
    [NormalizedEmail]      NVARCHAR (256)     NULL,
    [EmailConfirmed]       BIT                NOT NULL,
    [PasswordHash]         NVARCHAR (MAX)     NULL,
    [SecurityStamp]        NVARCHAR (MAX)     NULL,
    [ConcurrencyStamp]     NVARCHAR (MAX)     NULL,
    [PhoneNumber]          NVARCHAR (MAX)     NULL,
    [PhoneNumberConfirmed] BIT                NOT NULL,
    [TwoFactorEnabled]     BIT                NOT NULL,
    [LockoutEnd]           DATETIMEOFFSET (7) NULL,
    [LockoutEnabled]       BIT                NOT NULL,
    [AccessFailedCount]    INT                NOT NULL,
    [fullName]             NVARCHAR (100)     NOT NULL, -- Custom property
    [profilePicture]       NVARCHAR (MAX)     NULL,     -- Custom property
    [createdAt]            DATETIME2 (7)      NULL,     -- Custom property
    [isActive]             BIT                NOT NULL, -- Custom property
    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO
CREATE NONCLUSTERED INDEX [EmailIndex] ON [dbo].[AspNetUsers]([NormalizedEmail] ASC);
GO
CREATE UNIQUE NONCLUSTERED INDEX [UserNameIndex] ON [dbo].[AspNetUsers]([NormalizedUserName] ASC) WHERE ([NormalizedUserName] IS NOT NULL);
GO

-- AspNetUserRoles Table
CREATE TABLE [dbo].[AspNetUserRoles] (
    [UserId] NVARCHAR (450) NOT NULL,
    [RoleId] NVARCHAR (450) NOT NULL,
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY CLUSTERED ([UserId] ASC, [RoleId] ASC),
    CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[AspNetRoles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO
CREATE NONCLUSTERED INDEX [IX_AspNetUserRoles_RoleId] ON [dbo].[AspNetUserRoles]([RoleId] ASC);
GO

-- Other Identity Tables (Optional but recommended)
CREATE TABLE [dbo].[AspNetUserClaims] (
    [Id]         INT            IDENTITY (1, 1) NOT NULL,
    [UserId]     NVARCHAR (450) NOT NULL,
    [ClaimType]  NVARCHAR (MAX) NULL,
    [ClaimValue] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO
*/

/*
-- SEED DATA INSERT SCRIPTS

-- 1. Insert Roles
INSERT INTO [dbo].[AspNetRoles] ([Id], [Name], [NormalizedName], [description])
VALUES 
(NEWID(), 'Admin', 'ADMIN', 'System Administrator with full access'),
(NEWID(), 'Customer', 'CUSTOMER', 'General user with shopping access');
GO

-- 2. Insert Categories
INSERT INTO [dbo].[Categories] ([name], [description], [isDelete], [createdAt], [createdBy])
VALUES 
('Electronics', 'Gadgets, devices and more', 0, GETUTCDATE(), 'System'),
('Clothing', 'Apparel and accessories', 0, GETUTCDATE(), 'System'),
('Home & Garden', 'Furniture and home decor', 0, GETUTCDATE(), 'System');
GO

-- 3. Insert Products
-- Note: Replace @CatId1, @CatId2 with actual IDs from the Categories table after execution
DECLARE @CatId1 INT = (SELECT TOP 1 Id FROM Categories WHERE name = 'Electronics');
DECLARE @CatId2 INT = (SELECT TOP 1 Id FROM Categories WHERE name = 'Clothing');

INSERT INTO [dbo].[Products] ([name], [description], [price], [stockQuantity], [categoryId], [isDelete], [createdAt], [createdBy])
VALUES 
('High-End Laptop', 'Powerful laptop for professionals', 1500.00, 50, @CatId1, 0, GETUTCDATE(), 'System'),
('Smartphone Pro', 'Latest flagship smartphone', 999.99, 100, @CatId1, 0, GETUTCDATE(), 'System'),
('Cotton T-Shirt', 'Comfortable daily wear t-shirt', 19.99, 500, @CatId2, 0, GETUTCDATE(), 'System');
GO
*/
#endregion




here i want to make a e commerce site , where complete authentication and authorization will be applicable , here admin will have different layout
and for general user will have different layout, check the above code, for dbcontext and authentication purpose, make different controller 
for different purpose and use already created service for for all the interface , here you check the purchaseLog table PurchaseLog, here in the all thable 
you have to inhehit the base entity class, now make all the required classes for e commerce site and and make a new file bussiness.md file 
where you will have to explain the bussiness of this project, 
let me explain one thing that this we are targetting millions of user for this site , so make sure you code and logic do not crashes the system,
make everything , here the server will be sql server and front end will be razor view, it will be a Monolith architecture