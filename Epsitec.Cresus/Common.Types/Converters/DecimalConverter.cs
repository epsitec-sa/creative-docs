//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support.Extensions;

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
			if (text.IsNullOrWhiteSpace ())
			{
				return new ConversionResult<decimal>
				{
					IsNull = true
				};
			}

			decimal result;

			if (decimal.TryParse (text, out result))
			{
				return new ConversionResult<decimal>
				{
					IsNull = false,
					Value = result,
				};
			}
			else
			{
				return new ConversionResult<decimal>
				{
					IsInvalid = true,
				};
			}
		}

		public override bool CanConvertFromString(string text)
		{
			decimal result;

			if (decimal.TryParse (text, out result))
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}
