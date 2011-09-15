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
	/// Choix des options pour commande.
	/// </summary>
	public class OrderConfigurationDocumentLogic : AbstractDocumentLogic
	{
		public OrderConfigurationDocumentLogic(BusinessContext businessContext, DocumentMetadataEntity documentMetadataEntity)
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
				return true;
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
				return true;
			}
		}

		public override bool IsDiscountEditionEnabled
		{
			get
			{
				return true;
			}
		}


		public override ArticleQuantityType MainArticleQuantityType
		{
			get
			{
				return ArticleQuantityType.None;
			}
		}

		public override IEnumerable<ArticleQuantityType> GetEnabledArticleQuantityTypes()
		{
			yield return ArticleQuantityType.Delayed;
			// retardé
			yield return ArticleQuantityType.Expected;
			// attendu
		}

		public override IEnumerable<ArticleQuantityType> GetPrintableArticleQuantityTypes()
		{
			yield return ArticleQuantityType.Ordered;
			// commandé
			yield return ArticleQuantityType.Delayed;
			// retardé
			yield return ArticleQuantityType.Expected;
			// attendu
		}
	}
}
