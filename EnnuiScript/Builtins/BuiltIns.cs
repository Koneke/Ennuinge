namespace EnnuiScript.Builtins
{
	public partial class BuiltIns
	{
		private static SymbolSpace globalSpace;

		public static void SetupBuiltins(SymbolSpace space)
		{
			globalSpace = space;
			SetupBind();
			SetupDeref();
			SetupEvaluate();
			SetupIn();
			SetupPrint();
			SetupNegate();
			SetupQuote();
			SetupUnquote();
			SetupMakeSpace();
			SetupDef();
			SetupGet();
			SetupAdd();
			SetupDefn();
		}
	}
}