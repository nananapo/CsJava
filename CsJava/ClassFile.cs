namespace CsJava;

public class ClassFile
{
    public uint Magic;
    public ushort MinorVersion;
    public ushort MajorVersion;
    public ushort ConstantPoolCount;
    public ConstantPoolInfo[] ConstantPoolInfos;
    public ushort AccessFlags;
    public ushort ThisClass;
    public ushort SuperClass;
    public ushort InterfacesCount;
    public ushort[] Interfaces;
    public ushort FieldsCount;
    //public ushort[] Fields;
    public ushort MethodsCount;
    public MethodInfo[] Methods;
    public ushort AttributesCount;
    public List<AttributeInfo> Attributes;
}