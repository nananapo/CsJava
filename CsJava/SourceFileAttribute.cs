namespace CsJava;

public class SourceFileAttribute : AttributeInfo
{
    public readonly ushort sourcefile_index;

    public SourceFileAttribute(uint attributeLength, ushort sourcefile_index) : base(attributeLength)
    {
        this.sourcefile_index = sourcefile_index;
    }
}