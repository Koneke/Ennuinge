namespace EnnuiScript.Builtins
{
	using Items;
	using Utils;
	
	public partial class BuiltIns
	{
		private static class Bind
		{
			public static void Setup()
			{
				var invo = new InvokeableItem();
				var fn = new Invokeable
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

				invo.AddInvokeable(fn);
				globalSpace.Bind("bind", invo);
			}
		}
	}
}