namespace Tapor.Services;

public class SystemUser
{
    public Guid _userId { get; private set; }
    // email
    // nickname

    public SystemUser()// UserRepository
    {
        
    }

    public void SetUserId(Guid currentUserId)
    {
        _userId = currentUserId;
        // get email from repo
    }
}