namespace MiAccessModifiers.Publico
{
    public class ExtendsMiShared : MiShared
    {

        public ExtendsMiShared()
        {
            sharedText += "-Extends";
            protectedText += "-Extends";
            internalText += "-Extends";
            Console.WriteLine($"Constructor.Public: {sharedText}");
            Console.WriteLine($"Constructor.Protected: {protectedText}");
            // error CS0122: 'MiShared.privateText' is inaccessible due to its protection level
            // Console.WriteLine($"Private: {privateText}");
            Console.WriteLine($"Constructor.Internal: {internalText}");
        }

        public void ExtendsDoSomething()
        {
            Console.WriteLine($"ExtendsPublic: {sharedText}");
            Console.WriteLine($"ExtendsProtected: {protectedText}");
            // error CS0122: 'MiShared.privateText' is inaccessible due to its protection level
            // Console.WriteLine($"Private: {privateText}");
            Console.WriteLine($"ExtendsInternal: {internalText}");
        }
    }
}

