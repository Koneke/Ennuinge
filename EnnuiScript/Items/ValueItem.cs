namespace EnnuiScript.Items
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

		public object Value { get; private set; }

		public ValueItem(ItemType type, object value) : base(type)
		{
			if (!IsValueType(type))
			{
				throw new Exception();
			}

			this.Value = CastedValue(type, value);
		}
	}
}