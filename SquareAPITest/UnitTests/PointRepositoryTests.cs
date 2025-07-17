using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using SquareDomain.Entities;
using SquareInfrastructure.Data;
using SquareInfrastructure.Repositories;

namespace SquareAPITest.UnitTests;

public class PointRepositoryTests
{
    private readonly Mock<SquareDataContext> _dbContextMock;
    private readonly PointRepository _repository;

    public PointRepositoryTests()
    {
        _dbContextMock = new Mock<SquareDataContext>(new DbContextOptions<SquareDataContext>());
        _repository = new PointRepository(_dbContextMock.Object);
    }

    #region GetAllAsync Tests

    [Fact]
    public async Task GetAllAsync_ReturnsAllPoints_WhenPointsExist()
    {
        var points = new List<Point>
        {
            new Point { X = 1, Y = 2 },
            new Point { X = 3, Y = 4 },
            new Point { X = 5, Y = 6 }
        };

        _dbContextMock.Setup(c => c.Points).ReturnsDbSet(points);

        var result = await _repository.GetAllAsync();

        var enumerable = result.ToList();
        Assert.Equal(3, enumerable.Count());
        Assert.Contains(enumerable, p => p is { X: 1, Y: 2 });
        Assert.Contains(enumerable, p => p is { X: 3, Y: 4 });
        Assert.Contains(enumerable, p => p is { X: 5, Y: 6 });
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEmptyList_WhenNoPointsExist()
    {
        var emptyPoints = new List<Point>();
        _dbContextMock.Setup(c => c.Points).ReturnsDbSet(emptyPoints);

        var result = await _repository.GetAllAsync();

        Assert.Empty(result);
    }

    #endregion

    #region AddAsync Tests

    [Fact]
    public async Task AddAsync_AddsPointToContext_AndSavesChanges()
    {
        var point = new Point { X = 1, Y = 2 };
        var dbSetMock = new Mock<DbSet<Point>>();
        
        _dbContextMock.Setup(c => c.Points).Returns(dbSetMock.Object);
        _dbContextMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1)
            .Verifiable();

        await _repository.AddAsync(point);

        dbSetMock.Verify(m => m.Add(point), Times.Once);
        _dbContextMock.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_RemovesPointFromContext_AndSavesChanges()
    {
        var point = new Point { X = 1, Y = 2 };
        var dbSetMock = new Mock<DbSet<Point>>();
        
        _dbContextMock.Setup(c => c.Points).Returns(dbSetMock.Object);
        _dbContextMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1)
            .Verifiable();

        await _repository.DeleteAsync(point);

        dbSetMock.Verify(m => m.Remove(point), Times.Once);
        _dbContextMock.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region GetPointAsync Tests

    [Fact]
    public async Task GetPointAsync_ReturnsPoint_WhenPointExists()
    {
        var points = new List<Point>
        {
            new Point { X = 1, Y = 2 },
            new Point { X = 3, Y = 4 },
            new Point { X = 5, Y = 6 }
        };

        _dbContextMock.Setup(c => c.Points).ReturnsDbSet(points);
        var searchPoint = new Point { X = 3, Y = 4 };

        var result = await _repository.GetPointAsync(searchPoint);

        Assert.NotNull(result);
        Assert.Equal(3, result.X);
        Assert.Equal(4, result.Y);
    }

    [Fact]
    public async Task GetPointAsync_ReturnsNull_WhenPointDoesNotExist()
    {
        var points = new List<Point>
        {
            new Point { X = 1, Y = 2 },
            new Point { X = 3, Y = 4 },
            new Point { X = 5, Y = 6 }
        };

        _dbContextMock.Setup(c => c.Points).ReturnsDbSet(points);
        var searchPoint = new Point { X = 7, Y = 8 };

        var result = await _repository.GetPointAsync(searchPoint);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetPointAsync_UsesCorrectFilter()
    {
        var points = new List<Point>
        {
            new Point { X = 1, Y = 2 },
            new Point { X = 1, Y = 3 },
            new Point { X = 2, Y = 2 },
            new Point { X = 3, Y = 4 }
        };

        _dbContextMock.Setup(c => c.Points).ReturnsDbSet(points);
        var searchPoint = new Point { X = 1, Y = 2 };

        var result = await _repository.GetPointAsync(searchPoint);

        Assert.NotNull(result);
        Assert.Equal(1, result.X);
        Assert.Equal(2, result.Y);
        
        Assert.NotEqual(3, result.Y);
        Assert.NotEqual(2, result.X);
    }

    #endregion
}