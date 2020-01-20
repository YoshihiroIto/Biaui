using System;
using BenchmarkDotNet.Running;

namespace Biaui.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<BiaNodeEditorHelperTest>();
        }
    }
}