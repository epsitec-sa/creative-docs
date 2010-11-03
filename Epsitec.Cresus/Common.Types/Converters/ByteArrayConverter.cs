//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Types.Converters
{
	class ByteArrayConverter : GenericConverter<byte[]>
	{
		public override string ConvertToString(byte[] value)
		{
			if (value == null)
			{
				return "N";
			}
			if (value.Length == 0)
			{
				return "0";
			}
			else
			{
				return "B" + System.Convert.ToBase64String (value);
			}
		}

		public override ConversionResult<byte[]> ConvertFromString(string text)
		{
			if (text.Length > 0)
			{
				char   code  = text[0];
				string value = text.Substring (1);

				switch (code)
				{
					case 'N':
						return new ConversionResult<byte[]> ()
						{
							IsNull = true
						};

					case '0':
						return new ConversionResult<byte[]> ()
						{
							Value = new byte[0]
						};

					case 'B':
						break;

					default:
						return new ConversionResult<byte[]> ()
						{
							IsInvalid = true
						};
				}

				try
				{
					return new ConversionResult<byte[]> ()
					{
						Value = System.Convert.FromBase64String (value)
					};
				}
				catch
				{
				}
			}

			return new ConversionResult<byte[]> ()
			{
				IsInvalid = true
			};
		}

		public override bool CanConvertFromString(string text)
		{
			if (text.Length > 0)
			{
				switch (text[0])
				{
					case '0':
					case 'N':
						return true;
					
					case 'B':
						return this.ConvertFromString (text).IsValid;
					
					default:
						break;
				}
			}

			return false;
		}
	}
}
