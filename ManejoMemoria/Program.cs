
using BenchmarkDotNet.Running;


// Program.cs
class Program
{
    static void Main(string[] args)
    {
        // var summary = BenchmarkRunner.Run<StringConcatBenchmark>();
        // var summary = BenchmarkRunner.Run<CollectionBenchmark>();
        // var summary = BenchmarkRunner.Run<PoolingBenchmark>();
        // var summary = BenchmarkRunner.Run<SpanBenchmark>();

        var summary = BenchmarkRunner.Run<ValueTaskBenchmark>();
    }
}