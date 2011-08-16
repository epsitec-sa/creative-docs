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
			this.masterEntitiesDirty = true;
		}


		public override bool IsLinesEditionEnabled
		{
			get
			{
				return this.IsSoloInvoice;
			}
		}

		public override bool IsArticleParametersEditionEnabled
		{
			get
			{
				return this.IsSoloInvoice;
			}
		}

		public override bool IsTextEditionEnabled
		{
			get
			{
				return this.IsSoloInvoice;
			}
		}

		public override bool IsPriceEditionEnabled
		{
			get
			{
				return this.IsSoloInvoice;
			}
		}

		public override bool IsDiscountEditionEnabled
		{
			get
			{
				return this.IsSoloInvoice;
			}
		}


		public override IEnumerable<ArticleQuantityType> ArticleQuantityTypeEditionEnabled
		{
			get
			{
				if (this.IsSoloInvoice)
				{
					yield return ArticleQuantityType.Ordered;
				}
				else
				{
					yield return ArticleQuantityType.Billed;				// facturé
				}
			}
		}


		private bool IsSoloInvoice
		{
			get
			{
				return this.MasterEntity == null;
			}
		}

		private AbstractEntity MasterEntity
		{
			get
			{
				if (this.masterEntitiesDirty)
				{
					this.masterEntities = this.businessContext.GetMasterEntities ();
					this.masterEntitiesDirty = false;
				}

				if (this.masterEntities.Count () == 1)
				{
					return this.masterEntities.First ();
				}
				else
				{
					return null;
				}
			}
		}


		private bool							masterEntitiesDirty;
		private IEnumerable<AbstractEntity>		masterEntities;
	}
}
