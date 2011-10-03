//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Epsitec.Cresus.Core.Library.Business.ContentAccessors
{
	public enum DocumentItemAccessorMode
	{
		None							= 0,
		Print							= 0x00000001,	// impression (utilise les colonnes MainQuantity et MainUnit)

		AdditionalQuantities			= 0x00000010,	// met les quantités additionnelles (si impression)
		AdditionalQuantitiesSeparate	= 0x00000020,	// met les quantités additionnelles séparément (en plus de AdditionalQuantities)
		MainQuantityOnTop				= 0x00000040,	// met la quantité principale sur la première ligne (si impression)
		NoPrices						= 0x00000080,	// aucun prix (si impression)

		ShowMyEyesOnly					= 0x00000100,	// inclu les lignes avant l'attribut MyEyesOnly

														// Ces 4 modes ne sont pas cumulables:
		EditArticleName					= 0x00010000,	// édite les descriptions courtes des articles
		EditArticleDescription			= 0x00020000,	// édite les descriptions longues des articles
		UseArticleName					= 0x00040000,	// utilise les descriptions courtes des articles
		UseArticleBoth					= 0x00080000,	// utilise les descriptions courtes + longues des articles
	}
}
