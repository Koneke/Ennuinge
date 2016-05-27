namespace EnnuiScript.Builtins
{
	using System.Linq;
	using Items;
	
	public partial class BuiltIns
	{
		private static class Equal
		{
			private static Invokeable CreateInvokeable()
			{
				return new Invokeable
				{
					ReturnType = ItemType.Bool,

					Function = (space, args) => new ValueItem(
						ItemType.Bool,
						args.Skip(1).All(arg => arg.Equals(args.First())))
				};
			}

			public static void Setup()
			{
				var invo = new InvokeableItem();
				invo.AddInvokeable(CreateInvokeable());
				globalSpace.Bind("=", invo);
			}
		}
	}
}