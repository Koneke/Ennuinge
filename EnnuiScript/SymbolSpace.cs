using System;
using System.Collections.Generic;

namespace EnnuiScript
{
	public class SymbolSpace
	{
		private Dictionary<string, Item> Bindings;
		public SymbolSpace Parent;

		public SymbolSpace()
		{
			this.Bindings = new Dictionary<string, Item>();
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
			else
			{
				if (this.Parent != null)
				{
					return this.Parent.Lookup(symbol);
				}
				else
				{
					throw new Exception("Symbol not defined.");
				}
			}
		}
	}
}