using System.Collections.Generic;
using System.Linq;

namespace Enigmatry.Entry.Core.Helpers
{
    public static class ListExtensions
    {
        public static IList<T> IntersectAll<T>(this IList<IEnumerable<T>> source)
        {
            if (!source.Any())
            {
                return new List<T>();
            }

            return source.Skip(1)
                .Aggregate(new HashSet<T>(source.First()), (h, e) =>
                {
                    h.IntersectWith(e);
                    return h;
                })
                .ToList();
        }

        public static bool SameAs<T>(this IList<T> collection, IList<T> givenCollection)
        {
            var first = collection.Except(givenCollection);

            if (first.Any())
            {
                return false;
            }

            return !givenCollection.Except(collection).Any();
        }

        public static void Replace<T>(this IList<T> data, T original, T replacement)
        {
            var index = data.IndexOf(original);
            data[index] = replacement;
        }
    }
}
