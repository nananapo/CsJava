namespace CsJava;

public class ConstantClassInfo : ConstantPoolInfo
{
    public readonly ushort name_index;

    public ConstantClassInfo(ushort nameIndex)
    {
        name_index = nameIndex;
    }

    public override string ToString()
    {
        return $"{GetType().Name}:\n"
               + $"    {nameof(name_index)} : {name_index}";
    }
}
