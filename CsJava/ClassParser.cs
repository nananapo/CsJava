namespace CsJava;

public class ClassParser
{

    private byte[] bytes;

    private int offset;

    private ClassFile classFile;

    public ClassParser(byte[] bytes)
    {
        this.bytes = bytes;
    }

    public ClassFile? Parse()
    {
        offset = 0;
        classFile = new ClassFile();
        
        if (!ReadMagic())
            return null;
        
        if (!ReadVersion())
            return null;
        
        if (!ReadConstantPoolCount())
            return null;

        if (!ReadConstantPoolInfos())
            return null;

        if (!ReadAccessFlags())
            return null;
        
        if (!ReadThisClass())
            return null;
        
        if (!ReadSuperClass())
            return null;

        if (!ReadInterfacesCount())
            return null;
        
        if (!ReadInterfaces())
            return null;

        if (!ReadFieldsCount())
            return null;

        if (!ReadMethodsCount())
            return null;
        
        if (!ReadMethods())
            return null;

        if (!ReadAttributesCount())
            return null;

        if (!ReadClassAttributes())
            return null;

        return classFile;
    }

    private bool CheckLength(int length)
    {
        return offset + length <= bytes.Length;
    }

    private bool CheckLengthU(uint length)
    {
        return offset + length <= bytes.Length;
    }

    private bool IsValidMagic()
    {
        return bytes.ReadBase16(offset, 4) == "cafebabe";
    }

    private bool ReadMagic()
    {
        if (!IsValidMagic())
        {
            Console.WriteLine("Magicが違う");
            return false;
        }

        Console.WriteLine("Magic : " + bytes.ReadBase16(offset, 4));
        
        classFile.Magic = bytes.ReadUInt(offset);
        offset += 4;

        return true;
    }

    private bool ReadVersion()
    {
        if (!CheckLength(4))
        {
            Console.WriteLine("minor_version, major_versionが読めない");
            return false;
        }

        classFile.MinorVersion = bytes.ReadUShort(offset);
        classFile.MajorVersion = bytes.ReadUShort(offset + 2);
        offset += 4;

        Console.WriteLine("MinorVersion : " + classFile.MinorVersion);
        Console.WriteLine("MajorVersion : " + classFile.MajorVersion);

        if (classFile.MinorVersion != 0 && classFile.MinorVersion != 65535)
        {
            Console.WriteLine("minorVersionがおかしい");
            return false;
        }
        return true;
    }
    
    private bool ReadConstantPoolCount()
    {
        if (!CheckLength(2))
        {
            Console.WriteLine("constant_pool_countを読めない");
            return false;
        }

        classFile.ConstantPoolCount = bytes.ReadUShort(offset);
        offset += 2;

        Console.WriteLine("constant_pool_count : " + classFile.ConstantPoolCount);
        return true;
    }

    private bool ReadConstantPoolInfos()
    {
        classFile.ConstantPoolInfos = new ConstantPoolInfo[classFile.ConstantPoolCount];

        Console.WriteLine("-----------------ConstantPoolInfos-----------------");
        for (int i=0; i < classFile.ConstantPoolCount - 1; i++)
        {
            var info = ReadConstantPoolInfo();
            if (info == null)
                return false;
            classFile.ConstantPoolInfos[i] = info!;
            Console.WriteLine(info);
        }
        Console.WriteLine("--------------------------------------------");
        
        return true;
    }

