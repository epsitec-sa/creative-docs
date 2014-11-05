//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Data.Helpers
{
	public static class DateTime
	{
		public static int GetTotalMonth(this System.DateTime date)
		{
			return date.Year*12 + date.Month;
		}


		//?public static string ToStringIO(this System.DateTime date)
		//?{
		//?	//?return date.ToString ("yyyy-MM-ddTHH:mm:ss");
		//?	return date.ToString ("yyyy-MM-dd");
		//?}
		//?
		//?public static System.DateTime ParseDateIO(this string s)
		//?{
		//?	//?return System.DateTime.ParseExact (s, "yyyy-MM-ddTHH:mm:ss", null);
		//?	return System.DateTime.ParseExact (s, "yyyy-MM-dd", null);
		//?}
	}
}
