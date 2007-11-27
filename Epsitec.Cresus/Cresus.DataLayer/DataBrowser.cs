//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Helpers;

using System.Collections.Generic;

namespace Epsitec.Cresus.DataLayer
{
	public class DataBrowser
	{
		public DataBrowser(DbInfrastructure infrastructure)
		{
			this.infrastructure = infrastructure;
			this.schemaEngine = SchemaEngine.GetSchemaEngine (this.infrastructure) ?? new SchemaEngine (this.infrastructure);
		}


		public void QueryByExample<T>(T example) where T : AbstractEntity, new ()
		{
			this.QueryByExample ((AbstractEntity) example);
		}

		public void QueryByExample(AbstractEntity example)
		{
			
		}

		
		readonly DbInfrastructure infrastructure;
		readonly SchemaEngine schemaEngine;
	}
}
