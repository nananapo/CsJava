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

        var istore = new int[4];

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
            
            var opcode = codeAttr.code[offset++];
            switch (opcode)
            {
                case 0x02:
                case 0x03:
                case 0x04:
                case 0x05:
                case 0x06:
                case 0x07:
                case 0x08:
                {
                    Console.WriteLine("iconst_" + (opcode - 3));

                    var data = new ConstantIntegerInfo(opcode - 3);
                    stack.Add(data);
                    Console.WriteLine("push : " + data);
                    break;
                }
                case 0x10:
                {
                    if (codeAttr.code.Length < offset + 1)
                    {
                        Console.WriteLine("intを読めない");
                        return;
                    }

                    var i = codeAttr.code[offset++].ToSignedByte();
                    Console.WriteLine("bipush " + i);

                    var data = new ConstantIntegerInfo(i);
                    stack.Add(data);
                    
                    Console.WriteLine("push : " + data);
                    break;
                }
                case 0x11:
                {
                    if (codeAttr.code.Length < offset + 2)
                    {
                        Console.WriteLine("byte1, byte2を読めない");
                        return;
                    }

                    var sho = codeAttr.code.ReadShort(offset);
                    offset += 2;
                    
                    Console.WriteLine("sipush  " + sho);

                    var data = new ConstantIntegerInfo(sho);
                    stack.Add(data);
                    Console.WriteLine("push : " + data);
                    break;
                }
                case 0x12:
                {
                    if (codeAttr.code.Length < offset + 1)
                    {
                        Console.WriteLine("indexを読めない");
                        return;
                    }

                    var index = codeAttr.code[offset++];

                    Console.WriteLine("ldc #" + index);

                    stack.Add(classFile.ConstantPoolInfos[index - 1]);
                    Console.WriteLine("push : " + classFile.ConstantPoolInfos[index - 1]);
                    break;
                }
                case 0x1a:
                case 0x1b:
                case 0x1c:
                case 0x1d:
                {
                    int n = opcode - 0x1a;

                    int i = istore[n];
                    Console.WriteLine($"iload_{n} " + i);
                    
                    var data = new ConstantIntegerInfo(i);
                    stack.Add(data);
                    Console.WriteLine("push : " + data);
                    break;
                }
                case 0x3b:
                case 0x3c:
                case 0x3d:
                case 0x3e:
                {
                    int n = opcode - 0x3b;

                    int i = (stack[stack.Count - 1] as ConstantIntegerInfo).bytes;
                    stack.RemoveAt(stack.Count - 1);

                    Console.WriteLine($"istore_{n} " + i);
                    istore[n] = i;
                    break;
                }
                case 0x60:
                {
                    int value1 = (stack[stack.Count - 1] as ConstantIntegerInfo).bytes;
                    stack.RemoveAt(stack.Count - 1);
                    
                    int value2 = (stack[stack.Count - 1] as ConstantIntegerInfo).bytes;
                    stack.RemoveAt(stack.Count - 1);

                    Console.WriteLine("iadd");
                    stack.Add(new ConstantIntegerInfo(value1 + value2));
                    break;
                }
                case 0x64:
                {
                    int value1 = (stack[stack.Count - 1] as ConstantIntegerInfo).bytes;
                    stack.RemoveAt(stack.Count - 1);
                    
                    int value2 = (stack[stack.Count - 1] as ConstantIntegerInfo).bytes;
                    stack.RemoveAt(stack.Count - 1);

                    Console.WriteLine("isub");
                    stack.Add(new ConstantIntegerInfo(value2 - value1));
                    break;
                }
                case 0x68:
                {
                    int value1 = (stack[stack.Count - 1] as ConstantIntegerInfo).bytes;
                    stack.RemoveAt(stack.Count - 1);
                    
                    int value2 = (stack[stack.Count - 1] as ConstantIntegerInfo).bytes;
                    stack.RemoveAt(stack.Count - 1);

                    Console.WriteLine("imul");
                    stack.Add(new ConstantIntegerInfo(value2 * value1));
                    break;
                }
                case 0x6c:
                {
                    int value1 = (stack[stack.Count - 1] as ConstantIntegerInfo).bytes;
                    stack.RemoveAt(stack.Count - 1);
                    
                    int value2 = (stack[stack.Count - 1] as ConstantIntegerInfo).bytes;
                    stack.RemoveAt(stack.Count - 1);

                    Console.WriteLine("imul");
                    stack.Add(new ConstantIntegerInfo(value2 / value1));
                    break;
                }
                case 0xb1:
                {
                    Console.WriteLine("return");
                    return;
                }
                case 0xb2:
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
                case 0xb6:
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

                    if (argument_count == 0)
                        argument_count++;
                    Console.WriteLine("ArgumentCount: " + argument_count + " : " + descriptor.str);

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
                        if (arguments[0] is ConstantStringInfo strinfo)
                        {
                            Console.WriteLine((classFile.ConstantPoolInfos[strinfo.string_index - 1] as ConstantUtf8Info).str);
                        }
                        else if (arguments[0] is ConstantIntegerInfo intInfo)
                        {                           
                            Console.WriteLine(intInfo.bytes);
                        }
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