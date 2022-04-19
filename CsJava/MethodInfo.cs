namespace CsJava;

public class MethodInfo
{
    public readonly ushort access_flags;
    public readonly ushort name_index;
    public readonly ushort descriptor_index;
    public readonly ushort attributes_count;
    public readonly List<AttributeInfo> attributes;

    public MethodInfo(ushort accessFlags, ushort nameIndex, ushort descriptorIndex, ushort attributesCount, List<AttributeInfo> attributes)
    {
        this.access_flags = accessFlags;
        this.name_index = nameIndex;
        this.descriptor_index = descriptorIndex;
        this.attributes_count = attributesCount;
        this.attributes = attributes;
    }

    public T? GetAttribute<T>() where T : AttributeInfo
    {
        foreach (var attribute in attributes)
        {
            if (attribute is T t)
                return t;
        }
        return null;
    }
}