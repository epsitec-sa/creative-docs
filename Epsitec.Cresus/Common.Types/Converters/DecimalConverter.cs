//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Types.Converters
{
	/// <summary>
	/// The <c>DecimalConverter</c> class does a transparent conversion;
	/// it is required so that the <see cref="Marshaler"/> can work with
	/// <c>decimal</c> values.
	/// </summary>
	public class DecimalConverter : GenericConverter<decimal>
	{
		public override string ConvertToString(decimal value)
		{
			return value.ToString ();
		}

		public override ConversionResult<decimal> ConvertFromString(string text)
		{
			if (text == null)
            {
				return new ConversionResult<decimal>
				{
					IsNull = true,
				};
            }
			else
			{
				return new ConversionResult<decimal>
				{
					Value = decimal.Parse (text),  // TODO: si exception ?
				};
			}
		}

		public override bool CanConvertFromString(string text)
		{
			return true;  // TODO ...
		}
	}
}
