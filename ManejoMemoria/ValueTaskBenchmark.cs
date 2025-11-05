using BenchmarkDotNet.Attributes;

[MemoryDiagnoser]
public class ValueTaskBenchmark
{
    private readonly Dictionary<int, string> _cache = new()
    {
        [1] = "cached"
    };

    [Benchmark(Baseline = true)]
    [Arguments(1)]
    public async Task<string> GetWithTask(int id)
    {
        if (_cache.TryGetValue(id, out var cached))
            return cached;  // ❌ Aún así alloca Task

        await Task.Delay(1);
        return "new";
    }

    [Benchmark]
    [Arguments(1)]
    public async ValueTask<string> GetWithValueTask(int id)
    {
        if (_cache.TryGetValue(id, out var cached))
            return cached;  // ✅ Sin allocación si está en cache

        await Task.Delay(1);
        return "new";
    }
}

// Con 99% cache hit:
// | Method            | Mean     | Allocated |
// |------------------ |---------:|----------:|
// | GetWithTask       | 45.23 ns |      48 B |
// | GetWithValueTask  |  8.12 ns |       0 B | (5.5x faster)|