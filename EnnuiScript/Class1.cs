namespace EnnuiScript
{
	using System;
	using System.Linq;
	using Items;
	using Utils;

	public class Class1
	{
		private Parser parser;
		private SymbolSpace globalSpace;

		private void SetupBind()
		{
			var fn = new InvokeableItem
			{
				ReturnType = ItemType.Symbol,

				Demands = InvokeableUtils.MakeDemands(
					InvokeableUtils.DemandTypes(ItemType.Symbol, ItemType.Space),
					args => args.Count == 2),

				Function = (space, args) =>
				{
					var symbol = args[0] as SymbolItem;
					var symbolSpace = args[1] as SymbolSpaceItem;

					symbol.BoundSpace = symbolSpace.Space;

					return symbol.Quote();
				}
			};

			this.globalSpace.Bind("bind", fn);
		}

		private Item Deref(SymbolSpace space, SymbolItem symbol)
		{
			var reference = space.Lookup(symbol.Name);

			return reference is EvaluateableItem
				? (reference as EvaluateableItem).Quote()
				: reference;
		}

		private void SetupDeref()
		{
			var fn = new InvokeableItem
			{
				ReturnType = ItemType.Any,

				Demands = InvokeableUtils.MakeDemands(
					InvokeableUtils.DemandType(0, ItemType.Symbol),
					args => args.Count == 1),

				Function = (space, args) =>
				{
					var symbol = args[0] as SymbolItem;

					var reference = space.Lookup(symbol.Name);

					return reference is EvaluateableItem
						? (reference as EvaluateableItem).Quote()
						: reference;
				}
			};

			this.globalSpace.Bind("deref-symbol", fn);
		}

		private void SetupEvaluate()
		{
			var fn = new InvokeableItem
			{
				ReturnType = ItemType.Any,

				Demands = InvokeableUtils.MakeDemands(
					InvokeableUtils.DemandOfAnyType(0,
						ItemType.Symbol,
						ItemType.List),
					args => args.Count == 1),

				Function = (space, args) =>
				{
					var evaluateable = args[0] as EvaluateableItem;

					var item = evaluateable is SymbolItem
						? (EvaluateableItem)(this.Deref(space, evaluateable as SymbolItem) as EvaluateableItem).Unquote()
						: evaluateable;

					return item.Evaluate(space);
				}
			};

			this.globalSpace.Bind(";", fn);
		}

		private void SetupIn()
		{
			var fn = new InvokeableItem
			{
				ReturnType = ItemType.Any,

				Demands = InvokeableUtils.MakeDemands(
					InvokeableUtils.DemandTypes(
						ItemType.Space,
						ItemType.List),
					args => args.Count == 2),

				Function = (space, args) =>
				{
					var symbolSpace = args[0] as SymbolSpaceItem;
					var expression = args[1] as ListItem;

					var spaceParent = symbolSpace.Space.GetParent();
					symbolSpace.Space.SetParent(space);
					space.SetParent(spaceParent);

					var result = expression.Evaluate(symbolSpace.Space);

					symbolSpace.Space.SetParent(spaceParent);

					return result;
				}
			};

			this.globalSpace.Bind("in", fn);
		}

		private void SetupPrint()
		{
			var fn = new InvokeableItem
			{
				ReturnType = ItemType.None,

				Demands = InvokeableUtils.MakeDemands(args => args.Count == 1),

				Function = (space, args) =>
				{
					Console.WriteLine(args[0].Print());
					return null;
				}
			};

			this.globalSpace.Bind("print", fn);
		}

		private void SetupNegate()
		{
			var fn = new InvokeableItem
			{
				ReturnType = ItemType.Number,

				Demands = InvokeableUtils.MakeDemands(
					args => args.Count == 1,
					InvokeableUtils.DemandType(0, ItemType.Number)),

				Function = (space, args) =>
				{
					var item = args[0] as ValueItem;

					return new ValueItem(ItemType.Number, -(double)item.Value);
				}
			};

			this.globalSpace.Bind("_", fn);
		}

		private void SetupQuote()
		{
			var fn = new InvokeableItem
			{
				ReturnType = ItemType.Symbol,

				Demands = InvokeableUtils.MakeDemands(InvokeableUtils.DemandOfAnyType(0, ItemType.List, ItemType.Symbol)),

				Function = (space, args) =>
				{
					var symbol = args[0] as EvaluateableItem;
					return symbol.Quote();
				}
			};

			this.globalSpace.Bind("`", fn);
			this.globalSpace.Bind("quote", fn);
		}

		private void SetupUnquote()
		{
			var fn = new InvokeableItem
			{
				ReturnType = ItemType.Symbol,

				Demands = InvokeableUtils.MakeDemands(InvokeableUtils.DemandOfAnyType(0, ItemType.List, ItemType.Symbol)),

				Function = (space, args) =>
				{
					var symbol = args[0] as EvaluateableItem;
					return symbol.Unquote();
				}
			};

			this.globalSpace.Bind(",", fn);
			this.globalSpace.Bind("unquote", fn);
		}

		private void SetupMakeSpace()
		{
			var fn = new InvokeableItem()
			{
				ReturnType = ItemType.Space,

				Demands = InvokeableUtils.MakeDemands(
					InvokeableUtils.DemandType(0, ItemType.List),
					args => (args[0] as ListItem).Expression.HasEvenLength(),
					args => (args[0] as ListItem).Expression
						.GroupingSelect(2)
						.All(pair => pair[0].ItemType == ItemType.Symbol)),

				Function = (space, args) =>
				{
					var newSpace = new SymbolSpace(space);
					var symbolValuePairs = (args[0] as ListItem).Expression.GroupingSelect(2);

					foreach (var pair in symbolValuePairs)
					{
						var symbol = pair[0] as SymbolItem;
						newSpace.Bind(symbol.Name, pair[1]);
					}

					return new SymbolSpaceItem(newSpace);
				}
			};

			this.globalSpace.Bind("make-space", fn);
		}

		private void SetupDef()
		{
			var fn = new InvokeableItem
			{
				ReturnType = ItemType.Symbol,

				Demands = InvokeableUtils.MakeDemands(InvokeableUtils.DemandType(0, ItemType.Symbol)),

				Function = (space, args) =>
				{
					var symbol = args[0] as SymbolItem;
					var value = args[1];
					var bindingSpace = symbol.BoundSpace ?? space;

					bindingSpace.Bind(symbol.Name, value);

					return symbol;
				}
			};

			this.globalSpace.Bind("def", fn);
		}

		private void SetupGet()
		{
			var fn = new InvokeableItem
			{
				ReturnType = ItemType.Symbol,

				Demands = InvokeableUtils.MakeDemands(
					InvokeableUtils.DemandTypes(
						ItemType.Space,
						ItemType.Symbol)),

				Function = (space, args) =>
				{
					var sourceSpace = args[0] as SymbolSpaceItem;
					var symbol = args[1] as SymbolItem;

					return new SymbolItem(symbol.Name, sourceSpace.Space);
				}
			};

			this.globalSpace.Bind("get", fn);
		}

		private void SetupDefn()
		{
			var fn = new InvokeableItem
			{
				ReturnType = ItemType.Invokeable,

				Demands = InvokeableUtils.MakeDemands(
					InvokeableUtils.DemandTypes(
						ItemType.Symbol,
						ItemType.Type,
						ItemType.List,
						ItemType.List),

					args => (args[2] as ListItem).Expression.Count % 2 == 0,

					args =>
					{
						var argumentList = (args[2] as ListItem).Expression;
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
					var argumentList = (args[2] as ListItem).Expression
						.GroupingSelect(2, xs => new Tuple<Item, Item>(xs[0], xs[1]));
					var body = args[3] as ListItem;

					var argumentTypes = 
						argumentList
							.Select(argument => (argument.Item2 as TypeItem).Type)
							.ToArray();

					var resultingInvokeable = new InvokeableItem()
					{
						ReturnType = returnType.Type,

						Demands = InvokeableUtils.MakeDemands(InvokeableUtils.DemandTypes(argumentTypes)),
						
						// create symbol space,
						// bind fnargs,
						// evaluate body with new symbol space
						Function = (fnspace, fnargs) => {
							var newSpace = new SymbolSpace(space);

							for (var index = 0; index < fnargs.Count; index++)
							{
								newSpace.Bind(
									(argumentList[index].Item1 as SymbolItem).Name,
									fnargs[index]);
							}

							body = body.Unquote() as ListItem;
							return body.Evaluate(newSpace);
						}
					};

					space.Bind(symbol.Name, resultingInvokeable);

					return resultingInvokeable;
				}
			};

			this.globalSpace.Bind("=>", fn);
		}

		private void SetupAdd()
		{
			var add = new InvokeableItem
			{
				ReturnType = ItemType.Number,

				Demands = InvokeableUtils.MakeDemands(
					args => args.All(arg => arg.ItemType == ItemType.Number)
				),

				Function = (space, args) => new ValueItem(
					ItemType.Number,
					args
						.Select(arg => arg as ValueItem)
						.Select(arg => (double)arg.Value)
						.Sum())
			};

			this.globalSpace.Bind("+", add);
		}

		private Item ParseAndEvaluate(string instring)
		{
			return this.parser.Parse(instring).Evaluate(this.globalSpace);
		}

		public void Main()
		{
			this.parser = new Parser();
			this.globalSpace = new SymbolSpace(null);

			// setup builtins
			this.SetupBind();
			this.SetupDeref();
			this.SetupEvaluate();
			this.SetupIn();
			this.SetupPrint();
			this.SetupNegate();
			this.SetupQuote();
			this.SetupUnquote();
			this.SetupMakeSpace();
			this.SetupDef();
			this.SetupGet();
			this.SetupAdd();
			this.SetupDefn();

			var strict = false;
			string accumulator = "";

			while (true)
			{
				var prompt = accumulator == ""
					? ">>>"
					: " > ";

				Console.Write($"{prompt} ");
				var input = Console.ReadLine();

				if (string.IsNullOrEmpty(input))
				{
					continue;
				}

				if (input.Last() == '\\')
				{
					accumulator += input.Substring(0, input.Length - 1) + " ";
					continue;
				}
				else
				{
					accumulator += input;
				}

				if (accumulator == "toggle-strict")
				{
					strict = !strict;
					Console.WriteLine($"Strict is now {strict}");
					Console.WriteLine();
					accumulator = "";
					continue;
				}

				if (accumulator == "quit" || accumulator == "q")
				{
					break;
				}

				try
				{
					if (string.IsNullOrEmpty(accumulator))
					{
						Console.WriteLine();
						continue;
					}

					var result = this.ParseAndEvaluate(accumulator);

					var output = result?.ToString() ?? "None";

					Console.WriteLine(": " + output);
				}
				catch (Exception e) when (!strict)
				{
					Console.WriteLine(e.Message);
				}

				accumulator = "";

				Console.WriteLine();
			}
		}
	}
}