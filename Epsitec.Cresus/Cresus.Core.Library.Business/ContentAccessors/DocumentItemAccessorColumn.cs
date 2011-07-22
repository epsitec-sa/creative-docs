//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Epsitec.Cresus.Core.Library.Business.ContentAccessors
{
	public enum DocumentItemAccessorColumn
	{
		MainQuantity,					// quantité principale
		MainUnit,

		AdditionalQuantity,				// autres quantités
		AdditionalUnit,
		AdditionalBeginDate,
		AdditionalEndDate,
		AdditionalType,

		GroupNumber,					// numéro de groupe
		LineNumber,						// numéro de ligne
		FullNumber,						// numéro de groupe et de ligne

		ArticleId,						// numéro d'article
		ArticleDescription,				// description de l'article

		VatCode,						// code de TVA
		VatRate,						// taux de TVA
		
		Discount,						// rabais
		UnitPrice,						// prix unitaire HT
		LinePrice,						// prix total HT
		Vat,							// TVA
		Total,							// prix total TTC
	}
}
