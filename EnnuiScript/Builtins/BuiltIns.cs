namespace EnnuiScript.Builtins
{
	public partial class BuiltIns
	{
		private static SymbolSpace globalSpace;

		public static void SetupBuiltins(SymbolSpace space)
		{
			globalSpace = space;
			Bind.Setup();
			DerefSymbol.Setup();
			Evaluate.Setup();
			In.Setup();
			Print.Setup();
			Negate.Setup();
			Quote.Setup();
			Unquote.Setup();
			MakeSpace.Setup();
			Def.Setup();
			Get.Setup();
			Add.Setup();
			Defn.Setup();
		}
	}
}