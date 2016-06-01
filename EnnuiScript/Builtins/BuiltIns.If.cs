namespace EnnuiScript.Builtins
{
	using Items;
	using Utils;
	
	public partial class BuiltIns
	{
		private static class If
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
							ItemType.Something,
							ItemType.Something),
						args => args.Count == 3),

					Function = (space, args) =>
					{
						var condition = args[0] as ValueItem;
						var ifBody = args[1];
						var elseBody = args[2];

						if ((bool)condition.Value)
						{
							return ifBody is ListItem
								? (ifBody as ListItem).Evaluate(space)
								: ifBody;
						}

						return elseBody is ListItem
							? (elseBody as ListItem).Evaluate(space)
							: elseBody;
					}
				};

				invo.AddInvokeable(fn);
				globalSpace.Bind("if", invo);
			}
		}
	}
}