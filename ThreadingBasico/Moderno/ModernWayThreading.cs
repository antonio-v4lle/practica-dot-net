namespace ThreadingBasico.Moderno
{
    public class ModernWayThreading
    {

        public static void InitThreads()
        {
            ModernWayThreading program = new ModernWayThreading();
            Console.WriteLine("Llamar Async Task");
            // error CS4033: The 'await' operator can only be used within an async method. Consider marking this method with the 'async' modifier and changing its return type to 'Task'.
            // await program.ModernWay();

            // warning CS4014: Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.
            // Programa termina antes que el hilo y se cierra.
            // program.ModernWay();

            // No bloquea pero no da tiempo a ejecutarse
            // program.ModernWay().ConfigureAwait(false);

            // Ejecuta pero no gestiona el bloqueo
            // An unhandled exception of type 'System.InvalidOperationException'
            // 'Start may not be called on a promise-style task.'
            // try
            // {
            //     program.ModernWay().Start();
            //     // Debug Output: 
            //     // Llamar Async Task
            //     // Task Thread: 4
            //     // Exception thrown: 'System.InvalidOperationException' in System.Private.CoreLib.dll
            //     // Start may not be called on a promise - style task.
            //     // Despúes de llamar ModernWay()
            // }
            // catch (InvalidOperationException ex)
            // {
            //     Console.WriteLine(ex.Message);
            // }
            // catch (Exception ex)
            // {
            //     Console.Write(ex.ToString());
            // }

            // Usar .GetAwaiter().GetResult()
            // Nota: Esto puede causar deadlocks en aplicaciones con interfaces gráficas (como WPF o WinForms) o ASP.NET debido al bloqueo del contexto de sincronización.
            // program.ModernWay().GetAwaiter().GetResult();

            // Usar .Result
            // Precaución: Al igual que con .GetAwaiter().GetResult(), esto puede bloquear el hilo y causar problemas en ciertos contextos.
            // Errores por ser una función void.
            // error CS1061: 'Task' does not contain a definition for 'Result' and no accessible extension method 'Result' accepting a first argument of type 'Task' could be found (are you missing a using directive or an assembly reference?)
            // error CS0201: Only assignment, call, increment, decrement, await, and new object expressions can be used as a statement
            // program.ModernWay().Result;

            // Ejecutar el método en un hilo separado con Task.Run
            // Si necesitas evitar bloqueos en el hilo principal, puedes ejecutar el método asíncrono en un hilo separado usando Task.Run.
            // Esto es más seguro en aplicaciones con interfaces gráficas o servidores web, ya que evita el bloqueo del SynchronizationContext principal.
            // var resultado = Task.Run(() => MetodoAsincrono()).GetAwaiter().GetResult();
            // Que en este contexto es lo mismo que:
            program.ModernWay().GetAwaiter().GetResult();
            Console.WriteLine("Despúes de llamar ModernWay()");
        }

        // ✅✅ Forma moderna - Task
        public async Task ModernWay()
        {
            Console.WriteLine("InsideModernWay");
            await Task.Run(() =>
            {
                Console.WriteLine($"Task 1 Thread: {Thread.CurrentThread.ManagedThreadId}");
                Thread.Sleep(2000); // Captura del contexto y no sigue la ejecución del principal.
                Console.WriteLine("Work 1 done!");
            }).ConfigureAwait(false); // con false no captura contexto: 
            await Task.Run(() =>
            {
                Console.WriteLine($"Task 2 Thread: {Thread.CurrentThread.ManagedThreadId}");
                Thread.Sleep(1000); // Captura del contexto y no sigue la ejecución del principal.
                Console.WriteLine("Work 2 done!");
            }).ConfigureAwait(false); // con false no captura contexto: 
            Console.WriteLine("InsideModernWayEnd");
        }
    }
}