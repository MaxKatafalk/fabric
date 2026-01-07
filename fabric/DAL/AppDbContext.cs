using Microsoft.EntityFrameworkCore;
using fabric.DAL.Models;

namespace fabric.DAL
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Material> Materials { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<ProductionTask> ProductionTasks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=fabric.db");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().HasKey(r => r.Id);
            modelBuilder.Entity<Role>().Property(r => r.Name).IsRequired();

            modelBuilder.Entity<User>().HasKey(u => u.Id);
            modelBuilder.Entity<User>().Property(u => u.Username).IsRequired();
            modelBuilder.Entity<User>().HasOne(u => u.Role).WithMany().HasForeignKey(u => u.RoleId);

            modelBuilder.Entity<Material>().HasKey(m => m.Id);
            modelBuilder.Entity<Material>().Property(m => m.Name).IsRequired();
            modelBuilder.Entity<Material>().Property(m => m.Quantity).HasDefaultValue(0);

            modelBuilder.Entity<Order>().HasKey(o => o.Id);
            modelBuilder.Entity<Order>().Property(o => o.OrderNumber).IsRequired();
            modelBuilder.Entity<Order>().Property(o => o.CustomerName).IsRequired();
            modelBuilder.Entity<Order>().Property(o => o.OrderDate).IsRequired();
            modelBuilder.Entity<Order>().Property(o => o.Status).HasDefaultValue(OrderStatus.Created);

            modelBuilder.Entity<ProductionTask>().HasKey(t => t.Id);
            modelBuilder.Entity<ProductionTask>().Property(t => t.Description).IsRequired(false);
            modelBuilder.Entity<ProductionTask>().Property(t => t.QuantityPerUnit).HasDefaultValue(0);

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Master" },
                new Role { Id = 2, Name = "Seamstress" },
                new Role { Id = 3, Name = "Storekeeper" },
                new Role { Id = 4, Name = "Manager" }
            );

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    PasswordHash = "8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918",
                    FullName = "Administrator",
                    IsActive = true,
                    RoleId = 4
                }
            );
        }
    }
}
