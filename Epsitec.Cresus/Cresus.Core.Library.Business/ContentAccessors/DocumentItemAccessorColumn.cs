//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Epsitec.Cresus.Core.Library.Business.ContentAccessors
{
	public enum DocumentItemAccessorColumn
	{
		MainQuantity,					//	quantité principale
		MainUnit,

		AdditionalQuantity,				//	autres quantités
		AdditionalUnit,
		AdditionalBeginDate,
		AdditionalEndDate,
		AdditionalType,

		GroupNumber,					//	numéro de groupe
		LineNumber,						//	numéro de ligne
		FullNumber,						//	numéro de groupe et de ligne

		ArticleId,						//	numéro d'article
		ArticleDescription,				//	description de l'article

		VatCode,						//	code de TVA
		VatRate,						//	taux de TVA
		VatRevenue,						//	montant HT sur lequel la TVA s'applique
		VatTotal,						//	montant total de TVA
		
		Discount,						//	rabais (ou arrondi)
		
		UnitPriceBeforeTax,				//	prix unitaire HT
		UnitPriceAfterTax,				//	prix unitaire TTC
		LinePriceBeforeTax,				//	prix de ligne HT
		LinePriceAfterTax,				//	prix de ligne TTC
		FinalPriceBeforeTax,			//	prix de ligne HT après rabais/arrondi
		FinalPriceAfterTax,				//	prix de ligne TTC après rabais/arrondi
		
		Revenue,						//	prix réel HT ou TTC, selon le contexte
		SubTotal,						//	prix produit par un sous-total
		
	}
}
