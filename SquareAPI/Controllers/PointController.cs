using Microsoft.AspNetCore.Mvc;
using SquareCore.Interfaces.Services;
using SquareCore.Requests;

namespace SquareAPI.Controllers;

public class PointController : BaseController
{
    private readonly IPointService _pointService;
    
    public PointController(IPointService pointService)
    {
        this._pointService = pointService;
    }
    
    [HttpPost]
    public async Task<IActionResult> CreatePoint([FromBody] PointRequest pointRequest)
    {
        await _pointService.AddPointAsync(pointRequest);
        return Created();
    }
    
    [HttpPost("import")]
    public async Task<IActionResult> ImportPoints([FromBody] List<PointRequest> pointRequests)
    {
        var result = await _pointService.ImportAsync(pointRequests);
        return Ok(result);
    }
    [HttpDelete("X/{x}/Y/{y}")]
    public async Task<IActionResult> DeletePoint(int x, int y)
    {
        await _pointService.DeletePointAsync(new PointRequest() { X = x, Y = y });
        return NoContent();
    }
}