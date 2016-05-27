namespace EnnuiScript
{
	using System;
	using System.Collections.Generic;
	using Items;

	public class SymbolSpace
	{
		public const string SuperString = "*super";
		public const string LocalString = "*local";

		public Dictionary<string, Item> Bindings;
		private SymbolSpace Parent { get; set; }

		public void SetParent(SymbolSpace space)
		{
			this.Parent = space;
			this.Bind(SuperString, new SymbolSpaceItem(space));
			this.Bind(LocalString, new SymbolSpaceItem(this));
		}

		public SymbolSpace(SymbolSpace parent)
		{
			this.Bindings = new Dictionary<string, Item>();
			this.SetParent(parent);
		}

		public void Bind(string symbol, Item item)
		{
			if (this.Bindings.ContainsKey(symbol))
			{
				this.Bindings.Remove(symbol);
			}

			this.Bindings.Add(symbol, item);
		}

		public Item Lookup(string symbol)
		{
			if (this.Bindings.ContainsKey(symbol))
			{
				return this.Bindings[symbol];
			}

			if (this.Parent != null)
			{
				return this.Parent.Lookup(symbol);
			}

			throw new Exception($"Symbol not defined: {symbol}");
		}

		public SymbolSpace Clone()
		{
			var newSpace = new SymbolSpace(this.Parent);

			foreach (var k in this.Bindings.Keys)
			{
				newSpace.Bind(k, this.Bindings[k]/*.Clone()*/);
			}

			return newSpace;
		}

		public SymbolSpace GetParent()
		{
			return this.Parent;
		}
	}
}