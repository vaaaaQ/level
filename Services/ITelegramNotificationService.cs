namespace level.Services;

public interface ITelegramNotificationService
{
    Task<bool> SendNotificationAsync(string message);
}
