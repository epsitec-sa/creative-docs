using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Infrastructure;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Schema
{


	/// <summary>
	/// The purpose of this class is to build the DbTables instances that represents all the SQL
	/// tables that are used by the tools that relate to the database in the Infrastructure
	/// namespace, such as the tables for the lock manager, , the entity modification log, the
	/// entity deletion log, etc.
	/// </summary>
	internal static class ServiceSchemaBuilder
	{


		public static IEnumerable<string> GetServiceTableNames()
		{
			return ServiceSchemaBuilder.GetServiceTableFactories ().Select (tf => tf.TableName);
		}


		public static IEnumerable<DbTable> BuildServiceTables()
		{
			return ServiceSchemaBuilder.GetServiceTableFactories ().Select (tf => tf.BuildTable ());
		}


		private static IEnumerable<ITableFactory> GetServiceTableFactories()
		{
			yield return ConnectionManager.TableFactory;
			yield return EntityDeletionLog.TableFactory;
			yield return InfoManager.TableFactory;
			yield return LockManager.TableFactory;
			yield return EntityModificationLog.TableFactory;
			yield return UidManager.TableFactory;
		}


	}


	internal interface ITableFactory
	{


		string TableName
		{
			get;
		}


		DbTable BuildTable();


	}


}
