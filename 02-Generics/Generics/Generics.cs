using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Text;

namespace Task.Generics {

	public static class ListConverter {

		private static char ListSeparator = ',';  

		public static string ConvertToString<T>(this IEnumerable<T> list) {
			if (list == null)
				throw new ArgumentNullException("List is null");

			StringBuilder stringBuilder = new StringBuilder();
			foreach (var item in list)
				stringBuilder.Append(item.ToString() + ListSeparator);

			return stringBuilder.ToString().Remove(stringBuilder.Length - 1);
		}

		public static IEnumerable<T> ConvertToList<T>(this string list) {
			if (list == null)
				throw new ArgumentNullException("List is null");

			var result = new List<T>();
			foreach (var item in list.Split(ListSeparator))
				result.Add((T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(item));

			return result;
		}
	}

	public static class ArrayExtentions {
		public static void SwapArrayElements<T>(this T[] array, int index1, int index2) {
			(array[index1], array[index2]) = (array[index2], array[index1]);
		}

		public static void SortTupleArray<T1, T2, T3>(this Tuple<T1, T2, T3>[] array, int sortedColumn, bool ascending)
		where T1 : IComparable
		where T2 : IComparable
		where T3 : IComparable
        {
            if (sortedColumn == 0)
                Array.Sort(array, (x, y) => y.Item1.CompareTo(x.Item1));
            else if (sortedColumn == 1)
                Array.Sort(array, (x, y) => y.Item2.CompareTo(x.Item2));
            else if (sortedColumn == 2)
                Array.Sort(array, (x, y) => y.Item3.CompareTo(x.Item3));
            else
                throw new IndexOutOfRangeException("Sorted column is less than 0 or more than 2");

            if (ascending)
				Array.Reverse(array);
	}
}

	public static class Singleton<T> where T : new()
	{
		static T _insance;
		private static readonly object close = new object();
		public static T Instance
		{
			get
			{
				lock (close)
				{
					if (_insance == null)
						_insance = new T();
					return _insance;
				}
			}
		}
	}

	public static class FunctionExtentions {
		public static T TimeoutSafeInvoke<T>(this Func<T> function) 
		{
			int countOfRestarts = 0;
			while (countOfRestarts < 3)
			{
				try
				{
					return function();
				}
				catch (Exception ex)
				{
					Trace.WriteLine(ex.ToString());
					countOfRestarts++;
				}
			}

			throw new WebException("The operation has timed out", WebExceptionStatus.Timeout);
		}

		public static Predicate<T> CombinePredicates<T>(Predicate<T>[] predicates) 
		{
			return (T x) =>
			{
				foreach (var predicate in predicates)
				{
					if (!predicate(x))
						return false;
				}

				return true;
			};
		}
	}
}