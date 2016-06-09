namespace Ennuinge
{
	using System.Linq;
	using System.Reflection;
	using EnnuiScript;

	class Program
	{
		// notice, we always use doubles, since ennuiscript/ykr only uses doubles internally
		// for everything (i.e. no floats, no ints).
		// it's important (for now atleast) that your exposed functions does this as well.
		static double TestFunc(double a, double b)
		{
			return a + b;
		}

		static void Main(string[] args)
		{
			var test = new Repl();

			// not the most beautiful way of exposing something to the scripting,
			// but having access to the methodinfo on the other end is neato.
			// we'll think of something.
			var mi = typeof(Program)
				.GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
				.First(m => m.Name == "TestFunc");

			test.BindMethod("test-func", mi, null);

			test.Main();
		}
	}
}