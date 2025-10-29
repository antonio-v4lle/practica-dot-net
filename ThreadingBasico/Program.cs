using ThreadingBasico.Moderno;
namespace ThreadingBasico
{
    class Program
    {
        // error CS4009: A void or int returning entry point cannot be async
        // error CS5001: Program does not contain a static 'Main' method suitable for an entry point
        static void Main(string[] args)
        {
            //OldWayThreading.InitThreads();
            //ModernWayThreading.InitThreads();

            UsoValueTasks.InitThreads();
        }
    }
}

