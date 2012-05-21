//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.BusinessDocumentControllers
{
	public abstract class AbstractDocumentLogic
	{
		protected AbstractDocumentLogic(BusinessContext businessContext, DocumentMetadataEntity documentMetadataEntity)
		{
			this.businessContext        = businessContext;
			this.documentMetadataEntity = documentMetadataEntity;
		}


		public virtual bool IsLinesEditionEnabled
		{
			//	Indique si on peut éditer librement toutes les lignes, c'est-à-dire en créer, modifier,
			//	déplacer et supprimer.
			get
			{
				return false;
			}
		}

		public virtual bool IsArticleParametersEditionEnabled
		{
			//	Indique si on peut choisir ou modifier un article.
			get
			{
				return false;
			}
		}

		public virtual bool IsTextEditionEnabled
		{
			//	Indique si on peut éditer les textes.
			get
			{
				return false;
			}
		}

		public virtual bool IsMyEyesOnlyEditionEnabled
		{
			//	Indique si on ne peut éditer que les textes 'MyEyesOnly', c'est-à-dire pour les documents
			//	interne à l'entreprise.
			get
			{
				return false;
			}
		}

		public virtual bool IsPriceEditionEnabled
		{
			//	Indique si on peut éditer les prix.
			get
			{
				return false;
			}
		}

		public virtual bool IsDiscountEditionEnabled
		{
			//	Indique si on peut éditer les rabais.
			get
			{
				return false;
			}
		}

		public virtual ArticleQuantityType MainArticleQuantityType
		{
			//	Retourne le type de quantité principal, c'est-à-dire celui qui est édité avec l'article.
			get
			{
				return ArticleQuantityType.None;
			}
		}

		public virtual bool IsMainArticleQuantityEnabled
		{
			//	Indique si la quantité principal est éditable.
			get
			{
				return true;
			}
		}


		//	Retourne la liste des types de quantité éditables.
		public abstract IEnumerable<ArticleQuantityType> GetEnabledArticleQuantityTypes();

		//	Retourne la liste des types de quantité imprimables.
		//	La première est la quantité principale.
		public abstract IEnumerable<ArticleQuantityType> GetPrintableArticleQuantityTypes();


		public virtual MailContactEntity PrimaryMailContact
		{
			//	Retourne l'adresse de l'expéditeur, dite adresse principale.
			//	C'est l'adresse de facturation qui est utilisée pour tous les documents, sauf le
			//	BL (DeliveryNote) qui utilise l'adresse de livraison.
			get
			{
				var businessDocument = this.BusinessDocument;

				if (businessDocument == null)
				{
					return null;
				}
				else
				{
					return businessDocument.BillToMailContact;
				}
			}
		}

		public MailContactEntity SecondaryMailContact
		{
			//	Retourne l'adresse secondaire.
			//	C'est l'adresse de livraison qui est utilisée pour tous les documents, sauf le
			//	BL (DeliveryNote) qui utilise l'adresse de facturation.
			get
			{
				var businessDocument = this.BusinessDocument;
				var mailContact = this.PrimaryMailContact;

				if (businessDocument == null || mailContact == null)
				{
					return null;
				}
				else
				{
					if (mailContact == businessDocument.BillToMailContact)
					{
						return businessDocument.ShipToMailContact;
					}
					else
					{
						return businessDocument.BillToMailContact;
					}
				}
			}
		}


		protected BusinessDocumentEntity BusinessDocument
		{
			get
			{
				var metadata = this.documentMetadataEntity;

				if (metadata.IsNull ())
				{
					return null;
				}
				else
				{
					return metadata.BusinessDocument as BusinessDocumentEntity;
				}
			}
		}


		protected readonly BusinessContext						businessContext;
		protected readonly DocumentMetadataEntity				documentMetadataEntity;
	}
}
