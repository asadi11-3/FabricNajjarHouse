using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NajjarFabricHouse.Data.Models;


namespace NajjarFabricHouse.Data.DataBase
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Customer> CustomerCustomer { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Classification> Classifications { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Manager> Managers { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<ProductVariant> ProductColors { get; set; }
        public DbSet<ClassificationProduct> ClassificationProducts { get; set; }




      
        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IdentityUserToken<string>>()
    .HasKey(ut => new { ut.UserId, ut.LoginProvider, ut.Name });
            modelBuilder.Entity<IdentityUserRole<string>>()
    .HasKey(ur => new { ur.UserId, ur.RoleId });
            modelBuilder.Entity<IdentityUserLogin<string>>()
       .HasKey(ul => new { ul.UserId, ul.LoginProvider });
            modelBuilder.Entity<ClassificationProduct>()
        .HasKey(pc => new { pc.ProductId, pc.CategoryId }); 

            modelBuilder.Entity<ClassificationProduct>()
                .HasOne(pc => pc.Product) 
                .WithMany(p => p.ClassificationProducts) 
                .HasForeignKey(pc => pc.ProductId);

            modelBuilder.Entity<ClassificationProduct>()
                .HasOne(pc => pc.Classification) 
                .WithMany(c => c.ClassificationProducts) 
                .HasForeignKey(pc => pc.CategoryId); 
            modelBuilder.Entity<Inventory>()
    .HasOne(i => i.ProductVariant)
    .WithMany(p => p.Inventories)
    .HasForeignKey(i => i.ProductVariantId)
    .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Product)
                .WithMany(p => p.OrderDetails)
                .HasForeignKey(od => od.ProductId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.Order)
                .WithOne(o => o.Invoice)
                .HasForeignKey<Invoice>(i => i.OrderId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.User)
                .WithMany(c => c.CartItems)
                .HasForeignKey(ci => ci.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Product)
                .WithMany(p => p.CartItems)
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Restrict); //إ الاحتفاظ بعناصر السلة حتى لو تم حذف المنت:


            modelBuilder.Entity<Review>()
                .HasOne(r => r.Product)
                .WithMany(p => p.Reviews)
                .HasForeignKey(r => r.ProductId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(c => c.Reviews)
                .HasForeignKey(r => r.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);



            modelBuilder.Entity<Employee>()
          .HasOne(e => e.Manager)
          .WithMany(m => m.Employees)
          .HasForeignKey(e => e.ManagerId)
          .OnDelete(DeleteBehavior.SetNull); //  حتى  لا يحذف الموظفين عند حذف المدير




            modelBuilder.Entity<Employee>()
       .HasOne(e => e.ApplicationUser)
       .WithOne()
       .HasForeignKey<Employee>(e => e.UserId)
       .OnDelete(DeleteBehavior.Cascade);

            // ربط Customers مع AspNetUsers
            modelBuilder.Entity<Customer>()
                .HasOne(c => c.ApplicationUser)
                .WithOne()
                .HasForeignKey<Customer>(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ربط Managers مع AspNetUsers
            modelBuilder.Entity<Manager>()
                .HasOne(m => m.ApplicationUser)
                .WithOne()
                .HasForeignKey<Manager>(m => m.UserId)
                 .OnDelete(DeleteBehavior.NoAction);//هذا يعني أن حذف المستخدم (User) لن يؤدي إلى حذف المدير (Manager) تلقائيًا.

            modelBuilder.Entity<ApplicationUser>(b =>
            {
                b.ToTable("Users");
            });

            modelBuilder.Entity<IdentityRole>(b =>
            {
                b.ToTable("Roles");
            });

            modelBuilder.Entity<IdentityUserRole<string>>(b =>
            {
                b.ToTable("UserRoles");
            });

            modelBuilder.Entity<IdentityUserClaim<string>>(b =>
            {
                b.ToTable("UserClaims");
            });

            modelBuilder.Entity<IdentityUserLogin<string>>(b =>


            {
                b.ToTable("UserLogins");
            });

            modelBuilder.Entity<IdentityRoleClaim<string>>(b =>
            {
                b.ToTable("RoleClaims");
            });

            modelBuilder.Entity<IdentityUserToken<string>>(b =>
            {
                b.ToTable("UserTokens");
            });


            var adminRole = new IdentityRole { Id = "0fe36a30-09ce-489d-8197-af8a7dcd5fe3", Name = "Admin", NormalizedName = "ADMIN" };

            var userRole = new IdentityRole { Id = "f5ac3c38-c10e-422b-9f87-b1bfe89b4e7b", Name = "User", NormalizedName = "USER" };
            var managerRole = new IdentityRole { Id = "cab5cdc7-8b2c-4d3b-96e2-dec82aff3be3", Name = "Manager", NormalizedName = "MANAGER" };
            var employeeRole = new IdentityRole { Id = "874b7c0c-a707-49a6-9e10-7a8697604042", Name = "Employee", NormalizedName = "EMPLOYEE" };


            modelBuilder.Entity<IdentityRole>().HasData(adminRole, userRole, managerRole, employeeRole);


            var hasher = new PasswordHasher<ApplicationUser>();
            var adminUser = new ApplicationUser
            {
                Id = "3bea86c0-f190-40ee-8d6a-334f5e81124c",
                UserName = "YasserNajjar",
                NormalizedUserName = "YASSERNAJJAR",
                Email = "YasserNajjar@example.com",
                NormalizedEmail = "YASSERNAJJAR@EXAMPLE.COM",
                EmailConfirmed = true,
                PasswordHash = hasher.HashPassword(null, "Aa@123"),
                SecurityStamp = Guid.NewGuid().ToString(),
                RefreshToken = string.Empty,
                RefreshTokenExpiry= DateTime.UtcNow.AddDays(30),
                TwoFactorEnabled=true,
            };

            modelBuilder.Entity<ApplicationUser>().HasData(adminUser);


            modelBuilder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            {
                RoleId = adminRole.Id,
                UserId = adminUser.Id
            });
        }
    }
}
