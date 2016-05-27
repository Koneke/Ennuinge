namespace EnnuiScript.Utils
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using EnnuiScript.Items;

	public class InvokeableUtils
	{
		public static List<Func<List<Item>, bool>> MakeDemands(params Func<List<Item>, bool>[] inDemands)
		{
			var demands = new List<Func<List<Item>, bool>>();

			foreach (var demand in inDemands)
			{
				demands.Add(demand);
			}

			return demands;
		}

		public static Func<List<Item>, bool> DemandOfAnyType(int index, params ItemType[] types)
		{
			return args => types.Contains(args[index].ItemType);
		}

		private static bool TypeMatches(Item item, ItemType type)
		{
			return type == ItemType.Any || item.ItemType == type;
		}

		public static Func<List<Item>, bool> DemandType(int index, ItemType type)
		{
			return args => TypeMatches(args[index], type);
		}

		public static Func<List<Item>, bool> DemandTypes(params ItemType[] types)
		{
			return DemandTypes(0, types);
		}

		public static Func<List<Item>, bool> DemandTypes(int startIndex, params ItemType[] types)
		{
			return args => Enumerable.Range(startIndex, types.Length)
				.All(index => TypeMatches(args[index], types[index]));
		}
	}
}