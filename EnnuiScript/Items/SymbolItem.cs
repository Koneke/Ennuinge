namespace EnnuiScript.Items
{
	using System.Text;
	using System.Collections.Generic;

	public class SymbolItem : EvaluateableItem
	{
		public string Name;
		public SymbolSpace BoundSpace;

		public SymbolItem(string name) : base(ItemType.Symbol)
		{
			this.Name = name;
		}

		public SymbolItem(string name, SymbolSpace space) : base(ItemType.Symbol)
		{
			this.Name = name;
			this.BoundSpace = space;
		}

		public Item Flatten(SymbolSpace space)
		{
			var current = space.Lookup(this.Name);

			while (current.ItemType == ItemType.Symbol)
			{
				var symbol = (SymbolItem)current;

				if (symbol.IsQuoted)
				{
					break;
				}

				current = symbol.Evaluate(space);
			}

			return current;
		}

		public override Item Evaluate(SymbolSpace space)
		{
			if (this.IsQuoted)
			{
				this.IsQuoted = false;
				return this;
			}

			return this.Flatten(this.BoundSpace ?? space);
		}

		public override string ToString()
		{
			var attributes = new List<string>();

			if (this.IsQuoted)
			{
				attributes.Add("quoted");
			}

			if (this.BoundSpace != null)
			{
				attributes.Add("bound");
			}

			attributes.Add("symbol");

			var attributeString = string.Join(" ", attributes);

			return $"{attributeString} {this.Name}";
		}
	}
}