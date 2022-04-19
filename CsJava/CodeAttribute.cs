namespace CsJava;

public class CodeAttribute : AttributeInfo
{
    public readonly ushort max_stack;
    public readonly ushort max_locals;
    public readonly uint code_length;
    public readonly byte[] code;
    public readonly ushort exception_table_length;
    //TODO exceptions
    public readonly ushort attributes_count;
    public readonly List<AttributeInfo> attributes;

    public CodeAttribute(uint attributeLength, ushort maxStack, ushort maxLocals, uint codeLength, 
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