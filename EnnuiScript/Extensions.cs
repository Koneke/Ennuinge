namespace EnnuiScript
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public static class Extensions
	{
		public static bool HasEvenLength<T>(this List<T> source)
		{
			return source.Count % 2 == 0;
		}

		public static List<List<T>> GroupingSelect<T>(this List<T> source, int groupSize)
		{
			if (source.Count % groupSize != 0)
			{
				throw new Exception();
			}

			var output = new List<List<T>>();

			for (var i = 0; i < source.Count(); i += groupSize)
			{
				var group = source.Skip(i).Take(groupSize).ToList();
				output.Add(new List<T>(group));
			}
			
			return output;
		}

		public static List<TOut> GroupingSelect<TIn, TOut>(this List<TIn> source, int groupSize, Func<List<TIn>, TOut> selector)
		{
			if (source.Count() % groupSize != 0)
			{
				throw new Exception();
			}

			var current = new List<TOut>();

			for (var i = 0; i < source.Count(); i += groupSize)
			{
				var group = source.Skip(i).Take(groupSize).ToList();
				current.Add(selector(group));
			}
			
			return current;
		}
	}
}