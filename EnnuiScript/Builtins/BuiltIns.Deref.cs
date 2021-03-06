﻿namespace EnnuiScript.Builtins
{
	using Items;
	using Utils;
	
	public partial class BuiltIns
	{
		private static Item Deref(SymbolSpace space, SymbolItem symbol)
		{
			var reference = space.Lookup(symbol.Name);

			return reference is EvaluateableItem
				? (reference as EvaluateableItem).Quote()
				: reference;
		}

		private static class DerefSymbol
		{
			public static void Setup()
			{
				var invo = new InvokeableItem();
				var fn = new Invokeable
				{
					ReturnType = ItemType.Something,

					Demands = InvokeableUtils.MakeDemands(
						InvokeableUtils.DemandType(0, ItemType.Symbol),
						args => args.Count == 1),

					Function = (space, args) =>
					{
						var symbol = args[0] as SymbolItem;

						var reference = space.Lookup(symbol.Name);

						return reference is EvaluateableItem
							? (reference as EvaluateableItem).Quote()
							: reference;
					}
				};

				invo.AddInvokeable(fn);
				globalSpace.Bind("deref-symbol", invo);
			}
		}
	}
}