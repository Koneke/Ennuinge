namespace EnnuiScript.Builtins
{
	using System;
	using Items;
	using Utils;
	
	public partial class BuiltIns
	{
		private static class Print
		{
			public static void Setup()
			{
				var invo = new InvokeableItem();
				var fn = new Invokeable
				{
					ReturnType = ItemType.None,

					Demands = InvokeableUtils.MakeDemands(args => args.Count == 1),

					Function = (space, args) =>
					{
						Console.WriteLine(args[0].Print());
						return null;
					}
				};

				invo.AddInvokeable(fn);
				globalSpace.Bind("print", invo);
			}
		}
	}
}