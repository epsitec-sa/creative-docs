using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Database;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Schema
{


	public sealed class EntityEngine
	{


		private EntityEngine(DbInfrastructure infrastructure, IEnumerable<Druid> entityTypeIds)
		{
			var typeEngine = new EntityTypeEngine (entityTypeIds);
			var schemaEngine = new EntitySchemaEngine (infrastructure, typeEngine);

			this.typeEngine = typeEngine;
			this.schemaEngine = schemaEngine;
		}


		internal EntitySchemaEngine SchemaEngine
		{
			get
			{
				return this.schemaEngine;
			}
		}


		internal EntityTypeEngine TypeEngine
		{
			get
			{
				return this.typeEngine;
			}
		}


		public static void Create(DbAccess access, IEnumerable<Druid> entityTypeIds)
		{
			access.ThrowIf (a => a.IsEmpty, "access is empty");
			entityTypeIds.ThrowIfNull ("entityTypeIds");

			using (DbInfrastructure infrastructure = new DbInfrastructure ())
			{
				infrastructure.AttachToDatabase (access);

				var relatedEntityTypeIds = EntityTypeEngine.GetRelatedEntityTypeIds (entityTypeIds).ToList ();

				var typeEngine = new EntityTypeEngine (relatedEntityTypeIds);
				var schemaBuilder = new EntitySchemaBuilder (typeEngine, infrastructure);

				schemaBuilder.RegisterSchema (relatedEntityTypeIds);
			}
		}


		public static bool Check(DbAccess access, IEnumerable<Druid> entityTypeIds)
		{
			access.ThrowIf (a => a.IsEmpty, "access is empty");
			entityTypeIds.ThrowIfNull ("entityTypeIds");

			using (DbInfrastructure infrastructure = new DbInfrastructure ())
			{
				infrastructure.AttachToDatabase (access);

				var relatedEntityTypeIds = EntityTypeEngine.GetRelatedEntityTypeIds (entityTypeIds).ToList ();

				var typeEngine = new EntityTypeEngine (relatedEntityTypeIds);
				var schemaBuilder = new EntitySchemaBuilder (typeEngine, infrastructure);

				return schemaBuilder.CheckSchema (relatedEntityTypeIds);
			}
		}


		public static void Update(DbAccess access, IEnumerable<Druid> entityTypeIds)
		{
			access.ThrowIf (a => a.IsEmpty, "access is empty");
			entityTypeIds.ThrowIfNull ("entityTypeIds");

			using (DbInfrastructure infrastructure = new DbInfrastructure ())
			{
				infrastructure.AttachToDatabase (access);

				var relatedEntityTypeIds = EntityTypeEngine.GetRelatedEntityTypeIds (entityTypeIds).ToList ();

				var typeEngine = new EntityTypeEngine (relatedEntityTypeIds);
				var schemaBuilder = new EntitySchemaBuilder (typeEngine, infrastructure);

				schemaBuilder.UpdateSchema (relatedEntityTypeIds);
			}
		}


		public static EntityEngine Connect(DbAccess access, IEnumerable<Druid> entityTypeIds)
		{
			access.ThrowIf (a => a.IsEmpty, "access is empty");
			entityTypeIds.ThrowIfNull ("entityTypeIds");

			using (DbInfrastructure infrastructure = new DbInfrastructure ())
			{
				infrastructure.AttachToDatabase (access);

				var relatedEntityTypeIds = EntityTypeEngine.GetRelatedEntityTypeIds (entityTypeIds);

				return new EntityEngine (infrastructure, relatedEntityTypeIds);
			}	
		}


		private readonly EntitySchemaEngine schemaEngine;


		private readonly EntityTypeEngine typeEngine;


	}


}
