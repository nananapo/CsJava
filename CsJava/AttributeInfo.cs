namespace CsJava;

public abstract class AttributeInfo
{
    public readonly uint attribute_length;

    public AttributeInfo(uint attributeLength)
    {
        this.attribute_length = attributeLength;
    }
}