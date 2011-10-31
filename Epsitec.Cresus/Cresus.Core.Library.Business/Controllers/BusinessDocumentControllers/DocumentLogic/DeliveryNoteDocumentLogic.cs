//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
	/// <summary>
	/// Bulletin de livraison.
	/// </summary>
	public class DeliveryNoteDocumentLogic : AbstractDocumentLogic
	{
		public DeliveryNoteDocumentLogic(BusinessContext businessContext, DocumentMetadataEntity documentMetadataEntity)
			: base (businessContext, documentMetadataEntity)
		{
		}


		public override bool IsLinesEditionEnabled
		{
			get
			{
				return false;
			}
		}

		public override bool IsArticleParametersEditionEnabled
		{
			get
			{
				return false;
			}
		}

		public override bool IsTextEditionEnabled
		{
			get
			{
				return true;
			}
		}

		public override bool IsPriceEditionEnabled
		{
			get
			{
				return false;
			}
		}

		public override bool IsDiscountEditionEnabled
		{
			get
			{
				return false;
			}
		}


		public override ArticleQuantityType MainArticleQuantityType
		{
			get
			{
				return ArticleQuantityType.Ordered;
			}
		}

		public override bool IsMainArticleQuantityEnabled
		{
			get
			{
				return false;
			}
		}

		public override IEnumerable<ArticleQuantityType> GetEnabledArticleQuantityTypes()
		{
			yield return ArticleQuantityType.Shipped;			// livré
			yield return ArticleQuantityType.Delayed;			// retardé
			yield return ArticleQuantityType.Expected;			// attendu
		}

		public override IEnumerable<ArticleQuantityType> GetPrintableArticleQuantityTypes()
		{
			yield return ArticleQuantityType.Shipped;			// livré
			yield return ArticleQuantityType.ShippedPreviously;	// livré précédemment
			yield return ArticleQuantityType.Delayed;			// retardé
			yield return ArticleQuantityType.Expected;			// attendu
		}


		public override MailContactEntity MailContact
		{
			//	Retourne l'adresse de l'expéditeur.
			//	Sur un bulletin de livraison, c'est l'adresse de livraison qu'il faut mettre !
			get
			{
				var businessDocument = this.BusinessDocument;

				if (businessDocument == null)
				{
					return null;
				}
				else
				{
					return businessDocument.ShipToMailContact;
				}
			}
		}
	}
}
