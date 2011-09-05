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
		public BusinessLogic(IBusinessContext businessContext, DocumentMetadataEntity documentMetadataEntity)
		{
			this.businessContext        = businessContext as BusinessContext;
			this.documentMetadataEntity = documentMetadataEntity;

			this.articleQuantityColumnEntities = this.businessContext.GetAllEntities<ArticleQuantityColumnEntity> ();

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


		public bool IsDebug
		{
			get;
			set;
		}

		public bool IsLinesEditionEnabled
		{
			//	Indique s'il est possible d'éditer les lignes du document, c'est-à-dire s'il est possible
			//	de créer des lignes, d'en supprimer, d'en déplacer ou de les modifier.
			get
			{
				return this.IsDebug || 
					(this.documentMetadataEntity.DocumentState != DocumentState.Inactive && this.documentBusinessLogic.IsLinesEditionEnabled);
			}
		}

		public bool IsArticleParametersEditionEnabled
		{
			//	Indique s'il est possibled'éditer les paramètres des articles.
			get
			{
				return this.IsDebug ||
					(this.documentMetadataEntity.DocumentState != DocumentState.Inactive && this.documentBusinessLogic.IsArticleParametersEditionEnabled);
			}
		}

		public bool IsTextEditionEnabled
		{
			//	Indique s'il est possible d'éditer les textes.
			get
			{
				return this.IsDebug ||
					(this.documentMetadataEntity.DocumentState != DocumentState.Inactive && this.documentBusinessLogic.IsTextEditionEnabled);
			}
		}

		public bool IsMyEyesOnlyEditionEnabled
		{
			//	Indique si on ne peut éditer que les textes 'MyEyesOnly', c'est-à-dire pour les documents
			//	interne à l'entreprise.
			get
			{
				return this.documentMetadataEntity.DocumentState != DocumentState.Inactive && this.documentBusinessLogic.IsMyEyesOnlyEditionEnabled;
			}
		}

		public bool IsPriceEditionEnabled
		{
			//	Indique s'il est possible d'éditer les prix.
			get
			{
				return this.IsDebug ||
					(this.documentMetadataEntity.DocumentState != DocumentState.Inactive && this.documentBusinessLogic.IsPriceEditionEnabled);
			}
		}

		public bool IsDiscountEditionEnabled
		{
			//	Indique s'il est possible d'éditer les rabais.
			get
			{
				return this.IsDebug ||
					(this.documentMetadataEntity.DocumentState != DocumentState.Inactive && this.documentBusinessLogic.IsDiscountEditionEnabled);
			}
		}


		public bool IsEditionEnabled(LineInformations info)
		{
			//	Indique s'il est possible d'éditer une ligne donnée.
			if (this.documentMetadataEntity.DocumentState == DocumentState.Inactive)
			{
				return false;
			}

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
				return this.documentMetadataEntity.DocumentState != DocumentState.Inactive &&
					   this.EnabledArticleQuantityTypes.Any ();
			}
		}

		public bool IsArticleQuantityTypeEditionEnabled(ArticleQuantityType type)
		{
			//	Indique s'il est possible d'éditer une quantité donnée.
			return this.documentMetadataEntity.DocumentState != DocumentState.Inactive &&
				   this.EnabledArticleQuantityTypes.Where (x => x == type).Any ();
		}

		public ArticleQuantityType MainArticleQuantityType
		{
			//	Retourne le type de quantité principal, c'est-à-dire celui qui est édité avec l'article.
			get
			{
				if (this.IsDebug)
				{
					return ArticleQuantityType.Ordered;
				}
				else
				{
					return this.documentBusinessLogic.MainArticleQuantityType;
				}
			}
		}

		public IEnumerable<ArticleQuantityType> EnabledArticleQuantityTypes
		{
			//	Retourne les types de quantité définis dans les réglages globaux, compatibles
			//	avec le document en cours.
			get
			{
				var list = this.IsDebug ? this.DebugArticleQuantityTypeEditionEnabled : this.documentBusinessLogic.EnabledArticleQuantityTypes;
				
				System.Diagnostics.Debug.Assert (list != null);

				return list;
			}
		}

		public IEnumerable<ArticleQuantityType> PrintableArticleQuantityTypes
		{
			//	Retourne la liste des types de quantité imprimables.
			//	La première est la quantité principale.
			get
			{
				var list = this.documentBusinessLogic.PrintableArticleQuantityTypes;

				System.Diagnostics.Debug.Assert (list != null);

				return list;
			}
		}

		private IEnumerable<ArticleQuantityType> DebugArticleQuantityTypeEditionEnabled
		{
			//	Retourne la liste complète des types de quantité, pour le debug.
			get
			{
				yield return ArticleQuantityType.Billed;				// facturé
				yield return ArticleQuantityType.Delayed;				// retardé
				yield return ArticleQuantityType.Expected;				// attendu
				yield return ArticleQuantityType.Shipped;				// livré
				yield return ArticleQuantityType.ShippedPreviously;		// livré précédemment
				yield return ArticleQuantityType.Information;			// information
			}
		}


		#region Process parent document types
		public static IEnumerable<DocumentType> GetProcessParentDocumentTypes(DocumentType type)
		{
			//	Retourne la liste de tous les types de documents succeptibles d'être parent d'un document d'un type donné.
			//	Par exemple:
			//		ProductionChecklist	-> OrderBooking, SalesQuote
			//		ProductionChecklist	-> ProductionOrder, OrderConfirmation, OrderBooking, SalesQuote
			//		Invoice				-> DeliveryNote, OrderConfirmation, OrderBooking, SalesQuote
			//	On remarque que bien qu'un DeliveryNote soit généralement créé après un ProductionChecklist, il n'y pas
			//	pour parents ProductionChecklist et ProductionOrder, mais OrderConfirmation !
			//	L'ordre complet de production des documents est:
			//		SalesQuote
			//		OrderBooking
			//		OrderConfirmation
			//		ProductionOrder
			//		ProductionChecklist
			//		DeliveryNote
			//		Invoice
			//		PaymentReminder
			var types = BusinessLogic.ParentDocumentTypes.Where (x => x.Contains (type)).FirstOrDefault ();

			if (types == null)
			{
				yield break;
			}
			else
			{
				int i = types.IndexOf (type);

				while (--i >= 0)
				{
					yield return types[i];
				}
			}
		}

		private static IEnumerable<List<DocumentType>> ParentDocumentTypes
		{
			get
			{
				yield return BusinessLogic.ParentDocumentTypes1;
				yield return BusinessLogic.ParentDocumentTypes2;
			}
		}

		private static List<DocumentType> ParentDocumentTypes1 = new List<DocumentType>
		{
			DocumentType.SalesQuote,
			DocumentType.OrderBooking,
			DocumentType.OrderConfirmation,

			DocumentType.ProductionOrder,
			DocumentType.ProductionChecklist,
		};

		private static List<DocumentType> ParentDocumentTypes2 = new List<DocumentType>
		{
			DocumentType.SalesQuote,
			DocumentType.OrderBooking,
			DocumentType.OrderConfirmation,
			
			DocumentType.DeliveryNote,
			DocumentType.Invoice,
			DocumentType.PaymentReminder,
		};

		#region Auto-test
		static BusinessLogic()
		{
			System.Diagnostics.Debug.Assert (BusinessLogic.Compare (BusinessLogic.GetProcessParentDocumentTypes (DocumentType.SalesQuote)));
			System.Diagnostics.Debug.Assert (BusinessLogic.Compare (BusinessLogic.GetProcessParentDocumentTypes (DocumentType.OrderBooking),                                                                                                                DocumentType.SalesQuote));
			System.Diagnostics.Debug.Assert (BusinessLogic.Compare (BusinessLogic.GetProcessParentDocumentTypes (DocumentType.OrderConfirmation),                                                                                DocumentType.OrderBooking, DocumentType.SalesQuote));
			System.Diagnostics.Debug.Assert (BusinessLogic.Compare (BusinessLogic.GetProcessParentDocumentTypes (DocumentType.ProductionOrder),                                                  DocumentType.OrderConfirmation, DocumentType.OrderBooking, DocumentType.SalesQuote));
			System.Diagnostics.Debug.Assert (BusinessLogic.Compare (BusinessLogic.GetProcessParentDocumentTypes (DocumentType.ProductionChecklist),                DocumentType.ProductionOrder, DocumentType.OrderConfirmation, DocumentType.OrderBooking, DocumentType.SalesQuote));
			System.Diagnostics.Debug.Assert (BusinessLogic.Compare (BusinessLogic.GetProcessParentDocumentTypes (DocumentType.DeliveryNote),                                                     DocumentType.OrderConfirmation, DocumentType.OrderBooking, DocumentType.SalesQuote));
			System.Diagnostics.Debug.Assert (BusinessLogic.Compare (BusinessLogic.GetProcessParentDocumentTypes (DocumentType.Invoice),                               DocumentType.DeliveryNote, DocumentType.OrderConfirmation, DocumentType.OrderBooking, DocumentType.SalesQuote));
			System.Diagnostics.Debug.Assert (BusinessLogic.Compare (BusinessLogic.GetProcessParentDocumentTypes (DocumentType.PaymentReminder), DocumentType.Invoice, DocumentType.DeliveryNote, DocumentType.OrderConfirmation, DocumentType.OrderBooking, DocumentType.SalesQuote));
		}

		private static bool Compare(IEnumerable<DocumentType> types1, params DocumentType[] types2)
		{
			if (types1.Count () != types2.Count ())
			{
				return false;
			}

			for (int i = 0; i < types1.Count (); i++)
			{
				if (types1.ElementAt (i) != types2.ElementAt (i))
				{
					return false;
				}
			}

			return true;
		}
		#endregion
		#endregion


		private readonly BusinessContext							businessContext;
		private readonly DocumentMetadataEntity						documentMetadataEntity;
		private readonly IEnumerable<ArticleQuantityColumnEntity>	articleQuantityColumnEntities;

		private AbstractDocumentBusinessLogic						documentBusinessLogic;
	}
}
