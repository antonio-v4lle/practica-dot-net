using MiAccessModifiers.Publico;

namespace ConsumerMiAccessModifiers
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Inicio ConsumerApp");

            // Clases Publicas
            Console.WriteLine("MiSharedBase".PadLeft(20, '='));
            MiShared shared = new MiShared();
            // error CS1061: 'MiShared' does not contain a definition for 'DoSomethingInternal' and no accessible extension method 'DoSomethingInternal' accepting a first argument of type 'MiShared' could be found (are you missing a using directive or an assembly reference?)
            // shared.DoSomethingInternal();
            shared.DoSomethingPublic();
            Console.WriteLine("MiSharedExtends".PadLeft(20, '='));
            ExtendsMiShared extendsShared = new ExtendsMiShared();
            // error CS1061: 'ExtendsMiShared' does not contain a definition for 'DoSomethingInternal' and no accessible extension method 'DoSomethingInternal' accepting a first argument of type 'ExtendsMiShared' could be found (are you missing a using directive or an assembly reference?)
            // extendsShared.DoSomethingInternal();
            extendsShared.ExtendsDoSomething();

            // Clases Internas
            Console.WriteLine("MiInternalBase".PadLeft(20, '='));
            // error CS0246: The type or namespace name 'MiInternal' could not be found(are you missing a using directive or an assembly reference?)
            // MiInternal internalInstance = new MiInternal();
            // internalInstance.DoSomethingInternal();
            Console.WriteLine("MiInternalExtends".PadLeft(20, '='));
            // error CS0246: The type or namespace name 'ExtendsMiInternal' could not be found (are you missing a using directive or an assembly reference?)
            // ExtendsMiInternal extendsInternalInstance = new ExtendsMiInternal();
            // extendsInternalInstance.DoSomethingInternal();
            // extendsInternalInstance.ExtendsDoSomething();
        }
    }
}