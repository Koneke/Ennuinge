namespace EnnuiScript.Builtins
{
	using Items;
	using Utils;

	public partial class BuiltIns
	{
		private static void SetupNegate()
		{
			var invo = new InvokeableItem();
			var fn = new Invokeable
			{
				ReturnType = ItemType.Number,

				Demands = InvokeableUtils.MakeDemands(
					args => args.Count == 1,
					InvokeableUtils.DemandType(0, ItemType.Number)),

				Function = (space, args) =>
				{
					var item = args[0] as ValueItem;

					return new ValueItem(ItemType.Number, -(double)item.Value);
				}
			};

			invo.AddInvokeable(fn);
			globalSpace.Bind("_", invo);
		}
	}
}