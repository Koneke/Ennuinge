namespace EnnuiScript.Items
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class InvokeableItem : Item // function
	{
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