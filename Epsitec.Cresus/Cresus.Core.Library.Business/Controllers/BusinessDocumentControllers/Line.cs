//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Library.Business.ContentAccessors;

using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.BusinessDocumentControllers
{
	public sealed class Line
	{
		//	Un article peut être composé des lignes suivantes:
		//		                             SublineIndex QuantityIndex ArticleQuantity
		//		Désignation de l'article           0           0            entity
		//		Premier rabais                     1                        null
		//		Deuxième rabais                    2                        null
		//		Première quantité différée         3           1            entity
		//		Deuxième quantité différée         4           2            entity

		public Line(DocumentItemAccessor documentItemAccessor, AbstractDocumentItemEntity abstractDocumentItem, ArticleQuantityEntity articleQuantity, int sublineIndex = 0, int quantityIndex = 0, DocumentItemAccessorError error = DocumentItemAccessorError.None)
		{
			this.DocumentItemAccessor = documentItemAccessor;
			this.DocumentItem         = abstractDocumentItem;
			this.ArticleQuantity      = articleQuantity;
			this.SublineIndex         = sublineIndex;
			this.QuantityIndex        = quantityIndex;
			this.Error                = error;
		}

		
		public AbstractDocumentItemEntity		DocumentItem
		{
			get;
			internal set;
		}

		public ArticleQuantityEntity			ArticleQuantity
		{
			get;
			internal set;
		}

		public DocumentItemAccessor				DocumentItemAccessor
		{
			get;
			internal set;
		}

		public int								SublineIndex
		{
			get;
			internal set;
		}

		public int								QuantityIndex
		{
			get;
			internal set;
		}

		public bool								IsQuantity
		{
			get
			{
				return this.DocumentItem is ArticleDocumentItemEntity &&
					   this.ArticleQuantity != null &&
					   this.QuantityIndex > 0;
			}
		}

		public DocumentItemAccessorError		Error
		{
			get;
			internal set;
		}

		
		public FormattedText GetColumnContent(DocumentItemAccessorColumn columnA, DocumentItemAccessorColumn columnB)
		{
			var text = this.GetColumnContent (columnA);

			if (text.IsNullOrEmpty ())
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
	}
}
