namespace EnnuiScript.Builtins
{
	using Items;
	using Utils;
	
	public partial class BuiltIns
	{
		private static void SetupEvaluate()
		{
			var invo = new InvokeableItem();
			var fn = new Invokeable
			{
				ReturnType = ItemType.Any,

				Demands = InvokeableUtils.MakeDemands(
					InvokeableUtils.DemandOfAnyType(0,
						ItemType.Symbol,
						ItemType.List),
					args => args.Count == 1),

				Function = (space, args) =>
				{
					var evaluateable = args[0] as EvaluateableItem;

					var item = evaluateable is SymbolItem
						? (EvaluateableItem)(Deref(space, evaluateable as SymbolItem) as EvaluateableItem).Unquote()
						: evaluateable;

					return item.Evaluate(space);
				}
			};

			invo.AddInvokeable(fn);
			globalSpace.Bind(";", invo);
		}
	}
}