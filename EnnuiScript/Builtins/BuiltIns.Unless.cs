namespace EnnuiScript.Builtins
{
	using Items;
	using Utils;
	
	public partial class BuiltIns
	{
		private static class Unless
		{
			public static void Setup()
			{
				var invo = new InvokeableItem();
				var fn = new Invokeable
				{
					ReturnType = ItemType.Any,

					Demands = InvokeableUtils.MakeDemands(
						InvokeableUtils.DemandTypes(
							ItemType.Bool,
							ItemType.Something),
						args => args.Count == 2),

					Function = (space, args) =>
					{
						var condition = args[0] as ValueItem;
						var ifBody = args[1];

						if (!(bool)condition.Value)
						{
							return ifBody is ListItem
								? (ifBody as ListItem).Evaluate(space)
								: ifBody;
						}

						return null;
					}
				};

				invo.AddInvokeable(fn);
				globalSpace.Bind("unless", invo);
			}
		}
	}
}