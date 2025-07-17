using AutoMapper;
using SquareCore.Interfaces.Repository;
using SquareCore.Interfaces.Services;
using SquareCore.Responses;
using SquareDomain.Entities;
using SquareDomain.Exeptions;

namespace SquareCore.Services;

public class SquareService : ISquareService
{
    private readonly IPointRepository _pointRepository;
    private readonly IMapper _mapper;
    
    public SquareService(IMapper mapper, IPointRepository pointRepository)
    {
        _mapper = mapper;
        _pointRepository = pointRepository;
    }

    public async Task<IEnumerable<SquareResponse>> GetSquaresAsync()
    {
        var points = await _pointRepository.GetAllAsync();
        var pointsList = points.ToList();
        if (pointsList.Count() < 4)
        {
            throw new BadRequestException("At least 4 points are required to form a square.");
        }
        var squarePointGroups = FindSquares(pointsList);

        var result = squarePointGroups.Select(square =>
            new SquareResponse
            {
                Points = _mapper.Map<IEnumerable<PointResponse>>(square)
            });

        return result;
    }
    
    private IEnumerable<List<Point>> FindSquares(IEnumerable<Point> inputPoints)
    {
        var pointList = inputPoints.ToList();
        var pointSet = new HashSet<(int, int)>(pointList.Select(p => (p.X, p.Y)));
        var seenSquares = new HashSet<string>();
        var results = new List<List<Point>>();

        for (int i = 0; i < pointList.Count; i++)
        {
            for (int j = i + 1; j < pointList.Count; j++)
            {
                var p1 = pointList[i];
                var p2 = pointList[j];

                int dx = p2.X - p1.X;
                int dy = p2.Y - p1.Y;

                var p3 = new Point(){ X = p1.X + dy, Y = p1.Y - dx };
                var p4 = new Point(){ X = p2.X + dy, Y = p2.Y - dx };

                if (pointSet.Contains((p3.X, p3.Y)) && pointSet.Contains((p4.X, p4.Y)))
                {
                    var square = new[] { p1, p2, p3, p4 }
                        .OrderBy(p => p.X).ThenBy(p => p.Y)
                        .ToList();

                    string key = string.Join(";", square.Select(p => $"{p.X},{p.Y}"));
                    if (seenSquares.Add(key))
                    {
                        results.Add(square);
                    }
                }
            }
        }

        return results;
    }

}