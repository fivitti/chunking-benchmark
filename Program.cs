﻿using System;

namespace chunking_benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkDotNet.Running.BenchmarkRunner.Run<Benchmark>();
            
        }
    }
}
