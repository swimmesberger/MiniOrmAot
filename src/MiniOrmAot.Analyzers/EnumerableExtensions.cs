using System.Collections.Generic;

namespace MiniOrmAot.Analyzers; 

public static class EnumerableExtensions {
    public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source, IEqualityComparer<T>? comparer = null) {
        return new HashSet<T>(source, comparer);
    }
}