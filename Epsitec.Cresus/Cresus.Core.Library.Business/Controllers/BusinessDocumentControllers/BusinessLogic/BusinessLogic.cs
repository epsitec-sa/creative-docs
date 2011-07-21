﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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

			this.CreateDocumentBusinessLogic ();
		}


		private void CreateDocumentBusinessLogic()
		{
			switch (this.documentMetadataEntity.DocumentCategory.DocumentType)
			{
				case DocumentType.SalesQuote:
					this.documentBusinessLogic = new SalesQuoteBusinessLogic (this.businessContext, this.documentMetadataEntity);
					break;

				case DocumentType.OrderConfiguration:
					this.documentBusinessLogic = new OrderConfigurationBusinessLogic (this.businessContext, this.documentMetadataEntity);
					break;

				case DocumentType.OrderBooking:
					this.documentBusinessLogic = new OrderBookingBusinessLogic (this.businessContext, this.documentMetadataEntity);
					break;

				case DocumentType.OrderConfirmation:
					this.documentBusinessLogic = new OrderConfirmationBusinessLogic (this.businessContext, this.documentMetadataEntity);
					break;

				case DocumentType.ProductionOrder:
					this.documentBusinessLogic = new ProductionOrderBusinessLogic (this.businessContext, this.documentMetadataEntity);
					break;

				case DocumentType.ProductionChecklist:
					this.documentBusinessLogic = new ProductionChecklistBusinessLogic (this.businessContext, this.documentMetadataEntity);
					break;

				case DocumentType.ShipmentChecklist:
					this.documentBusinessLogic = new ShipmentChecklistBusinessLogic (this.businessContext, this.documentMetadataEntity);
					break;

				case DocumentType.DeliveryNote:
					this.documentBusinessLogic = new DeliveryNoteBusinessLogic (this.businessContext, this.documentMetadataEntity);
					break;

				case DocumentType.Invoice:
					this.documentBusinessLogic = new InvoiceBusinessLogic (this.businessContext, this.documentMetadataEntity);
					break;

				case DocumentType.InvoiceProForma:
					this.documentBusinessLogic = new InvoiceProFormaBusinessLogic (this.businessContext, this.documentMetadataEntity);
					break;

				case DocumentType.PaymentReminder:
					this.documentBusinessLogic = new PaymentReminderBusinessLogic (this.businessContext, this.documentMetadataEntity);
					break;

				case DocumentType.Receipt:
					this.documentBusinessLogic = new ReceiptBusinessLogic (this.businessContext, this.documentMetadataEntity);
					break;

				case DocumentType.CreditMemo:
					this.documentBusinessLogic = new CreditMemoBusinessLogic (this.businessContext, this.documentMetadataEntity);
					break;

				case DocumentType.QuoteRequest:
					this.documentBusinessLogic = new QuoteRequestBusinessLogic (this.businessContext, this.documentMetadataEntity);
					break;

				case DocumentType.PurchaseOrder:
					this.documentBusinessLogic = new PurchaseOrderBusinessLogic (this.businessContext, this.documentMetadataEntity);
					break;
			}

			System.Diagnostics.Debug.Assert (this.documentBusinessLogic != null);
		}


		public bool IsLinesEditionEnabled
		{
			//	Indique s'il est possible d'éditer les lignes du document, c'est-à-dire s'il est possible
			//	de créer des lignes, d'en supprimer, d'en déplacer ou de les modifier.
			get
			{
				return this.documentBusinessLogic.IsLinesEditionEnabled;
			}
		}

		public bool IsArticleParametersEditionEnabled
		{
			//	Indique s'il est possibled'éditer les paramètres des articles.
			get
			{
				return this.documentBusinessLogic.IsArticleParametersEditionEnabled;
			}
		}

		public bool IsTextEditionEnabled
		{
			//	Indique s'il est possible d'éditer les textes.
			get
			{
				return this.documentBusinessLogic.IsTextEditionEnabled;
			}
		}

		public bool IsMyEyesOnlyEditionEnabled
		{
			//	Indique si on ne peut éditer que les textes 'MyEyesOnly', c'est-à-dire pour les documents
			//	interne à l'entreprise.
			get
			{
				return this.documentBusinessLogic.IsMyEyesOnlyEditionEnabled;
			}
		}

		public bool IsPriceEditionEnabled
		{
			//	Indique s'il est possible d'éditer les prix.
			get
			{
				return this.documentBusinessLogic.IsPriceEditionEnabled;
			}
		}

		public bool IsDiscountEditionEnabled
		{
			//	Indique s'il est possible d'éditer les rabais.
			get
			{
				return this.documentBusinessLogic.IsDiscountEditionEnabled;
			}
		}


		public bool IsEditionEnabled(LineInformations info)
		{
			//	Indique s'il est possible d'éditer une ligne donnée.
			if (this.IsLinesEditionEnabled)
			{
				return true;
			}

			if (this.IsArticleQuantityEditionEnabled)
			{
				if (info.IsQuantity)
				{
					return true;
				}
			}

			if (this.IsMyEyesOnlyEditionEnabled)
			{
				if (info.AbstractDocumentItemEntity is TextDocumentItemEntity)
				{
					var text = info.AbstractDocumentItemEntity as TextDocumentItemEntity;
					if (text.Attributes.HasFlag (DocumentItemAttributes.MyEyesOnly))
					{
						return true;
					}
				}
			}

			return false;
		}


		public ArticleQuantityColumnEntity GetArticleQuantityColumnEntity(ArticleQuantityType type)
		{
			//	Retourne une définition de type de quantité définie dans les réglages globaux.
			return this.articleQuantityColumnEntities.Where (x => x.QuantityType == type).FirstOrDefault ();
		}


		public bool IsArticleQuantityEditionEnabled
		{
			//	Indique s'il est possible d'éditer une ou plusieurs quantités.
			get
			{
				return this.ArticleQuantityTypeEditionEnabled.Count () != 0;
			}
		}

		public bool IsArticleQuantityTypeEditionEnabled(ArticleQuantityType type)
		{
			//	Indique s'il est possible d'éditer une quantité donnée.
			return this.ArticleQuantityTypeEditionEnabled.Where (x => x.Key == type).Any ();
		}

		public IEnumerable<EnumKeyValues<ArticleQuantityType>> ArticleQuantityTypeEditionEnabled
		{
			//	Retourne les types de quantité définis dans les réglages globaux, compatibles
			//	avec le document en cours.
			get
			{
				var enabled = this.documentBusinessLogic.ArticleQuantityTypeEditionEnabled;

				foreach (var e in this.articleQuantityColumnEntities)
				{
					if (enabled != null && enabled.Contains (e.QuantityType))
					{
						yield return EnumKeyValues.Create (e.QuantityType, e.Name);
					}
				}
			}
		}


		private readonly BusinessContext							businessContext;
		private readonly DocumentMetadataEntity						documentMetadataEntity;
		private readonly IEnumerable<ArticleQuantityColumnEntity>	articleQuantityColumnEntities;

		private AbstractDocumentBusinessLogic						documentBusinessLogic;
	}
}
