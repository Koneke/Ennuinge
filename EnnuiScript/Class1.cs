using System;
using System.Collections.Generic;
using System.Linq;

namespace EnnuiScript
{
	public class Class1
	{
		private bool TypeMatch(List<Item> items, params ItemType[] types)
		{
			return false;
		}

		public void Main()
		{
			var add = new Invokeable
			{
				//Demand = args => args.All(arg => arg.ItemType == ItemType.Number),
				Demands = Invokeable.MakeDemands(
					Invokeable.DemandTypeMany(arg => arg.ItemType == ItemType.Number)
				),

				Function = args => new ValueItem(
					ItemType.Number,
					args
						.Select(arg => arg as ValueItem)
						.Select(arg => (double)arg.Value) // we already know from our demand that it's all numbers
						.Sum())
			};

			var fn = new Invokeable
			{
				Demands = Invokeable.MakeDemands(
					Invokeable.DemandType(arg => arg.ItemType == ItemType.Symbol), // name
					Invokeable.DemandType(arg => arg.ItemType == ItemType.Type), // return type
					Invokeable.DemandType(arg => arg.ItemType == ItemType.List), // arguments
					Invokeable.DemandType(arg => arg.ItemType == ItemType.List) // body
				),
				Function = args => {
					var symbol = args[0] as SymbolItem;
					var returnType = args[1] as TypeItem;
					var arguments = args[2] as ListItem;
					var body = args[3] as ListItem;

					return null;
				}
			};

			var exp = new ListItem();

			/*exp.Add(
				new Symbol(add),
				new ValueItem(ItemType.Number, 1.0),
				new ValueItem(ItemType.Number, 1.0));*/

			exp.Add(
				new SymbolItem(add),
				new ListItem(
					add,
					new ValueItem(ItemType.Number, 1.0),
					new ValueItem(ItemType.Number, 1.0)),
				new ValueItem(ItemType.Number, 1.0));

			var result = exp.Evaluate();
			var a = 0;
		}
	}

	public enum ItemType
	{
		Type,

		Number,
		String,

		Symbol,
		List,
		Invokeable
	}

	public class TypeItem : Item
	{
		public ItemType Type { get; }

		public TypeItem(ItemType type) : base(ItemType.Type)
		{
			this.Type = type;
		}
	}

	public class Invokeable : Item // function
	{
		public static List<Func<List<Item>, DemandResult>> MakeDemands(params Func<List<Item>, DemandResult>[] inDemands)
		{
			var demands = new List<Func<List<Item>, DemandResult>>();

			foreach (var demand in inDemands)
			{
				demands.Add(demand);
			}

			return demands;
		}

		public static Func<List<Item>, DemandResult> DemandType(Func<Item, bool> condition)
		{
			return args =>
				new DemandResult
				{
					success = condition(args.First()),
					items = args.Skip(1).ToList()
				};
		}

		public static Func<List<Item>, DemandResult> DemandTypeMany(Func<Item, bool> condition, int count = -1)
		{
			return args => {
				var argCount = count == -1 ? args.Count : count;

				return new DemandResult
				{
					success = args.Take(argCount).All(condition),
					items = args.Skip(argCount).ToList()
				};
			};
		}

		public struct DemandResult
		{
			public bool success;
			public List<Item> items;

			public DemandResult(bool success, List<Item> items)
			{
				this.success = success;
				this.items = items;
			}
		}

		public Func<List<Item>, bool> Demand;
		public List<Func<List<Item>, DemandResult>> Demands;
		public Func<List<Item>, Item> Function;

		public Invokeable() : base(ItemType.Invokeable)
		{
		}

		private bool EvaluateDemands(List<Item> inputArgs)
		{
			var args = new List<Item>(inputArgs);

			foreach (var demand in this.Demands)
			{
				var result = demand(args);

				if (!result.success)
				{
					throw new Exception("Failed demands.");
				}

				args = result.items;
			}

			if (args.Count != 0)
			{
				throw new Exception("Failed demands (too many arguments).");
			}

			return true;
		}

		public Item Invoke(List<Item> items)
		{
			// if (!this.Demand(items))
			if (!this.EvaluateDemands(items))
			{
				throw new Exception("Failed demands.");
			}

			return this.Function(items);
		}
	}

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

	public abstract class EvaluateableItem : Item
	{
		protected bool isQuoted;
		public abstract Item Evaluate();

		protected EvaluateableItem(ItemType type) : base(type)
		{
		}

		public void Quote()
		{
			this.isQuoted = true;
		}
	}

	public class SymbolItem : EvaluateableItem
	{
		private Item reference;

		public SymbolItem(Item reference) : base(ItemType.Symbol)
		{
			this.reference = reference;
		}

		public Item Flatten()
		{
			var current = this.reference;

			while (current.ItemType == ItemType.Symbol)
			{
				var symbol = current as SymbolItem;

				if (symbol.isQuoted)
				{
					break;
				}

				current = symbol.Evaluate();
			}

			return current;
		}

		public override Item Evaluate()
		{
			if (this.isQuoted)
			{
				this.isQuoted = false;
				return this;
			}

			return this.Flatten();
		}
	}

	public class ListItem : EvaluateableItem
	{
		public readonly List<Item> expression;

		public ListItem(params Item[] items) : base(ItemType.List)
		{
			this.expression = new List<Item>();
			this.Add(items);
		}

		public ListItem Add(params Item[] items)
		{
			foreach (var item in items)
			{
				this.Add(item);
			}

			return this;
		}

		public ListItem Add(Item item)
		{
			this.expression.Add(item);
			return this;
		}

		private List<Item> Flatten()
		{
			var resultExp = new List<Item>();

			foreach (var item in this.expression)
			{
				resultExp.Add(
					item is EvaluateableItem
						? (item as EvaluateableItem).Evaluate()
						: item);
			}

			return resultExp;
		}

		public override Item Evaluate()
		{
			if (this.isQuoted)
			{
				this.isQuoted = false;
				return this;
			}

			var exp = this.Flatten();

			var head = exp.First() as Invokeable;
			var tail = exp.Skip(1).ToList();

			if (head == null)
			{
				throw new Exception();
			}

			return head.Invoke(tail);
		}
	}
}