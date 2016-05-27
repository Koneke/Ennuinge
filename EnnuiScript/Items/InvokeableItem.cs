namespace EnnuiScript.Items
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

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

		public override bool Compare(Item item)
		{
			if (!this.BasicCompare(item))
			{
				return false;
			}

			return item == this;
		}
	}
}