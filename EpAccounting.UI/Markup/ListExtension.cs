// ///////////////////////////////////
// File: ListExtension.cs
// Last Change: 17.02.2018, 14:28
// Author: Andre Multerer
// ///////////////////////////////////

namespace EpAccounting.UI.Markup
{
    using System;
    using System.Collections.Generic;
    using System.Linq;


    public static class ListExtension
    {
        public static void InsertOrderedBy<TSource, TKey>(this IList<TSource> source, TSource item, Func<TSource, TKey> keySelector) where TKey : IComparable<TKey>
        {
            var i = source.Select((value, index) => new {Value = value, Index = index}).FirstOrDefault(x => keySelector(x.Value).CompareTo(keySelector(item)) > 0);

            if (i == null)
            {
                source.Add(item);
            }
            else
            {
                source.Insert(i.Index, item);
            }
        }
    }
}