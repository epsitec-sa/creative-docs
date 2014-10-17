//	Copyright � 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	public enum TimelinesMode
	{
		Narrow,				// mode �troit, avec 3 lignes d'en-t�te (ann�e, mois, jour)
		Wide,				// mode normal, avec 2 lignes d'en-t�te (ann�e, date)
		GroupedByMonth,		// mode avec 1 cellule/mois, et donc plusieurs �v�nement par cellule
		GroupedByTrim,		// mode avec 1 cellule/trimestre, et donc plusieurs �v�nement par cellule
		GroupedByYear,		// mode avec 1 cellule/ann�e, et donc plusieurs �v�nement par cellule
	}
}
