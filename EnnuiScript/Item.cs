using System.Linq;

namespace EnnuiScript
{
	public class Item
	{
		public static bool IsValueType(ItemType type)
		{
			var valueTypes = new[] { ItemType.Number, ItemType.String };
			return valueTypes.Contains(type);
		}

		public ItemType ItemType { get; }

		public Item(ItemType itemType)
		{
			this.ItemType = itemType;
		}
	}
}