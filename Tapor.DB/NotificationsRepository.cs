using Microsoft.Extensions.Logging;

namespace Tapor.DB;

public class NotificationsRepository
{
    private readonly ILogger<NotificationsRepository> _logger;

    public NotificationsRepository(ILogger<NotificationsRepository> logger)
    {
        _logger = logger;
    }
    
    public void Save(string email, string text)
    {
        // создаем подключение к базе
        
        // формируем запрос
        
        // сохраняем в базу
        
        _logger.LogInformation("Notification saved");
    }
}