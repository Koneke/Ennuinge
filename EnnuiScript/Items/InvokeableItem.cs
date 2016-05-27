namespace EnnuiScript.Items
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class Invokeable // function
	{
		public List<Func<List<Item>, bool>> Demands;
		public Func<SymbolSpace, List<Item>, Item> Function;
		public ItemType ReturnType;

		public bool EvaluateDemands(List<Item> args)
		{
			foreach (var demand in this.Demands)
			{
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
	}

	public class InvokeableItem : Item // function
	{
		private readonly List<Invokeable> invokeables;

		public InvokeableItem() : base(ItemType.Invokeable)
		{
			this.invokeables = new List<Invokeable>();
		}

		public void AddInvokeable(Invokeable invokeable)
		{
			this.invokeables.Add(invokeable);
		}

		public Item Invoke(SymbolSpace space, List<Item> items)
		{
			foreach (var invokeable in this.invokeables)
			{
				if (invokeable.EvaluateDemands(items))
				{
					return invokeable.Invoke(space, items);
				}
			}

			throw new Exception("No matching signature in invokeable collection.");
		}

		public override string ToString()
		{
			return $"invokeable-collection";
		}

		public override string Print(int indent = 0)
		{
			return
				string.Concat(Enumerable.Repeat("\t", indent)) +
				this;
		}
	}
}