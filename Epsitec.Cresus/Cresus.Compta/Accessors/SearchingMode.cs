//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

namespace Epsitec.Cresus.Compta.Accessors
{
	public enum SearchingMode
	{
		Normal		= 0,	// contenu partiel, avec équivalence majuscules/minuscules et accents
		WholeWord	= 1,	// mot entier, avec équivalence majuscules/minuscules et accents
		Exact		= 2,	// contenu complet et exact
		Interval	= 3,	// intervalle de nombres décimaux ou de dates
		Empty		= 4,	// doit être vide
	}
}
