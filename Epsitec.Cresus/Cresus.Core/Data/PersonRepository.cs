//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Data
{
	
	class PersonRepository : Repository
	{

		public PersonRepository(DbInfrastructure datadbInfrastructure, DataContext dataContext)
			: base (datadbInfrastructure, dataContext)
		{
		}

		public IEnumerable<AbstractPersonEntity> GetAllPersons()
		{
			throw new System.NotImplementedException ();
		
		}
	}

}
