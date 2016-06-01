﻿namespace EnnuiScript
{
	using System;
	using System.Linq;
	using Items;
	using Builtins;

	public class Repl
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
			this.globalSpace.Bind("*global", new SymbolSpaceItem(this.globalSpace));

			var trueItem = new ValueItem(ItemType.Bool, true);
			var falseItem = new ValueItem(ItemType.Bool, false);

			this.globalSpace.Bind("*true", trueItem);
			this.globalSpace.Bind("*t", trueItem);
			this.globalSpace.Bind("*false", falseItem);
			this.globalSpace.Bind("*f", falseItem);

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

				// 4 spaces, because that's how the windows console works
				// we'll probably have to do a nicer solution later
				input = input.Replace("\t", "    ");

				if (string.IsNullOrEmpty(input))
				{
					continue;
				}

				if (input.Last() == '\\')
				{
					accumulator += input.Substring(0, input.Length - 1) + " ";
					Console.CursorTop -=1;
					Console.CursorLeft = prompt.Length + input.Length;
					Console.Write(' ');
					Console.CursorTop++;
					Console.CursorLeft = 0;
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