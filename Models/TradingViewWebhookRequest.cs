using System.ComponentModel.DataAnnotations;

namespace level.Models;

public sealed record TradingViewWebhookRequest
{
    [Required]
    [MinLength(1)]
    public string Instrument { get; init; } = string.Empty;

    [Required]
    [MinLength(1)]
    public string Timeframe { get; init; } = string.Empty;

    public string? Action { get; init; }
    public decimal? Price { get; init; }
    public string? Signal { get; init; }
}
