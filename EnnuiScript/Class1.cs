namespace EnnuiScript
{
	using System;
	using System.Linq;
	using Items;

	public class Class1
	{
		private Parser parser;
		private SymbolSpace globalSpace;

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

					args => {
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
			this.SetupAdd();
			this.SetupDefn();

			this.ParseAndEvaluate("(=> 'double :Number '(test :Number) '(+ test test))");
			var result = this.ParseAndEvaluate("(double 10)");

			var a = 0;
		}
	}
}