    private ConstantPoolInfo? ReadConstantPoolInfo()
    {
        if (!CheckLength(1))
        {
            Console.WriteLine("ConstantPoolのtagを読めない");
            return null;
        }
            
        var tag = bytes[offset++];
        switch (tag)
        {
            case 1:
            {
                if (!CheckLength(2))
                {
                    Console.WriteLine($"ConstantUtf8Infoのlengthを読めない");
                    return null;
                }
                    
                var length = bytes.ReadUShort(offset);
                offset += 2;
                
                if (bytes.Length < offset + length)
                {
                    Console.WriteLine($"ConstantUtf8Infoのbytesを読めない");
                    return null;
                }
                    
                byte[] b = new byte[length];
                for (int j = 0; j < length; j++)
                    b[j] = bytes[offset++];

                return new ConstantUtf8Info(length, b);
            }
            case 7:
            {
                if (!CheckLength(2))
                {
                    Console.WriteLine($"ConstantClassInfoのname_indexを読めない");
                    return null;
                }
                
                var name_index = bytes.ReadUShort(offset);
                offset += 2;

                return new ConstantClassInfo(name_index);
            }
            case 8:
            {
                if (!CheckLength(2))
                {
                    Console.WriteLine($"ConstantStringInfoのstring_indexを読めない");
                    return null;
                }
                    
                var string_index = bytes.ReadUShort(offset);
                offset += 2;

                return new ConstantStringInfo(string_index);
            }
            case 9:
            case 10:
            {
                if (!CheckLength(4))
                {
                    Console.WriteLine($"ConstantPool({tag})のclass_index, name_and_type_indexを読めない");
                    return null;
                }

                var class_index = bytes.ReadUShort(offset);
                var name_and_type_index = bytes.ReadUShort(offset + 2);
                offset += 4;

                if (tag == 9)
                    return new ConstantFieldRefInfo(class_index, name_and_type_index);
                else
                    return new ConstantMethodRefInfo(class_index, name_and_type_index);
            }
            case 12:
            {
                if (!CheckLength(4))
                {
                    Console.WriteLine($"ConstantNameAndTypeInfoのname_index , descriptor_indexを読めない");
                    return null;
                }

                var name_index = bytes.ReadUShort(offset);
                var descriptor_index = bytes.ReadUShort(offset + 2);
                offset += 4;

                return new ConstantNameAndTypeInfo(name_index, descriptor_index);
            }
            default:
                Console.WriteLine($"知らないtag : {tag}");
                return null;
        }
    }

    private bool ReadAccessFlags()
    {
        if (!CheckLength(2))
        {
            Console.WriteLine("access_flagsを読めない");
            return false;
        }

        classFile.AccessFlags = bytes.ReadUShort(offset);
        offset += 2;

        Console.WriteLine(nameof(classFile.AccessFlags) + " : " + classFile.AccessFlags);
        return true;
    }

    private bool ReadThisClass()
    {
        if (!CheckLength(2))
        {
            Console.WriteLine("this_classを読めない");
            return false;
        }

        classFile.ThisClass = bytes.ReadUShort(offset);
        offset += 2;

        Console.WriteLine("this_class : " + classFile.ThisClass);

        if (classFile.ThisClass > classFile.ConstantPoolCount)
        {
            Console.WriteLine($"this_class({classFile.ThisClass})が" +
                              $"ConstantPoolCount({classFile.ConstantPoolCount})よりも大きい");
            return false;
        }

        if (classFile.ConstantPoolInfos[classFile.ThisClass - 1].GetType() != typeof(ConstantClassInfo))
        {
            Console.WriteLine("this_classが指し示す型がConstantClassInfoではない");
            return false;
        }
        return true;
    }

    private bool ReadSuperClass()
    {
        if (!CheckLength(2))
        {
            Console.WriteLine("super_classを読めない");
            return false;
        }

        classFile.SuperClass = bytes.ReadUShort(offset);
        offset += 2;

        Console.WriteLine("super_class : " + classFile.SuperClass);

        if (classFile.SuperClass > classFile.ConstantPoolCount)
        {
            Console.WriteLine($"super_class({classFile.SuperClass})が" +
                              $"ConstantPoolCount({classFile.ConstantPoolCount})よりも大きい");
            return false;
        }

        if (classFile.ConstantPoolInfos[classFile.SuperClass - 1].GetType() != typeof(ConstantClassInfo))
        {
            Console.WriteLine("super_classが指し示す型がConstantClassInfoではない");
            return false;
        }
        return true;
    }
    
    private bool ReadInterfacesCount()
    {
        if (!CheckLength(2))
        {
            Console.WriteLine("interfaces_countを読めない");
            return false;
        }

        classFile.InterfacesCount = bytes.ReadUShort(offset);
        offset += 2;

        Console.WriteLine(nameof(classFile.InterfacesCount) + " : " + classFile.InterfacesCount);
        return true;
    }

