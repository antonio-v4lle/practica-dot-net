namespace MiAccessModifiers.Interno
{
    internal class MiInternal
    {
        // Nivel de restricción mayor a menor
        // private > protected > public
        public string sharedText { get; set; } = "parent";
        protected string protectedText { get; set; } = "parent"; // Protegido solo hijo: pero con acceso a modificarlo
        private string privateText { get; set; } = "private-parent"; // Solo padre
        // ✓ Propiedad protected con arrow function - SOLO LECTURA
        // protected string RealmenteProtegido => privateText; // Getter readonly
        // Esto es lo mismo que:
        protected string RealmenteProtegido
        {
            get => privateText;
            // Setter tambien puede ser protected => Inmutabilidad: Evita modificaciones accidentales desde derivadas
            // error CS0273: The accessibility modifier of the 'MiInternal.RealmenteProtegido.set' accessor must be more restrictive than the property or indexer 'MiInternal.RealmenteProtegido'
            private set => privateText = value;
        }

        internal string internalText { get; set; } = "parent";
        internal void DoSomethingInternal()
        {
            Console.WriteLine("DoSomethingInternal");
            Console.WriteLine($"Public: {sharedText}");
            Console.WriteLine($"Protected: {protectedText}");
            Console.WriteLine($"Private: {privateText}");
            Console.WriteLine($"Internal: {internalText}");
        }

        public void DoSomethingPublic()
        {
            Console.WriteLine("DoSomethingPublic");
            Console.WriteLine($"Public: {sharedText}");
            Console.WriteLine($"Protected: {protectedText}");
            Console.WriteLine($"Private: {privateText}");
            Console.WriteLine($"Internal: {internalText}");
        }
    }
}

