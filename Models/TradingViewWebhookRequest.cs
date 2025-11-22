using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace level.Models;

public sealed record TradingViewWebhookRequest
{
    [Required]
    [MinLength(1)]
    public string Instrument { get; init; } = string.Empty;

    [Required]
    [MinLength(1)]
    public string Timeframe { get; init; } = string.Empty;

    public decimal? Price { get; init; }
    
    public string? Comment { get; init; }

    /// <summary>
    /// Markdown шаблон для форматирования сообщения.
    /// Поддерживает интерполяцию полей: {instrument}, {timeframe}, {price}, {comment}
    /// Пример: "📊 *Алерт*\n\n🔹 *Инструмент:* {instrument}\n🕒 *Таймфрейм:* {timeframe}\n💰 *Цена:* {price}\n💬 {comment}"
    /// </summary>
    public string? Template { get; init; }

    /// <summary>
    /// Дополнительные пользовательские поля в формате JSON
    /// </summary>
    public JsonElement? Data { get; init; }
}
