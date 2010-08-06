//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Loader;

using System.Collections.Generic;


namespace Epsitec.Cresus.Core.Data
{
	public class ArticleGroupRepository : Repository
	{
		public ArticleGroupRepository(DataContext dataContext)
			: base (dataContext)
		{
		}


		public IEnumerable<ArticleGroupEntity> GetArticleGroupsByExample(ArticleGroupEntity example)
		{
			return this.GetEntitiesByExample<ArticleGroupEntity> (example);
		}


		public IEnumerable<ArticleGroupEntity> GetArticleGroupsByRequest(Request request)
		{
			return this.GetEntitiesByRequest<ArticleGroupEntity> (request);
		}


		public IEnumerable<ArticleGroupEntity> GetArticleGroupsByExample(ArticleGroupEntity example, int index, int count)
		{
			return this.GetEntitiesByExample<ArticleGroupEntity> (example, index, count);
		}


		public IEnumerable<ArticleGroupEntity> GetArticleGroupsByRequest(Request request, int index, int count)
		{
			return this.GetEntitiesByRequest<ArticleGroupEntity> (request, index, count);
		}


		public IEnumerable<ArticleGroupEntity> GetAllArticleGroups()
		{
			ArticleGroupEntity example = this.CreateArticleGroupExample ();

			return this.GetArticleGroupsByExample (example);
		}


		public IEnumerable<ArticleGroupEntity> GetAllArticleGroups(int index, int count)
		{
			ArticleGroupEntity example = this.CreateArticleGroupExample ();

			return this.GetArticleGroupsByExample (example, index, count);
		}



		public ArticleGroupEntity CreateArticleGroupExample()
		{
			return this.CreateExample<ArticleGroupEntity> ();
		}
	}
}
