using SquareCore.Responses;

namespace SquareCore.Interfaces.Services;

public interface ISquareService
{
    Task<IEnumerable<SquareResponse>> GetSquaresAsync();
}