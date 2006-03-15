//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types.Serialization.IO
{
	public static class XmlSupport
	{
		public static string IdToString(int id)
		{
			return string.Concat ("_", id.ToString (System.Globalization.CultureInfo.InvariantCulture));
		}

		public static int ParseId(string value)
		{
			System.Diagnostics.Debug.Assert (value.Length > 1);
			System.Diagnostics.Debug.Assert (value[0] == '_');

			return int.Parse (value.Substring (1), System.Globalization.CultureInfo.InvariantCulture);
		}
	}
}
