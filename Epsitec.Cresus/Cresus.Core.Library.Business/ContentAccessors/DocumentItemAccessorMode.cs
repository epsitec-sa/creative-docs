//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Epsitec.Cresus.Core.Library.Business.ContentAccessors
{
	public enum DocumentItemAccessorMode
	{
		None					= 0,
		UseMainColumns			= 0x00000001,	// utilise les colonnes MainQuantity et MainUnit, pour les documents imprimés
		AdditionalQuantities	= 0x00000002,	// met les quantités additionnelles
		ShowMyEyesOnly			= 0x00000004,	// inclu les lignes avant l'attribut MyEyesOnly

		IncludeTaxes			= 0x00001000,	// 
		ExclideTaxes			= 0x00002000,	// 

												// Ces 4 modes ne sont pas cumulables:
		EditArticleName			= 0x00010000,	// édite les descriptions courtes des articles
		EditArticleDescription	= 0x00020000,	// édite les descriptions longues des articles
		UseArticleName			= 0x00040000,	// utilise les descriptions courtes des articles
		UseArticleBoth			= 0x00080000,	// utilise les descriptions courtes + longues des articles
	}
}
