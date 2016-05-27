namespace EnnuiScript.Items
{
	using System;
	using System.Linq;

	public class ValueItem : Item
	{
		private static bool IsValueType(ItemType type)
		{
			var valueTypes = new[]
			{
				ItemType.Number,
				ItemType.String,
				ItemType.Bool
			};
			return valueTypes.Contains(type);
		}

		private static object CastedValue(ItemType type, object value)
		{
			switch (type)
			{
				case ItemType.Number:
					return (double)value;
				case ItemType.String:
					return (string)value;
				case ItemType.Bool:
					return (bool)value;
				default:
					throw new Exception();
			}
		}

		public ItemType Type { get; }
		public object Value { get; }

		public ValueItem(ItemType type, object value) : base(type)
		{
			if (!IsValueType(type))
			{
				throw new Exception();
			}

			this.Type = type;
			this.Value = CastedValue(type, value);
		}

		public override string ToString()
		{
			var typeName = Enum.GetName(typeof(ItemType), this.Type);
			var value = this.Value.ToString();

			if (this.Type == ItemType.String)
			{
				value = "\"" + value + "\"";
			}

			if (this.Type == ItemType.Bool)
			{
				value = "*" + value;
			}

			return $"value({typeName}):{value}";
		}

		public override string Print(int indent = 0)
		{
			return
				string.Concat(Enumerable.Repeat("\t", indent)) +
				this.Value;
		}

		public override bool Compare(Item item)
		{
			if (!this.BasicCompare(item))
			{
				return false;
			}

			var other = item as ValueItem;

			return other.Value.Equals(this.Value);
		}
	}
}