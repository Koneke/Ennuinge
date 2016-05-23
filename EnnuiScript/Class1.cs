using System;
using System.Collections.Generic;
using System.Linq;

namespace EnnuiScript
{
	public static class Extensions
	{
		public static List<U> GroupingSelect<T, U>(this List<T> source, int groupSize, Func<List<T>, U> selector)
		{
			if (source.Count() % groupSize != 0)
			{
				throw new Exception();
			}

			var current = new List<U>();

			for (var i = 0; i < source.Count(); i += groupSize)
			{
				var group = source.Skip(i).Take(groupSize).ToList();
				current.Add(selector(group));
			}
			
			return current;
		}
	}

	public class SymbolSpace
	{
		private Dictionary<string, Item> Bindings;
		public SymbolSpace Parent;

		public SymbolSpace()
		{
			this.Bindings = new Dictionary<string, Item>();
		}

		public void Bind(string symbol, Item item)
		{
			if (this.Bindings.ContainsKey(symbol))
			{
				this.Bindings.Remove(symbol);
			}

			this.Bindings.Add(symbol, item);
		}

		public Item Lookup(string symbol)
		{
			if (this.Bindings.ContainsKey(symbol))
			{
				return this.Bindings[symbol];
			}
			else
			{
				if (this.Parent != null)
				{
					return this.Parent.Lookup(symbol);
				}
				else
				{
					throw new Exception("Symbol not defined.");
				}
			}
		}
	}

	public class Class1
	{
		private Dictionary<string, Invokeable> builtins = new Dictionary<string, Invokeable>();

		private bool TypeMatch(List<Item> items, params ItemType[] types)
		{
			return false;
		}

		private void SetupDefn()
		{
			var fn = new Invokeable
			{
				ReturnType = ItemType.Invokeable,

				Demands = Invokeable.MakeDemands(
					Invokeable.DemandTypes(
						ItemType.Symbol,
						ItemType.Type,
						ItemType.List,
						ItemType.List),

					args => (args[2] as ListItem).expression.Count % 2 == 0,

					args => {
						var argumentList = (args[2] as ListItem).expression;
						return argumentList
							.GroupingSelect(2, xs => new Tuple<Item, Item>(xs[0], xs[1]))
							.All(pair =>
								pair.Item1.ItemType == ItemType.Symbol &&
								pair.Item2.ItemType == ItemType.Type);
					}
				),
				Function = (space, args) => {
					var symbol = args[0] as SymbolItem;
					var returnType = args[1] as TypeItem;
					var argumentList = (args[2] as ListItem).expression
						.GroupingSelect(2, xs => new Tuple<Item, Item>(xs[0], xs[1]));
					var body = args[3] as ListItem;

					var argumentTypes = 
						argumentList
							.Select(argument => (argument.Item2 as TypeItem).Type)
							.ToArray();

					var resultingInvokeable = new Invokeable()
					{
						ReturnType = returnType.Type,

						Demands = Invokeable.MakeDemands(Invokeable.DemandTypes(argumentTypes)),
						
						// create symbol space,
						// bind fnargs,
						// evaluate body
						// pop symbolspace
						Function = (fnspace, fnargs) => {
							var newSpace = new SymbolSpace {Parent = space};

							for (var index = 0; index < fnargs.Count; index++)
							{
								newSpace.Bind(
									(argumentList[index].Item1 as SymbolItem).name,
									fnargs[index]);
							}

							body = body.Unquote() as ListItem;
							return body.Evaluate(newSpace);
						}
					};

					space.Bind(symbol.name, resultingInvokeable);

					return resultingInvokeable;
				}
			};

			this.builtins.Add("=>", fn);
		}

		private void SetupAdd()
		{
			var add = new Invokeable
			{
				ReturnType = ItemType.Number,

				Demands = Invokeable.MakeDemands(
					args => args.All(arg => arg.ItemType == ItemType.Number)
				),

				Function = (space, args) => new ValueItem(
					ItemType.Number,
					args
						.Select(arg => arg as ValueItem)
						.Select(arg => (double)arg.Value) // we already know from our demand that it's all numbers
						.Sum())
			};

			this.builtins.Add("+", add);
		}

