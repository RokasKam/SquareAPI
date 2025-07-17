using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SquareDomain.Entities;

namespace SquareInfrastructure.Data.Configuration;

public class PointConfiguration : IEntityTypeConfiguration<Point>
{
    public void Configure(EntityTypeBuilder<Point> builder)
    {
        builder
            .HasKey(p => new { p.X, p.Y });
    }
}