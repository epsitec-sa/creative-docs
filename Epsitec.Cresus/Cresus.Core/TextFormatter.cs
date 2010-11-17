//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Library;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Core
{
	public static class TextFormatter
	{
		public static FormattedText FormatText(params object[] values)
		{
			var buffer = new System.Text.StringBuilder ();
			
			List<string> items = TextFormatter.ConvertItemsToStrings (values);

			TextFormatter.ProcessTags (items);
			TextFormatter.FormatText (buffer, items);

			return new FormattedText (string.Join (FormattedText.HtmlBreak, buffer.ToString ().Split (new string[] { FormattedText.HtmlBreak }, System.StringSplitOptions.RemoveEmptyEntries)).Replace ("()", ""));
		}


		private static List<string> ConvertItemsToStrings(object[] values)
		{
			var items  = new List<string> ();

			foreach (var value in values.Select (item => TextFormatter.ConvertToText (item)))
			{
				items.Add (value.Replace ("\n", FormattedText.HtmlBreak).Trim ());
			}

			return items;
		}

		private static void FormatText(System.Text.StringBuilder buffer, List<string> items)
		{
			bool emptyItem = true;
			int  count     = items.Count;

			for (int i = 0; i < count; i++)
			{
				bool isLast = (i == count-1);
				string text = items[i];
				string next = isLast ? "" : items[i+1];

				if (text.Length == 0)
				{
					emptyItem = true;
					continue;
				}

				char prefix = text.RemoveTag ().FirstCharacter ();

				if (prefix == Prefix.SkipItemIfPreviousEmpty)
				{
					if (emptyItem)
					{
						continue;
					}

					text = text.Substring (1);
					prefix = text.RemoveTag ().FirstCharacter ();
				}

				char suffix = text.RemoveTag ().LastCharacter ();

				if (suffix == Suffix.SkipItemIfNextEmpty)
				{
					if (next.Length == 0)
					{
						continue;
					}

					text = text.Substring (0, text.Length-1);
					suffix = text.RemoveTag ().LastCharacter ();
				}

				char lastCharacter = buffer.LastCharacter ();

				if ((prefix.IsPunctuationMark ()) &&
					((lastCharacter == prefix) || emptyItem))
				{
					//	Duplicate punctuation mark... or punctuation mark on a still empty line.
				}
				else
				{
					if ((emptyItem == false) &&
						(lastCharacter != '(') &&
						(prefix != '\n') &&
						(prefix.IsPunctuationMark () == false) &&
						(prefix != ')'))
					{
						buffer.Append (" ");
					}

					buffer.Append (text);
				}

				emptyItem = text.EndsWith (FormattedText.HtmlBreak) || string.IsNullOrEmpty (text.RemoveTag ());
			}
		}

		public static class Prefix
		{
			public const char SkipItemIfPreviousEmpty = '~';
		}

		public static class Suffix
		{
			public const char SkipItemIfNextEmpty = '~';
		}


		public const string Mark = "‼[mark]";
		public const string ClearGroupIfEmpty = "‼[clear-group-if-empty]";

		private static void ProcessTags(List<string> items)
		{
			int count = items.Count;
			
			for (int i = 0; i < count; i++)
			{
				string item = items[i];

				if ((item == TextFormatter.ClearGroupIfEmpty) &&
					(i > 0))
				{
					string probe = items[i-1];

					if (string.IsNullOrWhiteSpace (probe))
					{
						TextFormatter.ClearGroup (items, i);
						TextFormatter.ProcessTags (items);
						break;
					}
				}
			}

			items.RemoveAll (x => x.StartsWith ("‼"));
		}

		private static void ClearGroup(List<string> items, int index)
		{
			int startIndex = 0;

			for (int i = index-1; i > 0; i--)
			{
				if (items[i] == TextFormatter.Mark)
				{
					startIndex = i+1;
					break;
				}
			}

			int count = items.Count;
			int endIndex = count-1;

			for (int i = index+1; i < count; i++)
			{
				if (items[i] == TextFormatter.Mark)
				{
					endIndex = i-1;
					break;
				}
			}

			int num = endIndex - startIndex + 1;

			items.RemoveRange (startIndex, num);
		}


		public static string CurrentLanguageId
		{
			get
			{
				UI.CultureSettings culture = UI.Settings.CultureForData;
				return culture.LanguageId;
			}
		}
		
		public static string ConvertToText(object value)
		{
			if (value == null)
			{
				return "";
			}

			string text = value as string;

			if (text != null)
			{
				return text;
			}

			if (value is Date)
			{
				return ((Date) value).ToDateTime ().ToShortDateString ();
			}

			if (value is FormattedText)
			{
				FormattedText formattedText = (FormattedText) value;

				//	Multilingual texts must be "flattened" : only one language may survive the conversion
				//	to text, or else TextLayout would crash, not recognizing <div> tags:
				
				if (MultilingualText.IsMultilingual (formattedText))
                {
					MultilingualText multilingualText = new MultilingualText (formattedText);
					return multilingualText.GetTextOrDefault (TextFormatter.CurrentLanguageId).ToString ();
                }
			}

			return value.ToString ();
		}

		public static FormattedText Join(string separator, IEnumerable<FormattedText> collection)
		{
			return new FormattedText (string.Join (separator, collection.Select (x => TextFormatter.ConvertToText (x)).ToArray ()));
		}
	}
	
	static class StringExtension
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
	
	static class StringBuilderExtension
	{
		public static char LastCharacter(this System.Text.StringBuilder builder)
		{
			string text = builder.ToString ();
			return text.RemoveTag ().LastCharacter ();
		}
	}
}
