namespace ThreadingBasico.OldWay
{
    public class OldWayThreading
    {
        public static void InitThreads()
        {
            Thread thread = new Thread(() =>
            {
                Console.WriteLine($"Thread ID: {Thread.CurrentThread.ManagedThreadId}");
                Thread.Sleep(5000);
                Console.WriteLine("OldWay Work done!");
            });
            // Desde aquí interrumpe genera un bloqueo a la siguente declaración del hilo
            // thread.IsBackground = true; // No mantiene viva la app
            // thread.Start();
            // thread.Join(); // Esperar a que termine

            // Si no se lanza el thread esto no parece ejecutarse.
            // Si termina antes otro hilo este no termina de ejecutarse.
            ThreadPool.QueueUserWorkItem(_ =>
            {
                Console.WriteLine($"ThreadPool Thread: {Thread.CurrentThread.ManagedThreadId}");
                Thread.Sleep(3000);
                Console.WriteLine("PoolWay Work done!");
            });

            thread.IsBackground = true; // No mantiene viva la app
            thread.Start();
            thread.Join(); // Esperar a que termine
        }
    }
}

