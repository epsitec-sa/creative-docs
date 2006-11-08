//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
		public static IDbAbstraction FindDbAbstraction(DbAccess databaseAccess)
		{
			IDbAbstractionFactory factory = DbFactory.FindDbAbstractionFactory (databaseAccess.Provider);

			if (factory != null)
			{
				try
				{
					return factory.NewDbAbstraction (databaseAccess);
				}
				catch (Exceptions.FactoryException)
				{
					//	Eat the factory exception, if any.
				}
			}
			
			return null;
		}

		/// <summary>
		/// Finds the database abstraction factory based on a provider name.
		/// </summary>
		/// <param name="provider">The provider name.</param>
		/// <returns>The database abstraction factory.</returns>
		public static IDbAbstractionFactory FindDbAbstractionFactory(string provider)
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
		public static void DebugDumpRegisteredDbAbstractions()
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
		public static void RegisterDbAbstraction(IDbAbstractionFactory factory)
		{
			System.Diagnostics.Debug.Assert (factory != null);
			System.Diagnostics.Debug.Assert (factory.ProviderName != null);
			System.Diagnostics.Debug.Assert (DbFactory.factories.Contains (factory) == false);
			
			DbFactory.factories.Add (factory);
		}


		static List<IDbAbstractionFactory>		factories = new List<IDbAbstractionFactory> ();
	}
}
