namespace EnnuiScript.Items
{
	using System.Linq;

	public abstract class Item
	{
		public static bool IsValueType(ItemType type)
		{
			var valueTypes = new[] { ItemType.Number, ItemType.String };
			return valueTypes.Contains(type);
		}

		public ItemType ItemType { get; }

		protected Item(ItemType itemType)
		{
			this.ItemType = itemType;
		}

		public abstract string Print(int indent = 0);
	}
}