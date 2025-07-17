using Microsoft.AspNetCore.Mvc;
using SquareCore.Interfaces.Services;

namespace SquareAPI.Controllers;

public class SquareController : BaseController
{
    private readonly ISquareService _squareService;
    
    public SquareController(ISquareService squareService)
    {
        this._squareService = squareService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetSquare()
    {
        var square = await _squareService.GetSquaresAsync();
        return Ok(square);
    }
}