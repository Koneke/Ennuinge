namespace EnnuiScript.Items
{
	using System;
	using System.Collections.Generic;

	public class Invokeable // function
	{
		public List<Func<List<Item>, bool>> Demands;
		public Func<SymbolSpace, List<Item>, Item> Function;
		public ItemType ReturnType;

		public Invokeable()
		{
			this.Demands = new List<Func<List<Item>, bool>>();
		}

		public bool EvaluateDemands(List<Item> args)
		{
			for (int index = 0; index < this.Demands.Count; index++)
			{
				var demand = this.Demands[index];
				if (!demand(args))
				{
					return false;
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
				if (this.ReturnType != ItemType.None && this.ReturnType != ItemType.Any)
				{
					throw new Exception("Non-void function returned void.");
				}
			}
			else
			{
				if (this.ReturnType != ItemType.Something &&
					this.ReturnType != ItemType.Any &&
					this.ReturnType != result.ItemType)
				{
					throw new Exception("Function returned improper type.");
				}
			}

			return result;
		}
	}
}