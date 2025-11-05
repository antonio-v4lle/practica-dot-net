using System.Text;
using BenchmarkDotNet.Attributes;

[MemoryDiagnoser]  // Mide allocaciones de memoria
[SimpleJob(warmupCount: 3, iterationCount: 5)]

public class StringConcatBenchmark
{
    private const int N = 1000;

    [Benchmark(Baseline = true)]
    public string ConcatWithPlus()
    {
        string result = "";
        for (int i = 0; i < N; i++)
        {
            result += "a";  // ❌ Crea nueva string cada vez
        }
        return result;
    }

    [Benchmark]
    public string ConcatWithStringBuilder()
    {
        var sb = new StringBuilder();
        for (int i = 0; i < N; i++)
        {
            sb.Append("a");  // ✅ Modifica buffer interno
        }
        return sb.ToString();
    }

    [Benchmark]
    public string ConcatWithStringCreate()
    {
        return string.Create(N, 'a', (span, value) =>
        {
            span.Fill(value);  // ✅ Stackalloc + Fill
        });
    }
}