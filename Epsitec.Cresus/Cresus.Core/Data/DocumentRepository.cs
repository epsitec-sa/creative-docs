//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Entities;


using Epsitec.Cresus.DataLayer.Loader;
using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;


namespace Epsitec.Cresus.Core.Data
{
	public class DocumentRepository : Repository
	{
		public DocumentRepository(DataContext dataContext)
			: base (dataContext)
		{
		}


		public IEnumerable<DocumentEntity> GetDocumentsByExample(DocumentEntity example)
		{
			return this.GetEntitiesByExample<DocumentEntity> (example);
		}


		public IEnumerable<DocumentEntity> GetDocumentsByRequest(Request request)
		{
			return this.GetEntitiesByRequest<DocumentEntity> (request);
		}


		public IEnumerable<DocumentEntity> GetDocumentsByExample(DocumentEntity example, int index, int count)
		{
			return this.GetEntitiesByExample<DocumentEntity> (example, index, count);
		}


		public IEnumerable<DocumentEntity> GetDocumentsByRequest(Request request, int index, int count)
		{
			return this.GetEntitiesByRequest<DocumentEntity> (request, index, count);
		}


		public IEnumerable<DocumentEntity> GetAllDocuments()
		{
			DocumentEntity example = this.CreateArticleDefinitionExample ();

			return this.GetDocumentsByExample (example);
		}


		public IEnumerable<DocumentEntity> GetAllDocuments(int index, int count)
		{
			DocumentEntity example = this.CreateArticleDefinitionExample ();

			return this.GetDocumentsByExample (example, index, count);
		}



		public DocumentEntity CreateArticleDefinitionExample()
		{
			return this.CreateExample<DocumentEntity> ();
		}
	}
}
