//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>FormattedText</c> structure wraps a <c>string</c> which represents
	/// formatted text.
	/// </summary>
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
		/// Escapes the specified text, i.e. converts special characters found
		/// in the source text to their equivalent formatted encoding.
		/// </summary>
		/// <param name="text">The source text.</param>
		/// <returns>The escaped text.</returns>
		public static string Escape(string text)
		{
			return Converters.TextConverter.ConvertToTaggedText (text);
		}
		
		public static readonly FormattedText Null = new FormattedText (null);

		private readonly string text;
	}
}
