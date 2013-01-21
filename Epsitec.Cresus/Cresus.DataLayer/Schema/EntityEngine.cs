using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Database;

using System;

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


		public static void Create(DbInfrastructure dbInfrastructure, IEnumerable<Druid> entityTypeIds)
		{
			dbInfrastructure.ThrowIfNull ("dbInfrastructure");
			entityTypeIds.ThrowIfNull ("entityTypeIds");

			EntityEngine.ExecuteAction (entityTypeIds, tables =>
			{
				DbSchemaUpdater.UpdateSchema (dbInfrastructure, tables);
				return 0;
			});
		}


		public static bool Check(DbInfrastructure dbInfrastructure, IEnumerable<Druid> entityTypeIds)
		{
			dbInfrastructure.ThrowIfNull ("dbInfrastructure");
			entityTypeIds.ThrowIfNull ("entityTypeIds");

			return EntityEngine.ExecuteAction (entityTypeIds, tables =>
			{
				return DbSchemaChecker.CheckSchema (dbInfrastructure, tables);
			});
		}


		public static void Update(DbInfrastructure dbInfrastructure, IEnumerable<Druid> entityTypeIds)
		{
			dbInfrastructure.ThrowIfNull ("dbInfrastructure");
			entityTypeIds.ThrowIfNull ("entityTypeIds");

			EntityEngine.ExecuteAction (entityTypeIds, tables =>
			{
				DbSchemaUpdater.UpdateSchema (dbInfrastructure, tables);
				return 0;
			});
		}


		public static EntityEngine Connect(DbInfrastructure dbInfrastructure, IEnumerable<Druid> entityTypeIds)
		{
			dbInfrastructure.ThrowIfNull ("dbInfrastructure");
			entityTypeIds.ThrowIfNull ("entityTypeIds");

			var relatedEntityTypeIds = EntityTypeEngine.GetRelatedEntityTypeIds (entityTypeIds);

			return new EntityEngine (dbInfrastructure, relatedEntityTypeIds);
		}


		private static T ExecuteAction<T>(IEnumerable<Druid> entityTypeIds, Func<IList<DbTable>, T> action)
		{
			var relatedEntityTypeIds = EntityTypeEngine.GetRelatedEntityTypeIds (entityTypeIds).ToList ();

			var typeEngine = new EntityTypeEngine (relatedEntityTypeIds);

			var entityTables = EntitySchemaBuilder.BuildTables (typeEngine);
			var serviceTables = ServiceSchemaBuilder.BuildServiceTables ();

			var tables = entityTables.Concat (serviceTables).ToList ();

			return action (tables);
		}
		

		private readonly EntitySchemaEngine entitySchemaEngine;


		private readonly EntityTypeEngine entityTypeEngine;


		private readonly ServiceSchemaEngine serviceSchemaEngine;


	}


}
