using ThreadingBasico.Moderno;
namespace ThreadingBasico
{
    public class Program
    {
        // error CS4009: A void or int returning entry point cannot be async
        // error CS5001: Program does not contain a static 'Main' method suitable for an entry point
        public static void Main(string[] args)
        {
            // OldWayThreading.InitThreads();
            // ModernWayThreading.InitThreads();


            // +--------------------+---------------+---------------------+
            // |   Característica   |     Task      |      ValueTask      |
            // +--------------------+---------------+---------------------+
            // | Tipo               | Class (heap)  | Struct (stack/heap) |
            // | Await múltiple     | ✅ Sí         | ❌ No               |
            // | Almacenar en campo | ✅ Sí         | ❌ No               |
            // | Conversión         | -             | ValueTask → Task    |
            // | Performance sync   | Aloca siempre | Sin alocación       |
            // +--------------------+---------------+---------------------+
            // UsoValueTasks.InitThreads();

            // AsyncIOFiles.WorkAsync().GetAwaiter();

            AsyncAPIConsumer.NavigateAsync().GetAwaiter().GetResult();

        }
    }
}

