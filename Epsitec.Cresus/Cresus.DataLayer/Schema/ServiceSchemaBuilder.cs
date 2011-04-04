using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Infrastructure;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Schema
{


	internal static class ServiceSchemaBuilder
	{


		// TODO Comment this class
		// Marc


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
