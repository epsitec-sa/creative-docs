//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Library.Business.ContentAccessors;

using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.BusinessDocumentControllers
{
	public class LineInformations
	{
		public LineInformations(DocumentItemAccessor documentItemAccessor, AbstractDocumentItemEntity abstractDocumentItemEntity, ArticleQuantityEntity articleQuantityEntity, int sublineIndex)
		{
			this.DocumentItemAccessor       = documentItemAccessor;
			this.AbstractDocumentItemEntity = abstractDocumentItemEntity;
			this.ArticleQuantityEntity      = articleQuantityEntity;
			this.SublineIndex               = sublineIndex;
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

		public DocumentItemAccessor DocumentItemAccessor
		{
			get;
			internal set;
		}

		public FormattedText GetColumnContent(DocumentItemAccessorColumn column)
		{
			if (this.DocumentItemAccessor == null)
			{
				return null;
			}
			else
			{
				return this.DocumentItemAccessor.GetContent (this.SublineIndex, column).Lines.FirstOrDefault ();
			}
		}

		public int SublineIndex
		{
			get;
			internal set;
		}
	}
}
