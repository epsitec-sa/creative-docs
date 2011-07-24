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
	/// Check-list de production.
	/// </summary>
	public class ProductionChecklistBusinessLogic : AbstractDocumentBusinessLogic
	{
		public ProductionChecklistBusinessLogic(BusinessContext businessContext, DocumentMetadataEntity documentMetadataEntity)
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
				return false;
			}
		}

		public override bool IsMyEyesOnlyEditionEnabled
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
			//	Retourne le type de la quantité "principale" en fonction du type du document en cours.
			//	C'est cette quantité qui peuple la colonne "quantité".
			get
			{
				return ArticleQuantityType.Ordered;
			}
		}

		public override IEnumerable<ArticleQuantityType> ArticleQuantityTypeEditionEnabled
		{
			get
			{
				return null;
			}
		}
	}
}
