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
			//var output = new List<Item>();

			var current = new List<Item>(this.Expression);

			Func<List<Item>, bool> anyNonquoted = l => l
				.Select(item => item as EvaluateableItem)
				.Where(item => item != null)
				.Count(item => !item.IsQuoted) > 0;

			// Evaluate all non-quoted while there are still non-quoted.
			// Then evaluate all quoted items *ONCE*.

			while (anyNonquoted(current))
			{
				var output = new List<Item>();

				foreach (var item in current)
				{
					Item evaluated;
					/*var evaluated = 
						item is EvaluateableItem
							? ((EvaluateableItem) item).Evaluate(space)
							: item;*/

					if (item is EvaluateableItem)
					{
						var evaluateable = item as EvaluateableItem;
						evaluated = evaluateable.IsQuoted
							? evaluateable
							: evaluateable.Evaluate(space);
					}
					else
					{
						evaluated = item;
					}

					output.Add(evaluated);
				}

				current = output;
			}

			var otp = new List<Item>();

			foreach (var item in current)
			{
				Item evaluated;

				if (item is EvaluateableItem)
				{
					var evaluateable = item as EvaluateableItem;
					evaluated = evaluateable.IsQuoted
						? evaluateable.Evaluate(space)
						: evaluateable;
				}
				else
				{
					evaluated = item;
				}

				otp.Add(evaluated);
			}

			current = otp;

			return current;

			/*foreach (var item in this.Expression)
			{
				var evaluated = 
					item is EvaluateableItem
						? ((EvaluateableItem) item).Evaluate(space)
						: item;

				output.Add(evaluated);
			}

			return output;*/

			return this.Expression
				.Select(item =>
				item is EvaluateableItem
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

			var result = head.Invoke(space, tail);
			return result;
		}

		public override string ToString()
		{
			var attributes = new List<string>();

			if (this.IsQuoted)
			{
				attributes.Add("quoted");
			}

			attributes.Add("list");

			var attributeString = string.Join(" ", attributes);

			return $"{attributeString} ({string.Join(", ", this.Expression)})";
		}
	}
}