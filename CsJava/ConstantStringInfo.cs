namespace CsJava;

public class ConstantStringInfo : ConstantPoolInfo
{
    public readonly ushort string_index;

    public ConstantStringInfo(ushort stringIndex)
    {
        string_index = stringIndex;
    }

    public override string ToString()
    {
        return $"{GetType().Name}:\n"
               + $"    {nameof(string_index)} : {string_index}";
    }
}
