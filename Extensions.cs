using System.Collections.Generic;

namespace chunking_benchmark
{
    public static class Extensions
    {
        public static void ProduceAll<T>(this IEnumerable<IEnumerable<T>> collectionsOfItems)
        {
            foreach (var items in collectionsOfItems)
            {
                items.ProduceAll();
            }
        }

        public static void ProduceAll<T>(this IEnumerable<T> collection)
        {
            foreach (var item in collection)
            {
                // Pass
            }
        }
    }
}