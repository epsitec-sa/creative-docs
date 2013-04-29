using Epsitec.Common.Support;

using System.Text;

namespace Epsitec.Common.Text
{
	public static class NameProcessor
	{
		public static string GetAbbreviatedFirstname(string firstname)
		{
			if (string.IsNullOrEmpty (firstname))
			{
				return firstname;
			}

			// For the abbreviation, we will keep only the first part of the name, i.e. everything
			// that comes before the first space in the name.

			var spaceIndex = firstname.IndexOf (' ');
			if (spaceIndex >= 0)
			{
				firstname = firstname.Substring (0, spaceIndex);
			}

			// Now we want to abbreviate each token and keep the token separators. A token is
			// defineed as a sequence of letters (with accents). They are separated by separators,
			// which are everything else (usually '-').

			var result = new StringBuilder ();

			// Here we store the first symbol of the curren token, so that when we reach its end, we
			// can make the abbreviation.
			char? firstTokenSymbol = null;

			// Here we store the fact whether the token has one or more sympbols, so that we know
			// whether or not we must put a dot in the abbreviation.
			bool singleLetterToken = true;

			for(int i = 0; i < firstname.Length; i++) 
			{
				var c = firstname[i];

				bool isPartOfToken = NameProcessor.IsPartOfToken (c);
				bool isLastSymbol = (i == firstname.Length - 1);

				// Process the current sympbol as part of the token if it is.
				if (isPartOfToken)
				{
					if (firstTokenSymbol.HasValue)
					{
						singleLetterToken = false;
					}
					else
					{
						firstTokenSymbol = c;
						singleLetterToken = true;
					}
				}

				// Finish the token if the current symbol is not part of a token or if it is the
				// last symbol in the name.
				if (!isPartOfToken || isLastSymbol)
				{
					if (firstTokenSymbol.HasValue)
					{
						result.Append (firstTokenSymbol.Value);

						if (!singleLetterToken)
						{
							result.Append ('.');
						}

						firstTokenSymbol = null;
					}
				}

				// Adds the current symbol, if it is a token separator.
				if (!isPartOfToken)
				{
					result.Append (c);
				}
			}

			return result.ToString ();
		}

		private static bool IsPartOfToken(char c)
		{
			var converted = StringUtils.RemoveDiacritics (c.ToString ());

			for (int i = 0; i < converted.Length; i++)
			{
				if (!char.IsLetter (converted[i]))
				{
					return false;
				}
			}

			return true;
		}
	}
}
