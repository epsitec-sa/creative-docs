//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Library
{
	/// <summary>
	/// The <c>PrettyPrinter</c> class is used to format an object so that it can be
	/// displayed to the user, either using the object's own <see cref="ITextFormatter"/>
	/// interface, or using the associated <see cref="IPrettyPrinter"/>, if any.
	/// </summary>
	public static class PrettyPrinter
	{
		public static FormattedText ToFormattedText(object value, System.Globalization.CultureInfo culture = null, TextFormatterDetailLevel detailLevel = TextFormatterDetailLevel.Default)
		{
			if (value == null)
			{
				return FormattedText.Empty;
			}
			else
			{
				return PrettyPrinter.ToFormattedText (value, value.GetType (), culture ?? System.Globalization.CultureInfo.CurrentCulture, detailLevel);
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

			var prettyPrinter = Resolvers.PrettyPrinterResolver.Resolve (type);

			if (prettyPrinter == null)
			{
				return FormattedText.FromSimpleText (string.Format (culture, "{0}", value));
			}
			else
			{
				return prettyPrinter.ToFormattedText (value, culture, detailLevel);
			}
		}
	}
}
