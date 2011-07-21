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
	public abstract class AbstractDocumentBusinessLogic
	{
		public AbstractDocumentBusinessLogic(BusinessContext businessContext, DocumentMetadataEntity documentMetadataEntity)
		{
			this.businessContext        = businessContext;
			this.documentMetadataEntity = documentMetadataEntity;
		}


		public virtual bool IsLinesEditionEnabled
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsArticleParametersEditionEnabled
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsTextEditionEnabled
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsPriceEditionEnabled
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsDiscountEditionEnabled
		{
			get
			{
				return false;
			}
		}


		public virtual IEnumerable<ArticleQuantityType> ArticleQuantityTypeEditionEnabled
		{
			get
			{
				return null;
			}
		}


		protected readonly BusinessContext						businessContext;
		protected readonly DocumentMetadataEntity				documentMetadataEntity;
	}
}
