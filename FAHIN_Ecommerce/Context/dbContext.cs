using FAHIN_Ecommerce.Data.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FAHIN_Ecommerce.Context
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

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<PurchaseLog> PurchaseLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure soft delete query filter (using isDelete column)
            builder.Entity<Category>().HasQueryFilter(e => e.isDelete == 0);
            builder.Entity<Product>().HasQueryFilter(e => e.isDelete == 0);
            builder.Entity<Order>().HasQueryFilter(e => e.isDelete == 0);
            builder.Entity<OrderItem>().HasQueryFilter(e => e.isDelete == 0);
            builder.Entity<PurchaseLog>().HasQueryFilter(e => e.isDelete == 0);

            // Indexing for performance (High Scale)
            builder.Entity<Product>().HasIndex(p => p.name);
            builder.Entity<Product>().HasIndex(p => p.categoryId);
            builder.Entity<Order>().HasIndex(o => o.userId);
            builder.Entity<Order>().HasIndex(o => o.orderDate);
        }

        public override int SaveChanges()
        {
            UpdateAuditFields();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateAuditFields();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateAuditFields()
        {
            var entries = ChangeTracker.Entries<BaseEntity>();
            var currentUser = _httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? "System";

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.createdAt = DateTime.UtcNow;
                    entry.Entity.createdBy = currentUser;
                    entry.Entity.isDelete = 0;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.updatedAt = DateTime.UtcNow;
                    entry.Entity.updatedBy = currentUser;
                }
            }
        }
    }
}
