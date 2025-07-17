using Microsoft.EntityFrameworkCore;
using SquareDomain.Entities;
using SquareInfrastructure.Data.Configuration;

namespace SquareInfrastructure.Data;

public class SquareDataContext : DbContext
{
    public virtual DbSet<Point> Points { get; set; }
    
    public SquareDataContext(DbContextOptions<SquareDataContext> options) : base(options) { }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PointConfiguration).Assembly);
    }
}