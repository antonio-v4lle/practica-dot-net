namespace MiAccessModifiers.Publico
{
    public class MiShared
    {
        public string sharedText { get; set; } = "parent";
        protected string protectedText { get; set; } = "parent";
        private string privateText { get; set; } = "parent";
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

