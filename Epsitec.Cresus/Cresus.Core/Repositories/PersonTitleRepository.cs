//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Loader;
using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;


namespace Epsitec.Cresus.Core.Repositories
{
	public class PersonTitleRepository : Repository<PersonTitleEntity>
	{
		public PersonTitleRepository(CoreData data, DataContext context = null)
			: base (data, context, Data.DataLifetimeExpectancy.Stable)
		{
		}
	}
}
