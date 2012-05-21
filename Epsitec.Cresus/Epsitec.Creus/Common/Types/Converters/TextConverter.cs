//	Copyright © 2008-2012, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types.Converters
{
	/// <summary>
	/// The <c>TextConverter</c> class provides simple conversion methods to
	/// translate HTML like code to simple (raw) text.
	/// </summary>
	public static class TextConverter
	{
		/// <summary>
		/// Converts the specified HTML text to simple text.
		/// </summary>
		/// <param name="text">The HTML text.</param>
		/// <returns>The simple text.</returns>
		public static string ConvertToSimpleText(string text)
		{
			return TextConverter.ConvertToSimpleText (text, TextConverter.CodeObject);
		}

		/// <summary>
		/// Converts the specified HTML text to simple text. The image tag will
		/// be replaced with a specific image replacement character.
		/// </summary>
		/// <param name="text">The HTML text.</param>
		/// <param name="imageReplacement">The image replacement character.</param>
		/// <returns>The simple text.</returns>
		public static string ConvertToSimpleText(string text, char imageReplacement)
		{
			return TextConverter.ConvertToSimpleText (text, imageReplacement == 0 ? "" : imageReplacement.ToString ());
		}

		/// <summary>
		/// Converts the specified HTML text to simple text. The image tag will
		/// be replaced with a specific image replacement text.
		/// </summary>
		/// <param name="text">The HTML text.</param>
		/// <param name="imageReplacement">The image replacement text.</param>
		/// <returns>The simple text.</returns>
		public static string ConvertToSimpleText(string text, string imageReplacement)
		{
			if (string.IsNullOrEmpty (text))
			{
				return text;
			}

			int pos = text.IndexOfAny (TextConverter.StartElementOrEntityCharacters);

			if (pos < 0)
			{
				return text;
			}

			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();

			if (pos > 0)
			{
				buffer.Append (text.Substring (0, pos));
			}

			for (int offset = pos; offset < text.Length; )
			{
				char c = text[offset];

				if (c == '<')
				{
					int length = text.IndexOf (">", offset)-offset+1;
					if (length > 0)
					{
						string tag = text.Substring (offset, length);

						offset += length;

						if (tag.StartsWith ("<img "))
						{
							buffer.Append (imageReplacement);
						}
						if (tag.StartsWith ("<param "))
						{
							buffer.Append (imageReplacement);
						}
						if (tag.StartsWith ("<br/>"))
						{
							buffer.Append ("\n");
						}
						if (tag.StartsWith ("<tab/>") ||
							 tag.StartsWith ("<list"))
						{
							buffer.Append ("\t");
						}
					}
				}
				else if (c == '&')
				{
					buffer.Append (TextConverter.AnalyzeEntityChar (text, ref offset));
				}
				else
				{
					buffer.Append (c);
					offset++;
				}
			}

			return buffer.ToString ();
		}


		/// <summary>
		/// Gets the length of the equivalent simple text.
		/// </summary>
		/// <param name="text">The HTML text.</param>
		/// <returns>The length of the equivalent simple text.</returns>
		public static int GetSimpleTextLength(string text)
		{
			if (string.IsNullOrEmpty (text))
			{
				return 0;
			}
			else
			{
				string simple = TextConverter.ConvertToSimpleText (text);
				return simple.Length;
			}
		}


		/// <summary>
		/// Converts the specified simple text to HTML text.
		/// </summary>
		/// <param name="text">The simple text.</param>
		/// <returns>The HTML text.</returns>
		public static string ConvertToTaggedText(string text)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();

			if (text != null)
			{
				foreach (char c in text)
				{
					buffer.Append (TextConverter.ConvertToEntity (c));
				}
			}

			return buffer.ToString ();
		}

		/// <summary>
		/// Converts a character to an entity, if needed.
		/// </summary>
		/// <param name="c">The character.</param>
		/// <returns>The entity for the specified character.</returns>
		public static string ConvertToEntity(char c)
		{
			switch (c)
			{
				case '&':	return "&amp;";
				case '<':	return "&lt;";
				case '>':	return "&gt;";
				case '\"':	return "&quot;";
				case '\'':	return "&apos;";
				case '\n':	return "<br/>";
				case '\t':	return "<tab/>";
				case '\r':	return "";
			}

			return new string (c, 1);
		}


		/// <summary>
		/// Analyzes a character defined as an entity.
		/// </summary>
		/// <param name="text">The text to analyze.</param>
		/// <param name="offset">The offset, which will be updated to point after the
		/// entity or character.</param>
		/// <returns>The character.</returns>
		public static char AnalyzeEntityChar(string text, ref int offset)
		{
			//	Retourne le caractère à un offset quelconque, en interprétant les
			//	commandes &...;
			
			if (text[offset] == '&')
			{
				int length = text.IndexOf (";", offset)-offset+1;

				if (length < 3)
				{
					throw new System.FormatException (string.Format ("Invalid entity found (too short)."));
				}

				char code;
				string entity = text.Substring (offset, length);

				switch (entity)
				{
					case "&lt;":
						code = '<';
						break;
					case "&gt;":
						code = '>';
						break;
					case "&amp;":
						code = '&';
						break;
					case "&quot;":
						code = '"';
						break;
					case "&apos;":
						code = '\'';
						break;

					default:
						if (entity.StartsWith ("&#"))
						{
							entity = entity.Substring (2, entity.Length-3);
							code   = (char) System.Int32.Parse (entity, System.Globalization.CultureInfo.InvariantCulture);
						}
						else
						{
							throw new System.FormatException (string.Format ("Invalid entity {0} found.", entity));
						}
						break;
				}

				offset += length;
				return code;
			}

			return text[offset++];
		}

		
		public static string StripAccents(string text)
		{
			return TextConverter.ConvertString (text, TextConverter.Tables.StripAccents);
		}

		public static string ConvertToLowerAndStripAccents(string text)
		{
			return TextConverter.ConvertString (text, TextConverter.Tables.StripAccentsToLower);
		}

		public static string ConvertToUpperAndStripAccents(string text)
		{
			return TextConverter.ConvertString (text, TextConverter.Tables.StripAccentsToUpper);
		}
		
		public static string ConvertToLower(string text)
		{
			return TextConverter.ConvertString (text, TextConverter.Tables.ToLower);
		}

		public static string ConvertToUpper(string text)
		{
			return TextConverter.ConvertString (text, TextConverter.Tables.ToUpper);
		}

		
		private static string ConvertString(string text, char[] conversionTable)
		{
			if (string.IsNullOrEmpty (text))
			{
				return text;
			}

			char[] chars = null;

			for (int i = 0; i < text.Length; i++)
			{
				char c1 = text[i];
				char c2 = c1 < conversionTable.Length ? conversionTable[c1] : c1;

				if (c2 != c1)
				{
					//	Produce a character table in which we can replace the individual
					//	characters only if there is a difference:

					if (chars == null)
					{
						chars = text.ToCharArray ();
					}

					chars[i] = c2;
				}
			}

			//	If there is a character array, this means that we did an in-place
			//	replacement and that the output string will be different from the
			//	input:

			return chars == null ? text : new string (chars);
		}


		static TextConverter()
		{
			TextConverter.InitializeConversionTables ();
		}
		
		private static void InitializeConversionTables()
		{
			for (int i = 0; i < TextConverter.ConversionCharacterCount; i++)
			{
				char c = TextConverter.StripAccent ((char) i);

				TextConverter.Tables.StripAccents[i]        = c;
				TextConverter.Tables.StripAccentsToLower[i] = char.ToLower (c);
				TextConverter.Tables.StripAccentsToUpper[i] = char.ToUpper (c);
				TextConverter.Tables.ToLower[i]             = char.ToLower ((char) i);
				TextConverter.Tables.ToUpper[i]             = char.ToUpper ((char) i);
			}
		}
		
		private static char StripAccent(char c)
		{
			//	TODO: compléter la liste pour qu'elle soit complète

			switch (c)
			{
				case 'â':	return 'a';
				case 'ä':	return 'a';
				case 'á':	return 'a';
				case 'à':	return 'a';
				case 'å':	return 'a';
				case 'ã':	return 'a';
				case 'ç':	return 'c';
				case 'ê':	return 'e';
				case 'ë':	return 'e';
				case 'é':	return 'e';
				case 'è':	return 'e';
				case 'î':	return 'i';
				case 'ï':	return 'i';
				case 'í':	return 'i';
				case 'ì':	return 'i';
				case 'ñ':	return 'n';
				case 'ô':	return 'o';
				case 'ö':	return 'o';
				case 'ó':	return 'o';
				case 'ò':	return 'o';
				case 'ø':	return 'o';
				case 'õ':	return 'o';
				case 'û':	return 'u';
				case 'ü':	return 'u';
				case 'ú':	return 'u';
				case 'ù':	return 'u';
				case 'ÿ':	return 'y';
				
				case 'Â':	return 'A';
				case 'Å':	return 'A';
				case 'Ä':	return 'A';
				case 'Á':	return 'A';
				case 'À':	return 'A';
				case 'Ã':	return 'A';
				case 'Ç':	return 'C';
				case 'Ê':	return 'E';
				case 'Ë':	return 'E';
				case 'É':	return 'E';
				case 'È':	return 'E';
				case 'Î':	return 'I';
				case 'Ï':	return 'I';
				case 'Í':	return 'I';
				case 'Ì':	return 'I';
				case 'Ñ':	return 'N';
				case 'Ô':	return 'O';
				case 'Ö':	return 'O';
				case 'Ó':	return 'O';
				case 'Ò':	return 'O';
				case 'Ø':	return 'O';
				case 'Õ':	return 'O';
				case 'Û':	return 'U';
				case 'Ü':	return 'U';
				case 'Ú':	return 'U';
				case 'Ù':	return 'U';
				case 'Ÿ':	return 'Y';

				case '°':	return 'o';

				default:	return c;
			}
		}

		
		private const int ConversionCharacterCount = 512;

		private static class Tables
		{
			public static char[] StripAccents        = new char[TextConverter.ConversionCharacterCount];
			public static char[] StripAccentsToLower = new char[TextConverter.ConversionCharacterCount];
			public static char[] StripAccentsToUpper = new char[TextConverter.ConversionCharacterCount];
			public static char[] ToLower             = new char[TextConverter.ConversionCharacterCount];
			public static char[] ToUpper             = new char[TextConverter.ConversionCharacterCount];
		}

		public static readonly char[]			StartElementOrEntityCharacters = new char[] { '<', '&' };

		public const char						CodeObject		= '\ufffc';
	}
}
