using Microsoft.Extensions.Options;
using level.Options;

namespace level.Services;

public class TelegramNotificationService : ITelegramNotificationService
{
    private readonly HttpClient _httpClient;
    private readonly TelegramOptions _options;
    private readonly ILogger<TelegramNotificationService> _logger;

    public TelegramNotificationService(
        HttpClient httpClient,
        IOptions<TelegramOptions> options,
        ILogger<TelegramNotificationService> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<bool> SendNotificationAsync(string message)
    {
        if (string.IsNullOrWhiteSpace(_options.BotToken) || string.IsNullOrWhiteSpace(_options.ChatId))
        {
            _logger.LogWarning("Telegram bot token or chat ID is not configured");
            return false;
        }

        try
        {
            var url = $"https://api.telegram.org/bot{_options.BotToken}/sendMessage";
            var payload = new
            {
                chat_id = _options.ChatId,
                text = message,
                parse_mode = "Markdown"
            };

            var response = await _httpClient.PostAsJsonAsync(url, payload);
            
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Telegram notification sent successfully");
                return true;
            }
            
            var error = await response.Content.ReadAsStringAsync();
            _logger.LogError("Failed to send Telegram notification: {Error}", error);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception while sending Telegram notification");
            return false;
        }
    }
}
