//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Data.Helpers
{
	public static class DataIO
	{
		public static decimal? ReadDecimalAttribute(System.Xml.XmlReader reader, string name)
		{
			var s = reader[name];

			if (string.IsNullOrEmpty (s))
			{
				return null;
			}
			else
			{
				return decimal.Parse (s, System.Globalization.CultureInfo.InvariantCulture);
			}
		}

		public static void WriteDecimalAttribute(System.Xml.XmlWriter writer, string name, decimal? value)
		{
			if (value.HasValue)
			{
				writer.WriteAttributeString (name, value.Value.ToString (System.Globalization.CultureInfo.InvariantCulture));
			}
		}
	}
}
