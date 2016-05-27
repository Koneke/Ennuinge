namespace EnnuiScript.Builtins
{
	using Items;
	using Utils;
	
	public partial class BuiltIns
	{
		private static void SetupIn()
		{
			var invo = new InvokeableItem();
			var fn = new Invokeable
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

			invo.AddInvokeable(fn);
			globalSpace.Bind("in", invo);
		}
	}
}