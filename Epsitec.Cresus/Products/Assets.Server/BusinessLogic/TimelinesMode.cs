//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	public enum TimelinesMode
	{
		Narrow,				// mode étroit, avec 3 lignes d'en-tête (année, mois, jour)
		Wide,				// mode normal, avec 2 lignes d'en-tête (année, date)
		GroupedByMonth,		// mode avec 1 cellule/mois, et donc plusieurs événement par cellule
		GroupedByTrim,		// mode avec 1 cellule/trimestre, et donc plusieurs événement par cellule
		GroupedByYear,		// mode avec 1 cellule/année, et donc plusieurs événement par cellule
	}
}
