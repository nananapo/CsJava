namespace CsJava;

public class ConstantIntegerInfo : ConstantPoolInfo
{
    public readonly int bytes;

    public ConstantIntegerInfo(int bytes)
    {
        this.bytes = bytes;
    }

    public override string ToString()
    {
        return $"{GetType().Name}: {bytes}";
    }
}
