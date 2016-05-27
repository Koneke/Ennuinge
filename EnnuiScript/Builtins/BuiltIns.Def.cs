namespace EnnuiScript.Builtins
{
	using Items;
	using Utils;
	
	public partial class BuiltIns
	{
		private static class Def
		{
			public static void Setup()
			{
				var invo = new InvokeableItem();
				var fn = new Invokeable
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

				invo.AddInvokeable(fn);
				globalSpace.Bind("def", invo);
			}
		}
	}
}