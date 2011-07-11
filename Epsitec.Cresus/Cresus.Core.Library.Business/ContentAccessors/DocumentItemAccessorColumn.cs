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
		UniqueQuantity,					// toutes les quantités en vrac
		UniqueUnit,
		UniqueBeginDate,
		UniqueEndDate,
		UniqueType,

		OrderedQuantity,				// quantité commandée
		OrderedUnit,
		OrderedBeginDate,
		OrderedEndDate,

		BilledQuantity,					// quantité facturée
		BilledUnit,
		BilledBeginDate,
		BilledEndDate,

		DelayedQuantity,				// quantité retardée
		DelayedUnit,
		DelayedBeginDate,
		DelayedEndDate,

		ExpectedQuantity,				// quantité attendue
		ExpectedUnit,
		ExpectedBeginDate,
		ExpectedEndDate,

		ShippedQuantity,				// quantité livrée
		ShippedUnit,
		ShippedBeginDate,
		ShippedEndDate,

		ShippedPreviouslyQuantity,		// quantité livrée précédemment
		ShippedPreviouslyUnit,
		ShippedPreviouslyBeginDate,
		ShippedPreviouslyEndDate,

		InformationQuantity,			// quantité pour information
		InformationUnit,
		InformationBeginDate,
		InformationEndDate,

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
