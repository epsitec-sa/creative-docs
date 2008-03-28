//	Copyright © 2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
		/// <param name="text">The html text.</param>
		/// <returns>The simple text.</returns>
		public static string ConvertToSimpleText(string text)
		{
			return TextConverter.ConvertToSimpleText (text, TextConverter.CodeObject);
		}

		/// <summary>
		/// Converts the specified HTML text to simple text. The image tag will
		/// be repalced with a specific image replacement character.
		/// </summary>
		/// <param name="text">The html text.</param>
		/// <param name="imageReplacement">The image replacement character.</param>
		/// <returns>The simple text.</returns>
		public static string ConvertToSimpleText(string text, char imageReplacement)
		{
			return TextConverter.ConvertToSimpleText (text, imageReplacement == 0 ? "" : imageReplacement.ToString ());
		}

		/// <summary>
		/// Converts the specified HTML text to simple text. The image tag will
		/// be repalced with a specific image replacement text.
		/// </summary>
		/// <param name="text">The html text.</param>
		/// <param name="imageReplacement">The image replacement text.</param>
		/// <returns>The simple text.</returns>
		public static string ConvertToSimpleText(string text, string imageReplacement)
		{
			if (string.IsNullOrEmpty (text))
			{
				return text;
			}

			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();

			for (int offset = 0; offset < text.Length; )
			{
				char c = text[offset];

				if (c == '<')
				{
					int length = text.IndexOf (">", offset)-offset+1;
					if (length > 0)
					{
						string tag = text.Substring (offset, length);

						offset += length;

						if (tag.IndexOf ("<img ") == 0)
						{
							buffer.Append (imageReplacement);
						}
						if (tag.IndexOf ("<br/>") == 0)
						{
							buffer.Append ("\n");
						}
						if (tag.IndexOf ("<tab/>") == 0 ||
							 tag.IndexOf ("<list")  == 0)
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
		/// Converts the specified simple text to HTML text.
		/// </summary>
		/// <param name="text">The simple text.</param>
		/// <returns>The html text.</returns>
		public static string ConvertToTaggedText(string text)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();

			foreach (char c in text)
			{
				buffer.Append (TextConverter.ConvertToEntity (c));
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
		/// Analyses a character defined as an entity.
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


		public const char						CodeObject		= '\ufffc';
	}
}
