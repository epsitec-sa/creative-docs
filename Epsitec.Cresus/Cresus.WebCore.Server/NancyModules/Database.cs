using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Business;

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


		public string Name
		{
			get;
			set;
		}


		public string CssClass
		{
			get;
			set;
		}


		public abstract int GetCount(BusinessContext businessContext);


		public abstract IEnumerable<AbstractEntity> GetEntities(BusinessContext businessContext, int skip, int take);


		public abstract AbstractEntity Create(BusinessContext businessContext);


	}


}