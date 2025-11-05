
using BenchmarkDotNet.Running;
using System.Text;

// Program.cs
class Program
{
    static void Main(string[] args)
    {
        // var summary = BenchmarkRunner.Run<StringConcatBenchmark>();
        // var summary = BenchmarkRunner.Run<CollectionBenchmark>();
        // var summary = BenchmarkRunner.Run<PoolingBenchmark>();
        // var summary = BenchmarkRunner.Run<SpanBenchmark>();
        // var summary = BenchmarkRunner.Run<ValueTaskBenchmark>();


        // var summary = BenchmarkRunner.Run<StringInterningBenchmark>();
        string s1 = "MyTest";
        string s2 = new StringBuilder().Append("My").Append("Test").ToString();
        string s3 = String.Intern(s2);

        Console.WriteLine($"s1 == {s1}");
        Console.WriteLine($"s2 == {s2}");
        Console.WriteLine($"s3 == {s3}");
        Console.WriteLine($"Is s2 the same reference as s1?: {(object)s2 == (object)s1}");
        Console.WriteLine($"Is s3 the same reference as s1?: {(object)s3 == (object)s1}");


    }
}