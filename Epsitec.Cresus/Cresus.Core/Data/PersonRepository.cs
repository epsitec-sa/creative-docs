//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer;

using System.Collections.Generic;
using System.Linq;


namespace Epsitec.Cresus.Core.Data
{

	
	public class PersonRepository : Repository
	{

		public PersonRepository(DataContext dataContext) : base (dataContext)
		{
		}


		public IEnumerable<AbstractPersonEntity> GetAllPersons()
		{
			throw new System.NotImplementedException ();
		}


	}


}
