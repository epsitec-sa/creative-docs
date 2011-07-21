//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;
using Epsitec.Cresus.Core.Helpers;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Library;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.BusinessDocumentControllers
{
	public class BusinessLogic
	{
		public BusinessLogic(BusinessContext businessContext, DocumentMetadataEntity documentMetadataEntity)
		{
			this.businessContext        = businessContext;
			this.documentMetadataEntity = documentMetadataEntity;

			this.articleQuantityColumnEntities = this.businessContext.GetAllEntities<ArticleQuantityColumnEntity> ().OrderBy (x => x.QuantityType);
		}


		public ArticleQuantityColumnEntity GetArticleQuantityColumnEntity(ArticleQuantityType type)
		{
			return this.articleQuantityColumnEntities.Where (x => x.QuantityType == type).FirstOrDefault ();
		}

		public IEnumerable<EnumKeyValues<ArticleQuantityType>> PossibleValueArticleQuantityType
		{
			//	Retourne les types de quantité définis dans les réglages globaux.
			get
			{
				var possible = this.PossibleArticleQuantityType;

				foreach (var e in this.articleQuantityColumnEntities)
				{
					if (possible.Contains (e.QuantityType))
					{
						yield return EnumKeyValues.Create (e.QuantityType, e.Name);
					}
				}
			}
		}

		private IEnumerable<ArticleQuantityType> PossibleArticleQuantityType
		{
			//	Retourne les types de quantité éditable, en fonction du type du document en cours.
			get
			{
				switch (this.documentMetadataEntity.DocumentCategory.DocumentType)
				{
					//	Devis :
					case DocumentType.SalesQuote:
						yield return ArticleQuantityType.Ordered;				// commandé
						yield return ArticleQuantityType.Delayed;				// retardé
						yield return ArticleQuantityType.Information;			// information
						break;

					//	Choix des options pour commande :
					case DocumentType.OrderConfiguration:
						yield return ArticleQuantityType.Ordered;				// commandé
						yield return ArticleQuantityType.Delayed;				// retardé
						yield return ArticleQuantityType.Information;			// information
						break;

					//	Bon pour commande :
					case DocumentType.OrderBooking:
						yield return ArticleQuantityType.Ordered;				// commandé
						yield return ArticleQuantityType.Delayed;				// retardé
						yield return ArticleQuantityType.Information;			// information
						break;

					//	Confirmation de commande :
					case DocumentType.OrderConfirmation:
						yield return ArticleQuantityType.Ordered;				// commandé
						yield return ArticleQuantityType.Delayed;				// retardé
						yield return ArticleQuantityType.Information;			// information
						break;

					//	Ordre de production :
					case DocumentType.ProductionOrder:
						break;

					//	Check-list de production :
					case DocumentType.ProductionChecklist:
						break;

					//	Check-list d'expédition :
					case DocumentType.ShipmentChecklist:
						yield return ArticleQuantityType.Ordered;				// commandé
						yield return ArticleQuantityType.Delayed;				// retardé
						yield return ArticleQuantityType.Expected;				// attendu
						yield return ArticleQuantityType.Shipped;				// livré
						yield return ArticleQuantityType.ShippedPreviously;		// livré ultérieurement
						yield return ArticleQuantityType.Information;			// information
						break;

					//	Bulletin de livraison :
					case DocumentType.DeliveryNote:
						yield return ArticleQuantityType.Ordered;				// commandé
						yield return ArticleQuantityType.Delayed;				// retardé
						yield return ArticleQuantityType.Expected;				// attendu
						yield return ArticleQuantityType.Shipped;				// livré
						yield return ArticleQuantityType.ShippedPreviously;		// livré ultérieurement
						yield return ArticleQuantityType.Information;			// information
						break;

					//	Facture :
					case DocumentType.Invoice:
						yield return ArticleQuantityType.Billed;				// facturé
						yield return ArticleQuantityType.Delayed;				// retardé
						yield return ArticleQuantityType.Expected;				// attendu
						yield return ArticleQuantityType.Shipped;				// livré
						yield return ArticleQuantityType.ShippedPreviously;		// livré ultérieurement
						break;

					//	Facture pro forma :
					case DocumentType.InvoiceProForma:
						yield return ArticleQuantityType.Billed;				// facturé
						yield return ArticleQuantityType.Delayed;				// retardé
						yield return ArticleQuantityType.Expected;				// attendu
						yield return ArticleQuantityType.Shipped;				// livré
						yield return ArticleQuantityType.ShippedPreviously;		// livré ultérieurement
						break;

					//	Rappel :
					case DocumentType.PaymentReminder:
						break;

					//	Reçu :
					case DocumentType.Receipt:
						yield return ArticleQuantityType.Ordered;				// commandé
						yield return ArticleQuantityType.Billed;				// facturé
						yield return ArticleQuantityType.Delayed;				// retardé
						yield return ArticleQuantityType.Expected;				// attendu
						yield return ArticleQuantityType.Shipped;				// livré
						yield return ArticleQuantityType.ShippedPreviously;		// livré ultérieurement
						yield return ArticleQuantityType.Information;			// information
						break;

					//	Note de crédit :
					case DocumentType.CreditMemo:
						yield return ArticleQuantityType.Ordered;				// commandé
						yield return ArticleQuantityType.Billed;				// facturé
						yield return ArticleQuantityType.Delayed;				// retardé
						yield return ArticleQuantityType.Expected;				// attendu
						yield return ArticleQuantityType.Shipped;				// livré
						yield return ArticleQuantityType.ShippedPreviously;		// livré ultérieurement
						yield return ArticleQuantityType.Information;			// information
						break;

					//	Demande d'offre :
					case DocumentType.QuoteRequest:
						yield return ArticleQuantityType.Ordered;				// commandé
						yield return ArticleQuantityType.Billed;				// facturé
						yield return ArticleQuantityType.Delayed;				// retardé
						yield return ArticleQuantityType.Expected;				// attendu
						yield return ArticleQuantityType.Shipped;				// livré
						yield return ArticleQuantityType.ShippedPreviously;		// livré ultérieurement
						yield return ArticleQuantityType.Information;			// information
						break;

					//	Confirmation de commande :
					case DocumentType.PurchaseOrder:
						yield return ArticleQuantityType.Ordered;				// commandé
						yield return ArticleQuantityType.Billed;				// facturé
						yield return ArticleQuantityType.Delayed;				// retardé
						yield return ArticleQuantityType.Expected;				// attendu
						yield return ArticleQuantityType.Shipped;				// livré
						yield return ArticleQuantityType.ShippedPreviously;		// livré ultérieurement
						yield return ArticleQuantityType.Information;			// information
						break;

					default:
						yield return ArticleQuantityType.Ordered;				// commandé
						yield return ArticleQuantityType.Billed;				// facturé
						yield return ArticleQuantityType.Delayed;				// retardé
						yield return ArticleQuantityType.Expected;				// attendu
						yield return ArticleQuantityType.Shipped;				// livré
						yield return ArticleQuantityType.ShippedPreviously;		// livré ultérieurement
						yield return ArticleQuantityType.Information;			// information
						break;
				}
			}
		}


		private readonly BusinessContext							businessContext;
		private readonly DocumentMetadataEntity						documentMetadataEntity;
		private readonly IEnumerable<ArticleQuantityColumnEntity>	articleQuantityColumnEntities;
	}
}