		public void Main()
		{
			this.SetupAdd();
			this.SetupDefn();

			var exp = new ListItem();

			/*exp.Add(
				new Symbol(add),
				new ValueItem(ItemType.Number, 1.0),
				new ValueItem(ItemType.Number, 1.0));*/

			var space = new SymbolSpace();
			space.Bind("+", this.builtins["+"]);
			space.Bind("=>", this.builtins["=>"]);

			exp.Add(
				new SymbolItem("+"),
				new ListItem(
					new SymbolItem("+"),
					new ValueItem(ItemType.Number, 1.0),
					new ValueItem(ItemType.Number, 1.0)),
				new ValueItem(ItemType.Number, 1.0));

			var exp2 = new ListItem(
				new SymbolItem("=>"),
				new SymbolItem("my-fn").Quote(),
				new TypeItem(ItemType.Number),
				new ListItem(
					new SymbolItem("test"),
					new TypeItem(ItemType.Number)).Quote(),
				new ListItem(
					new SymbolItem("+"),
					new SymbolItem("test"),
					new SymbolItem("test")).Quote()
			);

			var exp3 = new ListItem(
				new SymbolItem("my-fn"),
				new ValueItem(ItemType.Number, 10.0));

			var result2 = exp2.Evaluate(space);
			var result3 = exp3.Evaluate(space);

			var result = exp.Evaluate(space);
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
		public static List<Func<List<Item>, bool>> MakeDemands(params Func<List<Item>, bool>[] inDemands)
		{
			var demands = new List<Func<List<Item>, bool>>();

			foreach (var demand in inDemands)
			{
				demands.Add(demand);
			}

			return demands;
		}

		public static Func<List<Item>, bool> DemandType(int index, ItemType type)
		{
			return args => args[index].ItemType == type;
		}

		public static Func<List<Item>, bool> DemandTypes(params ItemType[] types)
		{
			return DemandTypes(0, types);
		}

		public static Func<List<Item>, bool> DemandTypes(int startIndex, params ItemType[] types)
		{
			return args => Enumerable.Range(startIndex, types.Length)
				.All(index => args[index].ItemType == types[index]);
		}

		public List<Func<List<Item>, bool>> Demands;
		public Func<SymbolSpace, List<Item>, Item> Function;
		public ItemType ReturnType;

		public Invokeable() : base(ItemType.Invokeable)
		{
		}

		private bool EvaluateDemands(List<Item> args)
		{
			foreach (var demand in this.Demands)
			{
				if (!demand(args))
				{
					throw new Exception("Failed demands.");
				}
			}

			return true;
		}

		public Item Invoke(SymbolSpace space, List<Item> items)
		{
			if (!this.EvaluateDemands(items))
			{
				throw new Exception("Failed demands.");
			}

			var result = this.Function(space, items);

			if (result.ItemType != this.ReturnType)
			{
				throw new Exception("Function returned improper type.");
			}

			return result;
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
		public abstract Item Evaluate(SymbolSpace space);

		protected EvaluateableItem(ItemType type) : base(type)
		{
		}

		public Item Unquote()
		{
			this.isQuoted = false;
			return this;
		}

		public Item Quote()
		{
			this.isQuoted = true;
			return this;
		}
	}

	public class SymbolItem : EvaluateableItem
	{
		public string name;

		public SymbolItem(string name) : base(ItemType.Symbol)
		{
			this.name = name;
		}

		public Item Flatten(SymbolSpace space)
		{
			var current = space.Lookup(this.name);

			while (current.ItemType == ItemType.Symbol)
			{
				var symbol = current as SymbolItem;

				if (symbol.isQuoted)
				{
					break;
				}

				current = symbol.Evaluate(space);
			}

			return current;
		}

		public override Item Evaluate(SymbolSpace space)
		{
			if (this.isQuoted)
			{
				this.isQuoted = false;
				return this;
			}

			return this.Flatten(space);
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

		private List<Item> Flatten(SymbolSpace space)
		{
			var resultExp = new List<Item>();

			foreach (var item in this.expression)
			{
				resultExp.Add(
					item is EvaluateableItem
						? (item as EvaluateableItem).Evaluate(space)
						: item);
			}

			return resultExp;
		}

		public override Item Evaluate(SymbolSpace space)
		{
			if (this.isQuoted)
			{
				this.isQuoted = false;
				return this;
			}

			var exp = this.Flatten(space);

			var head = exp.First() as Invokeable;
			var tail = exp.Skip(1).ToList();

			if (head == null)
			{
				throw new Exception();
			}

			return head.Invoke(space, tail);
		}
	}
}
