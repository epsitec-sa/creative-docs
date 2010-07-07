//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Browser;

using System.Collections.Generic;


namespace Epsitec.Cresus.Core.Data
{
	public class ArticleDefinitionRepository : Repository
	{
		public ArticleDefinitionRepository(DataContext dataContext)
			: base (dataContext)
		{
		}


		public IEnumerable<ArticleDefinitionEntity> GetArticleDefinitionsByExample(ArticleDefinitionEntity example)
		{
			return this.GetEntitiesByExample<ArticleDefinitionEntity> (example);
		}


		public IEnumerable<ArticleDefinitionEntity> GetArticleDefinitionsByRequest(Request request)
		{
			return this.GetEntitiesByRequest<ArticleDefinitionEntity> (request);
		}


		public IEnumerable<ArticleDefinitionEntity> GetArticleDefinitionsByExample(ArticleDefinitionEntity example, int index, int count)
		{
			return this.GetEntitiesByExample<ArticleDefinitionEntity> (example, index, count);
		}


		public IEnumerable<ArticleDefinitionEntity> GetArticleDefinitionsByRequest(Request request, int index, int count)
		{
			return this.GetEntitiesByRequest<ArticleDefinitionEntity> (request, index, count);
		}


		public IEnumerable<ArticleDefinitionEntity> GetAllArticleDefinitions()
		{
			ArticleDefinitionEntity example = this.CreateArticleDefinitionExample ();

			return this.GetArticleDefinitionsByExample (example);
		}


		public IEnumerable<ArticleDefinitionEntity> GetAllArticleDefinitions(int index, int count)
		{
			ArticleDefinitionEntity example = this.CreateArticleDefinitionExample ();

			return this.GetArticleDefinitionsByExample (example, index, count);
		}



		public ArticleDefinitionEntity CreateArticleDefinitionExample()
		{
			return this.CreateExample<ArticleDefinitionEntity> ();
		}
	}
}
