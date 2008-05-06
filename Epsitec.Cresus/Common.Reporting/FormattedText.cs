//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Reporting
{
	public struct FormattedText : System.IEquatable<FormattedText>
	{
		public FormattedText(string text)
		{
			this.text = text;
		}

		public bool IsEmpty
		{
			get
			{
				return this.text == null;
			}
		}

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

		public static implicit operator FormattedText(string text)
		{
			return new FormattedText (text);
		}

		public static string Escape(string text)
		{
			return Epsitec.Common.Types.Converters.TextConverter.ConvertToTaggedText (text);
		}
		
		public static readonly FormattedText Empty = new FormattedText (null);

		#region IEquatable<FormattedText> Members

		public bool Equals(FormattedText other)
		{
			return this.text == other.text;
		}

		#endregion
		
		private readonly string text;
	}
}
