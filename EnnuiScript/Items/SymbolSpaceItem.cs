namespace EnnuiScript.Items
{
	using System;
	using System.Linq;

	class SymbolSpaceItem : Item
	{
		public SymbolSpace Space { get; }

		public SymbolSpaceItem(SymbolSpace space) : base(ItemType.Space)
		{
			this.Space = space;
		}

		public override string Print(int indent = 0)
		{
			if (this.Space == null)
			{
				return "{}";
			}

			Func<string, string> smartPrint = key =>
			{
				var item = this.Space.Bindings[key];

				var symbolSpaceItem = item as SymbolSpaceItem;
				if (symbolSpaceItem != null && symbolSpaceItem.Space == this.Space)
				{
					return "this";
				}

				var longest = this.Space.Bindings.Keys.OrderBy(k => k.Length).Last().Length;

				Func<string, string> spacing = k =>
					string.Concat(Enumerable.Repeat(' ', longest - k.Length));

				var keyString = item is SymbolSpaceItem
					? key
					: $"{key}{spacing(key)}";

				return $"{keyString} {this.Space.Bindings[key].Print(indent + 1).TrimStart('\t')}";
			};

			var indentation = string.Concat(Enumerable.Repeat("\t", indent));

			return
				"{\n" +
				string.Join(
					"\n",
					this.Space.Bindings.Keys
						.Where(k => k != "super")
						.Select(k => indentation + "\t" + smartPrint(k))
				) +
				"\n" +
				indentation +
				"}";
		}
	}
}