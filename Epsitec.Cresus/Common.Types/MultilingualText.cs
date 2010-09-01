//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Types
{
	public class MultilingualText
	{
		public MultilingualText()
		{
			this.texts = new Dictionary<string, string> ();
		}

		public MultilingualText(FormattedText text)
			: this ()
		{
			this.texts.AddRange (MultilingualText.GetLanguageTexts (text.ToString ()));
		}


		public FormattedText GetText(string languageId)
		{
			string text;

			if (this.texts.TryGetValue (languageId, out text))
			{
				return new FormattedText (text);
			}
			else
			{
				return this.GetDefaultText ();
			}
		}

		public FormattedText GetDefaultText()
		{
			string text;

			if (this.texts.TryGetValue (MultilingualText.DefaultLanguageId, out text))
			{
				return new FormattedText (text);
			}
			else
			{
				return FormattedText.Empty;
			}
		}

		public void SetText(string languageId, FormattedText text)
		{
			this.texts[languageId] = text.ToString ();
		}


		public FormattedText GetFormattedText()
		{
			if (this.texts.Count == 0)
			{
				return FormattedText.Empty;
			}

			if ((this.texts.Count == 1) &&
				(this.texts.ContainsKey (MultilingualText.DefaultLanguageId)))
			{
				return new FormattedText (this.texts[MultilingualText.DefaultLanguageId]);
			}

			var buffer = new System.Text.StringBuilder ();

			foreach (var item in this.texts.OrderBy (x => x.Key))
			{
				buffer.Append (MultilingualText.GetDivElement (item.Key, item.Value));
			}

			return new FormattedText (buffer.ToString ());
		}


		public static bool IsMultilingual(FormattedText text)
		{
			return MultilingualText.IsMultilingual (text.ToString ());
		}

		public static bool IsMultilingual(string text)
		{
			if (string.IsNullOrEmpty (text))
            {
				return false;
            }

			return text.StartsWith (MultilingualText.DivBeginWithLanguageAttribute);
		}

		private static IEnumerable<string> GetLanguageDivSections(string text)
		{
			int pos = 0;

			while (text.Substring (pos).StartsWith (MultilingualText.DivBeginWithLanguageAttribute))
			{
				//	TODO: handle nesting of <div> sections

				int end = text.IndexOf (MultilingualText.DivEnd, pos);

				if (end < 0)
				{
					break;
				}

				end += MultilingualText.DivEnd.Length;

				string div = text.Substring (pos, end-pos);

				yield return div;

				pos = end;
			}

			if ((pos == 0) ||
				(pos < text.Length))
			{
				yield return MultilingualText.GetDivElement (MultilingualText.DefaultLanguageId, text.Substring (pos));
			}
		}

		private static string GetDivElement(string languageId, string text)
		{
			return string.Concat (MultilingualText.DivBeginWithLanguageAttribute, @"""", languageId, @"""", ">", text, MultilingualText.DivEnd);
		}

		private static IEnumerable<KeyValuePair<string, string>> GetLanguageTexts(string text)
		{
			return MultilingualText.GetLanguageDivSections (text).Select (x => MultilingualText.GetLanguageKeyValuePairFromDivSection (x));
		}

		private static KeyValuePair<string, string> GetLanguageKeyValuePairFromDivSection(string section)
		{
			System.Diagnostics.Debug.Assert (section.StartsWith (MultilingualText.DivBeginWithLanguageAttribute));
			System.Diagnostics.Debug.Assert (section.EndsWith (MultilingualText.DivEnd));

			int quotePosBegin = MultilingualText.DivBeginWithLanguageAttribute.Length + 1;
			int quotePosEnd   = section.IndexOf ('"', quotePosBegin);
			int startDivEnd   = section.IndexOf ('>', quotePosEnd+1) + 1;
			int endDivStart   = section.Length - MultilingualText.DivEnd.Length;

			System.Diagnostics.Debug.Assert (quotePosEnd > 0);
			System.Diagnostics.Debug.Assert (endDivStart >= startDivEnd);

			string languageId = section.Substring (quotePosBegin, quotePosEnd - quotePosBegin);
			string languageText = section.Substring (startDivEnd, endDivStart - startDivEnd);

			return new KeyValuePair<string, string> (languageId, languageText);
		}

		private readonly Dictionary<string, string> texts;
		
		private const string DivBeginWithLanguageAttribute = MultilingualText.DivBegin + MultilingualText.LanguageAttribute;
		private const string LanguageAttribute = "lang=";
		private const string DivBegin = "<div ";
		private const string DivEnd   = "</div>";
		private const string DefaultLanguageId = "*";
	}
}
