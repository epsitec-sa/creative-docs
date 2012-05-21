//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

using Epsitec.Common.Support;

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>DbFactory</c> class gives access to the universal database abstractions,
	/// accessible through the <c>IDbAbstraction</c> interface.
	/// </summary>
	public static class DbFactory
	{
		static DbFactory()
		{
			DbFactory.Initialize ();
		}
		
		private static void Initialize()
		{
			System.Reflection.Assembly assembly = AssemblyLoader.Load ("Cresus.Database.Implementation");

			foreach (System.Type type in assembly.GetTypes ())
			{
				if ((type.IsClass) &&
					(type.GetInterface ("IDbAbstractionFactory") != null))
				{
					System.Activator.CreateInstance (type);
				}
			}
		}

		/// <summary>
		/// Finds the database abstraction for a given database access.
		/// </summary>
		/// <param name="databaseAccess">The database access.</param>
		/// <returns>
		/// The database abstraction or <c>null</c> if there is no supported means of
		/// connecting to the specified database.
		/// </returns>
		public static IDbAbstraction CreateDatabaseAbstraction(DbAccess databaseAccess)
		{
			IDbAbstractionFactory factory = DbFactory.FindDatabaseAbstractionFactory (databaseAccess.Provider);

			if (factory != null)
			{
				try
				{
					return factory.CreateDatabaseAbstraction (databaseAccess);
				}
				catch (Exceptions.FactoryException)
				{
					//	Eat the factory exception, if any.
				}
			}
			
			return null;
		}

		/// <summary>
		/// Gets the database file paths.
		/// </summary>
		/// <param name="databaseAccess">The database access.</param>
		/// <returns>A collection of file paths which make up the database on disk
		/// or <c>null</c>.</returns>
		public static IEnumerable<string> GetDatabaseFilePaths(DbAccess databaseAccess)
		{
			IDbAbstractionFactory factory = DbFactory.FindDatabaseAbstractionFactory (databaseAccess.Provider);

			if (factory != null)
			{
				return factory.QueryDatabaseFilePaths (databaseAccess);
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Finds the database abstraction factory based on a provider name.
		/// </summary>
		/// <param name="provider">The provider name.</param>
		/// <returns>The database abstraction factory.</returns>
		public static IDbAbstractionFactory FindDatabaseAbstractionFactory(string provider)
		{
			if (string.IsNullOrEmpty (provider))
			{
				throw new System.ArgumentNullException ("Null database provider specified");
			}
			
			foreach (IDbAbstractionFactory factory in DbFactory.factories)
			{
				if (factory.ProviderName == provider)
				{
					return factory;
				}
			}
			
			return null;
		}

		/// <summary>
		/// Dumps the registered database abstractions.
		/// </summary>
		public static void DebugDumpRegisteredDatabaseAbstractions()
		{
			foreach (IDbAbstractionFactory factory in DbFactory.factories)
			{
				System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
				
				buffer.Append (factory.ProviderName);
				buffer.Append (": type=");
				buffer.Append (factory.GetType ().FullName);
				buffer.Append (", assembly=");
				buffer.Append (factory.GetType ().Assembly.FullName);
				
				System.Diagnostics.Debug.WriteLine (buffer.ToString ());
			}
		}

		/// <summary>
		/// Registers the specified database abstraction factory.
		/// </summary>
		/// <param name="factory">The database factory abstraction.</param>
		public static void RegisterDatabaseAbstraction(IDbAbstractionFactory factory)
		{
			System.Diagnostics.Debug.Assert (factory != null);
			System.Diagnostics.Debug.Assert (factory.ProviderName != null);
			System.Diagnostics.Debug.Assert (DbFactory.factories.Contains (factory) == false);
			
			DbFactory.factories.Add (factory);
		}


		static List<IDbAbstractionFactory>		factories = new List<IDbAbstractionFactory> ();
	}
}
