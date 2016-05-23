namespace EnnuiScript.Items
{
	public class SymbolItem : EvaluateableItem
	{
		public string Name;

		public SymbolItem(string name) : base(ItemType.Symbol)
		{
			this.Name = name;
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

			return this.Flatten(space);
		}
	}
}