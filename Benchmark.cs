using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Columns;
using BenchmarkDotNet.Attributes.Exporters;
using BenchmarkDotNet.Attributes.Jobs;

namespace chunking_benchmark
{
    [CoreJob]
    //[AllStatisticsColumn]
    [MemoryDiagnoser]
    public class Benchmark
    {
        private IEnumerable<int> Data;

        [Params(1_000_000)]
        public int CollectionSize;

        [Params(2, 10, 100, 1000)]
        public int ChunkSize;

        [GlobalSetup]
        public void Setup()
        {
            Data = Enumerable.Range(0, CollectionSize);
        }

        [Benchmark]
        public void Range() => Data.ProduceAll();

        [Benchmark]
        public void ImplicitEnumeratorChunkAsList()
            => Chunking.ImplicitEnumeratorChunkAsList(Data, ChunkSize).ProduceAll();

        [Benchmark]
        public void ImplicitEnumeratorChunkAsArray()
            => Chunking.ImplicitEnumeratorChunkAsArray(Data, ChunkSize).ProduceAll();

        [Benchmark]
        public void ExplicitEnumeratorChunkAsList()
            => Chunking.ExplicitEnumeratorChunkAsList(Data, ChunkSize).ProduceAll();

        [Benchmark]
        public void ExplicitEnumeratorChunkAsArray()
            => Chunking.ExplicitEnumeratorChunkAsArray(Data, ChunkSize).ProduceAll();

        // [Benchmark]
        public void ExplicitEnumeratorChunkAsArrayReferenceInLoop()
            => Chunking.ExplicitIteratorChunkAsArrayRefInLoop(Data, ChunkSize).ProduceAll();

        // [Benchmark]
        public void ExplicitEnumeratorLazyChunk()
            => Chunking.ExplicitIteratorLazyChunk(Data, ChunkSize).ProduceAll();

        // [Benchmark]
        public void CustomEnumerator()
            => ChunkingExtension.Chunk(Data, ChunkSize).ProduceAll();

        // [Benchmark]
        public void LinqYieldTakeSkip()
            => Chunking.LinqYieldTakeSkip(Data, ChunkSize).ProduceAll();

        //[Benchmark]
        public void LinqWhereZipMultiple()
            => Chunking.LinqWhereZipMultiple(Data, ChunkSize).ProduceAll();

        // [Benchmark]
        // public void LinqGroupBy()
        //     => Chunking.LinqGroupByWithoutYield(Data, ChunkSize).ProduceAll();
    }
}