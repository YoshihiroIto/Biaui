using BenchmarkDotNet.Running;

namespace Biaui.Benchmark
{
    class Program
    {
        // ReSharper disable once UnusedParameter.Local
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<BiaNodeEditorHelperTest>();
        }
    }
}