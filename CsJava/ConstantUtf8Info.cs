namespace CsJava;

public class ConstantUtf8Info : ConstantPoolInfo
{
    public readonly ushort length;

    public readonly byte[] bytes;

    public readonly string str;

    public ConstantUtf8Info(ushort length, byte[] bytes)
    {
        this.length = length;
        this.bytes = bytes;
        this.str = System.Text.Encoding.UTF8.GetString(bytes);
    }

    public override string ToString()
    {
        return $"{GetType().Name}:\n"
               + $"    {nameof(length)} : {length}\n"
               + $"    {nameof(bytes)} : {str}";
    }
}
