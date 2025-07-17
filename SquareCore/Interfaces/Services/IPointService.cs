using SquareCore.Requests;
using SquareCore.Responses;

namespace SquareCore.Interfaces.Services;

public interface IPointService
{
    Task AddPointAsync(PointRequest pointRequest);
    Task<ImportResponse> ImportAsync(IEnumerable<PointRequest> pointsRequests);
    Task DeletePointAsync(PointRequest pointRequest);
}