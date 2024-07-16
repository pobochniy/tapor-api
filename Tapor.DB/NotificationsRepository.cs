using Tapor.Shared;

namespace Tapor.DB;

public class NotificationsRepository
{
    private readonly MySingleton _singleton;

    public NotificationsRepository(MySingleton singleton)
    {
        _singleton = singleton;
    }
    
    public void Save(string email, string text)
    {
        // создаем подключение к базе
        
        // формируем запрос
        
        // сохраняем в базу
    }
}