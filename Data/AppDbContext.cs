using Microsoft.EntityFrameworkCore;
using TaxAccount.Models;

namespace TaxAccount.Data
{
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) {  }

    public DbSet<Product> Products { get; set; }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<InvoiceItem> InvoiceItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //future proof for Identity inheritance or future ef features
            base.OnModelCreating(modelBuilder);

            // Product price precision
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);
            
            // Composite key for RolePermission
            modelBuilder.Entity<RolePermission>()
                .HasKey(rp => new { rp.RoleId, rp.PermissionId });

            // Role → RolePermission
            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleId);

            // Permission → RolePermission
            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(rp => rp.PermissionId);

            // User email must be unique
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Seed Roles
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Admin", Description = "Full access" },
                new Role { Id = 2, Name = "Manager", Description = "Manage operations" },
                new Role { Id = 3, Name = "Staff", Description = "Day to day operations" },
                new Role { Id = 4, Name = "Customer", Description = "View only access" }
            );

            // Seed Permissions
            modelBuilder.Entity<Permission>().HasData(
                new Permission { Id = 1, Name = "products.view", Description = "View products" },
                new Permission { Id = 2, Name = "products.create", Description = "Create products" },
                new Permission { Id = 3, Name = "products.edit", Description = "Edit products" },
                new Permission { Id = 4, Name = "products.delete", Description = "Delete products" },
                new Permission { Id = 5, Name = "invoices.view", Description = "View invoices" },
                new Permission { Id = 6, Name = "invoices.create", Description = "Create invoices" },
                new Permission { Id = 7, Name = "invoices.approve", Description = "Approve invoices" },
                new Permission { Id = 8, Name = "reports.view", Description = "View reports" },
                new Permission { Id = 9, Name = "users.manage", Description = "Manage users" },
                new Permission { Id = 10, Name = "accounts.manage", Description = "Manage accounts" }
            );
            // Seed RolePermissions
            modelBuilder.Entity<RolePermission>().HasData(
                // Admin - all permissions
                new RolePermission { RoleId = 1, PermissionId = 1 },
                new RolePermission { RoleId = 1, PermissionId = 2 },
                new RolePermission { RoleId = 1, PermissionId = 3 },
                new RolePermission { RoleId = 1, PermissionId = 4 },
                new RolePermission { RoleId = 1, PermissionId = 5 },
                new RolePermission { RoleId = 1, PermissionId = 6 },
                new RolePermission { RoleId = 1, PermissionId = 7 },
                new RolePermission { RoleId = 1, PermissionId = 8 },
                new RolePermission { RoleId = 1, PermissionId = 9 },
                new RolePermission { RoleId = 1, PermissionId = 10 },

                // Manager
                new RolePermission { RoleId = 2, PermissionId = 1 },
                new RolePermission { RoleId = 2, PermissionId = 2 },
                new RolePermission { RoleId = 2, PermissionId = 3 },
                new RolePermission { RoleId = 2, PermissionId = 5 },
                new RolePermission { RoleId = 2, PermissionId = 6 },
                new RolePermission { RoleId = 2, PermissionId = 7 },
                new RolePermission { RoleId = 2, PermissionId = 8 },

                // Staff
                new RolePermission { RoleId = 3, PermissionId = 1 },
                new RolePermission { RoleId = 3, PermissionId = 2 },
                new RolePermission { RoleId = 3, PermissionId = 5 },
                new RolePermission { RoleId = 3, PermissionId = 6 },

                // Customer
                new RolePermission { RoleId = 4, PermissionId = 1 },
                new RolePermission { RoleId = 4, PermissionId = 5 }
            );

        // Invoice precision
        modelBuilder.Entity<Invoice>()
        .Property(i => i.SubTotal).HasPrecision(18, 2);
        modelBuilder.Entity<Invoice>()
        .Property(i => i.TaxAmount).HasPrecision(18, 2);
        modelBuilder.Entity<Invoice>()
        .Property(i => i.TotalAmount).HasPrecision(18, 2);

        // InvoiceItem precision
        modelBuilder.Entity<InvoiceItem>()
        .Property(i => i.Quantity).HasPrecision(18, 2);
        modelBuilder.Entity<InvoiceItem>()
        .Property(i => i.UnitPrice).HasPrecision(18, 2);
        modelBuilder.Entity<InvoiceItem>()
        .Property(i => i.TaxPercent).HasPrecision(4, 2);
        modelBuilder.Entity<InvoiceItem>()
        .Property(i => i.TaxAmount).HasPrecision(18, 2);
        modelBuilder.Entity<InvoiceItem>()
        .Property(i => i.TotalAmount).HasPrecision(18, 2);

        // Invoice → Customer relationship
        modelBuilder.Entity<Invoice>()
        .HasOne(i => i.Customer)
        .WithMany()
        .HasForeignKey(i => i.CustomerId)
        .OnDelete(DeleteBehavior.Restrict);

        // Invoice → CreatedBy relationship
        modelBuilder.Entity<Invoice>()
        .HasOne(i => i.CreatedBy)
        .WithMany()
        .HasForeignKey(i => i.CreatedByUserId)
        .OnDelete(DeleteBehavior.Restrict);
        }
    }
}