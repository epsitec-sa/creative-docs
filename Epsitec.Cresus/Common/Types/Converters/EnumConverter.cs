//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Types.Converters
{
	public class EnumConverter<T> : GenericConverter<T>
	{
		public EnumConverter(System.Type systemType)
			: base (System.Globalization.CultureInfo.InvariantCulture)
		{
			this.enumType = EnumType.GetDefault (systemType);
		}

		public override string ConvertToString(T value)
		{
			return EnumConverter<T>.ConvertToNumericString (value);
		}

		public override ConversionResult<T> ConvertFromString(string text)
		{
			if (text.IsNullOrWhiteSpace ())
			{
				return new ConversionResult<T> ()
				{
					IsNull = true,
				};
			}
			return new ConversionResult<T> ()
			{
				Value = InvariantConverter.ToEnum<T> (text)
			};
		}

		public override bool CanConvertFromString(string text)
		{
			return this.enumType.IsValidValue (text);
		}


		public static string ConvertToNumericString(T value)
		{
			object boxedValue = value;
			return InvariantConverter.ToString (EnumType.ConvertToInt ((System.Enum) boxedValue));
		}

		private readonly EnumType enumType;
	}
}
