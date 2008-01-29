//	Copyright © 2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types.Converters
{
	public static class TextConverter
	{
		public static string ConvertToSimpleText(string text)
		{
			return TextConverter.ConvertToSimpleText (text, TextConverter.CodeObject);
		}

		public static string ConvertToSimpleText(string text, char imageReplacement)
		{
			return TextConverter.ConvertToSimpleText (text, imageReplacement == 0 ? "" : imageReplacement.ToString ());
		}

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
					buffer.Append (TextConverter.AnalyseEntityChar (text, ref offset));
				}
				else
				{
					buffer.Append (c);
					offset++;
				}
			}

			return buffer.ToString ();
		}

		public static char AnalyseEntityChar(string text, ref int offset)
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
