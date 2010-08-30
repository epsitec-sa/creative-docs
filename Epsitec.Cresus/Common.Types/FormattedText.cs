//	Copyright © 2008-2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>FormattedText</c> structure wraps a <c>string</c> which represents
	/// formatted text.
	/// Cette structure devrait être utilisée à la place de <c>string</c> partout
	/// où le texte peut contenir des tags de mise en page.
	/// Par exemple:
	///    FormattedText t = "<b>bold</b>";
	///    t.ToString () retourne la même chaîne
	///    t.ToSimpleText () retourne "bold"
	/// Autre exemple:
	///    FormattedText t = FormattedText.FromSimpleText ("a -> b");
	///    t.ToString () retourne "a -&gt; b"
	///    t.ToSimpleText () retourne "a -> b"
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

		/// <summary>
		/// Initializes a new instance of the <see cref="FormattedText"/> structure.
		/// </summary>
		/// <param name="formattedText">The formatted text.</param>
		public FormattedText(FormattedText formattedText)
		{
			this.text = formattedText.text;
		}


		public static implicit operator FormattedText(string formattedTextSource)
		{
			//	Permet de faire FormattedText f = "<b>bold</b>".
			return new FormattedText (formattedTextSource);
		}


		/// <summary>
		/// Gets a value indicating whether this instance represents a null
		/// text.
		/// </summary>
		/// <value><c>true</c> if this instance represents a null text; otherwise, <c>false</c>.</value>
		public bool IsNull
		{
			get
			{
				return this.text == null;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this text is null or empty.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this text is null or empty; otherwise, <c>false</c>.
		/// </value>
		public bool IsNullOrEmpty
		{
			get
			{
				return string.IsNullOrEmpty (this.text);
			}
		}

		/// <summary>
		/// Gets a value indicating whether this text is null, empty or consists only of white-space characters.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this text is null, empty or consists only of white-space characters; otherwise, <c>false</c>.
		/// </value>
		public bool IsNullOrWhiteSpace
		{
			get
			{
				return string.IsNullOrWhiteSpace (this.text);
			}
		}


		public FormattedText IfNullOrEmptyReplaceWith(FormattedText defaultText)
		{
			if (this.IsNullOrEmpty)
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

			foreach (var t in x)
			{
				list.Add (new FormattedText (t));
			}

			return list.ToArray ();
		}

		public static FormattedText[] Split(FormattedText text, string separator, System.StringSplitOptions options = System.StringSplitOptions.None)
		{
			if (text.IsNull)
			{
				text = new FormattedText ("");
			}

			return text.Split (separator, options);
		}

		public static FormattedText Join(string separator, params FormattedText[] values)
		{
			List<string> list = new List<string> ();

			foreach (var t in values)
			{
				list.Add (t.ToString ());
			}

			return new FormattedText (string.Join (separator, list));
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
			if (this.IsNullOrEmpty)
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

		/// <summary>
		/// Un-escapes the specified text, i.e. converts formatted encoding
		/// to their equivalent characters.
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

		public static readonly FormattedText Empty = new FormattedText ("");

		public static readonly FormattedText Null = new FormattedText (null);

		public const string HtmlBreak = "<br/>";
		
		private readonly string text;
	}
}
