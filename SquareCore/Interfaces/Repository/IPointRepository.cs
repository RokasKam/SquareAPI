using SquareDomain.Entities;

namespace SquareCore.Interfaces.Repository;

public interface IPointRepository
{
    Task<IEnumerable<Point>> GetAllAsync();
    Task AddAsync(Point point);
    Task DeleteAsync(Point point);
    Task<Point?> GetPointAsync(Point point);
}