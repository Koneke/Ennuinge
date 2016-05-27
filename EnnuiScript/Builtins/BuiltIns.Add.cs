namespace EnnuiScript.Builtins
{
	using System.Linq;
	using Items;
	using Utils;
	
	public partial class BuiltIns
	{
		private static void SetupAdd()
		{
			var invo = new InvokeableItem();
			var add = new Invokeable
			{
				ReturnType = ItemType.Number,

				Demands = InvokeableUtils.MakeDemands(
					args => args.All(arg => arg.ItemType == ItemType.Number)
					),

				Function = (space, args) => new ValueItem(
					ItemType.Number,
					args
						.Select(arg => arg as ValueItem)
						.Select(arg => (double)arg.Value)
						.Sum())
			};

			invo.AddInvokeable(add);
			globalSpace.Bind("+", invo);
		}
	}
}