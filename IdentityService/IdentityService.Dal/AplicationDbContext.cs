using Microsoft.EntityFrameworkCore;
using CoreLib.Entities;

namespace MyApp.DAL;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Role>().HasData(
            new Role { Id = Guid.NewGuid(), Name = "USER" },
            new Role { Id = Guid.NewGuid(), Name = "ADMIN" }
        );
    }
}