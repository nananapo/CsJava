namespace CsJava;

public class ConstantNameAndTypeInfo : ConstantPoolInfo
{
    public readonly ushort name_index;
    public readonly ushort descriptor_index;

    public ConstantNameAndTypeInfo(ushort nameIndex, ushort descriptorIndex)
    {
        name_index = nameIndex;
        descriptor_index = descriptorIndex;
    }

    public override string ToString()
    {
        return $"{GetType().Name}:\n"
               + $"    {nameof(name_index)} : {name_index}\n"
               + $"    {nameof(descriptor_index)} : {descriptor_index}";
    }
}
