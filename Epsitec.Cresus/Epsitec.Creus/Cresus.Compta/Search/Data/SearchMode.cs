//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

namespace Epsitec.Cresus.Compta.Search.Data
{
	public enum SearchMode
	{
		Fragment,		// contenu partiel
		StartsWith,		// au début
		EndsWith,		// à la fin
		WholeContent,	// contenu complet
		Jokers,			// avec jokers
		Interval,		// intervalle de nombres décimaux ou de dates
		Empty,			// doit être vide
	}
}
