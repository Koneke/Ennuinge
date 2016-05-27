namespace EnnuiScript
{
	using System;
	using System.Linq;
	using Items;
	using Builtins;

	public class Class1
	{
		private Parser parser;
		private SymbolSpace globalSpace;
		private Item ParseAndEvaluate(string instring)
		{
			return this.parser.Parse(instring).Evaluate(this.globalSpace);
		}

		public void Main()
		{
			this.parser = new Parser();
			this.globalSpace = new SymbolSpace(null);
			BuiltIns.SetupBuiltins(this.globalSpace);

			var strict = false;
			var accumulator = "";

			while (true)
			{
				var prompt = accumulator == ""
					? ">>>"
					: " > ";

				Console.Write($"{prompt} ");
				var input = Console.ReadLine();

				if (string.IsNullOrEmpty(input))
				{
					continue;
				}

				if (input.Last() == '\\')
				{
					accumulator += input.Substring(0, input.Length - 1) + " ";
					continue;
				}
				else
				{
					accumulator += input;
				}

				if (accumulator == "toggle-strict")
				{
					strict = !strict;
					Console.WriteLine($"Strict is now {strict}");
					Console.WriteLine();
					accumulator = "";
					continue;
				}

				if (accumulator == "quit" || accumulator == "q")
				{
					break;
				}

				try
				{
					if (string.IsNullOrEmpty(accumulator))
					{
						Console.WriteLine();
						continue;
					}

					var result = this.ParseAndEvaluate(accumulator);

					var output = result?.ToString() ?? "None";

					Console.WriteLine(": " + output);
				}
				catch (Exception e) when (!strict)
				{
					Console.WriteLine(e.Message);
				}

				accumulator = "";

				Console.WriteLine();
			}
		}
	}
}