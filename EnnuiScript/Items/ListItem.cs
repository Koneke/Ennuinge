namespace EnnuiScript.Items
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class ListItem : EvaluateableItem
	{
		public readonly List<Item> Expression;

		public ListItem(params Item[] items) : base(ItemType.List)
		{
			this.Expression = new List<Item>();
			this.Add(items);
		}

		public ListItem Add(params Item[] items)
		{
			foreach (var item in items)
			{
				this.Add(item);
			}

			return this;
		}

		public ListItem Add(Item item)
		{
			this.Expression.Add(item);
			return this;
		}

		private List<Item> Flatten(SymbolSpace space)
		{
			return this.Expression
				.Select(item => item is EvaluateableItem
					? ((EvaluateableItem) item).Evaluate(space)
					: item)
				.ToList();
		}

		public override Item Evaluate(SymbolSpace space)
		{
			if (this.IsQuoted)
			{
				this.IsQuoted = false;
				return this;
			}

			var exp = this.Flatten(space);

			var head = exp.First() as InvokeableItem;
			var tail = exp.Skip(1).ToList();

			if (head == null)
			{
				throw new Exception();
			}

			return head.Invoke(space, tail);
		}
	}
}