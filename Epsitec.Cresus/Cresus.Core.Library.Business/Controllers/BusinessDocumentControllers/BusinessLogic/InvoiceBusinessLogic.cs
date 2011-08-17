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
				return this.IsDirectInvoice;
			}
		}

		public override bool IsArticleParametersEditionEnabled
		{
			get
			{
				return this.IsDirectInvoice;
			}
		}

		public override bool IsTextEditionEnabled
		{
			get
			{
				return this.IsDirectInvoice;
			}
		}

		public override bool IsPriceEditionEnabled
		{
			get
			{
				return this.IsDirectInvoice;
			}
		}

		public override bool IsDiscountEditionEnabled
		{
			get
			{
				return this.IsDirectInvoice;
			}
		}


		public override IEnumerable<ArticleQuantityType> ArticleQuantityTypeEditionEnabled
		{
			get
			{
				yield return ArticleQuantityType.Billed;				// facturé
			}
		}


		private bool IsDirectInvoice
		{
			get
			{
				BusinessDocumentEntity document = this.documentMetadataEntity.BusinessDocument as BusinessDocumentEntity;

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
}
