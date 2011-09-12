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
		public LineInformations(DocumentItemAccessor documentItemAccessor, AbstractDocumentItemEntity abstractDocumentItemEntity, ArticleQuantityEntity articleQuantityEntity, int sublineIndex, DocumentItemAccessorError error = DocumentItemAccessorError.None)
		{
			this.DocumentItemAccessor       = documentItemAccessor;
			this.AbstractDocumentItemEntity = abstractDocumentItemEntity;
			this.ArticleQuantityEntity      = articleQuantityEntity;
			this.SublineIndex               = sublineIndex;
			this.Error                      = error;
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

		public DocumentItemAccessorError Error
		{
			get;
			internal set;
		}

		public FormattedText GetColumnContent(DocumentItemAccessorColumn columnA, DocumentItemAccessorColumn columnB)
		{
			var text = this.GetColumnContent (columnA);
			
			if (text.IsNullOrEmpty)
			{
				text = this.GetColumnContent (columnB);
			}
			
			return text;
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

		public DocumentItemAccessorError GetColumnError(DocumentItemAccessorColumn columnA, DocumentItemAccessorColumn columnB)
		{
			var error = this.GetColumnError (columnA);
			
			if (error == DocumentItemAccessorError.None)
			{
				error = this.GetColumnError (columnB);
			}
			
			return error;
		}

		public DocumentItemAccessorError GetColumnError(DocumentItemAccessorColumn column)
		{
			if (this.DocumentItemAccessor == null)
			{
				return DocumentItemAccessorError.None;
			}
			else
			{
				return this.DocumentItemAccessor.GetError (this.SublineIndex, column);
			}
		}

		public int SublineIndex
		{
			get;
			internal set;
		}

		public bool IsQuantity
		{
			get
			{
				return this.AbstractDocumentItemEntity is ArticleDocumentItemEntity &&
					   this.ArticleQuantityEntity != null &&
					   this.SublineIndex > 0;
			}
		}
	}
}
