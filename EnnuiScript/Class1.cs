namespace EnnuiScript
{
	using System;
	using System.Linq;
	using Items;

	public class Class1
	{
		private Parser parser;
		private SymbolSpace globalSpace;

		private void SetupUnquote()
		{
			var fn = new InvokeableItem
			{
				ReturnType = ItemType.Symbol,

				Demands = InvokeableItem.MakeDemands(InvokeableItem.DemandOfAnyType(0, ItemType.List, ItemType.Symbol)),

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
				ReturnType = ItemType.SymbolSpace,

				Demands = InvokeableItem.MakeDemands(
					InvokeableItem.DemandType(0, ItemType.List),
					args => (args[0] as ListItem).Expression.HasEvenLength(),
					args => (args[0] as ListItem).Expression
						.GroupingSelect(2)
						.All(pair => pair[0].ItemType == ItemType.Symbol)),

				Function = (space, args) =>
				{
					var newSpace = new SymbolSpace();
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

				Demands = InvokeableItem.MakeDemands(InvokeableItem.DemandType(0, ItemType.Symbol)),

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

				Demands = InvokeableItem.MakeDemands(
					InvokeableItem.DemandTypes(
						ItemType.SymbolSpace,
						ItemType.Symbol)),

				Function = (space, args) =>
				{
					var sourceSpace = args[0] as SymbolSpaceItem;
					var symbol = args[1] as SymbolItem;

					return new SymbolItem(symbol.Name, sourceSpace.Space).Quote();
				}
			};

			this.globalSpace.Bind("get", fn);
		}

		private void SetupDefn()
		{
			var fn = new InvokeableItem
			{
				ReturnType = ItemType.Invokeable,

				Demands = InvokeableItem.MakeDemands(
					InvokeableItem.DemandTypes(
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

						Demands = InvokeableItem.MakeDemands(InvokeableItem.DemandTypes(argumentTypes)),
						
						// create symbol space,
						// bind fnargs,
						// evaluate body with new symbol space
						Function = (fnspace, fnargs) => {
							var newSpace = new SymbolSpace {Parent = space};

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

				Demands = InvokeableItem.MakeDemands(
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
			this.globalSpace = new SymbolSpace();

			// setup builtins
			this.SetupUnquote();
			this.SetupMakeSpace();
			this.SetupDef();
			this.SetupGet();
			this.SetupAdd();
			this.SetupDefn();

			while (true)
			{
				var input = Console.ReadLine();

				if (input == "quit" || input == "q")
				{
					break;
				}

				var result = this.ParseAndEvaluate(input);
				Console.WriteLine(result);
				Console.WriteLine();
			}
		}
	}
}