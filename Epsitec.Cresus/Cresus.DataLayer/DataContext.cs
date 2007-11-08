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
	public class DataContext
	{
		public DataContext(DbInfrastructure infrastructure)
		{
			this.infrastructure = infrastructure;
			this.schemaEngine = new SchemaEngine (this.infrastructure);
			this.entityContext = EntityContext.Current;
		}

		public SchemaEngine SchemaEngine
		{
			get
			{
				return this.schemaEngine;
			}
		}

		public EntityContext EntityContext
		{
			get
			{
				return this.entityContext;
			}
		}

		DbInfrastructure infrastructure;
		SchemaEngine schemaEngine;
		EntityContext entityContext;
	}
}
