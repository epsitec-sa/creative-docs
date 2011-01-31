//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.DataLayer.Loader;
using Epsitec.Cresus.DataLayer.Context;

using System.Linq;
using System.Collections.Generic;

namespace Epsitec.Cresus.Core.Repositories
{
	public class DocumentCategoryRepository : Repository<DocumentCategoryEntity>
	{
		public DocumentCategoryRepository(CoreData data, DataContext context = null)
			: base (data, context)
		{
		}

		public IEnumerable<DocumentCategoryEntity> Find(DocumentType type)
		{
			var example = this.CreateExample ();

			example.IsArchive    = false;
			example.DocumentType = type;

			return this.GetByExample (example).OrderBy (x => x.Rank);
		}
	}
}
