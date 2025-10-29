namespace ThreadingBasico.Moderno
{
    public class UsoValueTasks
    {

        private Dictionary<int, int> cache = new Dictionary<int, int>();


        public static void InitThreads()
        {
            UsoValueTasks usoValueTasks = new UsoValueTasks();
            var value1 = usoValueTasks.GetValueAsync().GetAwaiter().GetResult(); // Retorna el valir con el tipo de dato
            Console.WriteLine($"value1: {value1}");
            var value2 = usoValueTasks.GetValueStructAsync(value1, $"value1: {value1}"); // retorna un promesa
            Console.WriteLine($"value2: {value2}");
            var value3 = usoValueTasks.GetValueStructAsync(value1, $"value1: {value1}"); // retorna un promesa y con el valor cacheado
            Console.WriteLine($"value3: {value3}");
            var value4 = usoValueTasks.GetValueStructAsync(value3.Result, $"value3: {value3.Result}"); // retorna un promesa
            Console.WriteLine($"value4: {value4}");


            // ❌ NO hacer esto con ValueTask:
            // var task = GetValueAsync();
            // await task; // Primera await
            // await task; // Segunda await - PROHIBIDO!

            // ❌ NO almacenar como campo
            // private ValueTask<int> _cachedTask; // MAL

            // // ✅ Usar directamente
            // var result = await GetValueAsync(); // BIEN ó .GetAwaiter().GetResult()
        }

        // Task - Referencia en el Heap (montón)
        // Siempre aloca un objeto Task en el heap
        public async Task<int> GetValueAsync()
        {
            await Task.Delay(100);
            return 29;
        }

        // ValueTask - Struct optimizado

        // Cuándo usar ValueTask:
        // ✅ Cuando el resultado puede estar disponible síncronamente frecuentemente
        // ✅ En hot paths(código crítico de performance)
        // ✅ En bibliotecas de alto rendimiento
        public async ValueTask<int> GetValueStructAsync(int key, string origin)
        {
            if (cache.TryGetValue(key, out var value))
            {
                Console.WriteLine($"Origen: {origin}: {value}");
                return value; // No aloca, retorna directamente
            }


            var result = await Task.Run(() =>
            {
                Task.Delay(200);
                return new Random().Next(0, 100);
            }); // Solo aloca si hay await real
            cache[key] = result;
            return result;
        }
    }
}