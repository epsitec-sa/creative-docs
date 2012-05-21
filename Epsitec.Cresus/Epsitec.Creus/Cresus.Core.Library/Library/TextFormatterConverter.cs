//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Text;
using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Library
{
	/// <summary>
	/// The <c>TextFormatterConverter</c> class is used to format an object so that it can be
	/// displayed to the user, either using the object's own <see cref="ITextFormatter"/>
	/// interface, or using the associated <see cref="ITextFormatterConverter"/>, if any.
	/// </summary>
	public static class TextFormatterConverter
	{
		public static FormattedText ToFormattedText(object value, System.Globalization.CultureInfo culture = null, TextFormatterDetailLevel detailLevel = TextFormatterDetailLevel.Default)
		{
			if (value == null)
			{
				return FormattedText.Empty;
			}
			else
			{
				return TextFormatterConverter.ToFormattedText (value, value.GetType (), culture ?? System.Globalization.CultureInfo.CurrentCulture, detailLevel);
			}
		}
		
		private static FormattedText ToFormattedText(object value, System.Type type, System.Globalization.CultureInfo culture, TextFormatterDetailLevel detailLevel)
		{
			System.Diagnostics.Debug.Assert (value != null);
			System.Diagnostics.Debug.Assert (type != null);
			System.Diagnostics.Debug.Assert (culture != null);

			if (type == typeof (FormattedText))
			{
				return (FormattedText) value;
			}
			if (type == typeof (string))
			{
				return new FormattedText ((string) value);
			}

			var autoConvert = value as ITextFormatter;

			if (autoConvert != null)
			{
				return autoConvert.ToFormattedText (culture, detailLevel);
			}

			var prettyPrinter = Resolvers.TextFormatterConverterResolver.Resolve (type);

			if (prettyPrinter == null)
			{
				return TextFormatterConverter.PrettyPrintUsingStringFormat (value, type, culture);
			}
			else
			{
				return prettyPrinter.ToFormattedText (value, culture, detailLevel);
			}
		}
		
		/// <summary>
		/// Replaces the minus dash <c>"-"</c> with a real minus sign.
		/// </summary>
		/// <param name="value">The text representing a numeric value.</param>
		/// <returns>The text with the proper minus sign, if any.</returns>
		public static string ReplaceMinusSign(string value)
		{
			if (string.IsNullOrEmpty (value))
			{
				return value;
			}

			if (value[0] == '-')
			{
				return string.Concat (TextFormatterConverter.MinusSign, value.Substring (1));
			}
			else
			{
				return value;
			}
		}

		private static FormattedText PrettyPrintUsingStringFormat(object value, System.Type type, System.Globalization.CultureInfo culture)
		{
			var simple = string.Format (culture, "{0}", value);

			if (type.IsValueType)
			{
				//	Post-process signed numeric types; we only check if we know that the type is
				//	a value type, so we can avoid a call to GetTypeCode :

				switch (System.Type.GetTypeCode (type))
				{
					case System.TypeCode.Decimal:
					case System.TypeCode.Double:
					case System.TypeCode.Int16:
					case System.TypeCode.Int32:
					case System.TypeCode.Int64:
					case System.TypeCode.Single:
					case System.TypeCode.SByte:
						simple = TextFormatterConverter.ReplaceMinusSign (simple);
						break;
				}
			}

			return FormattedText.FromSimpleText (simple);
		}
		
		public static readonly string			MinusSign = Unicode.ToString (Unicode.Code.MinusSign);
	}
}
