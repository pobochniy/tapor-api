using Tapor.DB;

namespace Tapor.Services;

public class NotificationService
{
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
        var notificationRepository = new NotificationsRepository();
        notificationRepository.Save(email, text);
    }
}