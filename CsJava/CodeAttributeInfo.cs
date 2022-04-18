namespace CsJava;

public class CodeAttributeInfo : AttributeInfo
{
    public readonly ushort max_stack;
    public readonly ushort max_locals;
    public readonly ushort code_length;
    public readonly byte[] code;
    public readonly ushort exception_table_length;
    public readonly ushort attributes_count;
    public readonly List<AttributeInfo> attributes;

    public CodeAttributeInfo(uint attributeLength, ushort maxStack, ushort maxLocals, ushort codeLength, 
        byte[] code, ushort exceptionTableLength, ushort attributesCount, List<AttributeInfo> attributes) 
        : base(attributeLength)
    {
        this.max_stack = maxStack;
        this.max_locals = maxLocals;
        this.code_length = codeLength;
        this.code = code;
        this.exception_table_length = exceptionTableLength;
        this.attributes_count = attributesCount;
        this.attributes = attributes;
    }
}