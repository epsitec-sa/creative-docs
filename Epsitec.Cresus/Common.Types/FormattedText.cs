﻿//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>FormattedText</c> structure wraps a <c>string</c> which represents
	/// formatted text.
	/// </summary>
	
	[System.ComponentModel.TypeConverter (typeof (FormattedText.Converter))]
	
	public struct FormattedText : System.IEquatable<FormattedText>, System.IComparable<FormattedText>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FormattedText"/> structure.
		/// </summary>
		/// <param name="text">The formatted text.</param>
		public FormattedText(string text)
		{
			this.text = text;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FormattedText"/> structure.
		/// </summary>
		/// <param name="text">The formatted text.</param>
		public FormattedText(FormattedText text)
		{
			this.text = text.text;
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
			return this.text;
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

		public static FormattedText Parse(string text)
		{
			return new FormattedText (text);
		}

		/// <summary>
		/// Performs an implicit conversion from type <see cref="string"/> to
		/// type <see cref="FormattedText"/>. The source text will be escaped
		/// to produce a compatible formatted text.
		/// </summary>
		/// <param name="text">The source text.</param>
		/// <returns>The result of the conversion.</returns>
		public static implicit operator FormattedText(string text)
		{
			return new FormattedText (FormattedText.Escape (text));
		}

		/// <summary>
		/// Performs an explicit conversion from type <see cref="FormattedText"/>
		/// to type <see cref="string"/>. The source text will be un-escaped
		/// to produce a compatible simple text.
		/// </summary>
		/// <param name="text">The source text.</param>
		/// <returns>The result of the conversion.</returns>
		public static explicit operator string(FormattedText text)
		{
			if (text.IsNullOrEmpty)
			{
				return text.text;
			}
			else
			{
				return FormattedText.Unescape (text.text);
			}
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

		public static readonly FormattedText Null = new FormattedText (null);

		private readonly string text;
	}
}
