//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Loader;

using System.Collections.Generic;


namespace Epsitec.Cresus.Core.Data
{
	public class ArticleCategoryRepository : Repository<ArticleCategoryEntity>
	{
		public ArticleCategoryRepository(DataContext dataContext)
			: base (dataContext)
		{
		}


		public IEnumerable<ArticleCategoryEntity> GetArticleCategoriesByExample(ArticleCategoryEntity example)
		{
			return this.GetEntitiesByExample<ArticleCategoryEntity> (example);
		}


		public IEnumerable<ArticleCategoryEntity> GetArticleCategoriesByRequest(Request request)
		{
			return this.GetEntitiesByRequest<ArticleCategoryEntity> (request);
		}


		public IEnumerable<ArticleCategoryEntity> GetArticleCategoriesByExample(ArticleCategoryEntity example, int index, int count)
		{
			return this.GetEntitiesByExample<ArticleCategoryEntity> (example, index, count);
		}


		public IEnumerable<ArticleCategoryEntity> GetArticleCategoriesByRequest(Request request, int index, int count)
		{
			return this.GetEntitiesByRequest<ArticleCategoryEntity> (request, index, count);
		}


		public IEnumerable<ArticleCategoryEntity> GetAllArticleCategories()
		{
			ArticleCategoryEntity example = this.CreateArticleCategoryExample ();

			return this.GetArticleCategoriesByExample (example);
		}


		public IEnumerable<ArticleCategoryEntity> GetAllArticleCategories(int index, int count)
		{
			ArticleCategoryEntity example = this.CreateArticleCategoryExample ();

			return this.GetArticleCategoriesByExample (example, index, count);
		}



		public ArticleCategoryEntity CreateArticleCategoryExample()
		{
			return this.CreateExample<ArticleCategoryEntity> ();
		}
	}
}
