namespace EnnuiScript.Builtins
{
	using Items;
	using Utils;
	
	public partial class BuiltIns
	{
		private static void SetupGet()
		{
			var invo = new InvokeableItem();
			var fn = new Invokeable
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

			invo.AddInvokeable(fn);
			globalSpace.Bind("get", invo);
		}
	}
}