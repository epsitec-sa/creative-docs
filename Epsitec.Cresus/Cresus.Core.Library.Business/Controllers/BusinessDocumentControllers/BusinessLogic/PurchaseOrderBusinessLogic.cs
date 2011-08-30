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
	/// Confirmation de commande.
	/// </summary>
	public class PurchaseOrderBusinessLogic : AbstractDocumentBusinessLogic
	{
		public PurchaseOrderBusinessLogic(BusinessContext businessContext, DocumentMetadataEntity documentMetadataEntity)
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
				return ArticleQuantityType.Ordered;						// commandé
			}
		}

		public override IEnumerable<ArticleQuantityType> EnabledArticleQuantityTypes
		{
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

		public override IEnumerable<ArticleQuantityType> PrintableArticleQuantityTypes
		{
			get
			{
				yield return ArticleQuantityType.Ordered;				// commandé

				yield return ArticleQuantityType.Billed;				// facturé
				yield return ArticleQuantityType.Delayed;				// retardé
				yield return ArticleQuantityType.Expected;				// attendu
				yield return ArticleQuantityType.Shipped;				// livré
				yield return ArticleQuantityType.ShippedPreviously;		// livré précédemment
			}
		}


		public override IEnumerable<DocumentType> ProcessParentDocumentTypes
		{
			get
			{
				yield break;
			}
		}
	}
}
