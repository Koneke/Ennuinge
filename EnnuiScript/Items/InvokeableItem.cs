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
			//return args => type == ItemType.Any || args[index].ItemType == type;
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

			if (result == null)
			{
				if (this.ReturnType != ItemType.None)
				{
					throw new Exception("Non-void function returned void.");
				}
			}
			else
			{
				if (this.ReturnType != ItemType.Any && this.ReturnType != result.ItemType)
				{
					throw new Exception("Function returned improper type.");
				}
			}

			return result;
		}

		public override string ToString()
		{
			return $"invokeable:{this.ReturnType}";
		}

		public override string Print(int indent = 0)
		{
			return
				string.Concat(Enumerable.Repeat("\t", indent)) +
				this;
		}
	}
}