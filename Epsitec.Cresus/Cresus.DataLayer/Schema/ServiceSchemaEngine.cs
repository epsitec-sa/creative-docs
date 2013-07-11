using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types.Collections;

using Epsitec.Cresus.Database;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Schema
{


	/// <summary>
	/// The purpose of this class is to maintain a local cache of all the DbTable instances that
	/// represent an SQL table used by a component in the Infrastructure namespace, such as the
	/// lock manager, the entity modification log, the entity deletion log, etc.
	/// </summary>
	internal sealed class ServiceSchemaEngine
	{


		/*
		 * All the method of this class are thread safe, but the DbTable, DbColumn, etc objects that
		 * it returns are not. There is no formal guarantee whatsoever that they are thread safe.
		 * However, given how these objects are used within the DataLayer project (they are accessed
		 * only for read operations) and that they are not modified by the DataBase project
		 * (they are supposed to be accessed only for read operation and are not supposed to be
		 * modified and that this class calls the appropriate methods so that their internal state
		 * is supposed to be stable at the end of the constructor execution, they can be used in a
		 * thread safe way by the DataLayer project.
		 * However, I repeat, there are no formal guarantees on that. These objects are not
		 * synchronized and are mutable. This is some kind of "we know that it will work, so finger
		 * crossed" situation. And of course, if they are modified in any way, all those assumptions
		 * might turn out to be false and then we'll be screwed up.
		 * Marc
		 */


		public ServiceSchemaEngine(DbInfrastructure dbInfrastructure, IEnumerable<string> tableNames)
		{
			dbInfrastructure.ThrowIfNull ("dbInfrastructure");
			tableNames.ThrowIfNull ("tableNames");

			var tableNamesAsList = tableNames.ToList ();

			tableNamesAsList.ThrowIf (names => names.Any (n => string.IsNullOrEmpty (n)), "talbe names cannot be null or empty");

			using (DbTransaction transaction = dbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
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
