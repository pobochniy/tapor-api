namespace Tapor.Shared;

public class MySingleton: IMySingleton
{
    public int _id; 
    
    public MySingleton(int id)
    {
        _id = id;
    }
}

public interface IMySingleton
{
    
}