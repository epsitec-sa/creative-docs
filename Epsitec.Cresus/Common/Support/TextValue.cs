//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Types;

namespace Epsitec.Common.Support
{
	/// <summary>
	/// The <c>TextValue</c> structure is used to represent a text value of a given
	/// type, provided by an <see cref="AbstractEntity"/>.
	/// </summary>
	public struct TextValue : System.IEquatable<TextValue>
	{
		public TextValue(string simpleText)
			: this (TextValueType.Summary, simpleText)
		{
		}

		public TextValue(TextValueType type, string simpleText)
		{
			this.type = type;
			this.simpleText = simpleText;
			this.formattedText = FormattedText.Empty;
		}

		public TextValue(FormattedText formattedText)
			: this (TextValueType.Summary, formattedText)
		{
		}
		
		public TextValue(TextValueType type, FormattedText formattedText)
		{
			this.type = type;
			this.simpleText = null;
			this.formattedText = TextFormatter.FormatText (formattedText);
		}

		public TextValue(TextValue keyword)
		{
			this.type          = keyword.type;
			this.simpleText    = keyword.simpleText;
			this.formattedText = keyword.formattedText;
		}
		

		public TextValueType					Type
		{
			get
			{
				return this.type;
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
			return this.type.GetHashCode ()
				^ (this.simpleText == null ? 0 : this.simpleText.GetHashCode ())
				^ (this.formattedText.IsNull ? 0 : this.formattedText.GetHashCode ());
		}


		#region IEquatable<Keyword> Members

		public bool Equals(TextValue other)
		{
			if (this.type == other.type)
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

		
		private readonly TextValueType			type;
		private readonly string					simpleText;
		private readonly FormattedText			formattedText;
	}
}
