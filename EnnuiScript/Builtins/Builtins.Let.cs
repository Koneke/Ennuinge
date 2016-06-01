namespace EnnuiScript.Builtins
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Items;
	using Utils;
	
	public partial class BuiltIns
	{
		private static class Let
		{
			private static Invokeable setup()
			{
				Func<List<Item>, List<List<Item>>> getPairs = a =>
					(a[0] as ListItem).Expression
					.GroupingSelect(2);

				Func<List<Item>, Tuple<SymbolItem, Item>> pairUp = pair =>
					new Tuple<SymbolItem, Item>(pair[0] as SymbolItem, pair[1]);

				return new Invokeable
				{
					ReturnType = ItemType.Something,

					Demands = InvokeableUtils.MakeDemands(
						InvokeableUtils.DemandTypes(
							ItemType.List,
							ItemType.List),
						args => getPairs(args).All(pair => pair[0] is SymbolItem),
						args => args.Count == 2),

					Function = (space, args) =>
						(args[1] as ListItem).Evaluate(new SymbolSpace(space, getPairs(args).Select(pairUp)))
				};
			}

			public static void Setup()
			{
				var invo = new InvokeableItem();

				invo.AddInvokeable(setup());

				globalSpace.Bind("let", invo);
			}
		}
	}
}
