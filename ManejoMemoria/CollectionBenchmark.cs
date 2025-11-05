using BenchmarkDotNet.Attributes;

[MemoryDiagnoser]  // Mide allocaciones de memoria
public class CollectionBenchmark
{
    private readonly int[] _data = Enumerable.Range(0, 10000).ToArray();

    [Benchmark(Baseline = true)]
    public int SumWithForeach()
    {
        int sum = 0;
        foreach (var item in _data)
        {
            sum += item;
        }
        return sum;
    }

    [Benchmark]
    public int SumWithFor()
    {
        int sum = 0;
        for (int i = 0; i < _data.Length; i++)
        {
            sum += _data[i];
        }
        return sum;
    }

    [Benchmark]
    public int SumWithLinq()
    {
        return _data.Sum();
    }

    [Benchmark]
    public int SumWithSpan()
    {
        var span = _data.AsSpan();
        int sum = 0;
        for (int i = 0; i < span.Length; i++)
        {
            sum += span[i];
        }
        return sum;
    }

    // [Benchmark]
    // public int SumWithSIMD()
    // {
    //     // Ver sección SIMD más adelante
    //     return VectorSum(_data);
    // }
}

// Resultados:
// | Method          | Mean      | Allocated |
// |---------------- |----------:|----------:|
// | SumWithForeach  | 5.234 μs  |       0 B | (baseline)
// | SumWithFor      | 5.201 μs  |       0 B | (1.006x faster)
// | SumWithLinq     | 12.45 μs  |      40 B | (0.42x slower)
// | SumWithSpan     | 4.987 μs  |       0 B | (1.05x faster)
// | SumWithSIMD     | 1.234 μs  |       0 B | (4.24x faster)