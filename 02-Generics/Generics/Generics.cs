using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;

namespace Task.Generics
{

    public static class ListConverter
    {

        private static char ListSeparator = ',';

        public static string ConvertToString<T>(this IEnumerable<T> list)
        {
            return string.Join(ListSeparator.ToString(), list);
        }

        public static IEnumerable<T> ConvertToList<T>(this string list)
        {
            var desctiptor = TypeDescriptor.GetConverter(typeof(T));
            foreach (var item in list.Split(ListSeparator))
            {
                yield return (T)desctiptor.ConvertFrom(item);
            }
        }
    }

    public static class ArrayExtentions
    {
        public static void SwapArrayElements<T>(this T[] array, int index1, int index2)
        {
            (array[index1], array[index2]) = (array[index2], array[index1]);
        }

        public static void SortTupleArray<T1, T2, T3>(this Tuple<T1, T2, T3>[] array, int sortedColumn, bool ascending)
        where T1 : IComparable
        where T2 : IComparable
        where T3 : IComparable
        {
            if (sortedColumn >= array.Length || sortedColumn < 0)
                throw new IndexOutOfRangeException();

            Array.Sort(array, (x, y) =>
            {
                dynamic a = y.GetType().GetProperty($"Item{sortedColumn + 1}").GetValue(y);
                dynamic b = x.GetType().GetProperty($"Item{sortedColumn + 1}").GetValue(x);
                return ascending ? -a.CompareTo(b) : a.CompareTo(b);
            });
        }
    }

    public static class Singleton<T> where T : new()
    {
        private static readonly Lazy<T> lazy = new Lazy<T>(() => new T());
        public static T Instance
        {
            get
            {
                return lazy.Value;
            }
        }
    }

    public static class FunctionExtentions
    {
        public static T TimeoutSafeInvoke<T>(this Func<T> function)
        {
            for (int attempts = 0; attempts < 3; attempts++)
            {
                try
                {
                    return function();
                }
                catch (WebException ex)
                {
                    Trace.WriteLine(ex.ToString());
                }
            }

            throw new WebException("The operation has timed out", WebExceptionStatus.Timeout);
        }

        public static Predicate<T> CombinePredicates<T>(Predicate<T>[] predicates)
        {
            return (T x) => predicates.All(predicate => predicate.Invoke(x));
        }
    }
}