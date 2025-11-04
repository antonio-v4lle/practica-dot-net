namespace ThreadingBasico.Moderno;

public class PropagarCancelacion
{
    private static int accumulator = 0;
    public static void DoMyXYZAsync(int Id, CancellationToken cancellationToken)
    {

        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            for (int i = 0; i < 100; i++)
            {
                // Antes de cualquier otra operación para controlar el último estado completado conocido.
                cancellationToken.ThrowIfCancellationRequested();

                accumulator++;
                Console.WriteLine("Hola Mundo desde DoMyXYZAsync en Thread {0} Iteración: {1}", Thread.CurrentThread.ManagedThreadId, i);
                Thread.Sleep(25);

                // if (cancellationToken.IsCancellationRequested)
                // {
                //     return;
                // }
            }
        }
        catch (OperationCanceledException opEx) when (cancellationToken.IsCancellationRequested)
        {
            // Esta es una cancelación esperada, no un error
            Console.WriteLine(opEx.Message);
            throw; // Re-lanzar para propagar la cancelación

        }
        catch (Exception ex)
        {
            // Otros errores
            Console.WriteLine(ex.Message);
            throw;
        }



    }

    public static void DoMyABCAsync(int Id)
    {
        for (int i = 0; i < 100; i++)
        {
            Console.WriteLine("Hola Mundo desde DoMyABCAsync en Thread {0} Iteración: {1}", Thread.CurrentThread.ManagedThreadId, i);
            Thread.Sleep(75);
        }
    }
}