    private bool ReadInterfaces()
    {
        classFile.Interfaces = new ushort[classFile.InterfacesCount];

        if (!CheckLength(2 * classFile.InterfacesCount))
        {
            Console.WriteLine("interfacesを読めない");
            return false;
        }

        for (int i=0;i < classFile.InterfacesCount; i++)
        {
            ushort interfaces_index = bytes.ReadUShort(offset);
            offset += 2;

            if (interfaces_index > classFile.ConstantPoolCount)
            {
                Console.WriteLine($"interfaces_index({interfaces_index})が" +
                                $"ConstantPoolCount({classFile.ConstantPoolCount})よりも大きい");
                return false;
            }

            if (classFile.ConstantPoolInfos[interfaces_index].GetType() != typeof(ConstantClassInfo))
            {
                Console.WriteLine("interfaces_indexが指し示す型がConstantClassInfoではない");
                return false;
            }

            classFile.Interfaces[i] = interfaces_index;
            Console.WriteLine(nameof(interfaces_index) + " : " + interfaces_index);
        }

        return true;
    }
    
    private bool ReadFieldsCount()
    {
        if (!CheckLength(2))
        {
            Console.WriteLine("fields_countを読めない");
            return false;
        }

        classFile.FieldsCount = bytes.ReadUShort(offset);
        offset += 2;

        Console.WriteLine(nameof(classFile.FieldsCount) + " : " + classFile.FieldsCount);
        return true;
    }
    
    private bool ReadMethodsCount()
    {
        if (!CheckLength(2))
        {
            Console.WriteLine("methods_countを読めない");
            return false;
        }

        classFile.MethodsCount = bytes.ReadUShort(offset);
        offset += 2;

        Console.WriteLine(nameof(classFile.MethodsCount) + " : " + classFile.MethodsCount);
        return true;
    }

    private bool ReadMethods()
    {
        Console.WriteLine("-----------------Methods-----------------");
        
        classFile.Methods = new MethodInfo[classFile.MethodsCount];
        for (int i = 0; i < classFile.MethodsCount; i++)
        {
            var method = ReadMethodInfo();
            if (method == null)
                return false;
            classFile.Methods[i] = method;
        }

        Console.WriteLine("--------------------------------------------");
        return true;
    }

    private MethodInfo? ReadMethodInfo()
    {
        if (!CheckLength(8))
        {
            Console.WriteLine("methodsを読めない");
            return null;
        }

        var access_flags = bytes.ReadUShort(offset);
        var name_index = bytes.ReadUShort(offset + 2);
        var descriptor_index = bytes.ReadUShort(offset + 4);
        var attributes_count = bytes.ReadUShort(offset + 6);
        offset += 8;
            
        Console.WriteLine("Method---");
        Console.WriteLine(nameof(access_flags) + " : " + access_flags);
        Console.WriteLine(nameof(name_index) + " : " + name_index);
        Console.WriteLine(nameof(descriptor_index) + " : " + descriptor_index);
        Console.WriteLine(nameof(attributes_count) + " : " + attributes_count);

        var attributes = ReadAttributes(attributes_count);
        if (attributes == null)
            return null;
        Console.WriteLine("--------");

        return new MethodInfo(access_flags, name_index, descriptor_index, attributes_count, attributes);
    }

    public List<AttributeInfo> ReadAttributes(ushort attributes_count)
    {
        var list = new List<AttributeInfo>();
        for (ushort i = 0; i < attributes_count; i++)
        {
            var attribute = ReadAttributeInfo();
            if (attribute == null)
                return null;
            list.Add(attribute);
        }
        return list;
    }

