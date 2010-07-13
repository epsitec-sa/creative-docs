//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Browser;

using System.Collections.Generic;


namespace Epsitec.Cresus.Core.Data
{
	public class InvoiceDocumentRepository : Repository
	{
		public InvoiceDocumentRepository(DataContext dataContext)
			: base (dataContext)
		{
		}


		public IEnumerable<InvoiceDocumentEntity> GetInvoiceDocumentsByExample(InvoiceDocumentEntity example)
		{
			return this.GetEntitiesByExample<InvoiceDocumentEntity> (example);
		}


		public IEnumerable<InvoiceDocumentEntity> GetInvoiceDocumentsByRequest(Request request)
		{
			return this.GetEntitiesByRequest<InvoiceDocumentEntity> (request);
		}


		public IEnumerable<InvoiceDocumentEntity> GetInvoiceDocumentsByExample(InvoiceDocumentEntity example, int index, int count)
		{
			return this.GetEntitiesByExample<InvoiceDocumentEntity> (example, index, count);
		}


		public IEnumerable<InvoiceDocumentEntity> GetInvoiceDocumentsByRequest(Request request, int index, int count)
		{
			return this.GetEntitiesByRequest<InvoiceDocumentEntity> (request, index, count);
		}


		public IEnumerable<InvoiceDocumentEntity> GetAllInvoiceDocuments()
		{
			InvoiceDocumentEntity example = this.CreateArticleDefinitionExample ();

			return this.GetInvoiceDocumentsByExample (example);
		}


		public IEnumerable<InvoiceDocumentEntity> GetAllInvoiceDocuments(int index, int count)
		{
			InvoiceDocumentEntity example = this.CreateArticleDefinitionExample ();

			return this.GetInvoiceDocumentsByExample (example, index, count);
		}



		public InvoiceDocumentEntity CreateArticleDefinitionExample()
		{
			return this.CreateExample<InvoiceDocumentEntity> ();
		}
	}
}
