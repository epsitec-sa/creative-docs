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
	public class InvoiceBusinessLogic : AbstractDocumentBusinessLogic
	{
		public InvoiceBusinessLogic(BusinessContext businessContext, DocumentMetadataEntity documentMetadataEntity)
			: base (businessContext, documentMetadataEntity)
		{
		}


		public override bool IsLinesEditionEnabled
		{
			get
			{
				return InvoiceBusinessLogic.IsDirectInvoice (this.documentMetadataEntity);
			}
		}

		public override bool IsArticleParametersEditionEnabled
		{
			get
			{
				return InvoiceBusinessLogic.IsDirectInvoice (this.documentMetadataEntity);
			}
		}

		public override bool IsTextEditionEnabled
		{
			get
			{
				return InvoiceBusinessLogic.IsDirectInvoice (this.documentMetadataEntity);
			}
		}

		public override bool IsPriceEditionEnabled
		{
			get
			{
				return InvoiceBusinessLogic.IsDirectInvoice (this.documentMetadataEntity);
			}
		}

		public override bool IsDiscountEditionEnabled
		{
			get
			{
				return InvoiceBusinessLogic.IsDirectInvoice (this.documentMetadataEntity);
			}
		}


		public override ArticleQuantityType MainArticleQuantityType
		{
			get
			{
				if (InvoiceBusinessLogic.IsDirectInvoice (this.documentMetadataEntity))
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
				if (InvoiceBusinessLogic.IsDirectInvoice (this.documentMetadataEntity))
				{
					// rien
				}
				else
				{
					yield return ArticleQuantityType.Billed;				// facturé
				}
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