    private AttributeInfo? ReadAttributeInfo()
    {
        
        if (!CheckLength(6))
        {
            Console.WriteLine("attribute_infoを読めない");
            return null;
        }
        
        var attribute_name_index = bytes.ReadUShort(offset);
        var attribute_length = bytes.ReadUInt(offset + 2);
        offset += 6;
        
        Console.WriteLine(nameof(attribute_name_index) + " : " + attribute_name_index);
        Console.WriteLine(nameof(attribute_length) + " : " + attribute_length);
        
        if (attribute_name_index > classFile.ConstantPoolCount)
        {
            Console.WriteLine($"attribute_name_index({attribute_name_index})が" +
                              $"ConstantPoolCount({classFile.ConstantPoolCount})よりも大きい");
            return null;
        }
        
        if (classFile.ConstantPoolInfos[attribute_name_index - 1].GetType() != typeof(ConstantUtf8Info))
        {
            Console.WriteLine("attribute_name_indexが指し示す型がConstantUtf8Infoではない");
            return null;
        }

        var attribute_name = ((ConstantUtf8Info)classFile.ConstantPoolInfos[attribute_name_index - 1]).str;
        
        switch (attribute_name)
        {
            case "Code":
            {
                if (!CheckLength(8))
                {
                    Console.WriteLine("Codeを読めない");
                    return null;
                }
        
                ushort max_stack = bytes.ReadUShort(offset);
                ushort max_locals = bytes.ReadUShort(offset + 2);
                uint code_length = bytes.ReadUInt(offset + 4);
                offset += 8;
                
                Console.WriteLine(nameof(max_stack) + " : " + max_stack);
                Console.WriteLine(nameof(max_locals) + " : " + max_locals);
                Console.WriteLine(nameof(code_length) + " : " + code_length);
        
                if (!CheckLengthU(code_length))
                {
                    Console.WriteLine("Codeのcodeを読めない");
                    return null;
                }
                
                byte[] code = new byte[code_length];
                for (int k = 0; k < code_length; k++)
                {
                    //TODO
                    code[k] = bytes[offset];
                    offset++;
                }

                if (!CheckLength(2))
                {
                    Console.WriteLine("exception_table_lengthを読めない");
                    return null;
                }
        
                var exception_table_length = bytes.ReadUShort(offset);
                offset += 2;
                
                Console.WriteLine(nameof(exception_table_length) + " : " + exception_table_length);
        
                for (int k = 0; k < exception_table_length; k++)
                {
                    //TODO
                }
                
                if (!CheckLength(2))
                {
                    Console.WriteLine("attributes_countを読めない");
                    return null;
                }
                
                var attributes_count = bytes.ReadUShort(offset);
                offset += 2;
                
                Console.WriteLine(nameof(attributes_count) + " : " + attributes_count);
                
                var attributes = ReadAttributes(attributes_count);
                if (attributes == null)
                    return null;
                
                return new CodeAttribute(attribute_length, max_stack, max_locals, code_length, code, exception_table_length, attributes_count, attributes);
            }
            case "LineNumberTable":
            {
                if (!CheckLength(2))
                {
                    Console.WriteLine("line_number_table_lengthを読めない");
                    return null;
                }

                var line_number_table_length = bytes.ReadUShort(offset);
                offset += 2;

                if (!CheckLength(4 * line_number_table_length))
                {
                    Console.WriteLine("line_number_tableを読めない");
                    return null;
                }

                var list = new List<(ushort, ushort)>();
                for (int i=0;i<line_number_table_length;i++)
                {
                    list.Add((bytes.ReadUShort(offset), bytes.ReadUShort(offset + 2)));
                    offset += 4;
                }

                return new LineNumberAttribute(attribute_length, line_number_table_length, list);
            }
            case "SourceFile":
            {
                if (!CheckLength(2))
                {
                    Console.WriteLine("sourcefile_indexを読めない");
                    return null;
                }

                var sourcefile_index = bytes.ReadUShort(offset);
                offset += 2;

                return new SourceFileAttribute(attribute_length, sourcefile_index);
            }
            default:
                Console.WriteLine("分からない名前 : " + attribute_name);
                break;
        }
        
        return null;
    }
    
    private bool ReadAttributesCount()
    {
        if (!CheckLength(2))
        {
            Console.WriteLine("attributes_countを読めない");
            return false;
        }

        classFile.AttributesCount = bytes.ReadUShort(offset);
        offset += 2;

        Console.WriteLine(nameof(classFile.AttributesCount) + " : " + classFile.AttributesCount);
        return true;
    }

    private bool ReadClassAttributes()
    {
        var attributes = ReadAttributes(classFile.AttributesCount);
        if (attributes == null)
            return false;
        
        classFile.Attributes = attributes;
        return true;
    }
}