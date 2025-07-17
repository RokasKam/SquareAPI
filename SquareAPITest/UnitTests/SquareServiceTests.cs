using AutoMapper;
using Moq;
using SquareCore.Interfaces.Repository;
using SquareCore.Responses;
using SquareCore.Services;
using SquareDomain.Entities;
using SquareDomain.Exeptions;

namespace SquareAPITest.UnitTests;

public class SquareServiceTests
{
    private readonly Mock<IPointRepository> _mockPointRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly SquareService _service;

    public SquareServiceTests()
    {
        _mockPointRepository = new Mock<IPointRepository>();
        _mockMapper = new Mock<IMapper>();

        _service = new SquareService(
            _mockMapper.Object,
            _mockPointRepository.Object);
    }

    #region GetSquaresAsync Tests

    [Fact]
    public async Task GetSquaresAsync_ThrowsException_WhenLessThanFourPoints()
    {
        var points = new List<Point>
        {
            new Point { X = 0, Y = 0 },
            new Point { X = 1, Y = 0 },
            new Point { X = 0, Y = 1 }
        };

        _mockPointRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(points);

        var exception = await Assert.ThrowsAsync<BadRequestException>(() => _service.GetSquaresAsync());
        Assert.Equal("At least 4 points are required to form a square.", exception.Message);
    }

    [Fact]
    public async Task GetSquaresAsync_ReturnsEmptyList_WhenNoSquaresFound()
    {
        var points = new List<Point>
        {
            new Point { X = 0, Y = 0 },
            new Point { X = 1, Y = 0 },
            new Point { X = 0, Y = 1 },
            new Point { X = 2, Y = 2 }
        };

        _mockPointRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(points);
        _mockMapper.Setup(m => m.Map<IEnumerable<PointResponse>>(It.IsAny<List<Point>>()))
            .Returns(new List<PointResponse>());

        var result = await _service.GetSquaresAsync();

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetSquaresAsync_ReturnsOneSquare_WhenOneSquareIsFound()
    {
        var points = new List<Point>
        {
            new Point { X = 0, Y = 0 },
            new Point { X = 1, Y = 0 },
            new Point { X = 0, Y = 1 },
            new Point { X = 1, Y = 1 }
        };

        var pointResponses = new List<PointResponse>
        {
            new PointResponse(),
            new PointResponse(),
            new PointResponse(),
            new PointResponse()
        };

        _mockPointRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(points);
        _mockMapper.Setup(m => m.Map<IEnumerable<PointResponse>>(It.IsAny<List<Point>>()))
            .Returns(pointResponses);

        var result = await _service.GetSquaresAsync();

        var squareResponses = result.ToList();
        Assert.Single(squareResponses);
        var squareResponse = squareResponses.First();
        Assert.Equal(4, squareResponse.Points.Count());
    }

    [Fact]
    public async Task GetSquaresAsync_ReturnsMultipleSquares_WhenMultipleSquaresAreFound()
    {
        var points = new List<Point>
        {
            new Point { X = 0, Y = 0 },
            new Point { X = 1, Y = 0 },
            new Point { X = 2, Y = 0 },
            new Point { X = 0, Y = 1 },
            new Point { X = 1, Y = 1 },
            new Point { X = 2, Y = 1 }
        };

        _mockPointRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(points);
        
        _mockMapper.Setup(m => m.Map<IEnumerable<PointResponse>>(It.Is<List<Point>>(p => 
            p.Any(point => point.X == 0 && point.Y == 0))))
            .Returns(new List<PointResponse> { new PointResponse(), new PointResponse(), new PointResponse(), new PointResponse() });
        
        _mockMapper.Setup(m => m.Map<IEnumerable<PointResponse>>(It.Is<List<Point>>(p => 
            p.Any(point => point.X == 2 && point.Y == 0))))
            .Returns(new List<PointResponse> { new PointResponse(), new PointResponse(), new PointResponse(), new PointResponse() });

        var result = await _service.GetSquaresAsync();

        var squareResponses = result.ToList();
        Assert.Equal(2, squareResponses.Count());
        foreach (var square in squareResponses)
        {
            Assert.Equal(4, square.Points.Count());
        }
    }

    [Fact]
    public async Task GetSquaresAsync_HandlesRotatedSquares()
    {
        var points = new List<Point>
        {
            new Point { X = 0, Y = 0 },
            new Point { X = 1, Y = 1 },
            new Point { X = 0, Y = 2 },
            new Point { X = -1, Y = 1 }
        };

        var pointResponses = new List<PointResponse>
        {
            new PointResponse(),
            new PointResponse(),
            new PointResponse(),
            new PointResponse()
        };

        _mockPointRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(points);
        _mockMapper.Setup(m => m.Map<IEnumerable<PointResponse>>(It.IsAny<List<Point>>()))
            .Returns(pointResponses);

        var result = await _service.GetSquaresAsync();

        var squareResponses = result as SquareResponse[] ?? result.ToArray();
        Assert.Single(squareResponses);
        var squareResponse = squareResponses.First();
        Assert.Equal(4, squareResponse.Points.Count());
    }

    #endregion

    #region FindSquares Tests (Testing Private Method Through Public Interface)

    [Fact]
    public async Task FindSquares_IdentifiesSquareWithExactCoordinates()
    {
        var points = new List<Point>
        {
            new Point { X = 0, Y = 0 },
            new Point { X = 1, Y = 0 },
            new Point { X = 0, Y = 1 },
            new Point { X = 1, Y = 1 }
        };

        var pointResponses = new List<PointResponse>
        {
            new PointResponse(),
            new PointResponse(),
            new PointResponse(),
            new PointResponse()
        };

        _mockPointRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(points);
        _mockMapper.Setup(m => m.Map<IEnumerable<PointResponse>>(It.IsAny<List<Point>>()))
            .Returns(pointResponses);
        
        var result = await _service.GetSquaresAsync();
        
        Assert.Single(result);
    }

    [Fact]
    public async Task FindSquares_IdentifiesSquareWithLargerDimensions()
    {
        var points = new List<Point>
        {
            new Point { X = 0, Y = 0 },
            new Point { X = 2, Y = 0 },
            new Point { X = 0, Y = 2 },
            new Point { X = 2, Y = 2 }
        };

        var pointResponses = new List<PointResponse>
        {
            new PointResponse(),
            new PointResponse(),
            new PointResponse(),
            new PointResponse()
        };

        _mockPointRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(points);
        _mockMapper.Setup(m => m.Map<IEnumerable<PointResponse>>(It.IsAny<List<Point>>()))
            .Returns(pointResponses);
        
        var result = await _service.GetSquaresAsync();
        
        Assert.Single(result);
    }

    [Fact]
    public async Task FindSquares_WorksWithNegativeCoordinates()
    {
        var points = new List<Point>
        {
            new Point { X = -1, Y = -1 },
            new Point { X = 0, Y = -1 },
            new Point { X = -1, Y = 0 },
            new Point { X = 0, Y = 0 }
        };

        var pointResponses = new List<PointResponse>
        {
            new PointResponse(),
            new PointResponse(),
            new PointResponse(),
            new PointResponse()
        };

        _mockPointRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(points);
        _mockMapper.Setup(m => m.Map<IEnumerable<PointResponse>>(It.IsAny<List<Point>>()))
            .Returns(pointResponses);

        var result = await _service.GetSquaresAsync();

        Assert.Single(result);
    }

    #endregion
}