using CsJava;

public class Program
{
    static void Main(string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine("Usage: csjava fileName");
            return;
        }

        var fileName = args[0];
        byte[] bytes;

        try
        {
            bytes = File.ReadAllBytes(fileName);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        int offset = 0;

        /*-------MinorVersion , MajorVersion-------*/

        /*-------ConstantPoolCount-------*/
        if (bytes.Length < offset + 2)
        {
            Console.WriteLine("constant_pool_countを読めない");
            return;
        }

        int constant_pool_count = bytes.ReadUShort(offset);
        offset += 2;
        Console.WriteLine("constant_pool_count : " + constant_pool_count);

        /*-------ConstantPool-------*/
        Console.WriteLine("-----------------ConstantPool-----------------");
        List<ConstantPoolInfo> constantPoolInfos = new();

        for (int i = 0; i < constant_pool_count - 1; i++)
        {
            if (bytes.Length < offset + 1)
            {
                Console.WriteLine("ConstantPoolのtagを読めない");
                return; 
            }
            
            var tag = bytes[offset++];
            switch (tag)
            {
                case 1:
                {
                    if (bytes.Length < offset + 2)
                    {
                        Console.WriteLine($"ConstantPoolの{tag}を読めない");
                        return; 
                    }
                    
                    var length = bytes.ReadUShort(offset);
                    offset += 2;
                    
                    if (bytes.Length < offset + length)
                    {
                        Console.WriteLine($"ConstantStringInfoのbytesを読めない");
                        return; 
                    }
                    
                    byte[] b = new byte[length];
                    for (int j = 0; j < length; j++)
                        b[j] = bytes[offset++];

                    ConstantPoolInfo info = new ConstantUtf8Info(length, b);
                    constantPoolInfos.Add(info);
                    Console.WriteLine(info);
                    break;
                }
                case 7:
                {
                    if (bytes.Length < offset + 2)
                    {
                        Console.WriteLine($"ConstantPoolの{tag}を読めない");
                        return; 
                    }
                    
                    var name_index = bytes.ReadUShort(offset);
                    offset += 2;

                    ConstantPoolInfo info = new ConstantClassInfo(name_index);
                    constantPoolInfos.Add(info);
                    Console.WriteLine(info);
                    break;
                }
                case 8:
                {
                    if (bytes.Length < offset + 2)
                    {
                        Console.WriteLine($"ConstantPoolの{tag}を読めない");
                        return; 
                    }
                    
                    var string_index = bytes.ReadUShort(offset);
                    offset += 2;

                    ConstantPoolInfo info = new ConstantStringInfo(string_index);
                    constantPoolInfos.Add(info);
                    Console.WriteLine(info);
                    break;
                }
                case 9:
                case 10:
                {
                    if (bytes.Length < offset + 4)
                    {
                        Console.WriteLine($"ConstantPoolの{tag}を読めない");
                        return; 
                    }

                    var class_index = bytes.ReadUShort(offset);
                    var name_and_type_index = bytes.ReadUShort(offset + 2);
                    offset += 4;

                    ConstantPoolInfo info;
                    if (tag == 9)
                        info = new ConstantFieldRefInfo(class_index, name_and_type_index);
                    else
                        info = new ConstantMethodRefInfo(class_index, name_and_type_index);
                    constantPoolInfos.Add(info);
                    Console.WriteLine(info);
                    break;
                }
                case 12:
                {
                    if (bytes.Length < offset + 4)
                    {
                        Console.WriteLine($"ConstantPoolの{tag}を読めない");
                        return; 
                    }

                    var name_index = bytes.ReadUShort(offset);
                    var descriptor_index = bytes.ReadUShort(offset + 2);
                    offset += 4;

                    ConstantPoolInfo info = new ConstantNameAndTypeInfo(name_index, descriptor_index);
                    constantPoolInfos.Add(info);
                    Console.WriteLine(info);
                    break;
                }
                default:
                    Console.WriteLine($"知らないtag : {tag}");
                    return; 
            }
        }
        Console.WriteLine("--------------------------------------------");

        /*-------access_flags, this_class, super_class-------*/
        if (bytes.Length < offset + 6)
        {
            Console.WriteLine("access_flags, this_class, super_classを読めない");
            return;
        }

        int access_flags = bytes.ReadUShort(offset);
        int this_class = bytes.ReadUShort(offset + 2);
        int super_class = bytes.ReadUShort(offset + 4);
        offset += 6;

        Console.WriteLine(nameof(access_flags) + " : " + access_flags);
        Console.WriteLine(nameof(this_class) + " : " + this_class);
        Console.WriteLine(nameof(super_class) + " : " + super_class);

        /*-------interfaces_count-------*/
        if (bytes.Length < offset + 2)
        {
            Console.WriteLine("interfaces_countを読めない");
            return;
        }

        int interfaces_count = bytes.ReadUShort(offset);
        offset += 2;

        Console.WriteLine(nameof(interfaces_count) + " : " + interfaces_count);


        /*-------interfaces-------*/
        List<ushort> interfaces = new ();

        for (int i = 0; i < interfaces_count; i++)
        {
            if (bytes.Length < offset + 2)
            {
                Console.WriteLine("interfacesを読めない");
                return;
            }
            ushort interfaces_index = bytes.ReadUShort(offset);
            offset += 2;
            interfaces.Add(interfaces_index);
            
            Console.WriteLine(nameof(interfaces_index) + " : " + interfaces_index);
        }

        /*-------fields_count-------*/
        if (bytes.Length < offset + 2)
        {
            Console.WriteLine("fields_countを読めない");
            return;
        }

        int fields_count = bytes.ReadUShort(offset);
        offset += 2;

        Console.WriteLine(nameof(fields_count) + " : " + fields_count);

        //TODO fields


        /*-------methods_count-------*/
        if (bytes.Length < offset + 2)
        {
            Console.WriteLine("methods_countを読めない");
            return;
        }

        int methods_count = bytes.ReadUShort(offset);
        offset += 2;

        Console.WriteLine(nameof(methods_count) + " : " + methods_count);



        Console.WriteLine("-----------------Methods-----------------");
        /*-------methods-------*/
        for (int i = 0; i < methods_count; i++)
        {
            if (bytes.Length < offset + 8)
            {
                Console.WriteLine("methodsを読めない");
                return;
            }

            var method_access_flags = bytes.ReadUShort(offset);
            var method_name_index = bytes.ReadUShort(offset + 2);
            var method_descriptor_index = bytes.ReadUShort(offset + 4);
            var method_attributes_count = bytes.ReadUShort(offset + 6);
            offset += 8;
            
            Console.WriteLine($"Methods[{i}]");
            Console.WriteLine(nameof(method_access_flags) + " : " + method_access_flags);
            Console.WriteLine(nameof(method_name_index) + " : " + method_name_index + " : " + ((ConstantUtf8Info)constantPoolInfos[method_name_index - 1]).str);
            Console.WriteLine(nameof(method_descriptor_index) + " : " + method_descriptor_index);
            Console.WriteLine(nameof(method_attributes_count) + " : " + method_attributes_count);

            for (int j = 0; j < method_attributes_count; j++)
            {
                //return;
            }
        }
        Console.WriteLine("--------------------------------------------");
    }

}