using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wox.Plugin.Choco.Extensions
{
    public static class Lists
    {
        public static List<T> Of<T>(T item)
        {
            return new List<T> { item };
        }

        internal static IList<TR> FullOuterJoin<TA, TB, TK, TR>(
        this IEnumerable<TA> a,
        IEnumerable<TB> b,
        Func<TA, TK> selectKeyA,
        Func<TB, TK> selectKeyB,
        Func<TA, TB, TK, TR> projection,
        TA defaultA = default(TA),
        TB defaultB = default(TB),
        IEqualityComparer<TK> cmp = null)
        {
            cmp = cmp ?? EqualityComparer<TK>.Default;
            var alookup = a.ToLookup(selectKeyA, cmp);
            var blookup = b.ToLookup(selectKeyB, cmp);

            var keys = new HashSet<TK>(alookup.Select(p => p.Key), cmp);
            keys.UnionWith(blookup.Select(p => p.Key));

            var join = from key in keys
                       from xa in alookup[key].DefaultIfEmpty(defaultA)
                       from xb in blookup[key].DefaultIfEmpty(defaultB)
                       select projection(xa, xb, key);

            return join.ToList();
        }
    }
}
