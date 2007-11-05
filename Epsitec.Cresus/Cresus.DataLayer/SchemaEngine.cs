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

		internal string GetTableName(Druid entityId)
		{
			Caption entityCaption = this.resourceManager.GetCaption (entityId);
			string  entityDruid   = Druid.ToFullString (entityId.ToLong ());
			string  entityName    = entityCaption.Name;

			return string.Concat ("@", entityDruid, ":", entityName);
		}

		internal string GetTypeName(Druid typeId)
		{
			Caption typeCaption = this.resourceManager.GetCaption (typeId);
			string typeDruid   = Druid.ToFullString (typeId.ToLong ());
			string typeName    = typeCaption.Name;

			return string.Concat ("@", typeDruid);
		}

		internal string GetColumnName(Druid columnId)
		{
			Caption columnCaption = this.resourceManager.GetCaption (columnId);
			string  columnDruid   = Druid.ToFullString (columnId.ToLong ());
			string  columnName    = columnCaption.Name;

			return string.Concat ("@", columnDruid, ":", columnCaption);
		}


		DbInfrastructure infrastructure;
		DbContext context;
		ResourceManager resourceManager;
	}
}
