//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Types;

namespace Epsitec.Common.Support
{
	/// <summary>
	/// The <c>TextValue</c> structure is used to represent a text value of a given
	/// category, provided by an <see cref="AbstractEntity"/>.
	/// </summary>
	public struct TextValue : System.IEquatable<TextValue>
	{
		public TextValue(string simpleText)
			: this (TextValueCategory.Summary, simpleText)
		{
		}

		public TextValue(TextValueCategory category, string simpleText)
		{
			this.category = category;
			this.simpleText = simpleText;
			this.formattedText = FormattedText.Empty;
		}

		public TextValue(FormattedText formattedText)
			: this (TextValueCategory.Summary, formattedText)
		{
		}

		public TextValue(TextValueCategory category, FormattedText formattedText)
		{
			this.category = category;
			this.simpleText = null;
			this.formattedText = TextFormatter.FormatText (formattedText);
		}

		public TextValue(TextValue keyword)
		{
			this.category          = keyword.category;
			this.simpleText    = keyword.simpleText;
			this.formattedText = keyword.formattedText;
		}
		

		public TextValueCategory				Category
		{
			get
			{
				return this.category;
			}
		}

		public string							SimpleText
		{
			get
			{
				if (this.simpleText == null)
				{
					return this.formattedText.ToSimpleText ();
				}
				else
				{
					return this.simpleText;
				}
			}
		}

		public FormattedText					FormattedText
		{
			get
			{
				if (this.formattedText.IsNull)
				{
					return FormattedText.FromSimpleText (this.simpleText);
				}
				else
				{
					return this.formattedText;
				}
			}
		}

		public TextFormat						Format
		{
			get
			{
				var format = TextFormat.None;

				if (this.simpleText != null)
				{
					format |= TextFormat.Simple;
				}
				if (this.formattedText.IsNull == false)
				{
					format |= TextFormat.Formatted;
				}

				return format;
			}
		}


		public override bool Equals(object obj)
		{
			if (obj is TextValue)
			{
				return this.Equals ((TextValue) obj);
			}
			else
			{
				return false;
			}
		}

		public override int GetHashCode()
		{
			return this.category.GetHashCode ()
				^ (this.simpleText == null ? 0 : this.simpleText.GetHashCode ())
				^ (this.formattedText.IsNull ? 0 : this.formattedText.GetHashCode ());
		}


		#region IEquatable<Keyword> Members

		public bool Equals(TextValue other)
		{
			if (this.category == other.category)
			{
				if ((this.simpleText != null) ||
					(other.simpleText != null))
				{
					return this.SimpleText == other.SimpleText;
				}
				else
				{
					return this.FormattedText == other.FormattedText;
				}
			}
			
			return false;
		}

		#endregion


		private readonly TextValueCategory		category;
		private readonly string					simpleText;
		private readonly FormattedText			formattedText;
	}
}
