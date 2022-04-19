namespace CsJava;

public class VirtualMachine
{
    public ClassFile classFile;

    public List<ConstantPoolInfo> stack;

    public void Run(ClassFile classFile)
    {
        Console.WriteLine("----Run-------------------------------------");

        this.classFile = classFile;
        this.stack = new List<ConstantPoolInfo>();

        var mainMethod = GetMethodByName("main");
        if (mainMethod == null)
        {
            Console.WriteLine("mainが見つかりません。");
            return;
        }

        RunMethod(mainMethod);
    }

    private void RunMethod(MethodInfo methodInfo)
    {
        var codeAttr = methodInfo.GetAttribute<CodeAttribute>();

        if (codeAttr == null)
        {
            Console.WriteLine("CodeAttributeが見つかりません。");
            return;
        }

        int offset = 0;
        while (offset < codeAttr.code_length)
        {
            if (codeAttr.code.Length < offset + 1)
            {
                Console.WriteLine("OpCodeを読めない : " + offset);
                return;
            }
            
            var opcode = codeAttr.code[offset++].ToBase16();
            switch (opcode)
            {
                case "12":
                {
                    if (codeAttr.code.Length < offset + 1)
                    {
                        Console.WriteLine("indexを読めない");
                        return;
                    }

                    var index = codeAttr.code[offset++];

                    Console.WriteLine("ldc #" + index);

                    stack.Add(classFile.ConstantPoolInfos[index - 1]);
                    break;
                }
                case "b1":
                {
                    Console.WriteLine("return");
                    return;
                }
                case "b2":
                {
                    if (codeAttr.code.Length < offset + 2)
                    {
                        Console.WriteLine("indexbyte1, indexbyte2を読めない");
                        return;
                    }

                    var index = codeAttr.code.ReadUShort(offset);
                    offset += 2;

                    Console.WriteLine("getstatic #" + index);

                    stack.Add(classFile.ConstantPoolInfos[index - 1]);
                    Console.WriteLine("push : " + classFile.ConstantPoolInfos[index - 1]);
                    break;
                }
                case "b6":
                {
                    if (codeAttr.code.Length < offset + 2)
                    {
                        Console.WriteLine("indexbyte1, indexbyte2を読めない");
                        return;
                    }

                    var index = codeAttr.code.ReadUShort(offset);
                    offset += 2;

                    Console.WriteLine("invokevirtual #" + index);


                    var methodRef = ((ConstantMethodRefInfo)classFile.ConstantPoolInfos[index - 1]);
                    var classRef = ((ConstantClassInfo)classFile.ConstantPoolInfos[methodRef.class_index - 1]);
                    var nameAndType = ((ConstantNameAndTypeInfo)classFile.ConstantPoolInfos[methodRef.name_and_type_index - 1]);

                    var className = ((ConstantUtf8Info)classFile.ConstantPoolInfos[classRef.name_index - 1]);
                    var methodName = ((ConstantUtf8Info)classFile.ConstantPoolInfos[nameAndType.name_index - 1]);
                    var descriptor = ((ConstantUtf8Info)classFile.ConstantPoolInfos[nameAndType.descriptor_index - 1]);

/*
                    Console.WriteLine("methodRef " + methodRef);
                    Console.WriteLine("classRef     ->" + classRef);
                    Console.WriteLine("             ->" + className.str);
                    Console.WriteLine("nameAndType  ->" + nameAndType);
                    Console.WriteLine("             ->" + methodName.str);
                    Console.WriteLine("             ->" + descriptor.str);
*/

                    var argument_count = descriptor.str.Split(";").Count() - 1;
                    Console.WriteLine("ArgumentCount: " + argument_count);

                    var arguments = new List<ConstantPoolInfo>();

                    for (int i=0;i<argument_count;i++)
                    {
                        var data = stack[stack.Count - 1];
                        stack.RemoveAt(stack.Count - 1);
                        arguments.Add(data);
                        Console.WriteLine("pop : " + data);
                    }

                    
                    var field = stack[stack.Count - 1] as ConstantFieldRefInfo;
                    Console.WriteLine("pop : " + field);
                    stack.RemoveAt(stack.Count - 1);

                    var fieldClass = classFile.ConstantPoolInfos[field.class_index - 1] as ConstantClassInfo;
                    var fieldClassName = (classFile.ConstantPoolInfos[fieldClass.name_index - 1] as ConstantUtf8Info).str;
                    var fieldNameAndType = (classFile.ConstantPoolInfos[field.name_and_type_index - 1] as ConstantNameAndTypeInfo);
                    var fieldName = (classFile.ConstantPoolInfos[fieldNameAndType.name_index - 1] as ConstantUtf8Info).str;
                    var fieldDescriptor = (classFile.ConstantPoolInfos[fieldNameAndType.descriptor_index - 1] as ConstantUtf8Info).str;

                    Console.WriteLine("Class : " + fieldClassName);
                    Console.WriteLine("field : " + fieldName + " (" + fieldDescriptor + ")");
                    Console.WriteLine("method: " + methodName.str + " (" + descriptor.str + ")");

                    if (fieldClassName + "." + fieldName + "." +methodName.str == "java/lang/System.out.println")
                    {
                        Console.WriteLine((classFile.ConstantPoolInfos[(arguments[0] as ConstantStringInfo).string_index - 1] as ConstantUtf8Info).str);
                    }

                    break;
                }
                default:
                    Console.WriteLine($"知らないopcode {opcode}");
                    return;//exit
            }
        }
    }

    private MethodInfo? GetMethodByName(string name)
    {
        foreach (var method in classFile.Methods)
        {
            var info = classFile.ConstantPoolInfos[method.name_index - 1] as ConstantUtf8Info;
            if (info.str == name)
                return method;
        }
        return null;
    }
}