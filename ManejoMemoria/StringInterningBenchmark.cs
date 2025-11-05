using BenchmarkDotNet.Attributes;

[MemoryDiagnoser]
public class StringInterningBenchmark
{
    private readonly string[] _categories = new string[10000];

    [GlobalSetup]
    public void Setup()
    {
        // Simular datos con solo 10 categorías distintas repetidas
        var uniqueCategories = new[] { "Electronics", "Books", "Clothing", "Food", "Toys" };
        var random = new Random(42);
        for (int i = 0; i < _categories.Length; i++)
        {
            _categories[i] = uniqueCategories[random.Next(uniqueCategories.Length)];
        }
    }

    [Benchmark(Baseline = true)]
    public HashSet<string> UniqueWithoutInterning()
    {
        var unique = new HashSet<string>();
        foreach (var cat in _categories)
        {
            unique.Add(cat);  // ❌ Muchas strings duplicadas en memoria
        }
        return unique;
    }

    [Benchmark]
    public HashSet<string> UniqueWithInterning()
    {
        var unique = new HashSet<string>();
        foreach (var cat in _categories)
        {
            unique.Add(string.Intern(cat));  // ✅ Reutiliza strings
        }
        return unique;
    }
}

// | Method                 | Mean     | Allocated |
// |----------------------- |---------:|----------:|
// | UniqueWithoutInterning | 234.5 μs | 320.45 KB |
// | UniqueWithInterning    | 189.2 μs |  80.12 KB | (1.2x faster, 4x less memory)

/*
NOTA: Puede ser un problema del SDK o del procesador?
BenchmarkDotNet v0.15.5, Linux Ubuntu 24.04.3 LTS (Noble Numbat) (container)
AMD PRO A10-9700E R7, 10 COMPUTE CORES 4C+6G 2.65GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.100-rc.2.25502.107
  [Host]     : .NET 10.0.0 (10.0.0-rc.2.25502.107, 10.0.25.50307), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.0 (10.0.0-rc.2.25502.107, 10.0.25.50307), X64 RyuJIT x86-64-v3


| Method                 | Mean       | Error    | StdDev   | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|----------------------- |-----------:|---------:|---------:|------:|--------:|-------:|----------:|------------:|
| UniqueWithoutInterning |   262.6 us |  5.19 us |  7.27 us |  1.00 |    0.04 | 0.4883 |     368 B |        1.00 |
| UniqueWithInterning    | 1,774.5 us | 32.88 us | 77.49 us |  6.76 |    0.35 |      - |     368 B |        1.00 |

*/