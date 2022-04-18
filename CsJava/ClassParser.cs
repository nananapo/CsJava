namespace CsJava;

public class ClassParser
{

    private byte[] bytes;

    private int offset;

    public ClassParser(byte[] bytes)
    {
        this.bytes = bytes;
    }

    public ClassFile? Parse()
    {
        offset = 0;

        if (!IsValidMagic())
        {
            Console.WriteLine("Magicが違う");
            return null;
        }
            
        var ret = new ClassFile();
        ret.Magic = bytes.ReadUInt(offset);
        
        if (!CheckLength(4))
        {
            Console.WriteLine("minor_version, major_versionが読めない");
            return null;
        }

        var minor_version = bytes.ReadUShort(offset);
        var major_version = bytes.ReadUShort(offset + 2);
        offset += 4;

        Console.WriteLine("MinorVersion : " + minor_version);
        Console.WriteLine("MajorVersion : " + major_version);

        if (minor_version != 0 && minor_version != 65535)
        {
            Console.WriteLine("minorVersionがおかしい");
            return;
        }
    }

    private bool CheckLength(int length)
    {
        return offset + length > bytes.Length;
    }

    private bool IsValidMagic()
    {
        return bytes.ReadBase16(offset, 4) == "cafebabe";
    }
    
    private AttributeInfo ReadAttributeInfo()
    {
        
        Console.WriteLine("-----------------Attribute-----------------");
        
        if (bytes.Length < offset + 6)
        {
            Console.WriteLine("attribute_infoを読めない");
            return;
        }
        
        var attribute_name_index = bytes.ReadUShort(offset);
        var attribute_length = bytes.ReadUInt(offset + 2);
        offset += 6;
        
        Console.WriteLine(nameof(attribute_name_index) + " : " + attribute_name_index);
        Console.WriteLine(nameof(attribute_length) + " : " + attribute_length);
        
        if (attribute_name_index > constant_pool_count)
        {
            Console.WriteLine($"attribute_name_index({attribute_name_index})が" +
                              $"ConstantPoolCount({constant_pool_count})よりも大きい");
            return;
        }
        
        if (constantPoolInfos[attribute_name_index - 1].GetType() != typeof(ConstantUtf8Info))
        {
            Console.WriteLine("attribute_name_indexが指し示す型がConstantUtf8Infoではない");
            return;
        }
        
        var attribute_name = ((ConstantUtf8Info) constantPoolInfos[attribute_name_index - 1]).str;
        Console.WriteLine(nameof(attribute_name) + " : " + attribute_name);
        
        switch (attribute_name)
        {
            case "Code":
                if (bytes.Length < offset + 8)
                {
                    Console.WriteLine("Codeを読めない");
                    return;
                }
        
                ushort max_stack = bytes.ReadUShort(offset);
                ushort max_locals = bytes.ReadUShort(offset + 2);
                uint code_length = bytes.ReadUInt(offset + 4);
                offset += 8;
                
                Console.WriteLine(nameof(max_stack) + " : " + max_stack);
                Console.WriteLine(nameof(max_locals) + " : " + max_locals);
                Console.WriteLine(nameof(code_length) + " : " + code_length);
        
                if (bytes.Length < offset + code_length)
                {
                    Console.WriteLine("Codeのcodeを読めない");
                    return;
                }
                
                for (int k = 0; k < code_length; k++)
                {
                    //TODO 
                    offset++;
                }
                
                if (bytes.Length < offset + 2)
                {
                    Console.WriteLine("exception_table_lengthを読めない");
                    return;
                }
        
                var exception_table_length = bytes.ReadUShort(offset);
                offset += 2;
                
                Console.WriteLine(nameof(exception_table_length) + " : " + exception_table_length);
        
                for (int k = 0; k < exception_table_length; k++)
                {
                    //TODO
                }
                
                if (bytes.Length < offset + 2)
                {
                    Console.WriteLine("attributes_countを読めない");
                    return;
                }
                
                var attributes_count = bytes.ReadUShort(offset);
                offset += 2;
                
                Console.WriteLine(nameof(attributes_count) + " : " + attributes_count);
                
                
                break;
        }
        
        Console.WriteLine("--------------------------------------------");
    }
}