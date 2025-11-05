namespace ThreadingBasico.Moderno;

public class SinManejoConcurrencia
{
    private static int recursoCompartido = 10_000;

    // No thred-safe: genera inconsistencias
    public static void DisminuirRecurso(int cantidadPorDisminuir, int sender)
    {

        if (recursoCompartido >= cantidadPorDisminuir)
        {
            recursoCompartido -= cantidadPorDisminuir;
            Console.WriteLine($"Tarea: {sender} afecto: {cantidadPorDisminuir}, queda: {recursoCompartido}");
        }
        else
        {
            Console.WriteLine($"Intento con: {cantidadPorDisminuir} y fallo");
        }

    }

    // Mejora pero no es ideal dependencia ; mejor pero crea boxing
    public static void DisminuirRecursoConLock(int cantidadPorDisminuir, int sender, Object _lock)
    {

        lock (_lock)
        {
            if (recursoCompartido >= cantidadPorDisminuir)
            {
                recursoCompartido -= cantidadPorDisminuir;
                Console.WriteLine($"Tarea: {sender} afecto: {cantidadPorDisminuir}, queda: {recursoCompartido}");
            }
            else
            {
                Console.WriteLine($"Intento con: {cantidadPorDisminuir} y fallo");
            }
        }
    }
}