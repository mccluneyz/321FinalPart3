using Microsoft.EntityFrameworkCore;
using CoffeeCo.Models;

namespace CoffeeCo.Data;

public class CoffeeDbContext : DbContext
{
    public CoffeeDbContext(DbContextOptions<CoffeeDbContext> options) : base(options)
    {
    }

    public DbSet<Shop> Shop { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Shop>(entity =>
        {
            entity.ToTable("Shop");
            entity.HasKey(e => e.ShopID);
            entity.Property(e => e.ShopID).HasColumnName("ShopID");
            entity.Property(e => e.ShopName).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Rating).HasColumnType("DECIMAL(3,2)").HasDefaultValue(0.00m);
            entity.Property(e => e.DateEntered).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Favorited).HasDefaultValue(false);
            entity.Property(e => e.Deleted).HasDefaultValue(false);
        });
    }
}

