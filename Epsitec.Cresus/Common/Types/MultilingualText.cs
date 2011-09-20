//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>MultilingualText</c> class implements transforms from and to global,
	/// multilingual <see cref="FormattedText"/> strings.
	/// </summary>
	public class MultilingualText : System.IEquatable<MultilingualText>
	{
		public MultilingualText()
		{
			this.texts = new Dictionary<string, string> ();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MultilingualText"/> class.
		/// </summary>
		/// <param name="globalText">The global text (containing all languages).</param>
		public MultilingualText(FormattedText globalText)
			: this ()
		{
			this.texts.AddRange (MultilingualText.GetLanguageTexts (globalText.ToString ()));
		}


		/// <summary>
		/// Gets the number of texts stored in this multilingual text instance.
		/// </summary>
		/// <value>The number of texts.</value>
		public int Count
		{
			get
			{
				return this.texts.Count;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this multilingual text instance contains any localizations.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this multilingual text instance contains any localizations; otherwise, <c>false</c>.
		/// </value>
		public bool ContainsLocalizations
		{
			get
			{
				int count = this.Count;
				
				if (count > 1)
                {
					return true;
                }
				else if (count == 1)
                {
					if (this.ContainsLanguage (MultilingualText.DefaultLanguageId) == false)
					{
						return true;
					}
                }

				return false;
			}
		}

		/// <summary>
		/// Determines whether this multilingual text contains the specified language.
		/// </summary>
		/// <param name="languageId">The language id.</param>
		/// <returns>
		/// 	<c>true</c> if the specified language exists; otherwise, <c>false</c>.
		/// </returns>
		public bool ContainsLanguage(string languageId)
		{
			if (string.IsNullOrEmpty (languageId))
			{
				return false;
			}
			else
			{
				return this.texts.ContainsKey (languageId);
			}
		}

		/// <summary>
		/// Gets the collection of language ids contained in this text.
		/// </summary>
		/// <returns>The collection of language ids.</returns>
		public IEnumerable<string> GetContainedLanguageIds()
		{
			return this.texts.Keys.OrderBy (x => x);
		}


		/// <summary>
		/// Gets the formatted text for the specified language, or the
		/// default language text if the language cannot be found.
		/// </summary>
		/// <param name="languageId">The language id.</param>
		/// <returns>The <see cref="FormattedText"/> for the specified language or the default language text if the specified language does not exist.</returns>
		public FormattedText GetTextOrDefault(string languageId)
		{
			string text;

			MultilingualText.FixLanguageId (ref languageId);			

			if (this.texts.TryGetValue (languageId, out text))
			{
				return new FormattedText (text);
			}
			else
			{
				return this.GetDefaultText ();
			}
		}

		/// <summary>
		/// Gets the formatted text for the specified language.
		/// </summary>
		/// <param name="languageId">The language id.</param>
		/// <returns>The <see cref="FormattedText"/> for the specified language if it exists; otherwise, <c>null</c>.</returns>
		public FormattedText? GetText(string languageId)
		{
			string text;

			MultilingualText.FixLanguageId (ref languageId);

			if (this.texts.TryGetValue (languageId, out text))
			{
				return new FormattedText (text);
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Gets the text in the default language.
		/// </summary>
		/// <returns>The <see cref="FormattedText"/> in the default language.</returns>
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

		/// <summary>
		/// Sets the text for the specified language.
		/// </summary>
		/// <param name="languageId">The language id.</param>
		/// <param name="text">The formatted text.</param>
		public void SetText(string languageId, FormattedText text)
		{
			if (text.IsNull)
			{
				this.ClearText (languageId);
			}
			else
			{
				MultilingualText.FixLanguageId (ref languageId);
				this.texts[languageId] = text.ToString ();
			}
		}

		public void SetDefaultText(FormattedText formattedText)
		{
			this.SetText (MultilingualText.DefaultLanguageId, formattedText);
		}

		/// <summary>
		/// Clears the text for the specified language.
		/// </summary>
		/// <param name="languageId">The language id.</param>
		public void ClearText(string languageId)
		{
			MultilingualText.FixLanguageId (ref languageId);			
			this.texts.Remove (languageId);
		}


		/// <summary>
		/// Gets the global text containing all languages.
		/// </summary>
		/// <returns>The <see cref="FormattedText"/> containing all texts embedded in <c>&lt;div&gt;</c> elements.</returns>
		public FormattedText GetGlobalText()
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

		public override string ToString()
		{
			return this.GetGlobalText ().ToString ();
		}

		public override bool Equals(object obj)
		{
			return this.Equals (obj as MultilingualText);
		}

		public override int GetHashCode()
		{
			return this.texts.GetHashCode ();
		}


		/// <summary>
		/// Determines whether the specified text is multilingual.
		/// </summary>
		/// <param name="text">The formatted text.</param>
		/// <returns>
		/// 	<c>true</c> if the specified text is multilingual; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsMultilingual(FormattedText text)
		{
			return MultilingualText.IsMultilingual (text.ToString ());
		}

		public static bool IsDefaultLanguageId(string languageId)
		{
			if (string.IsNullOrEmpty (languageId))
            {
				return true;
            }
			return (languageId == MultilingualText.DefaultLanguageId);
		}

		/// <summary>
		/// Creates a multilingual <see cref="FormattedText"/>, based on a collection of prefixed
		/// texts, such as <c>"fr:Texte"</c>, <c>"de:Text"</c>, etc.
		/// </summary>
		/// <param name="prefixedTexts">The prefixed texts.</param>
		/// <returns>The multilingual (global) text.</returns>
		public static FormattedText CreateText(params string[] prefixedTexts)
		{
			var text = new MultilingualText ();

			foreach (var prefixedText in prefixedTexts)
			{
				if ((prefixedText.Length < 3) ||
					(prefixedText[2] != ':'))
				{
					throw new System.FormatException (string.Format ("Argument '{0}' does not contain valid prefix", prefixedText));
				}

				string languageId   = prefixedText.Substring (0, 2);
				string languageText = prefixedText.Substring (3);

				text.SetText (languageId, languageText);
			}

			return text.GetGlobalText ();
		}

		#region IEquatable<MultilingualText> Members

		public bool Equals(MultilingualText other)
		{
			if (other == null)
			{
				return false;
			}
			else
			{
				return this.GetGlobalText () == other.GetGlobalText ();
			}
		}

		#endregion


		private static void FixLanguageId(ref string languageId)
		{
			if (string.IsNullOrEmpty (languageId))
			{
				languageId = MultilingualText.DefaultLanguageId;
			}
		}
		
		private static bool IsMultilingual(string text)
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


		public static readonly string DefaultLanguageId = "*";
		
		private const string DivBeginWithLanguageAttribute = MultilingualText.DivBegin + MultilingualText.LanguageAttribute;
		private const string LanguageAttribute = "lang=";
		private const string DivBegin = "<div ";
		private const string DivEnd   = "</div>";

		private readonly Dictionary<string, string> texts;
	}
}
