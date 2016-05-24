﻿namespace EnnuiScript.Items
{
	using System;

	public class ValueItem : Item
	{
		private static object CastedValue(ItemType type, object value)
		{
			switch (type)
			{
				case ItemType.Number:
					return (double)value;
				case ItemType.String:
					return (string)value;
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

			return $"value ({typeName}) {value}";
		}
	}
}