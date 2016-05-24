namespace EnnuiScript.Items
{
	class SymbolSpaceItem : Item
	{
		public SymbolSpace Space { get; }

		public SymbolSpaceItem(SymbolSpace space) : base(ItemType.SymbolSpace)
		{
			this.Space = space;
		}
	}
}