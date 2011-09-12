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
	/// Facture.
	/// </summary>
	public class InvoiceDocumentLogic : AbstractDocumentLogic
	{
		public InvoiceDocumentLogic(BusinessContext businessContext, DocumentMetadataEntity documentMetadataEntity)
			: base (businessContext, documentMetadataEntity)
		{
		}


		public override bool IsLinesEditionEnabled
		{
			get
			{
				return this.IsDirect;
			}
		}

		public override bool IsArticleParametersEditionEnabled
		{
			get
			{
				return this.IsDirect;
			}
		}

		public override bool IsTextEditionEnabled
		{
			get
			{
				return this.IsDirect;
			}
		}

		public override bool IsPriceEditionEnabled
		{
			get
			{
				return this.IsDirect;
			}
		}

		public override bool IsDiscountEditionEnabled
		{
			get
			{
				return this.IsDirect;
			}
		}


		public override ArticleQuantityType MainArticleQuantityType
		{
			get
			{
				if (this.IsDirect)
				{
					return ArticleQuantityType.Billed;  // facturé
				}
				else
				{
					return ArticleQuantityType.None;
				}
			}
		}

		public override IEnumerable<ArticleQuantityType> EnabledArticleQuantityTypes
		{
			get
			{
				if (this.IsDirect)
				{
					// rien
				}
				else
				{
					yield return ArticleQuantityType.Billed;				// facturé
				}
			}
		}

		public override IEnumerable<ArticleQuantityType> PrintableArticleQuantityTypes
		{
			get
			{
				yield return ArticleQuantityType.Billed;				// facturé

				yield return ArticleQuantityType.Shipped;				// livré
				yield return ArticleQuantityType.ShippedPreviously;		// livré précédemment
				yield return ArticleQuantityType.Delayed;				// retardé
				yield return ArticleQuantityType.Expected;				// attendu
			}
		}


		private bool IsDirect
		{
			get
			{
				return InvoiceDocumentLogic.IsDirectInvoice (this.documentMetadataEntity);
			}
		}

		public static bool IsDirectInvoice(DocumentMetadataEntity documentMetadata)
		{
			if (documentMetadata.DocumentCategory.DocumentType != DocumentType.Invoice)
			{
				return false;
			}

			BusinessDocumentEntity document = documentMetadata.BusinessDocument as BusinessDocumentEntity;

			if (document == null)
			{
				return false;
			}
			else
			{
				return string.IsNullOrEmpty (document.BaseDocumentCode);
			}
		}
	}
}
