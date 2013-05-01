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

		public static string GetShortenedLastname(string lastname)
		{
			if (string.IsNullOrEmpty (lastname))
			{
				return lastname;
			}

			// We keep only the part of the name before the first space. The only exception is that
			// we also keep the particules that are separated from the main name by a space. So we
			// consider as particules all names that have at most 5 characters that are at the start
			// of the name. Note that we can have more than one particule.
			// I chose 5 as the length of the particule because I looked that on Wikipedia and they
			// have examples of particules in severy languages. The longest ones where Dell', Dall'
			// and Della (in Italian).
			// This simple algorithm will of course produce false positives, because it won't
			// shorten some names that could be. But it will probably never shorten an name that
			// shouldn't.

			// Here we store the size of the shortened version of the name.
			var shortSize = 0;

			// Here we store the size of the current token, that we use to check if it is  particule
			// or not.
			var tokenSize = 0;

			for (int i = 0; i < lastname.Length; i++)
			{
				if (lastname[i] == ' ')
				{
					// This is a separator, so we must terminate the token.

					if (tokenSize <= 5)
					{
						// The current token is a particule. So we reset the particule size.
						tokenSize = 0;
						shortSize++;
					}
					else
					{
						// The current token is not a particule, so we can return.
						break;
					}
				}
				else
				{
					// Regular symbol, so we increment the sizes.
					shortSize++;
					tokenSize++;
				}
			}

			return lastname.Substring (0, shortSize);
		}
	}
}
