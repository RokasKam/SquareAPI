using System.ComponentModel.DataAnnotations;

namespace SquareCore.Requests;

public class PointRequest
{
    [Required(ErrorMessage = "X coordinate is required")]
    public int X { get; set; }
    [Required(ErrorMessage = "Y coordinate is required")]
    public int Y { get; set; }
}