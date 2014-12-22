//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Data.Helpers
{
	public static class DateTime
	{
		public static int Days(System.DateTime a, System.DateTime b)
		{
			//	Retourne le nombre de jours effectifs entre 2 dates.
			return System.Math.Abs (a.Subtract (b).Days);
		}

		public static int Days30(System.DateTime a, System.DateTime b)
		{
			//	Retourne le nombre de jours entre 2 dates, sur la base de
			//	12 mois de 30 jours.
			var aa = DateTime.GetDaysCount (a);
			var bb = DateTime.GetDaysCount (b);

			return System.Math.Abs (aa - bb);
		}

		public static int Months(System.DateTime a, System.DateTime b)
		{
			//	Retourne le nombre de mois entre 2 dates.
			var aa = DateTime.GetMonthsCount (a);
			var bb = DateTime.GetMonthsCount (b);

			return System.Math.Abs (aa - bb);
		}


		private static int GetMonthsCount(this System.DateTime date)
		{
			//	Retourne le nombre de mois écoulés depuis le 01.01.0000.
			//	L'origine est sans importance, car le résultat est utilisé pour
			//	calculer une différence entre 2 dates !
			return date.Year*12
				+ (date.Month-1);
		}

		private static int GetDaysCount(this System.DateTime date)
		{
			//	Retourne le nombre de jours écoulés depuis le 01.01.0000,
			//	en se basant sur 12 mois à 30 jours par année.
			//	L'origine est sans importance, car le résultat est utilisé pour
			//	calculer une différence entre 2 dates !
			return date.Year*12*30
				+ (date.Month-1)*30
				+ System.Math.Min ((date.Day-1), 30-1);
		}

	}
}
