using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TopBookStore.Domain.Entities;
using TopBookStore.Infrastructure.Identity;

namespace TopBookStore.Infrastructure.Identity;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<Customer> Customers { get; set; } = null!;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);

        // Map ApplicationUser to Customer
        builder.Entity<ApplicationUser>()
            .HasOne<Customer>()
            .WithOne()
            .HasForeignKey<ApplicationUser>(a => a.CustomerId);
    }
}
