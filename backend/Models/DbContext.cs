using Microsoft.EntityFrameworkCore;

namespace PolygonApp.Models;

public class ApplicationDbContext : DbContext
{
    public DbSet<PolygonData> Polygons { get; set; }
    public DbSet<PointData> Points { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PolygonData>()
            .HasMany(p => p.Points)
            .WithOne(pt => pt.PolygonData)
            .HasForeignKey(pt => pt.PolygonDataId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

