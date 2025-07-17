using AutoMapper;
using Moq;
using SquareCore.Interfaces.Repository;
using SquareCore.Requests;
using SquareCore.Services;
using SquareDomain.Entities;
using SquareDomain.Exeptions;

namespace SquareAPITest.UnitTests;

public class PointServiceTests
{
    private readonly Mock<IPointRepository> _mockPointRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly PointService _service;

    public PointServiceTests()
    {
        _mockPointRepository = new Mock<IPointRepository>();
        _mockMapper = new Mock<IMapper>();

        _service = new PointService(
            _mockMapper.Object,
            _mockPointRepository.Object);
    }

    #region AddPointAsync Tests

    [Fact]
    public async Task AddPointAsync_AddsPoint_WhenPointDoesNotExist()
    {
        var pointRequest = new PointRequest();
        var point = new Point();

        _mockMapper.Setup(m => m.Map<Point>(pointRequest)).Returns(point);
        _mockPointRepository.Setup(r => r.GetPointAsync(point)).ReturnsAsync((Point)null);

        await _service.AddPointAsync(pointRequest);

        _mockPointRepository.Verify(r => r.AddAsync(point), Times.Once);
    }

    [Fact]
    public async Task AddPointAsync_ThrowsException_WhenPointAlreadyExists()
    {
        var pointRequest = new PointRequest();
        var point = new Point();
        var existingPoint = new Point();

        _mockMapper.Setup(m => m.Map<Point>(pointRequest)).Returns(point);
        _mockPointRepository.Setup(r => r.GetPointAsync(point)).ReturnsAsync(existingPoint);

        var exception = await Assert.ThrowsAsync<BadRequestException>(() => _service.AddPointAsync(pointRequest));
        Assert.Equal("Point already exists.", exception.Message);
        _mockPointRepository.Verify(r => r.AddAsync(It.IsAny<Point>()), Times.Never);
    }

    #endregion

    #region ImportAsync Tests

    [Fact]
    public async Task ImportAsync_AddsAllPoints_WhenNoneExist()
    {
        var pointRequests = new List<PointRequest> 
        { 
            new PointRequest(),
            new PointRequest() 
        };

        var points = new List<Point>
        {
            new Point(),
            new Point()
        };

        _mockMapper.Setup(m => m.Map<List<Point>>(pointRequests)).Returns(points);
        
        _mockPointRepository.Setup(r => r.GetPointAsync(points[0])).ReturnsAsync((Point)null);
        _mockPointRepository.Setup(r => r.GetPointAsync(points[1])).ReturnsAsync((Point)null);

        var result = await _service.ImportAsync(pointRequests);

        _mockPointRepository.Verify(r => r.AddAsync(points[0]), Times.Once);
        _mockPointRepository.Verify(r => r.AddAsync(points[1]), Times.Once);
        Assert.Equal("All points were added successfully.", result.Message);
    }

    [Fact]
    public async Task ImportAsync_SkipsExistingPoints_WhenSomeExist()
    {
        var pointRequests = new List<PointRequest>
        {
            new PointRequest(),
            new PointRequest()
        };

        var points = new List<Point>
        {
            new Point(),
            new Point()
        };

        var existingPoint = new Point();

        _mockMapper.Setup(m => m.Map<List<Point>>(pointRequests)).Returns(points);
        
        _mockPointRepository.Setup(r => r.GetPointAsync(points[0])).ReturnsAsync(existingPoint);
        _mockPointRepository.Setup(r => r.GetPointAsync(points[1])).ReturnsAsync((Point)null);

        var result = await _service.ImportAsync(pointRequests);

        _mockPointRepository.Verify(r => r.AddAsync(points[0]), Times.Never);
        _mockPointRepository.Verify(r => r.AddAsync(points[1]), Times.Once);
        Assert.Equal("1 points were added successfully.", result.Message);
    }

    [Fact]
    public async Task ImportAsync_SkipsAllPoints_WhenAllExist()
    {
        var pointRequests = new List<PointRequest>
        {
            new PointRequest(),
            new PointRequest()
        };

        var points = new List<Point>
        {
            new Point(),
            new Point()
        };

        var existingPoint1 = new Point();
        var existingPoint2 = new Point();

        _mockMapper.Setup(m => m.Map<List<Point>>(pointRequests)).Returns(points);
        
        _mockPointRepository.Setup(r => r.GetPointAsync(points[0])).ReturnsAsync(existingPoint1);
        _mockPointRepository.Setup(r => r.GetPointAsync(points[1])).ReturnsAsync(existingPoint2);

        var result = await _service.ImportAsync(pointRequests);

        _mockPointRepository.Verify(r => r.AddAsync(It.IsAny<Point>()), Times.Never);
        Assert.Equal("0 points were added successfully.", result.Message);
    }

    [Fact]
    public async Task ImportAsync_ReturnsCorrectMessage_WithEmptyList()
    {
        var pointRequests = new List<PointRequest>();
        var points = new List<Point>();

        _mockMapper.Setup(m => m.Map<List<Point>>(pointRequests)).Returns(points);

        var result = await _service.ImportAsync(pointRequests);

        _mockPointRepository.Verify(r => r.AddAsync(It.IsAny<Point>()), Times.Never);
        Assert.Equal("All points were added successfully.", result.Message);
    }

    #endregion

    #region DeletePointAsync Tests

    [Fact]
    public async Task DeletePointAsync_DeletesPoint_WhenPointExists()
    {
        var pointRequest = new PointRequest();
        var point = new Point();
        var existingPoint = new Point();

        _mockMapper.Setup(m => m.Map<Point>(pointRequest)).Returns(point);
        _mockPointRepository.Setup(r => r.GetPointAsync(point)).ReturnsAsync(existingPoint);

        await _service.DeletePointAsync(pointRequest);

        _mockPointRepository.Verify(r => r.DeleteAsync(existingPoint), Times.Once);
    }

    [Fact]
    public async Task DeletePointAsync_ThrowsException_WhenPointNotFound()
    {
        var pointRequest = new PointRequest();
        var point = new Point();

        _mockMapper.Setup(m => m.Map<Point>(pointRequest)).Returns(point);
        _mockPointRepository.Setup(r => r.GetPointAsync(point)).ReturnsAsync((Point)null);

        var exception = await Assert.ThrowsAsync<BadRequestException>(() => _service.DeletePointAsync(pointRequest));
        Assert.Equal("Point not found.", exception.Message);
        _mockPointRepository.Verify(r => r.DeleteAsync(It.IsAny<Point>()), Times.Never);
    }

    #endregion
}