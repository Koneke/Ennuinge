namespace EnnuiScript.Items
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class InvokeableItem : Item // function
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

		public static Func<List<Item>, bool> DemandType(int index, ItemType type)
		{
			return args => args[index].ItemType == type;
		}

		public static Func<List<Item>, bool> DemandTypes(params ItemType[] types)
		{
			return DemandTypes(0, types);
		}

		public static Func<List<Item>, bool> DemandTypes(int startIndex, params ItemType[] types)
		{
			return args => Enumerable.Range(startIndex, types.Length)
				.All(index => args[index].ItemType == types[index]);
		}

		public List<Func<List<Item>, bool>> Demands;
		public Func<SymbolSpace, List<Item>, Item> Function;
		public ItemType ReturnType;

		public InvokeableItem() : base(ItemType.Invokeable)
		{
		}

		private bool EvaluateDemands(List<Item> args)
		{
			foreach (var demand in this.Demands)
			{
				if (!demand(args))
				{
					throw new Exception("Failed demands.");
				}
			}

			return true;
		}

		public Item Invoke(SymbolSpace space, List<Item> items)
		{
			if (!this.EvaluateDemands(items))
			{
				throw new Exception("Failed demands.");
			}

			var result = this.Function(space, items);

			if (result.ItemType != this.ReturnType)
			{
				throw new Exception("Function returned improper type.");
			}

			return result;
		}
	}
}