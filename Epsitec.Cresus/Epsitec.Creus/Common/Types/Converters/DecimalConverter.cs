//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Pierre ARNAUD

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
	public class DecimalConverter : GenericConverter<decimal, DecimalConverter>
	{
		public DecimalConverter()
			: this (null)
		{
			//	Keep the constructor with no argument -- it is required by the conversion
			//	framework. We cannot collapse both constructors to a single one with a
			//	default culture set to null, since this won't produce the parameterless
			//	constructor.
		}

		public DecimalConverter(System.Globalization.CultureInfo culture)
			: base (culture)
		{
			this.Multiplier = 1;
		}


		/// <summary>
		/// Gets or sets the format used to convert the decimal value to a string.
		/// This should be something like <c>{0:0.00}</c> for instance.
		/// </summary>
		/// <value>
		/// The format.
		/// </value>
		public string							Format
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the multiplier for the value. The value will be multiplied
		/// before it is converted to a string.
		/// </summary>
		/// <value>
		/// The multiplier.
		/// </value>
		public decimal							Multiplier
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the filter. The filter may process the string before it
		/// gets parsed; this can be used to remove unwanted characters, such as
		/// a traling <c>%</c>.
		/// </summary>
		/// <value>
		/// The filter function.
		/// </value>
		public System.Func<string, string>		Filter
		{
			get;
			set;
		}

		
		public override string ConvertToString(decimal value)
		{
			value *= this.Multiplier;

			if (string.IsNullOrEmpty (this.Format))
			{
				return value.ToString (this.GetCurrentCulture ());
			}
			else
			{
				return string.Format (this.GetCurrentCulture (), this.Format, value);
			}
		}

		public override ConversionResult<decimal> ConvertFromString(string text)
		{
			if (this.Filter != null)
			{
				text = this.Filter (text);
			}

			if (text.IsNullOrWhiteSpace ())
			{
				return new ConversionResult<decimal>
				{
					IsNull = true
				};
			}

			decimal result;

			if (decimal.TryParse (text, System.Globalization.NumberStyles.Number, this.GetCurrentCulture (), out result))
			{
				return new ConversionResult<decimal>
				{
					IsNull = false,
					Value = result / this.Multiplier,
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
			if (this.Filter != null)
			{
				text = this.Filter (text);
			}
			
			decimal result;

			if (decimal.TryParse (text, System.Globalization.NumberStyles.Number, this.GetCurrentCulture (), out result))
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
