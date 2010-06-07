using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.DataLayer;

using System.Collections.Generic;



namespace Epsitec.Cresus.Core.Data
{


	public class MailContactRepository : AbstractContactRepository
	{


		public MailContactRepository(DataContext dataContext) : base (dataContext)
		{
		}


	}


}
