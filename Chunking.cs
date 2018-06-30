using System.Collections.Generic;
using System.Linq;

namespace chunking_benchmark
{
    public static class Chunking
    {
        public static IEnumerable<IEnumerable<T>> ImplicitEnumeratorChunkAsList<T>(IEnumerable<T> items, int size)
        {
            var chunk = new List<T>(size);

            foreach (var item in items)
            {
                chunk.Add(item);
                if (chunk.Count == size)
                {
                    yield return chunk;
                    chunk = new List<T>(size);
                }
            }
        }

        public static IEnumerable<IEnumerable<T>> ImplicitEnumeratorChunkAsArray<T>(IEnumerable<T> items, int size)
        {
            T[] chunk = new T[size];
            int count = 0;

            foreach (var item in items)
            {
                chunk[count] = item;
                count += 1;

                if (count == size)
                {
                    yield return chunk;
                    chunk = new T[size];
                    count = 0;
                }
            }
        }

        public static IEnumerable<IEnumerable<T>> ExplicitEnumeratorChunkAsList<T>(IEnumerable<T> items, int size)
        {
            var enumerator = items.GetEnumerator();
            var chunk = new List<T>(size);
            while(enumerator.MoveNext())
            {
                chunk.Add(enumerator.Current);
                if (chunk.Count == size)
                {
                    yield return chunk;
                    chunk = new List<T>(size);
                }
            }
        }

        public static IEnumerable<IEnumerable<T>> ExplicitEnumeratorChunkAsArray<T>(IEnumerable<T> items, int size)
        {
            var enumerator = items.GetEnumerator();
            var chunk = new T[size];
            var count = 0;

            while(enumerator.MoveNext())
            {
                chunk[count] = enumerator.Current;
                count += 1;
                if (count == size)
                {
                    yield return chunk;
                    chunk = new T[size];
                    count = 0;
                }
            }
        }

        public static IEnumerable<IEnumerable<T>> ExplicitIteratorChunkAsArrayRefInLoop<T>(IEnumerable<T> items, int size)
        {
            var enumerator = items.GetEnumerator();
            bool inProgress = enumerator.MoveNext();
            while(inProgress)
            {
                var result = new T[size];
                for (int i = 0; i < size && inProgress; i++, inProgress = enumerator.MoveNext())
                {
                    result[i] = enumerator.Current;
                }
                yield return result;
            }
        }

        public static IEnumerable<IEnumerable<T>> ExplicitIteratorLazyChunk<T>(IEnumerable<T> items, int size)
        {
            using(var enumerator = items.GetEnumerator())
            while(enumerator.MoveNext())
            {
                yield return LazyChunkGenerator(enumerator, size);
            }
        }

        private static IEnumerable<T> LazyChunkGenerator<T>(IEnumerator<T> enumerator, int size)
        {
            int count = 0;
            do
            {
                yield return enumerator.Current;
                count += 1;
            }
            while (count < size && enumerator.MoveNext());
        }

        public static IEnumerable<IEnumerable<T>> LinqYieldTakeSkip<T>(IEnumerable<T> items, int size)
        {
            while (items.Any())
            {
                yield return items.Take(size);
                items = items.Skip(size);
            }
        }

        private static IEnumerable<IEnumerable<T>> ZipMultiple<T>(IEnumerable<IEnumerable<T>> parts)
        {
            var enumerators = parts.Select(p => p.GetEnumerator()).ToList();

            while(enumerators.All(e => e.MoveNext())) {
                var items = enumerators.Select(e => e.Current);
                yield return items;
            }

            foreach (var enumerator in enumerators)
            {
                enumerator.Dispose();
            }
        }

        public static IEnumerable<IEnumerable<T>> LinqWhereZipMultiple<T>(IEnumerable<T> items, int size)
        {
            IEnumerable<IEnumerable<T>> parts = Enumerable.Range(0, size)
                .Select(i => items.Where((val, idx) => idx / size == i));

            return ZipMultiple(parts);
        }
        
        public static IEnumerable<IEnumerable<T>> LinqGroupByWithoutYield<T>(IEnumerable<T> items, int size)
        {
            return items
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / size)
                .Select(x => x.Select(v => v.Value));
        }
    }
}