// using ThreadingBasico.Moderno;
// using static ThreadingBasico.Moderno.PropagarCancelacion;
using static ThreadingBasico.Moderno.SinManejoConcurrencia;
namespace ThreadingBasico
{
    public class Program
    {
        private static int accumulator = 0;

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

            // AsyncAPIConsumer.NavigateAsync().GetAwaiter().GetResult();


            // Uso de CancellationToken para detener una tarea.
            // CancellationTokenSource taskExecution = new CancellationTokenSource();
            // CancellationToken cancellationToken = taskExecution.Token;

            // Task task1 = Task.Run(() => DoMyABCAsync(0));

            // Task task2 = Task.Run(() => DoMyXYZAsync(1, cancellationToken));

            // for (int i = 0; i < 100; i++)
            // {
            //     accumulator += 12;
            //     Thread.Sleep(40);
            //     if (accumulator > 100)
            //     {
            //         Console.WriteLine($"Cancelled Task: DoMyXYZAsync at value: {accumulator}");
            //         taskExecution.Cancel();
            //         break;
            //     }
            // }
            var _lock = new object();
            for (int i = 0; i < 70; i++)
            {
                // Task.Run(() => DisminuirRecurso(new Random().Next(1, 60) * 10, i));
                Task.Run(() => DisminuirRecursoConLock(new Random().Next(1, 60) * 10, i, _lock));
            }



            Console.ReadLine();
        }


    }
}

