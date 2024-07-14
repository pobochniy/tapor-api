using Tapor.DB;

namespace Tapor.Services;

public class NotificationService
{
    private readonly NotificationsRepository _repository;

    public NotificationService(NotificationsRepository repository)
    {
        _repository = repository;
    }
    
    public void IssueNotify(Guid userId, bool isAssignee, long issueId)
    {
        // фомируем текст письма для исполнителя или ответственного
        var text = isAssignee
            ? $"На вас упала задача {issueId}"
            : $"Вы поставили задачу {issueId}";
        
        // получаем эмейл пользователя
        var profileRepository = new ProfileRepository();
        var email = profileRepository.GetEmail(userId);
        
        // сохраняем письмо в базу
        _repository.Save(email, text);
    }
}