﻿//	Copyright © 2008-2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>FormattedText</c> structure wraps a <c>string</c> which represents
	/// formatted text. It should be used instead of <c>string</c> wherever text
	/// contains formatting.
	/// <example>
	/// <code>
	///    FormattedText t = "&lt;b&gt;bold&lt;/b&gt;";
	///    t.ToString () == "&lt;b&gt;bold&lt;/b&gt;"
	///    t.ToSimpleText () == "bold"
	/// </code>
	/// </example>
	/// <example>
	/// <code>
	///    FormattedText t = FormattedText.FromSimpleText ("a &gt; b");
	///    t.ToString () == "a &amp;gt; b"
	///    t.ToSimpleText () == "a &gt; b"
	///	</code>
	///	</example>
	/// </summary>
	
	[System.ComponentModel.TypeConverter (typeof (FormattedText.Converter))]
	
	public struct FormattedText : System.IEquatable<FormattedText>, System.IComparable<FormattedText>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FormattedText"/> structure.
		/// </summary>
		/// <param name="formattedTextSource">The formatted text source.</param>
		public FormattedText(string formattedTextSource)
		{
			this.text = formattedTextSource;
		}

		public FormattedText(System.Text.StringBuilder formattedTextSource)
		{
			if (formattedTextSource == null)
			{
				this.text = null;
			}
			else
			{
				this.text = formattedTextSource.ToString ();
			}
		}


		/// <summary>
		/// Performs an implicit conversion from <see cref="string"/> to <see cref="FormattedText"/>.
		/// This can be used to write <code>FormattedText t = "&lt;b&gt;bold&lt;b&gt;"</code>.
		/// </summary>
		/// <param name="formattedTextSource">The formatted text source.</param>
		/// <returns>The result of the conversion.</returns>
		public static implicit operator FormattedText(string formattedTextSource)
		{
			return new FormattedText (formattedTextSource);
		}

		public static implicit operator FormattedText(System.Text.StringBuilder formattedTextSource)
		{
			return new FormattedText (formattedTextSource);
		}

		
		/// <summary>
		/// Gets the length of the formatted text. If the text is <c>null</c>, returns <c>0</c>.
		/// </summary>
		public int								Length
		{
			get
			{
				if (this.text == null)
				{
					return 0;
				}
				else
				{
					return this.text.Length;
				}
			}
		}



		/// <summary>
		/// Gets a value indicating whether this instance represents a null
		/// text.
		/// </summary>
		/// <value><c>true</c> if this instance represents a null text; otherwise, <c>false</c>.</value>
		public bool IsNull()
		{
			return this.text == null;
		}

		/// <summary>
		/// Gets a value indicating whether this text is null or empty.
		/// </summary>
		/// <value>
		/// <c>true</c> if this text is null or empty; otherwise, <c>false</c>.
		/// </value>
		public bool IsNullOrEmpty()
		{
			return string.IsNullOrEmpty (this.text);
		}

		/// <summary>
		/// Gets a value indicating whether this text is null, empty or consists only of white-space characters.
		/// </summary>
		/// <value>
		/// <c>true</c> if this text is null, empty or consists only of white-space characters; otherwise, <c>false</c>.
		/// </value>
		public bool IsNullOrWhiteSpace()
		{
			return this.text.IsNullOrWhiteSpace ();
		}

		/// <summary>
		/// Gets a value indicating whether this instance represents a non-null
		/// text.
		/// </summary>
		/// <value><c>false</c> if this instance represents a null text; otherwise, <c>true</c>.</value>
		public bool IsNotNull()
		{
			return this.text != null;
		}

		
		public FormattedText GetValueOrDefault(params FormattedText[] defaultTexts)
		{
			if (this.IsNullOrEmpty ())
			{
				foreach (var text in defaultTexts)
				{
					if (text.IsNullOrEmpty () == false)
					{
						return text;
					}
				}
			}
			
			return this;
		}

		public IEnumerable<FormattedText> Lines
		{
			get
			{
				if (string.IsNullOrEmpty (this.text))
				{
					yield break;
				}

				int pos = 0;
				
				while (pos < this.text.Length)
				{
					int br = this.text.IndexOf (FormattedText.HtmlBreak, pos);

					if (br < 0)
					{
						yield return new FormattedText (this.text.Substring (pos));
						break;
					}

					yield return new FormattedText (this.text.Substring (pos, br-pos));

					pos = br + FormattedText.HtmlBreak.Length;
				}
			}
		}

		public FormattedText AppendLineIfNotNull(FormattedText line = default (FormattedText))
		{
			if (line.IsNullOrEmpty ())
			{
				return this;
			}
			else
			{
				return this.AppendLine (line);
			}
		}

		public FormattedText AppendLine(FormattedText line = default (FormattedText))
		{
			if (this.IsNullOrEmpty ())
			{
				return line;
			}

			return FormattedText.Concat (this, FormattedText.HtmlBreak, line);
		}


		public FormattedText ApplyBold()
		{
			return this.ApplyElement ("<b>", "</b>");
		}

		public FormattedText ApplyItalic()
		{
			return this.ApplyElement ("<i>", "</i>");
		}

		public FormattedText ApplyFontSize(double size)
		{
			return this.ApplyElement (string.Concat ("<font size=\"", InvariantConverter.ToString (size), "\">"), "</font>");
		}

		public FormattedText ApplyFontSizePercent(double size)
		{
			return this.ApplyElement (string.Concat ("<font size=\"", InvariantConverter.ToString (size), "%\">"), "</font>");
		}

		public FormattedText ApplyFontColor(Drawing.Color color)
		{
			return this.ApplyElement (string.Concat ("<font color=\"#", Drawing.Color.ToHexa (color), "\">"), "</font>");
		}

		private FormattedText ApplyElement(string elementBegin, string elementEnd)
		{
			// NOTE It is pointless to add formatting tags around an empty text, so the text is
			// empty, simply return the text without modifications.

			if (this.IsNullOrEmpty ())
			{
				return this;
			}
			
			return FormattedText.Concat (elementBegin, this.text, elementEnd);
		}


		public FormattedText Replace(FormattedText pattern, FormattedText replacement, System.StringComparison comparison = System.StringComparison.Ordinal)
		{
			if (this.IsNullOrEmpty ())
			{
				return this;
			}
			else
			{
				return new FormattedText (this.text.Replace (pattern.text, replacement.text, comparison));
			}
		}


		/// <summary>
		/// If this text is null or empty, return the default text instead.
		/// </summary>
		/// <param name="defaultText">The default text.</param>
		/// <returns>The original text if it is not empty; otherwise, the default text provided by the caller.</returns>
		public FormattedText IfNullOrEmptyReplaceWith(FormattedText defaultText)
		{
			if (this.IsNullOrEmpty ())
			{
				return defaultText;
			}
			else
			{
				return this;
			}
		}

		public FormattedText[] Split(string separator, System.StringSplitOptions options = System.StringSplitOptions.None)
		{
			string[] x = this.ToString ().Split (new string[] { separator }, options);

			List<FormattedText> list = new List<FormattedText> ();

			// TODO: properly handle special characters ! And avoid splitting &gt; if the user provides 'g' as the separator.

			foreach (var t in x)
			{
				list.Add (new FormattedText (t));
			}

			return list.ToArray ();
		}

		public static FormattedText[] Split(FormattedText text, string separator, System.StringSplitOptions options = System.StringSplitOptions.None)
		{
			if (text.IsNull ())
			{
				return new FormattedText[] { FormattedText.Empty };
			}

			return text.Split (separator, options);
		}

		public static FormattedText Join(FormattedText separator, IEnumerable<FormattedText> values)
		{
			return FormattedText.Join (separator, values.ToArray ());
		}

		public static FormattedText Join(FormattedText separator, params FormattedText[] values)
		{
			List<string> list = new List<string> ();

			foreach (var t in values)
			{
				list.Add (t.ToString ());
			}

			return new FormattedText (string.Join (separator.ToString (), list.ToArray ()));
		}

		public static FormattedText Concat(params FormattedText[] values)
		{
			List<string> list = new List<string> ();

			foreach (var t in values)
			{
				list.Add (t.ToString ());
			}

			return new FormattedText (string.Concat (list));
		}

		public static FormattedText Format(FormattedText format, object arg)
		{
			string escapedArg = FormattedText.Escape (arg);
			return new FormattedText (string.Format (format.text, escapedArg));
		}

		public static FormattedText Format(FormattedText format, params object[] args)
		{
			string[] escapedArgs = args.Select (x => FormattedText.Escape (x)).ToArray ();
			return new FormattedText (string.Format (format.text, escapedArgs));
		}

		#region IEquatable<FormattedText> Members

		public bool Equals(FormattedText other)
		{
			return this.text == other.text;
		}

		#endregion

		#region IComparable<FormattedText> Members

		public int CompareTo(FormattedText other)
		{
			return string.CompareOrdinal (this.text, other.text);
		}

		#endregion

		/// <summary>
		/// Returns the raw (encoded) text, as it is stored by this formatted
		/// text object. Use <c>(string)</c> to cast with conversion.
		/// </summary>
		/// <returns>
		/// The raw (encoded) text.
		/// </returns>
		public override string ToString()
		{
			return this.text ?? "";
		}

		public override bool Equals(object obj)
		{
			if (obj is FormattedText)
			{
				return this.Equals ((FormattedText) obj);
			}
			else
			{
				return false;
			}
		}

		public override int GetHashCode()
		{
			if (this.text == null)
			{
				return 0;
			}
			else
			{
				return this.text.GetHashCode ();
			}
		}

		public static bool operator==(FormattedText a, FormattedText b)
		{
			return a.text == b.text;
		}

		public static bool operator!=(FormattedText a, FormattedText b)
		{
			return a.text != b.text;
		}

		/// <summary>
		/// Converts a <see cref="FormattedText"/> to a simple <see cref="string"/>.
		/// The source text will be un-escaped to produce a compatible simple text.
		/// </summary>
		/// <returns>The result of the conversion.</returns>
		public string ToSimpleText()
		{
			if (this.IsNullOrEmpty ())
			{
				return this.text;
			}
			else
			{
				return FormattedText.Unescape (this.text);
			}
		}

		/// <summary>
		/// Converts a text represented as a simple <see cref="string"/> to
		/// a <see cref="FormattedText"/> object. The source text will be properly
		/// escaped to produce a compatible formatted text.
		/// </summary>
		/// <param name="value">The source text.</param>
		/// <returns>The result of the conversion.</returns>
		public static FormattedText FromSimpleText(string value)
		{
			return new FormattedText (FormattedText.Escape (value));
		}

		/// <summary>
		/// Converts the texts represented as a simple <see cref="string"/>s to
		/// a <see cref="FormattedText"/> object. The source texts will be properly
		/// escaped to produce a compatible formatted text.
		/// </summary>
		/// <param name="values">The source texts.</param>
		/// <returns>The result of the conversion.</returns>
		public static FormattedText FromSimpleText(params string[] values)
		{
			return FormattedText.FromSimpleText (string.Join ("", values));
		}

		/// <summary>
		/// Casts the specified value to a string. If the value is a formatted
		/// text, then it will be unwrapped and the corresponding string will
		/// be returned.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <exception cref="System.InvalidCastException">Thrown if the value is neither a string nor a formatted text object.</exception>
		/// <returns>The value cast to a string.</returns>
		public static string CastToString(object value)
		{
			if (value == null)
			{
				return null;
			}
			
			if (value is FormattedText)
			{
				FormattedText text = (FormattedText) value;
				return text.ToString ();
			}
			
			if (value is string)
			{
				return value as string;
			}
			
			throw new System.InvalidCastException (string.Format ("Cannot cast value from {0} to string", value.GetType ().Name));
		}

		public static FormattedText CastToFormattedText(object value)
		{
			if (value == null)
			{
				return new FormattedText ();
			}

			if (value is FormattedText)
			{
				return (FormattedText) value;
			}

			if (value is string)
			{
				return new FormattedText (value as string);
			}

			throw new System.InvalidCastException (string.Format ("Cannot cast value from {0} to string", value.GetType ().Name));
		}

		/// <summary>
		/// Escapes the specified text, i.e. converts special characters found
		/// in the source text to their equivalent formatted encoding.
		/// <example>"a&amp;b" returns "a&amp;amp;b"</example>
		/// <example>"a&amp;amp;b" return "a&amp;amp;amp;b</example>
		/// </summary>
		/// <param name="text">The source text.</param>
		/// <returns>The escaped text.</returns>
		public static string Escape(string text)
		{
			if (string.IsNullOrEmpty (text))
			{
				return text;
			}
			else
			{
				return Converters.TextConverter.ConvertToTaggedText (text);
			}
		}

		public static string Escape(object value)
		{
			if (value is FormattedText)
			{
				return ((FormattedText) value).text;
			}
			else if (value == null)
			{
				return null;
			}
			else
			{
				return FormattedText.Escape (value.ToString ());
			}
		}

		/// <summary>
		/// Un-escapes the specified text, i.e. converts formatted encoding
		/// to their equivalent characters.
		/// <example>"a&amp;amp;b" returns "a&amp;b"</example>
		/// <example>"a&amp;b" throws an exception</example>
		/// <example>"x&lt;b&gt;y&lt;/b&gt;z&lt;/b&gt;" returns "xyz"</example>
		/// </summary>
		/// <param name="text">The escaped text.</param>
		/// <returns>The source text.</returns>
		public static string Unescape(string text)
		{
			if (string.IsNullOrEmpty (text))
			{
				return text;
			}
			else
			{
				return Converters.TextConverter.ConvertToSimpleText (text);
			}
		}
		
		#region Converter Class
		
		public class Converter : AbstractStringConverter
		{
			public override object ParseString(string value, System.Globalization.CultureInfo culture)
			{
				return new FormattedText (value);
			}

			public override string ToString(object value, System.Globalization.CultureInfo culture)
			{
				FormattedText text = (FormattedText) value;
				return text.ToString ();
			}
		}
		
		#endregion


		public static readonly FormattedText	Empty = new FormattedText ("");
		public static readonly FormattedText	Null = new FormattedText ((string)null);
		public const string						HtmlBreak = "<br/>";
		
		private readonly string					text;
	}
}