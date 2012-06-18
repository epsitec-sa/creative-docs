using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.NancyModules
{
	
	
	public abstract class Database
	{


		public string Title
		{
			get;
			set;
		}


		public string DatabaseName
		{
			get;
			set;
		}


		public string CssClass
		{
			get;
			set;
		}


		public abstract int GetCount(DataContext dataContext);


		public abstract IEnumerable<AbstractEntity> GetEntities(DataContext dataContext, int skip, int take);


	}


}