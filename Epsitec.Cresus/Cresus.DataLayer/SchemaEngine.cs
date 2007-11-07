//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Helpers;

using System.Collections.Generic;

namespace Epsitec.Cresus.DataLayer
{
	public class SchemaEngine
	{
		public SchemaEngine(DbInfrastructure infrastructure)
		{
			this.infrastructure  = infrastructure;
			this.context         = this.infrastructure.DefaultContext;
			this.resourceManager = this.context.ResourceManager;
		}


		public DbInfrastructure Infrastructure
		{
			get
			{
				return this.infrastructure;
			}
		}

		public DbTable CreateTableDefinition(Druid entityId)
		{
			SchemaEngineTableBuilder builder = new SchemaEngineTableBuilder (this);

			using (builder.BeginTransaction ())
			{
				builder.Add (entityId);
				builder.CommitTransaction ();
			}

			return builder.GetFirstTable ();
		}



		internal StructuredType GetEntityType(Druid entityId)
		{
			return TypeRosetta.CreateTypeObject (this.resourceManager, entityId) as StructuredType;
		}


		DbInfrastructure infrastructure;
		DbContext context;
		ResourceManager resourceManager;
	}
}
