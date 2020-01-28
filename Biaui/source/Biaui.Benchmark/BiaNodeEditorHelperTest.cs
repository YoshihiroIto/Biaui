using System;
using BenchmarkDotNet.Attributes;
using Biaui.Internals;

namespace Biaui.Benchmark
{
    [ShortRunJob]
    [MemoryDiagnoser]
    public class BiaNodeEditorHelperTest
    {
        public ImmutableVec2_float[]? Data;

        [GlobalSetup]
        public void Setup()
        {
            var r = new Random();
            
            Data = new ImmutableVec2_float[4 * 1000000];

            for (var i = 0; i != Data.Length; ++i)
                Data[i] = new ImmutableVec2_float((float) r.NextDouble(), (float) r.NextDouble());
        }

        [Benchmark(Baseline = true)]
        public float MakeBoundingBox()
        {
            var x = 0.0f;
            var y = 0.0f;

            for (var i = 0; i < Data!.Length; i += 4)
            {
                var r = BiaNodeEditorHelper.MakeBoundingBox(Data[i + 0], Data[i + 1], Data[i + 2], Data[i + 3]);

                x += r.X + r.Width;
                y += r.Y + r.Height;
            }

            return x + y;
        }

        [Benchmark]
        public float MakeBoundingBoxA()
        {
            var x = 0.0f;
            var y = 0.0f;

            for (var i = 0; i < Data!.Length; i += 4)
            {
                var r = BiaNodeEditorHelper.MakeBoundingBox(Data.AsSpan(i, 4));

                x += r.X + r.Width;
                y += r.Y + r.Height;
            }

            return x + y;
        }

        #if false
        [Benchmark]
        public float MakeBoundingBox2()
        {
            var x = 0.0f;
            var y = 0.0f;

            for (var i = 0; i < _data.Length; i += 4)
            {
                var r = BiaNodeEditorHelper.MakeBoundingBox2(_data.AsSpan(i, 4));

                x += r.X + r.Width;
                y += r.Y + r.Height;
            }

            return x + y;
        }
#endif
    }
}