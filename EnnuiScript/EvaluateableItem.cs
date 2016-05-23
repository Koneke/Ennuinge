namespace EnnuiScript
{
	public abstract class EvaluateableItem : Item
	{
		protected bool isQuoted;
		public abstract Item Evaluate(SymbolSpace space);

		protected EvaluateableItem(ItemType type) : base(type)
		{
		}

		public Item Unquote()
		{
			this.isQuoted = false;
			return this;
		}

		public Item Quote()
		{
			this.isQuoted = true;
			return this;
		}
	}
}