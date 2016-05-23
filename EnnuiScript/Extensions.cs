namespace EnnuiScript
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public static class Extensions
	{
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