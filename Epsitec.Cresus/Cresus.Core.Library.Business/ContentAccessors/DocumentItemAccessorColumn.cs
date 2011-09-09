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
		
		UnitDiscount,					//	rabais (ou arrondi) d'unité
		LineDiscount,					//	rabais (ou arrondi) de ligne
		
		UnitPriceBeforeTax1,			//	prix unitaire HT
		LinePriceBeforeTax1,			//	prix de ligne HT
		LinePriceBeforeTax2,			//	prix de ligne HT après rabais/arrondi
		
		LinePriceAfterTax1,				//	prix de ligne TTC
		UnitPriceAfterTax1,				//	prix unitaire TTC
		LinePriceAfterTax2,				//	prix de ligne TTC après rabais/arrondi
		
		Revenue,						//	prix réel HT ou TTC, selon le contexte
		SubTotal,						//	prix produit par un sous-total
		SubTotalDiscount,				//	rabais au sein d'un sous-total
		Total,							//	prix produit par un grand total
	}
}
