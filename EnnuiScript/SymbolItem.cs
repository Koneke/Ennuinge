namespace EnnuiScript
{
	public class SymbolItem : EvaluateableItem
	{
		public string name;

		public SymbolItem(string name) : base(ItemType.Symbol)
		{
			this.name = name;
		}

		public Item Flatten(SymbolSpace space)
		{
			var current = space.Lookup(this.name);

			while (current.ItemType == ItemType.Symbol)
			{
				var symbol = current as SymbolItem;

				if (symbol.isQuoted)
				{
					break;
				}

				current = symbol.Evaluate(space);
			}

			return current;
		}

		public override Item Evaluate(SymbolSpace space)
		{
			if (this.isQuoted)
			{
				this.isQuoted = false;
				return this;
			}

			return this.Flatten(space);
		}
	}
}