using Microsoft.EntityFrameworkCore;
using SquareCore.Interfaces.Repository;
using SquareDomain.Entities;
using SquareInfrastructure.Data;

namespace SquareInfrastructure.Repositories;

public class PointRepository : IPointRepository
{
    private readonly SquareDataContext _context;

    public PointRepository(SquareDataContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<Point>> GetAllAsync()
    {
        return await _context.Points.ToListAsync();
    }

    public async Task AddAsync(Point point)
    {
        _context.Points.Add(point); 
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Point point)
    {
        _context.Points.Remove(point);
        await _context.SaveChangesAsync();
    }

    public Task<Point?> GetPointAsync(Point point)
    {
        return _context.Points
            .FirstOrDefaultAsync(p => p.X == point.X && p.Y == point.Y);
    }
}