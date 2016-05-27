namespace EnnuiScript.Builtins
{
	using System.Linq;
	using Items;
	using Utils;
	
	public partial class BuiltIns
	{
		private static class Add
		{
			private static Invokeable AddNumbers()
			{
				return new Invokeable
				{
					ReturnType = ItemType.Number,

					Demands = InvokeableUtils.MakeDemands(
						args => args.All(arg => arg.ItemType == ItemType.Number)),

					Function = (space, args) => new ValueItem(
						ItemType.Number,
						args
							.Select(arg => arg as ValueItem)
							.Select(arg => (double)arg.Value)
							.Sum())
				};
			}

			private static Invokeable AddStrings()
			{
				return new Invokeable
				{
					ReturnType = ItemType.String,

					Demands = InvokeableUtils.MakeDemands(
						args => args.All(arg => arg.ItemType == ItemType.String)),

					Function = (space, args) => new ValueItem(
						ItemType.String,
						string.Join("", args
							.Select(arg => arg as ValueItem)
							.Select(arg => (string)arg.Value)))
				};
			}

			public static void Setup()
			{
				var invo = new InvokeableItem();

				invo.AddInvokeable(AddNumbers());
				invo.AddInvokeable(AddStrings());

				globalSpace.Bind("+", invo);
			}
		}
	}
}