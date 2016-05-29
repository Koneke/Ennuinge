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
			if (!(obj is Item))
			{
				return false;
			}

			var other = (Item)obj;

			switch (other.ItemType)
			{
				// value comparison
				case ItemType.Bool:
				case ItemType.Number:
				case ItemType.String:
					return ((ValueItem)obj).Compare(this);
				case ItemType.List:
					return ((ListItem)obj).Compare(this);
				case ItemType.Symbol:
					return ((SymbolItem)obj).Compare(this);
				case ItemType.Type:
					return ((TypeItem)obj).Compare(this);

				// strict reference comparison
				case ItemType.Space:
					return ((SymbolSpaceItem)obj).Compare(this);
				case ItemType.Invokeable:
					return ((InvokeableItem)obj).Compare(this);

				default:
					return false;
			}
		}
	}
}