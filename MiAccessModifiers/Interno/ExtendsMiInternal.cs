namespace MiAccessModifiers.Interno
{
    internal class ExtendsMiInternal : MiInternal
    {

        public ExtendsMiInternal()
        {
            // ArrowFunction(Getter) error CS0200: Property or indexer 'MiInternal.RealmenteProtegido' cannot be assigned to -- it is read only
            // Con modificadores de acceso en getter/setter error CS0272: The property or indexer 'MiInternal.RealmenteProtegido' cannot be used in this context because the set accessor is inaccessible
            // RealmenteProtegido = "lo que sea";
            var valor = RealmenteProtegido;    // ✓ Solo puede leer
            Console.WriteLine($"Constructor.ProtectedReadOnly: {valor}"); // ShouldBe: private-parent (string RealmenteProtegido => privateText;)
            sharedText += "-Extends";
            protectedText += "-Extends";
            internalText += "-Extends";
            Console.WriteLine($"Constructor.Public: {sharedText}");
            Console.WriteLine($"Constructor.Protected: {protectedText}");
            // error CS0122: 'MiInternal.privateText' is inaccessible due to its protection level
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

