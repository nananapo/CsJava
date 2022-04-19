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

        var parser = new ClassParser(bytes);
        var result = parser.Parse();

        if (result == null)
        {
            Console.WriteLine("パース失敗");
            return;
        }

        var vm = new VirtualMachine();
        vm.Run(result);
    }

}