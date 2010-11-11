//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Finance
{
	public class DocumentPriceCalculator
	{
		public DocumentPriceCalculator(BusinessDocumentEntity document)
		{
			this.document = document;
		}


		public void Update()
		{
			var articles = this.GetArticles ().ToList ();
			var totals   = this.GetTotals ().ToList ();
			
			
		}

		private IEnumerable<ArticlePriceCalculator> GetArticles()
		{
			return from line in this.document.Lines.OfType<ArticleDocumentItemEntity> ()
				   orderby line.GroupIndex
				   select new ArticlePriceCalculator (this.document, line);
		}

		private IEnumerable<TotalPriceCalculator> GetTotals()
		{
			return this.document.Lines
				.OfType<TotalDocumentItemEntity> ()
				.Select ((line, index) => new { Line = line, Index = index })
				.OrderBy (x => x.Line.GroupIndex)
				.ThenBy (x => x.Index)
				.Select (x => new TotalPriceCalculator (this.document, x.Line));
		}
		
		private readonly BusinessDocumentEntity		document;
	}
}
