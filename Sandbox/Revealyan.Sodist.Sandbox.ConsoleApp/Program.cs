using Grpc.Net.Client;
using Revealyan.Sodist.Sandbox.GRPCService;
using System;
using System.Reflection;

namespace Revealyan.Sodist.Sandbox.ConsoleApp
{
    class Program
    {
        static string _libName = $@"Revealyan.Sodist.Sandbox.LibraryCore";
        static string _libPath = $@"D:\Development\src\Sandbox\Revealyan.Sodist.Sandbox.LibraryCore\bin\Debug\netcoreapp3.1\{_libName}.dll";
        static void Main(string[] args)
        {
            TestGRPC();
            //TestLibLoad();
            Console.ReadKey();
        }
        static void TestLibLoad()
        {
            var asm = Assembly.LoadFrom(_libPath);
            var asm3 = Assembly.LoadFrom(_libPath);
            var asm2 = Assembly.Load(_libName);
        }
        static void TestGRPC()
        {
            Greeter.GreeterClient g1 = new Greeter.GreeterClient(GrpcChannel.ForAddress("https://localhost:5001/"));
            Greeter2.Greeter2Client g2 = new Greeter2.Greeter2Client(GrpcChannel.ForAddress("https://localhost:5001/"));
            var b = true;
            while (b)
            {
                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.F1:
                        Console.WriteLine(g1.SayHello(new HelloRequest() { Name = "G1" }).Message);
                        break;
                    case ConsoleKey.F2:
                        Console.WriteLine(g2.SayHello(new HelloRequest2() { Name = "G2" }).Message);
                        break;
                    case ConsoleKey.Escape:
                        b = false;
                        break;
                    default:
                        Console.WriteLine("ты долбаёб");
                        break;
                }
            }
        }
    }
}
