namespace EnnuiScript
{
	public class TypeItem : Item
	{
		public ItemType Type { get; }

		public TypeItem(ItemType type) : base(ItemType.Type)
		{
			this.Type = type;
		}
	}
}