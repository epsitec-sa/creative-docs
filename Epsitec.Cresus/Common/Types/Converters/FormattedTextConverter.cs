//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Types.Converters
{
	/// <summary>
	/// The <c>FormattedTextConverter</c> class does a transparent conversion;
	/// it is required so that the <see cref="Marshaler"/> can work with
	/// <c>FormattedText</c> values.
	/// </summary>
	public class FormattedTextConverter : GenericConverter<FormattedText, FormattedTextConverter>
	{
		public FormattedTextConverter()
			: base (System.Globalization.CultureInfo.InvariantCulture)
		{
		}
		
		public override string ConvertToString(FormattedText text)
		{
			return text.IsNull () ? null : text.ToString ();
		}

		public override ConversionResult<FormattedText> ConvertFromString(string text)
		{
			if (text == null)
			{
				return new ConversionResult<FormattedText>
				{
					IsNull = true,
				};
			}
			else
			{
				return new ConversionResult<FormattedText>
				{
					Value = new FormattedText (text),
				};
			}
		}

		public override bool CanConvertFromString(string text)
		{
			return true;
		}
	}
}
