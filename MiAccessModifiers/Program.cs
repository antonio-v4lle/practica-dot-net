using MiAccessModifiers.Publico;
using MiAccessModifiers.Interno;
namespace MiAccessModifiers
{
    class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Inicio App");

            // Clases Publicas
            // Console.WriteLine("MiSharedBase".PadLeft(20, '='));
            // MiShared shared = new MiShared();
            // shared.DoSomethingInternal();
            // Console.WriteLine("MiSharedExtends".PadLeft(20, '='));
            // ExtendsMiShared extendsShared = new ExtendsMiShared();
            // extendsShared.DoSomethingInternal();
            // extendsShared.ExtendsDoSomething();

            // Clases Internas

            Console.WriteLine("MiInternalBase".PadLeft(20, '='));
            MiInternal internalInstance = new MiInternal();
            internalInstance.DoSomethingInternal();
            Console.WriteLine("MiInternalExtends".PadLeft(20, '='));
            ExtendsMiInternal extendsInternalInstance = new ExtendsMiInternal();
            extendsInternalInstance.DoSomethingInternal();
            extendsInternalInstance.ExtendsDoSomething();
        }
    }
}


