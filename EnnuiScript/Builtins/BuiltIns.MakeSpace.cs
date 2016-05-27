namespace EnnuiScript.Builtins
{
	using System.Linq;
	using Items;
	using Utils;
	
	public partial class BuiltIns
	{
		private static class MakeSpace
		{
			public static void Setup()
			{
				var invo = new InvokeableItem();
				var fn = new Invokeable
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

				invo.AddInvokeable(fn);
				globalSpace.Bind("make-space", invo);
			}
		}
	}
}