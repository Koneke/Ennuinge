namespace EnnuiScript.Builtins
{
	using Items;
	using Utils;
	
	public partial class BuiltIns
	{
		private static void SetupQuote()
		{
			var invo = new InvokeableItem();
			var fn = new Invokeable
			{
				ReturnType = ItemType.Symbol,

				Demands = InvokeableUtils.MakeDemands(InvokeableUtils.DemandOfAnyType(0, ItemType.List, ItemType.Symbol)),

				Function = (space, args) =>
				{
					var symbol = args[0] as EvaluateableItem;
					return symbol.Quote();
				}
			};

			invo.AddInvokeable(fn);
			globalSpace.Bind("`", invo);
			globalSpace.Bind("quote", invo);
		}
	}
}