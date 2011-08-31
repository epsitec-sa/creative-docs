//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

namespace Epsitec.Cresus.Core.Library.Internal
{
	static internal class StringExtensions
	{
		public static char LastCharacter(this string text)
		{
			int n = text.Length - 1;
			return n < 0 ? (char) 0 : text[n];
		}

		public static char FirstCharacter(this string text)
		{
			int n = text.Length;
			return n < 1 ? (char) 0 : text[0];
		}

		public static string RemoveTag(this string text)
		{
			return FormattedText.Unescape (text);
		}

		public static bool IsPunctuationMark(this char c)
		{
			// Exclut le caractère '/', pour permettre de numéroter une facture "1000 / 45 / bg" (par exemple).

			switch (c)
			{
				case ',':
				case ';':
				case '.':
				case ':':
				case '!':
				case '?':
					return true;

				default:
					return false;
			}
		}
	}
}
