using AutoMapper;
using SquareCore.Requests;
using SquareCore.Responses;
using SquareDomain.Entities;

namespace SquareCore.Mappings;

public class PointMappingProfile : Profile
{
    public PointMappingProfile()
    {
        CreateMap<PointRequest, Point>();
        CreateMap<Point, PointResponse>();
    }
}