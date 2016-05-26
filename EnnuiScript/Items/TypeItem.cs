namespace EnnuiScript.Items
{
	using System;
	using System.Linq;

	public class TypeItem : Item
	{
		public ItemType Type { get; }

		public TypeItem(ItemType type) : base(ItemType.Type)
		{
			this.Type = type;
		}

		public override string Print(int indent = 0)
		{
			return
				string.Concat(Enumerable.Repeat("\t", indent)) +
				":" + Enum.GetName(typeof(ItemType), this.Type);
		}
	}
}