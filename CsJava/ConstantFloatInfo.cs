namespace CsJava;

public class ConstantFloatInfo : ConstantPoolInfo
{
    public readonly float bytes;

    public ConstantFloatInfo(float bytes)
    {
        this.bytes = bytes;
    }

    public override string ToString()
    {
        return $"{GetType().Name}: {bytes}";
    }
}
