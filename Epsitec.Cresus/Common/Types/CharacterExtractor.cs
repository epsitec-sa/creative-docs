//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>CharacterExtractor</c> class is used to extract one or more chunks of characters
	/// from a source string (e.g. text/number/text).
	/// </summary>
	public sealed class CharacterExtractor
	{
		public CharacterExtractor(string value)
		{
			this.value  = value ?? "";
			this.buffer = new System.Text.StringBuilder (this.value.Length);
			this.index  = 0;
		}


		public bool								IsEmpty
		{
			get
			{
				return this.index >= this.value.Length;
			}
		}

		
		public string GetNextText()
		{
			return this.GetNextValue (c => char.IsDigit (c));
		}

		public int? GetNextDigits()
		{
			var result = this.GetNextValue (c => !char.IsDigit (c));

			if (result.Length == 0)
			{
				return null;
			}
			else
			{
				return InvariantConverter.ConvertFromString<int> (result);
			}
		}

		public string GetNextValue(System.Predicate<char> endPredicate)
		{
			int length = this.value.Length;

			while (this.index < length)
			{
				char c = this.value[this.index];

				if (endPredicate (c))
				{
					break;
				}

				this.buffer.Append (c);
				this.index++;
			}

			string result = this.buffer.ToString ();
			this.buffer.Clear ();
			return result;
		}

		
		private readonly string					value;
		private readonly System.Text.StringBuilder buffer;
		private int								index;
	}
}
