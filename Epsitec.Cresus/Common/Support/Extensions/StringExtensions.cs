//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD & Marc BETTEX, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;

using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Support.Extensions
{
	/// <summary>
	/// The <c>StringExtensions</c> class contains some useful extension method for instances of
	/// <see cref="System.String"/>.
	/// </summary>
	public static class StringExtensions
	{
		public static bool IsNullOrWhiteSpace(this string text)
		{
#if DOTNET35
			return string.IsNullOrEmpty (text)
						|| text.Trim ().Length == 0;
#else
			return string.IsNullOrWhiteSpace (text);
#endif
		}

		public static bool StartsWith(this string text, char character)
		{
			if ((text != null) &&
				(text.Length > 0))
			{
				return text[0] == character;
			}
			else
			{
				return false;
			}
		}

		public static string StripSuffix(this string text, string suffix)
		{
			if (string.IsNullOrEmpty (suffix))
			{
				return text;
			}

			if ((string.IsNullOrEmpty (text)) ||
				(text.EndsWith (suffix) == false))
			{
				throw new System.ArgumentException (string.Format ("Suffix {0} not found in {1}", suffix, text), "suffix");
			}

			return text.Substring (0, text.Length - suffix.Length);
		}

		public static string Replace(this string text, string pattern, string replacement, System.StringComparison comparison)
		{
			if (comparison == System.StringComparison.Ordinal)
			{
				return text.Replace (pattern, replacement);
			}

			int pos = 0;

			while (true)
			{
				int hit = text.IndexOf (pattern, pos, comparison);

				if (hit < 0)
				{
					return text;
				}

				string before = text.Substring (0, hit);
				string after  = text.Substring (hit + pattern.Length);

				text = string.Concat (before, replacement, after);

				pos = hit + replacement.Length;
			}
		}

		/// <summary>
		/// Determines whether the string contains the search text at the specified position.
		/// </summary>
		/// <param name="text">The string.</param>
		/// <param name="search">The search text.</param>
		/// <param name="pos">The position where the search is done.</param>
		/// <returns>
		///   <c>true</c> if the string contains the search text at the specified position; otherwise, <c>false</c>.
		/// </returns>
		public static bool ContainsAtPosition(this string text, string search, int pos)
		{
			if ((pos < 0) ||
				(pos > text.Length))
			{
				return false;
			}

			int i = 0;

			while (i < search.Length)
			{
				if (pos >= text.Length)
				{
					return false;
				}

				if (text[pos++] != search[i++])
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Checks that <paramref name="value"/> is an alpha numeric <see cref="System.String"/>,
		/// i.e. that it is empty or that it contains only lower case letters, upper case letters
		/// or numbers.
		/// </summary>
		/// <param name="value">The <see cref="System.String"/> to check.</param>
		/// <returns><c>true</c> if <paramref name="value"/> is alpha numeric, <c>false</c> if it isn't.</returns>
		/// <exception cref="System.ArgumentNullException">If <paramref name="value"/> is <c>null</c>.</exception>
		public static bool IsAlphaNumeric(this string value)
		{
			if (value == null)
			{
				throw new System.ArgumentNullException ("value");
			}

			return new Regex ("^[a-zA-Z0-9]*$").IsMatch (value);
		}


		/// <summary>
		/// Splits <paramref name="value"/> into an array of <see cref="string"/> that contains all
		/// its substring that are separated by <paramref name="separator"/>.
		/// </summary>
		/// <param name="value">The <see cref="System.String"/> to split.</param>
		/// <param name="separator">The <see cref="System.String"/> used to separate the substrings.</param>
		/// <returns>The separated substrings.</returns>
		/// <exception cref="System.ArgumentNullException">If <paramref name="value"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentNullException">If <paramref name="separator"/> is <c>null</c> or empty.</exception>
		public static string[] Split(this string value, string separator)
		{
			if (value == null)
			{
				throw new System.ArgumentNullException ("value");
			}
			if (string.IsNullOrEmpty (separator))
			{
				throw new System.ArgumentException ("separator");
			}

			return value.Split (new string[] { separator }, System.StringSplitOptions.None);
		}

		/// <summary>
		/// Gets the first token found in the text; the text gets split at every
		/// occurrence of the separator.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <param name="separator">The separator.</param>
		/// <returns>The first token found in the text. If the text is <c>null</c>, returns an empty string.</returns>
		public static string FirstToken(this string text, string separator)
		{
			if ((string.IsNullOrEmpty (text)) ||
				(string.IsNullOrEmpty (separator)))
			{
				return "";
			}
			
			int pos = text.IndexOf (separator);

			if (pos < 0)
			{
				return text;
			}
			else
			{
				return text.Substring (0, pos);
			}
		}

		/// <summary>
		/// Removes the first token in the text; the text gets split at every
		/// occurrence of the separator.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <param name="separator">The separator.</param>
		/// <returns>The text without the first token. If the text is <c>null</c>, returns an empty string.</returns>
		public static string RemoveFirstToken(this string text, string separator)
		{
			if ((string.IsNullOrEmpty (text)) ||
				(string.IsNullOrEmpty (separator)))
			{
				return text ?? "";
			}

			int pos = text.IndexOf (separator);

			if (pos < 0)
			{
				return "";
			}
			else
			{
				return text.Substring (pos + separator.Length);
			}
		}
	}
}
