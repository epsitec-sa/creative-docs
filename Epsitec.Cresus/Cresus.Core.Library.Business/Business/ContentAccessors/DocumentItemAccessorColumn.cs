//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Epsitec.Cresus.Core.Library.Business.ContentAccessors
{
	public enum DocumentItemAccessorColumn
	{
		OrderedQuantity,				//	quantité commandée, pour les factures
		OrderedUnit,

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
		VatRevenue,						//	montant HT imposé à la TVA
		VatInfo,						//	info de TVA (taux, montant, code, etc.)
		
		UnitDiscount,					//	rabais (ou arrondi) d'unité
		LineDiscount,					//	rabais (ou arrondi) de ligne
		
		UnitPrice,						//	prix unitaire HT ou TTC
		LinePrice,						//	prix de ligne HT ou TTC
		TotalPrice,						//	prix de ligne total HT ou TTC, total de sous-total
	}
}
