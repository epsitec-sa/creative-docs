using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Database;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Schema
{


	public sealed class EntityEngine
	{


		// TODO Comment this class
		// Marc


		private EntityEngine(DbInfrastructure infrastructure, IEnumerable<Druid> entityTypeIds)
		{
			var serviceTableNames = ServiceSchemaBuilder.GetServiceTableNames ();

			var entityTypeEngine = new EntityTypeEngine (entityTypeIds);
			var entitySchemaEngine = new EntitySchemaEngine (infrastructure, entityTypeEngine);
			var serviceSchemaEngine = new ServiceSchemaEngine (infrastructure, serviceTableNames);

			this.entityTypeEngine = entityTypeEngine;
			this.entitySchemaEngine = entitySchemaEngine;
			this.serviceSchemaEngine = serviceSchemaEngine;
		}


		internal EntitySchemaEngine EntitySchemaEngine
		{
			get
			{
				return this.entitySchemaEngine;
			}
		}


		internal EntityTypeEngine EntityTypeEngine
		{
			get
			{
				return this.entityTypeEngine;
			}
		}


		internal ServiceSchemaEngine ServiceSchemaEngine
		{
			get
			{
				return this.serviceSchemaEngine;
			}
		}


		public static void Create(DbAccess access, IEnumerable<Druid> entityTypeIds)
		{
			access.ThrowIf (a => a.IsEmpty, "access is empty");
			entityTypeIds.ThrowIfNull ("entityTypeIds");

			System.Func<DbInfrastructure, IList<DbTable>, int> action = (infrastructure, tables) =>
			{
				DbSchemaUpdater.UpdateSchema (infrastructure, tables);
				return 0;
			};

			EntityEngine.ExecuteAction (access, entityTypeIds, action);
		}


		public static bool Check(DbAccess access, IEnumerable<Druid> entityTypeIds)
		{
			access.ThrowIf (a => a.IsEmpty, "access is empty");
			entityTypeIds.ThrowIfNull ("entityTypeIds");

			System.Func<DbInfrastructure, IList<DbTable>, bool> action = (infrastructure, tables) =>

				DbSchemaChecker.CheckSchema (infrastructure, tables);

			return EntityEngine.ExecuteAction (access, entityTypeIds, action);
		}


		public static void Update(DbAccess access, IEnumerable<Druid> entityTypeIds)
		{
			access.ThrowIf (a => a.IsEmpty, "access is empty");
			entityTypeIds.ThrowIfNull ("entityTypeIds");

			System.Func<DbInfrastructure, IList<DbTable>, int> action = (infrastructure, tables) =>
			{
				DbSchemaUpdater.UpdateSchema (infrastructure, tables);
				return 0;
			};

			EntityEngine.ExecuteAction (access, entityTypeIds, action);
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


		private static T ExecuteAction<T>(DbAccess access, IEnumerable<Druid> entityTypeIds, System.Func<DbInfrastructure, IList<DbTable>, T> action)
		{
			using (DbInfrastructure infrastructure = new DbInfrastructure ())
			{
				infrastructure.AttachToDatabase (access);

				var relatedEntityTypeIds = EntityTypeEngine.GetRelatedEntityTypeIds (entityTypeIds).ToList ();

				var typeEngine = new EntityTypeEngine (relatedEntityTypeIds);

				var entityTables = EntitySchemaBuilder.BuildTables (typeEngine);
				var serviceTables = ServiceSchemaBuilder.BuildServiceTables ();

				var tables = entityTables.Concat (serviceTables).ToList ();

				return action (infrastructure, tables);
			}
		}
		

		private readonly EntitySchemaEngine entitySchemaEngine;


		private readonly EntityTypeEngine entityTypeEngine;


		private readonly ServiceSchemaEngine serviceSchemaEngine;


	}


}
