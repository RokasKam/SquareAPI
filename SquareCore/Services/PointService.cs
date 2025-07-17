using AutoMapper;
using SquareCore.Interfaces.Repository;
using SquareCore.Interfaces.Services;
using SquareCore.Requests;
using SquareCore.Responses;
using SquareDomain.Entities;
using SquareDomain.Exeptions;

namespace SquareCore.Services;

public class PointService : IPointService
{
    private readonly IPointRepository _pointRepository;
    private readonly IMapper _mapper;

    public PointService(IMapper mapper, IPointRepository pointRepository)
    {
        _mapper = mapper;
        _pointRepository = pointRepository;
    }

    public async Task AddPointAsync(PointRequest pointRequest)
    {
        var point = _mapper.Map<Point>(pointRequest); 
        var existingPoint = await _pointRepository.GetPointAsync(point);
        if (existingPoint != null)
        {
            throw new BadRequestException("Point already exists.");
        }
        await _pointRepository.AddAsync(point);
    }

    public async Task<ImportResponse> ImportAsync(IEnumerable<PointRequest> pointsRequests)
    {
        var points = _mapper.Map<List<Point>>(pointsRequests);
        var importResponse = new ImportResponse();
        var pointsAdded = 0;

        foreach (var point in points)
        {
            var existingPoint = _pointRepository.GetPointAsync(point).Result;
            if (existingPoint == null)
            {
                await _pointRepository.AddAsync(point);
                pointsAdded++;
            }
        }
        if (pointsAdded < points.Count)
        {
            importResponse.Message = $"{pointsAdded} points were added successfully.";
        }
        else
        {
            importResponse.Message = "All points were added successfully.";
        }
        return importResponse;
    }

    public Task DeletePointAsync(PointRequest pointRequest)
    {
        var point = _mapper.Map<Point>(pointRequest);
        var existingPoint = _pointRepository.GetPointAsync(point).Result;
        if (existingPoint == null)
        {
            throw new BadRequestException("Point not found.");
        }
        return _pointRepository.DeleteAsync(existingPoint);
    }
}