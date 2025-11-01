using System.ComponentModel.DataAnnotations;

namespace level.Models;

public sealed record TradingViewOrderRequest
{
    [Required]
    [RegularExpression("(?i)long|short")]
    public string Type { get; init; } = string.Empty;

    [Required]
    [MinLength(3)]
    public string Asset { get; init; } = string.Empty;

    [Range(typeof(decimal), "0.00000001", "79228162514264337593543950335")]
    public decimal Size { get; init; }

    public decimal? Stop { get; init; }

    public decimal? Take { get; init; }
}
