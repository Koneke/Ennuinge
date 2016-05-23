namespace EnnuiScript.Items
{
	public abstract class EvaluateableItem : Item
	{
		protected bool IsQuoted;
		public abstract Item Evaluate(SymbolSpace space);

		protected EvaluateableItem(ItemType type) : base(type)
		{
		}

		public Item Unquote()
		{
			this.IsQuoted = false;
			return this;
		}

		public Item Quote()
		{
			this.IsQuoted = true;
			return this;
		}
	}
}