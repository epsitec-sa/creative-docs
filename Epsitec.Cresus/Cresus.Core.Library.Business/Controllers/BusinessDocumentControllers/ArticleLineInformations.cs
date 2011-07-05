//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Cresus.Core.Entities;

namespace Epsitec.Cresus.Core.Controllers.BusinessDocumentControllers
{
	public class ArticleLineInformations
	{
		public ArticleLineInformations(AbstractDocumentItemEntity abstractDocumentItemEntity, ArticleQuantityEntity articleQuantityEntity, int lineIndex, int quantityIndex)
		{
			this.AbstractDocumentItemEntity = abstractDocumentItemEntity;
			this.ArticleQuantityEntity      = articleQuantityEntity;
			this.LineIndex                  = lineIndex;
			this.QuantityIndex              = quantityIndex;
		}

		public AbstractDocumentItemEntity AbstractDocumentItemEntity
		{
			get;
			internal set;
		}

		public ArticleQuantityEntity ArticleQuantityEntity
		{
			get;
			internal set;
		}

		public int LineIndex
		{
			get;
			internal set;
		}

		public int QuantityIndex
		{
			get;
			internal set;
		}
	}
}
