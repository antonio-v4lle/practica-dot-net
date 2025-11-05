using BenchmarkDotNet.Attributes;

[MemoryDiagnoser]
public class SpanBenchmark
{
    private readonly string _text = "Hello, World! This is a test.";

    [Benchmark(Baseline = true)]
    public string SubstringWithString()
    {
        return _text.Substring(0, 5);  // ❌ Crea nueva string
    }

    [Benchmark]
    public ReadOnlySpan<char> SubstringWithSpan()
    {
        return _text.AsSpan(0, 5);  // ✅ Sin allocación
    }

    [Benchmark]
    public int ParseIntFromString()
    {
        var numberStr = _text.Substring(15, 2);  // ❌ Allocación
        return int.Parse(numberStr);
    }

    [Benchmark]
    public int ParseIntFromSpan()
    {
        var span = _text.AsSpan(15, 2);  // ✅ Sin allocación
        return int.Parse(span);
    }
}

// Resultados:
// | Method                | Mean     | Allocated |
// |---------------------- |---------:|----------:|
// | SubstringWithString   | 12.34 ns |      32 B |
// | SubstringWithSpan     |  1.23 ns |       0 B | (10x faster)
// | ParseIntFromString    | 45.67 ns |      56 B |
// | ParseIntFromSpan      | 23.45 ns |      24 B | (1.9x faster)

// BenchmarkDotNet v0.15.5, Linux Ubuntu 24.04.3 LTS (Noble Numbat) (container)
// AMD PRO A10-9700E R7, 10 COMPUTE CORES 4C+6G 2.99GHz, 1 CPU, 4 logical and 2 physical cores
// .NET SDK 10.0.100-rc.2.25502.107
//   [Host]     : .NET 10.0.0 (10.0.0-rc.2.25502.107, 10.0.25.50307), X64 RyuJIT x86-64-v3
//   DefaultJob : .NET 10.0.0 (10.0.0-rc.2.25502.107, 10.0.25.50307), X64 RyuJIT x86-64-v3

/*
| Method              | Mean       | Error     | StdDev    | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|-------------------- |-----------:|----------:|----------:|------:|--------:|-------:|----------:|------------:|
| SubstringWithString | 26.6674 ns | 0.7191 ns | 1.9565 ns |  1.01 |    0.10 | 0.0612 |      32 B |        1.00 |
| SubstringWithSpan   |  0.5605 ns | 0.0789 ns | 0.1383 ns |  0.02 |    0.01 |      - |         - |        0.00 |
| ParseIntFromString  |         NA |        NA |        NA |     ? |       ? |     NA |        NA |           ? |
| ParseIntFromSpan    |         NA |        NA |        NA |     ? |       ? |     NA |        NA |           ? |
*/


// Benchmarks with issues :
//   SpanBenchmark.ParseIntFromString : DefaultJob
//   SpanBenchmark.ParseIntFromSpan : DefaultJob