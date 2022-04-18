namespace CsJava;

public class ConstantMethodRefInfo : ConstantPoolInfo
{
    public readonly ushort class_index;
    public readonly ushort name_and_type_index;

    public ConstantMethodRefInfo(ushort classIndex, ushort nameAndTypeIndex)
    {
        class_index = classIndex;
        name_and_type_index = nameAndTypeIndex;
    }

    public override string ToString()
    {
        return $"{GetType().Name}:\n" 
               + $"    {nameof(class_index)} : {class_index}\n"
               + $"    {nameof(name_and_type_index)} : {name_and_type_index}";
    }
}
