using System.Collections.Generic;
using System.Linq;

namespace VibeScheduler
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> AsCircular<T>(this IEnumerable<T> source)
        {
            var list = source.ToList();
            var enumerator = list.GetEnumerator();

            while (true)
            {
                var moveNext = enumerator.MoveNext();

                if (!moveNext)
                {
                    enumerator.Dispose();

                    enumerator = list.GetEnumerator();

                    enumerator.MoveNext();
                }

                yield return enumerator.Current;
            }
        }
    }
}