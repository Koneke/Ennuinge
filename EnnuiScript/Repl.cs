namespace EnnuiScript
{
	using System;
	using System.Linq;
	using System.Reflection;
	using System.Collections.Generic;
	using Items;
	using Builtins;
	using Utils;

	public class Repl
	{
		private readonly Parser parser;
		private readonly SymbolSpace globalSpace;

		private Item ParseAndEvaluate(string instring)
		{
			return this.parser.Parse(instring).Evaluate(this.globalSpace);
		}

		private ItemType GetReturnTypeFromMethodInfo(MethodInfo mi)
		{
			if (mi.ReturnType == typeof(void))
			{
				return ItemType.None;
			}

			return this.GetTypeFromClrType(mi.ReturnType);
		}

		private ItemType GetTypeFromClrType(Type type)
		{
			if (type == typeof(int) || type == typeof(float) || type == typeof(double))
			{
				return ItemType.Number;
			}

			if (type == typeof(string))
			{
				return ItemType.String;
			}

			if (type == typeof(object))
			{
				return ItemType.Any;
			}

			if (type == typeof(bool))
			{
				return ItemType.Bool;
			}

			throw new ArgumentException();
		}

		private List<Func<List<Item>, bool>> GetParameterDemandsFromMethodInfo(MethodInfo mi)
		{
			var parameters = mi.GetParameters();

			Func<ParameterInfo, int, Func<List<Item>, bool>> getDemandForParameter = (p, i) =>
				InvokeableUtils.DemandType(i, this.GetTypeFromClrType(p.ParameterType));

			var countDemand = InvokeableUtils.DemandCount(parameters.Count());

			var typeDemands = parameters
				.Select((p, i) => getDemandForParameter(p, i));

			var demands = new List<Func<List<Item>, bool>> { countDemand };
			demands.AddRange(typeDemands);

			return InvokeableUtils.MakeDemands(demands.ToArray());
		}

		public void BindMethod(string symbol, MethodInfo mi, object boundObject)
		{
			var invokeable = new InvokeableItem();

			var fn = new Invokeable()
			{
				ReturnType = this.GetReturnTypeFromMethodInfo(mi),

				Demands = this.GetParameterDemandsFromMethodInfo(mi),

				Function = (space, args) =>
				{
					if (!args.All(i => ValueItem.IsValueType(i.ItemType)))
					{
						throw new Exception("Tried calling bound method with non-value arguments.");
					}

					var arguments = args
						.Select(i => i as ValueItem)
						.Select(i => i.Value)
						.ToArray();

					var result = mi.Invoke(boundObject, arguments);

					if (result != null)
					{
						var resultType = this.GetTypeFromClrType(result.GetType());
						var resultItem = new ValueItem(resultType, result);
						return resultItem;
					}

					return null;
				}
			};

			invokeable.AddInvokeable(fn);
			this.globalSpace.Bind(symbol, invokeable);
		}

		public Repl()
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
		}

		public void Main()
		{
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
				for (var i = 0; i < input.Length; i++)
				{
					var c = input[i];

					if (c == '\t')
					{
						var tabLength = 8 - (1 + i + prompt.Length) % 8;
						var tab = tabLength == 0
							? ""
							: string.Join(
								"",
								Enumerable.Repeat(" ", tabLength));

						input = input
							.Remove(i, 1)
							.Insert(i, tab);

						i += tabLength - 1;
					}
				}

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