namespace EnnuiScript.Builtins
{
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using Items;
	using Utils;
	
	public partial class BuiltIns
	{
		private static void SetupDefn()
		{
			const int typeIndex = 0;
			const int paramsIndex = 1;
			const int bodyIndex = 2;

			Func<Item, List<Tuple<Item, Item>>> groupArguments = l =>
				(l as ListItem).Expression
					.GroupingSelect(2, xs => new Tuple<Item, Item>(xs[0], xs[1]));

			Func<List<Item>, ListItem> getParams = args => (args[paramsIndex] as ListItem);

			// now to the meat of things

			var invo = new InvokeableItem();

			var fn = new Invokeable
			{
				ReturnType = ItemType.Invokeable,

				Demands = InvokeableUtils.MakeDemands(
					InvokeableUtils.DemandTypes(ItemType.Type, ItemType.List, ItemType.List),
					args => args.Count == 3,
					args => getParams(args).Expression.HasEvenLength(),
					args => groupArguments(getParams(args)).All(pair =>
						pair.Item1.ItemType == ItemType.Symbol &&
						pair.Item2.ItemType == ItemType.Type)
					),

				Function = (space, args) =>
				{
					var returnType = args[typeIndex] as TypeItem;
					var argumentList = groupArguments(args[paramsIndex]);
					var body = args[bodyIndex] as ListItem;

					var argumentTypes = 
						argumentList
							.Select(argument => (argument.Item2 as TypeItem).Type)
							.ToArray();

					var resultingInvokeable = new InvokeableItem();

					resultingInvokeable.AddInvokeable(new Invokeable()
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
					});

					return resultingInvokeable;
				}
			};

			invo.AddInvokeable(fn);

			globalSpace.Bind("=>", invo);
		}

	}
}