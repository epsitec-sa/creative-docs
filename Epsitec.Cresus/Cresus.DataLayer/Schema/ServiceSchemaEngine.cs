using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types.Collections;

using Epsitec.Cresus.Database;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Schema
{
	

	internal sealed class ServiceSchemaEngine
	{


		// TODO Comment this class
		// Marc


		public ServiceSchemaEngine(DbInfrastructure dbInfrastructure, IEnumerable<string> tableNames)
		{
			dbInfrastructure.ThrowIfNull ("dbInfrastructure");
			tableNames.ThrowIfNull ("tableNames");

			var tableNamesAsList = tableNames.ToList ();

			tableNamesAsList.ThrowIf (names => names.Any (n => string.IsNullOrEmpty (n)), "talbe names cannot be null or empty");

			using (DbTransaction transaction = dbInfrastructure.InheritOrBeginTransaction(DbTransactionMode.ReadOnly))
			{
				this.serviceTableCache = this.ComputeServiceTableCache (dbInfrastructure, tableNames);

				transaction.Commit ();
			}

			this.EnsureReferencedObjectsAreDeserialized ();
		}


		private ReadOnlyDictionary<string, DbTable> ComputeServiceTableCache(DbInfrastructure dbInfrastructure, IEnumerable<string> tableNames)
		{
			return tableNames
				.Select (name => this.ComputeServiceTable (dbInfrastructure, name))
				.ToDictionary (t => t.Name)
				.AsReadOnlyDictionary ();
		}
		

		private DbTable ComputeServiceTable(DbInfrastructure dbInfrastructure, string tableName)
		{
			DbTable table = dbInfrastructure.ResolveDbTable (tableName);

			if (table == null)
			{
				throw new System.ArgumentException ("Table for service " + tableName + " is not defined in the database.");
			}

			return table;
		}


		private static TValue GetFromCache<TKey, TValue>(IDictionary<TKey, TValue> cache, TKey key)
		{
			TValue value;

			if (!cache.TryGetValue (key, out value))
			{
				throw new System.ArgumentException ("Element not found!");
			}

			return value;
		}


		public DbTable GetServiceTable(string tableName)
		{
			tableName.ThrowIfNull ("tableName");

			return ServiceSchemaEngine.GetFromCache (this.serviceTableCache, tableName);
		}


		private void EnsureReferencedObjectsAreDeserialized()
		{
			foreach (DbTable table in this.serviceTableCache.Values)
			{
				table.EnsureIsDeserialized ();
			}
		}


		private readonly ReadOnlyDictionary<string, DbTable> serviceTableCache;


	}


}
