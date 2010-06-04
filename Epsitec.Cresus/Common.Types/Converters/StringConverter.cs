//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Types.Converters
{
	/// <summary>
	/// The <c>StringConverter</c> class does a transparent conversion;
	/// it is required so that the <see cref="Marshaler"/> can work with
	/// <c>string</c> values.
	/// </summary>
	public class StringConverter : GenericConverter<string>
	{
		public override string ConvertToString(string text)
		{
			return text;
		}

		public override ConversionResult<string> ConvertFromString(string text)
		{
			if (text == null)
            {
				return new ConversionResult<string>
				{
					IsNull = true,
				};
            }
			else
			{
				return new ConversionResult<string>
				{
					Value = text,
				};
			}
		}

		public override bool CanConvertFromString(string text)
		{
			return true;
		}
	}
}
