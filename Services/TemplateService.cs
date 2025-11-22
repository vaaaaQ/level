using System.Text.Json;
using System.Text.RegularExpressions;

namespace level.Services;

public interface ITemplateService
{
    string RenderTemplate(string? template, object data);
}

public class TemplateService : ITemplateService
{
    private static readonly string DefaultTemplate = 
        "📊 *TradingView Alert*\n\n" +
        "🔹 *Instrument:* {instrument}\n" +
        "🕒 *Timeframe:* {timeframe}\n" +
        "💰 *Price:* {price}\n" +
        "💬 {comment}";

    public string RenderTemplate(string? template, object data)
    {
        var templateText = string.IsNullOrWhiteSpace(template) ? DefaultTemplate : template;
        
        // Конвертируем объект в словарь для удобной интерполяции
        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions 
        { 
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
        });
        var fields = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json) 
            ?? new Dictionary<string, JsonElement>();

        // Заменяем {fieldName} на значения из объекта
        var result = Regex.Replace(templateText, @"\{(\w+)\}", match =>
        {
            var fieldName = match.Groups[1].Value.ToLower();
            
            if (fields.TryGetValue(fieldName, out var value))
            {
                return value.ValueKind switch
                {
                    JsonValueKind.String => value.GetString() ?? "",
                    JsonValueKind.Number => value.GetDecimal().ToString(),
                    JsonValueKind.True => "true",
                    JsonValueKind.False => "false",
                    JsonValueKind.Null => "",
                    JsonValueKind.Object => value.ToString(),
                    JsonValueKind.Array => value.ToString(),
                    _ => ""
                };
            }

            // Если поле не найдено, оставляем пустую строку
            return "";
        });

        // Убираем пустые строки, которые остались от неиспользованных полей
        result = Regex.Replace(result, @"\n+", "\n");
        result = Regex.Replace(result, @"\n\s*\n", "\n");
        
        return result.Trim();
    }
}
