//	Copyright © 2010-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
		public int								Count
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
		public bool								ContainsLocalizations
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
					if (this.ContainsLanguage (MultilingualText.DefaultTwoLetterISOLanguageToken) == false)
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
		/// <param name="twoLetterISOLanguageName">The language id.</param>
		/// <returns>
		/// 	<c>true</c> if the specified language exists; otherwise, <c>false</c>.
		/// </returns>
		public bool ContainsLanguage(string twoLetterISOLanguageName)
		{
			if (string.IsNullOrEmpty (twoLetterISOLanguageName))
			{
				return false;
			}
			else
			{
				return this.texts.ContainsKey (twoLetterISOLanguageName);
			}
		}

		/// <summary>
		/// Gets the collection of language ids contained in this text.
		/// </summary>
		/// <returns>The collection of language ids.</returns>
		public IEnumerable<string> GetContainedTwoLetterISOLanguageNames()
		{
			return this.texts.Keys.OrderBy (x => x);
		}


		/// <summary>
		/// Gets the formatted text for the specified language, or the
		/// default language text if the language cannot be found.
		/// </summary>
		/// <param name="twoLetterISOLanguageName">The language id.</param>
		/// <returns>The <see cref="FormattedText"/> for the specified language or the default language text if the specified language does not exist.</returns>
		public FormattedText GetTextOrDefault(string twoLetterISOLanguageName)
		{
			string text;

			MultilingualText.FixTwoLetterISOLanguageName (ref twoLetterISOLanguageName);			

			if (this.texts.TryGetValue (twoLetterISOLanguageName, out text))
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
		/// <param name="twoLetterISOLanguageName">The language id.</param>
		/// <returns>The <see cref="FormattedText"/> for the specified language if it exists; otherwise, <c>null</c>.</returns>
		public FormattedText? GetText(string twoLetterISOLanguageName)
		{
			string text;

			MultilingualText.FixTwoLetterISOLanguageName (ref twoLetterISOLanguageName);

			if (this.texts.TryGetValue (twoLetterISOLanguageName, out text))
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

			if (this.texts.TryGetValue (MultilingualText.DefaultTwoLetterISOLanguageToken, out text))
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
		/// <param name="twoLetterISOLanguageName">The language id.</param>
		/// <param name="text">The formatted text.</param>
		public void SetText(string twoLetterISOLanguageName, FormattedText text)
		{
			if (text.IsNull)
			{
				this.ClearText (twoLetterISOLanguageName);
			}
			else
			{
				MultilingualText.FixTwoLetterISOLanguageName (ref twoLetterISOLanguageName);
				this.texts[twoLetterISOLanguageName] = text.ToString ();
			}
		}

		public void SetDefaultText(FormattedText formattedText)
		{
			this.SetText (MultilingualText.DefaultTwoLetterISOLanguageToken, formattedText);
		}

		/// <summary>
		/// Clears the text for the specified language.
		/// </summary>
		/// <param name="twoLetterISOLanguageName">The language id.</param>
		public void ClearText(string twoLetterISOLanguageName)
		{
			MultilingualText.FixTwoLetterISOLanguageName (ref twoLetterISOLanguageName);			
			this.texts.Remove (twoLetterISOLanguageName);
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

			if (this.texts.Count == 1 &&
				this.texts.ContainsKey (MultilingualText.DefaultTwoLetterISOLanguageToken))
			{
				return new FormattedText (this.texts[MultilingualText.DefaultTwoLetterISOLanguageToken]);
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
		/// Retourne la concaténation de plusieurs textes monolingues ou multilingues.
		/// </summary>
		/// <param name="values">The multilingual texts.</param>
		/// <returns>The multilingual (global) text.</returns>
		public static FormattedText Concat(params FormattedText[] values)
		{
			var result = values.First ();

			for (int i = 1; i < values.Count (); i++)
			{
				result = MultilingualText.Concat (result, values[i]);
			}

			return result;
		}

		private static FormattedText Concat(FormattedText t1, FormattedText t2)
		{
			var m1 = new MultilingualText (t1);
			var m2 = new MultilingualText (t2);

			bool b1 = MultilingualText.IsMultilingual (t1);
			bool b2 = MultilingualText.IsMultilingual (t2);

			if (b1 && b2)  // multi + multi ?
			{
				var result = new MultilingualText (t1);

				foreach (var pair2 in m2.texts)
				{
					if (m1.texts.ContainsKey (pair2.Key))
					{
						result.texts[pair2.Key] = string.Concat (m1.texts[pair2.Key], pair2.Value);
					}
					else
					{
						result.texts.Add (pair2.Key, pair2.Value);
					}
				}

				return result.GetGlobalText ();
			}
			else if (b1)  // multi + mono ?
			{
				var result = new MultilingualText ();

				foreach (var pair1 in m1.texts)
				{
					result.texts[pair1.Key] = string.Concat (m1.texts[pair1.Key], t2);
				}

				return result.GetGlobalText ();
			}
			else if (b2)  // mono + multi ?
			{
				var result = new MultilingualText ();

				foreach (var pair2 in m2.texts)
				{
					result.texts[pair2.Key] = string.Concat (t1, m2.texts[pair2.Key]);
				}

				return result.GetGlobalText ();
			}
			else  // mono + mono ?
			{
				return FormattedText.Concat (t1, t2);
			}
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

		public static bool IsDefaultTwoLetterISOLanguageName(string twoLetterISOLanguageName)
		{
			if (string.IsNullOrEmpty (twoLetterISOLanguageName))
			{
				return true;
			}
			else
			{
				return (twoLetterISOLanguageName == MultilingualText.DefaultTwoLetterISOLanguageToken);
			}
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
				if (prefixedText.Length < 3 || prefixedText[2] != ':')
				{
					throw new System.FormatException (string.Format ("Argument '{0}' does not contain valid prefix", prefixedText));
				}

				string twoLetterISOLanguageName = prefixedText.Substring (0, 2);
				string languageText             = prefixedText.Substring (3);

				text.SetText (twoLetterISOLanguageName, languageText);
			}

			return text.GetGlobalText ();
		}


		/// <summary>
		/// Retourne la langue ("*", "de", "en", etc.) en fonction d'un index dans un texte multilingue.
		/// Par exemple:
		/// <div lang="de">Hans</div><div lang="en">John</div>
		///                  ^                        ^
		///             index=17 -> "de"         index=42 -> "en"
		/// </summary>
		/// <param name="text">Multilingual text</param>
		/// <param name="index">Index into text</param>
		/// <returns></returns>
		public static string GetTwoLetterISOLanguageName(FormattedText text, int index)
		{
			if (!text.IsNullOrEmpty)
			{
				string s = text.ToString ();

				if (index >= 0 && index < s.Length)
				{
					int i = s.Substring (0, index).LastIndexOf (MultilingualText.DivBeginWithLanguageAttribute);

					if (i != -1)
					{
						var twoLetter = s.Substring (i+MultilingualText.DivBeginWithLanguageAttribute.Length+1, 2);

						if (twoLetter != "*\"")
						{
							return twoLetter;
						}
					}
				}
			}

			return MultilingualText.DefaultTwoLetterISOLanguageToken;
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


		private static void FixTwoLetterISOLanguageName(ref string twoLetterISOLanguageName)
		{
			if (string.IsNullOrEmpty (twoLetterISOLanguageName))
			{
				twoLetterISOLanguageName = MultilingualText.DefaultTwoLetterISOLanguageToken;
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

			if (pos == 0 || pos < text.Length)
			{
				yield return MultilingualText.GetDivElement (MultilingualText.DefaultTwoLetterISOLanguageToken, text.Substring (pos));
			}
		}

		private static string GetDivElement(string twoLetterISOLanguageName, string text)
		{
			return string.Concat (MultilingualText.DivBeginWithLanguageAttribute, @"""", twoLetterISOLanguageName, @"""", ">", text, MultilingualText.DivEnd);
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

			string twoLetterISOLanguageName = section.Substring (quotePosBegin, quotePosEnd - quotePosBegin);
			string languageText             = section.Substring (startDivEnd, endDivStart - startDivEnd);

			return new KeyValuePair<string, string> (twoLetterISOLanguageName, languageText);
		}


		#region Self test
		static MultilingualText()
		{
			var m1 = new MultilingualText ();
			m1.SetText ("fr", "bonjour");
			m1.SetText ("en", "hello");

			var m1i = new MultilingualText ();
			m1i.SetText ("fr", "bonjour");
			m1i.SetText ("it", "ciao");

			var m2 = new MultilingualText ();
			m2.SetText ("fr", "Jean");
			m2.SetText ("en", "John");

			{
				var t = MultilingualText.Concat ("");
				System.Diagnostics.Debug.Assert (t == "");
			}

			{
				var t = MultilingualText.Concat ("abc");
				System.Diagnostics.Debug.Assert (t == "abc");
			}

			{
				var t = MultilingualText.Concat ("ab", "", "cd");
				System.Diagnostics.Debug.Assert (t == "abcd");
			}

			{
				var t = MultilingualText.Concat ("", "ab", "cd");
				System.Diagnostics.Debug.Assert (t == "abcd");
			}

			{
				var t = MultilingualText.Concat ("ab", "cd", "ef");
				System.Diagnostics.Debug.Assert (t == "abcdef");
			}

			{
				var t = MultilingualText.Concat (m1.GetGlobalText (), "_", m2.GetGlobalText ());
				System.Diagnostics.Debug.Assert (t == "<div lang=\"en\">hello_John</div><div lang=\"fr\">bonjour_Jean</div>");
			}

			{
				var t = MultilingualText.Concat (m1i.GetGlobalText (), "_", m2.GetGlobalText ());
				System.Diagnostics.Debug.Assert (t == "<div lang=\"en\">John</div><div lang=\"fr\">bonjour_Jean</div><div lang=\"it\">ciao_</div>");
			}

			{
				var t = MultilingualText.Concat (m1.GetGlobalText (), m1i.GetGlobalText (), "_", m2.GetGlobalText ());
				System.Diagnostics.Debug.Assert (t == "<div lang=\"en\">hello_John</div><div lang=\"fr\">bonjourbonjour_Jean</div><div lang=\"it\">ciao_</div>");
			}
		}
		#endregion


		public static readonly string DefaultTwoLetterISOLanguageToken		= "*";

		private const string DivBeginWithLanguageAttribute	= MultilingualText.DivBegin + MultilingualText.LanguageAttribute;  // "<div lang="
		private const string LanguageAttribute				= "lang=";
		private const string DivBegin						= "<div ";
		private const string DivEnd							= "</div>";

		private readonly Dictionary<string, string> texts;  // key=TwoLetterISOLanguageName, value=Text
	}
}
