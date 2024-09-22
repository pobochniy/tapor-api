namespace Dodo.Tools.DB.MySql;

public class UUId
{
    public UUId(){}
    public UUId(byte[] value){}

    public byte[] ToByteArray()
    {
        return new byte[0x20];
    }
}
public class Uuid
{
    public Uuid(){}
    public Uuid(byte[] value){}

    public byte[] ToByteArray()
    {
        return new byte[0x20];
    }
}