//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Support.Extensions;
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
	public sealed class DocumentLogic
	{
		public DocumentLogic(IBusinessContext businessContext, DocumentMetadataEntity documentMetadata)
		{
			this.businessContext  = businessContext as BusinessContext;
			this.documentMetadata = documentMetadata;

			this.articleQuantityColumns = this.businessContext.GetAllEntities<ArticleQuantityColumnEntity> ().ToList ();

			this.documentLogic = this.CreateDocumentLogic (this.documentMetadata.DocumentCategory.DocumentType);
		}

		public DocumentMetadataEntity			DocumentMetadata
		{
			get
			{
				return this.documentMetadata;
			}
		}

		public BusinessDocumentEntity			BusinessDocument
		{
			get
			{
				return this.documentMetadata.BusinessDocument as BusinessDocumentEntity;
			}
		}

		public bool								IsDebug
		{
			get;
			set;
		}

		public bool								IsLinesEditionEnabled
		{
			//	Indique s'il est possible d'éditer les lignes du document, c'est-à-dire s'il est possible
			//	de créer des lignes, d'en supprimer, d'en déplacer ou de les modifier.
			get
			{
				return this.IsDebug || 
					(this.documentMetadata.DocumentState != DocumentState.Inactive && this.documentLogic.IsLinesEditionEnabled);
			}
		}

		public bool								IsArticleParametersEditionEnabled
		{
			//	Indique s'il est possibled'éditer les paramètres des articles.
			get
			{
				return this.IsDebug ||
					(this.documentMetadata.DocumentState != DocumentState.Inactive && this.documentLogic.IsArticleParametersEditionEnabled);
			}
		}

		public bool								IsTextEditionEnabled
		{
			//	Indique s'il est possible d'éditer les textes.
			get
			{
				return this.IsDebug ||
					(this.documentMetadata.DocumentState != DocumentState.Inactive && this.documentLogic.IsTextEditionEnabled);
			}
		}

		public bool								IsMyEyesOnlyEditionEnabled
		{
			//	Indique si on ne peut éditer que les textes 'MyEyesOnly', c'est-à-dire pour les documents
			//	interne à l'entreprise.
			get
			{
				return this.documentMetadata.DocumentState != DocumentState.Inactive && this.documentLogic.IsMyEyesOnlyEditionEnabled;
			}
		}

		public bool								IsPriceEditionEnabled
		{
			//	Indique s'il est possible d'éditer les prix.
			get
			{
				return this.IsDebug ||
					(this.documentMetadata.DocumentState != DocumentState.Inactive && this.documentLogic.IsPriceEditionEnabled);
			}
		}

		public bool								IsDiscountEditionEnabled
		{
			//	Indique s'il est possible d'éditer les rabais.
			get
			{
				return this.IsDebug ||
					(this.documentMetadata.DocumentState != DocumentState.Inactive && this.documentLogic.IsDiscountEditionEnabled);
			}
		}

		public bool								IsArticleQuantityEditionEnabled
		{
			//	Indique s'il est possible d'éditer une ou plusieurs quantités.
			get
			{
				return this.documentMetadata.DocumentState != DocumentState.Inactive &&
					   this.GetEnabledArticleQuantityTypes ().Any ();
			}
		}

		public ArticleQuantityType				MainArticleQuantityType
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
					return this.documentLogic.MainArticleQuantityType;
				}
			}
		}

		public bool								IsMainArticleQuantityEnabled
		{
			//	Indique si la quantité principal est éditable.
			get
			{
				if (this.IsDebug)
				{
					return true;
				}
				else
				{
					return this.documentLogic.IsMainArticleQuantityEnabled;
				}
			}
		}
		

		public bool IsEditionEnabled(Line info)
		{
			//	Indique s'il est possible d'éditer une ligne donnée.
			if (this.documentMetadata.DocumentState == DocumentState.Inactive)
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
				if (info.DocumentItem is TextDocumentItemEntity)
				{
					var text = info.DocumentItem as TextDocumentItemEntity;
					if (text.Attributes.HasFlag (DocumentItemAttributes.MyEyesOnly))
					{
						return true;
					}
				}
			}

			return false;
		}

		public bool IsArticleQuantityTypeEditionEnabled(ArticleQuantityType type)
		{
			//	Indique s'il est possible d'éditer une quantité donnée.
			return this.documentMetadata.DocumentState != DocumentState.Inactive &&
				   this.GetEnabledArticleQuantityTypes ().Where (x => x == type).Any ();
		}

		public ArticleQuantityColumnEntity GetArticleQuantityColumnEntity(ArticleQuantityType type)
		{
			//	Retourne une définition de type de quantité définie dans les réglages globaux.
			return this.articleQuantityColumns.Where (x => x.QuantityType == type).FirstOrDefault ();
		}

		public IEnumerable<ArticleQuantityType> GetEnabledArticleQuantityTypes()
		{
			var collection = this.IsDebug ? this.GetDebugArticleQuantityTypeEditionEnabled () : this.documentLogic.GetEnabledArticleQuantityTypes ();
			System.Diagnostics.Debug.Assert (collection != null);
			return collection;
		}

		public IEnumerable<ArticleQuantityType> GetPrintableArticleQuantityTypes()
		{
			var collection = this.documentLogic.GetPrintableArticleQuantityTypes ();
			System.Diagnostics.Debug.Assert (collection != null);
			return collection;
		}

		private IEnumerable<ArticleQuantityType> GetDebugArticleQuantityTypeEditionEnabled()
		{
			yield return ArticleQuantityType.Billed;			// facturé
			yield return ArticleQuantityType.Delayed;			// retardé
			yield return ArticleQuantityType.Expected;			// attendu
			yield return ArticleQuantityType.Shipped;			// livré
			yield return ArticleQuantityType.ShippedPreviously;	// livré précédemment
			yield return ArticleQuantityType.Information;		// information
		}


		public FormattedText PrimaryMailContactText
		{
			//	Retourne le texte multilignes de l'adresse principale.
			get
			{
				var mail = this.PrimaryMailContact;

				if (mail == null)
				{
					return FormattedText.Empty;
				}
				else
				{
					return mail.GetSummary ();
				}
			}
		}

		public FormattedText SecondaryMailContactText
		{
			//	Retourne le texte monoligne de l'adresse secondaire.
			get
			{
				var mail    = this.SecondaryMailContact;
				var primary = this.PrimaryMailContact;

				if (mail != null && mail != primary)
				{
					if (mail == this.BusinessDocument.BillToMailContact)
					{
						return FormattedText.Concat ("Adresse de facturation: ", mail.GetCompactSummary ());
					}
					else
					{
						return FormattedText.Concat ("Adresse de livraison: ", mail.GetCompactSummary ());
					}
				}

				return FormattedText.Empty;
			}
		}

		private MailContactEntity PrimaryMailContact
		{
			//	Retourne l'adresse de l'expéditeur, dite adresse principale.
			//	C'est l'adresse de facturation qui est utilisée pour tous les documents, sauf le
			//	BL (DeliveryNote) qui utilise l'adresse de livraison.
			get
			{
				return this.documentLogic.PrimaryMailContact;
			}
		}

		private MailContactEntity SecondaryMailContact
		{
			//	Retourne l'adresse secondaire.
			//	C'est l'adresse de livraison qui est utilisée pour tous les documents, sauf le
			//	BL (DeliveryNote) qui utilise l'adresse de facturation.
			get
			{
				return this.documentLogic.SecondaryMailContact;
			}
		}


		private AbstractDocumentLogic CreateDocumentLogic(DocumentType documentType)
		{
			switch (documentType)
			{
				case DocumentType.SalesQuote:
					return new SalesQuoteDocumentLogic (this.businessContext, this.documentMetadata);

				case DocumentType.OrderConfiguration:
					return new OrderConfigurationDocumentLogic (this.businessContext, this.documentMetadata);

				case DocumentType.OrderBooking:
					return new OrderBookingDocumentLogic (this.businessContext, this.documentMetadata);

				case DocumentType.OrderConfirmation:
					return new OrderConfirmationDocumentLogic (this.businessContext, this.documentMetadata);

				case DocumentType.ProductionOrder:
					return new ProductionOrderDocumentLogic (this.businessContext, this.documentMetadata);

				case DocumentType.ProductionChecklist:
					return new ProductionChecklistDocumentLogic (this.businessContext, this.documentMetadata);

				case DocumentType.ShipmentChecklist:
					return new ShipmentChecklistDocumentLogic (this.businessContext, this.documentMetadata);

				case DocumentType.DeliveryNote:
					return new DeliveryNoteDocumentLogic (this.businessContext, this.documentMetadata);

				case DocumentType.Invoice:
					return new InvoiceDocumentLogic (this.businessContext, this.documentMetadata);

				case DocumentType.InvoiceProForma:
					return new InvoiceProFormaDocumentLogic (this.businessContext, this.documentMetadata);

				case DocumentType.PaymentReminder:
					return new PaymentReminderDocumentLogic (this.businessContext, this.documentMetadata);

				case DocumentType.Receipt:
					return new ReceiptDocumentLogic (this.businessContext, this.documentMetadata);

				case DocumentType.CreditMemo:
					return new CreditMemoDocumentLogic (this.businessContext, this.documentMetadata);

				case DocumentType.QuoteRequest:
					return new QuoteRequestDocumentLogic (this.businessContext, this.documentMetadata);

				case DocumentType.PurchaseOrder:
					return new PurchaseOrderDocumentLogic (this.businessContext, this.documentMetadata);

				default:
					throw new System.NotSupportedException (string.Format ("{0} not supported", documentType.GetQualifiedName ()));
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
			var types = DocumentLogic.ParentDocumentTypes.Where (x => x.Contains (type)).FirstOrDefault ();

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
				yield return DocumentLogic.ParentDocumentTypes1;
				yield return DocumentLogic.ParentDocumentTypes2;
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
		static DocumentLogic()
		{
			//	Le constructeur statique est utilisé pour effectuer un auto-test à chaque exécution !
			System.Diagnostics.Debug.Assert (DocumentLogic.Compare (DocumentLogic.GetProcessParentDocumentTypes (DocumentType.SalesQuote)));
			System.Diagnostics.Debug.Assert (DocumentLogic.Compare (DocumentLogic.GetProcessParentDocumentTypes (DocumentType.OrderBooking),                                                                                                                DocumentType.SalesQuote));
			System.Diagnostics.Debug.Assert (DocumentLogic.Compare (DocumentLogic.GetProcessParentDocumentTypes (DocumentType.OrderConfirmation),                                                                                DocumentType.OrderBooking, DocumentType.SalesQuote));
			System.Diagnostics.Debug.Assert (DocumentLogic.Compare (DocumentLogic.GetProcessParentDocumentTypes (DocumentType.ProductionOrder),                                                  DocumentType.OrderConfirmation, DocumentType.OrderBooking, DocumentType.SalesQuote));
			System.Diagnostics.Debug.Assert (DocumentLogic.Compare (DocumentLogic.GetProcessParentDocumentTypes (DocumentType.ProductionChecklist),                DocumentType.ProductionOrder, DocumentType.OrderConfirmation, DocumentType.OrderBooking, DocumentType.SalesQuote));
			System.Diagnostics.Debug.Assert (DocumentLogic.Compare (DocumentLogic.GetProcessParentDocumentTypes (DocumentType.DeliveryNote),                                                     DocumentType.OrderConfirmation, DocumentType.OrderBooking, DocumentType.SalesQuote));
			System.Diagnostics.Debug.Assert (DocumentLogic.Compare (DocumentLogic.GetProcessParentDocumentTypes (DocumentType.Invoice),                               DocumentType.DeliveryNote, DocumentType.OrderConfirmation, DocumentType.OrderBooking, DocumentType.SalesQuote));
			System.Diagnostics.Debug.Assert (DocumentLogic.Compare (DocumentLogic.GetProcessParentDocumentTypes (DocumentType.PaymentReminder), DocumentType.Invoice, DocumentType.DeliveryNote, DocumentType.OrderConfirmation, DocumentType.OrderBooking, DocumentType.SalesQuote));
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


		private readonly BusinessContext		businessContext;
		private readonly DocumentMetadataEntity	documentMetadata;
		private readonly List<ArticleQuantityColumnEntity> articleQuantityColumns;

		private AbstractDocumentLogic			documentLogic;
	}
}
