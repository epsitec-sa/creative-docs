//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	public static class LogicRange
	{
		public static IEnumerable<DateRange> GetRanges(DateRange range, int period)
		{
			//	Retourne la liste des intervalles pour lesquels il faudra générer
			//	des amortissements. Par exemple:
			//
			//	range	01.01.2013 .. 31.12.2013
			//	period	6
			//	out		01.01.2013 .. 30.06.2013
			//			01.07.2013 .. 31.12.2013
			//
			//	range	01.08.2013 .. 01.08.2014
			//	period	3
			//	out		01.07.2013 .. 31.09.2013
			//			01.10.2013 .. 31.06.2013
			//			01.01.2014 .. 31.03.2014
			//			01.04.2014 .. 30.06.2014

			System.Diagnostics.Debug.Assert (period >= 0);
			var start = new System.DateTime (range.IncludeFrom.Date.Year, 1, 1);

			while (start < range.IncludeTo.Date)
			{
				var end = start.AddMonths (period).AddDays (-1);

				if (range.IsInside (end))
				{
					yield return new DateRange (start, end);
				}

				start = start.AddMonths (period);
			}
		}
	}
}