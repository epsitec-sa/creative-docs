//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Library
{
	public abstract class TextFormatterConverter<T> : ITextFormatterConverter
	{
		#region ITextFormatterConverter Members

		public IEnumerable<System.Type> GetConvertibleTypes()
		{
			yield return typeof (T);
		}

		public FormattedText ToFormattedText(object value, System.Globalization.CultureInfo culture, TextFormatterDetailLevel detailLevel)
		{
			if ((value is T) &&
						(value != null))
			{
				switch (detailLevel)
				{
					case TextFormatterDetailLevel.Default:
					case TextFormatterDetailLevel.Title:
					case TextFormatterDetailLevel.Compact:
					case TextFormatterDetailLevel.Full:
						return this.ToFormattedText ((T) value, culture, detailLevel);

					default:
						throw new System.NotSupportedException (string.Format ("Detail level {0} not supported", detailLevel));
				}
			}
			else
			{
				return FormattedText.Empty;
			}
		}

		#endregion

		protected abstract FormattedText ToFormattedText(T value, System.Globalization.CultureInfo culture, TextFormatterDetailLevel detailLevel);
	}
}
