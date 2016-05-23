namespace EnnuiScript
{
	using System;
	using System.Collections.Generic;

	class Parser
	{
		private char GetPairchar(char c)
		{
			switch (c)
			{
				case LeftParen: return RightParen;
				case LeftBracket: return RightBracket;
				case Quote: return Quote;
				default: throw new ArgumentException();
			}
		}

		public int FindPairIndex(string instring, int startIndex, char c)
		{
			var depth = 1;
			var pairChar = this.GetPairchar(c);

			for (var i = startIndex + 1; i < instring.Length; i++)
			{
				var isEscaped = i > 0 && instring[i - 1] == '\\';

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

		public List<string> RespectingSplit(string instring)
		{
			var results = new List<string>();
			var current = instring;
			var parens = new List<char>() { '(', '[' };

			var last = 0;
			var list = false;
			for (var i = 0; i < instring.Length; i++)
			{
				var c = instring[i];
				if (c == Space)
				{
					var cap = instring.Substring(last, i - last);
					results.Add((list?"list: ":"") + cap);
					last = i + 1;
					list = false;
				}
				else if (parens.Contains(c))
				{
					i = this.FindPairIndex(instring, i, c);
					list = true;
				}
			}
			results.Add(instring.Substring(last));
			var a = 0;
			return results;
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
				var type = (ItemType)Enum.Parse(typeof(ItemType), instring.Substring(1));
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
		private const char Quote = '"';
		private const char Space = ' ';

		public ListItem Parse(string instring)
		{
			var current = instring;

			// get quote
			var quote = false;
			if (current[0] == '\'')
			{
				quote = true;
				current = current.Substring(1);
			}

			// strip parens
			if (current[0] == '(')
			{
				current = current.Substring(1, current.Length - 2);
			}

			var results = new List<Item>();
			var parens = new List<char>() { Quote, LeftParen, LeftBracket };

			var last = 0;
			var list = false;
			for (var i = 0; i < current.Length; i++)
			{
				var c = current[i];
				if (c == ' ')
				{
					var cap = current.Substring(last, i - last);

					results.Add(list
						? this.Parse(cap)
						: this.ParseValue(cap));

					last = i + 1;
					list = false;
				}
				else if (parens.Contains(c))
				{
					i = this.FindPairIndex(current, i, c);
					list = c != Quote;
				}
			}

			var final = current.Substring(last);

			results.Add(list
				? this.Parse(final)
				: this.ParseValue(final));

			var result = new ListItem(results.ToArray());

			if (quote)
			{
				result.Quote();
			}

			return result;
		}
	}
}