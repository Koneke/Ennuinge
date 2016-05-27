namespace EnnuiScript.Items
{
	using System.Linq;

	public abstract class Item
	{
		public ItemType ItemType { get; }

		protected Item(ItemType itemType)
		{
			this.ItemType = itemType;
		}

		public abstract string Print(int indent = 0);

		public bool BasicCompare(Item item)
		{
			if (item?.ItemType != this.ItemType)
			{
				return false;
			}

			return true;
		}

		public abstract bool Compare(Item item);

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}

			if (!(obj is Item))
			{
				return false;
			}

			var other = obj as Item;

			switch (other.ItemType)
			{
				case ItemType.Bool:
				case ItemType.Number:
				case ItemType.String:
					return ((ValueItem)obj).Compare(this);

				default:
					return false;
			}
		}
	}
}