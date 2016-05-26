namespace EnnuiScript
{
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using Items;

	class Parser
	{
		private char GetPairchar(char c)
		{
			switch (c)
			{
				case LeftParen: return RightParen;
				case LeftBracket: return RightBracket;
				case DoubleQuote: return DoubleQuote;
				default: throw new ArgumentException();
			}
		}

		public static bool IsEscaped(int index, string instring)
		{
			return index > 0 && instring[index - 1] == '\\';
		}

		public static bool InString(int index, string instring)
		{
			if (index == 0)
			{
				return false;
			}

			var atIndex = instring[index];
			var isQuote = atIndex == '"' && !IsEscaped(index, instring);

			return instring
				.Substring(0, index)
				.Replace("\\\"", "")
				.Count(c => c == '"') % 2 == 1 && !isQuote;
		}

		public int FindPairIndex(string instring, int startIndex, char c)
		{
			var depth = 1;
			var pairChar = this.GetPairchar(c);

			for (var i = startIndex + 1; i < instring.Length; i++)
			{
				var isEscaped = IsEscaped(i, instring);

				var at = instring[i];
				if (at == pairChar && !isEscaped)
				{
					depth--;
					if (depth == 0)
					{
						return i;
					}
				}
				else if (at == c && !isEscaped)
				{
					depth++;
				}
			}

			throw new ArgumentException();
		}

		public Item ParseValue(string instring)
		{
			// number
			if (char.IsDigit(instring[0]))
			{
				return new ValueItem(ItemType.Number, double.Parse(instring));
			}

			// string
			if (instring[0] == '"')
			{
				return new ValueItem(
					ItemType.String,
					instring.Substring(1, instring.Length - 2));
			}

			// Type
			if (instring[0] == ':')
			{
				var type = (ItemType)Enum.Parse(typeof(ItemType), instring.Substring(1), true);
				return new TypeItem(type);
			}

			// symbol
			var current = instring;
			var quote = false;
			if (instring[0] == '\'')
			{
				current = current.Substring(1);
				quote = true;
			}

			var symbol = new SymbolItem(current);

			if (quote)
			{
				symbol.Quote();
			}

			return symbol;
		}

		private const char LeftParen = '(';
		private const char RightParen = ')';
		private const char LeftBracket = '[';
		private const char RightBracket = '[';
		private const char DoubleQuote = '"';
		private const char SingleQuote = '\'';
		private const char Space = ' ';

		private static string SpaceExpression(string instring)
		{
			var current = instring;
			var preSpacers = new [] { LeftParen, LeftBracket, SingleQuote };

			for (var i = 0; i < current.Length; i++)
			{
				var at = current[i];

				if (i > 0 && preSpacers.Contains(at) && !InString(i, current) && !IsEscaped(i, current))
				{
					if (current[i - 1] != ' ' && current[i - 1] != '\'')
					{
						current = current.Insert(i, " ");
						i+=1;
					}
				}
			}

			return current;
		}

		public ListItem Parse(string instring)
		{
			var current = instring;
			current = SpaceExpression(current);

			// get quote
			var quote = false;
			if (current[0] == SingleQuote)
			{
				quote = true;
				current = current.Substring(1);
			}

			// strip parens
			if (current[0] == LeftParen)
			{
				current = current.Substring(1, current.Length - 2);
			}

			var results = new List<Item>();
			var parens = new List<char> { DoubleQuote, LeftParen, LeftBracket };

			var last = 0;
			var list = false;
			for (var i = 0; i < current.Length; i++)
			{
				var c = current[i];
				if (c == Space)
				{
					var cap = current.Substring(last, i - last);

					if (string.IsNullOrEmpty(cap))
					{
						last = i + 1;
						continue;
					}

					results.Add(list
						? this.Parse(cap)
						: this.ParseValue(cap));

					last = i + 1;
					list = false;
				}
				else if (parens.Contains(c))
				{
					i = this.FindPairIndex(current, i, c);
					list = c != DoubleQuote;
				}
			}

			var final = current.Substring(last);

			if (!string.IsNullOrEmpty(final))
			{
				results.Add(list
					? this.Parse(final)
					: this.ParseValue(final));
			}

			var result = new ListItem(results.ToArray());

			if (quote)
			{
				result.Quote();
			}

			return result;
		}
	}
}