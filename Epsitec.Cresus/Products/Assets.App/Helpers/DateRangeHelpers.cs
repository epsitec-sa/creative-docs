//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Core.Helpers;
using Epsitec.Cresus.Assets.Data;

namespace Epsitec.Cresus.Assets.App.Helpers
{
	public static class DateRangeHelpers
	{
		public static string ToNiceString(this DateRange range)
		{
			//	Retourne une jolie description d'une période, sous la forme
			//	"2014" ou "du 01.01.2014 au 31.03.2014".

			var f = range.IncludeFrom;
			var t = range.ExcludeTo.AddDays (-1);  // date de fin inclue

			if (f.Day   ==  1 &&
				f.Month ==  1 &&	// du premier janvier ?
				t.Day   == 31 &&
				t.Month == 12 &&	// au 31 décembre ?
				f.Year  == t.Year)	// de la même année ?
			{
				return f.Year.ToString ();
			}
			else
			{
				return string.Format ("du {0} au {1}",
					TypeConverters.DateToString (f),
					TypeConverters.DateToString (t));
			}
		}
	}
}
