namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// La classe DbFactory permet d'obtenir une référence à une instance du gestionnaire
	/// de données universel (IDbAbstraction), à partir d'une définition d'accès DbAccess.
	/// </summary>
	public class DbFactory
	{
		private DbFactory()
		{
			//	instanciation impossible
		}
		
		public static void Initialise()
		{
			if (DbFactory.Initialised == false)
			{
				DbFactory.Initialised = true;
				
				System.Reflection.Assembly assembly = System.Reflection.Assembly.LoadWithPartialName ("Database.Implementation");
				System.Type[] types_in_assembly = assembly.GetTypes ();
				
				foreach (System.Type type in types_in_assembly)
				{
					if (type.Name.EndsWith ("AbstractionFactory"))
					{
						System.Activator.CreateInstance (type);
					}
				}
			}
		}
		
		public static IDbAbstraction FindDbAbstraction(DbAccess db_access)
		{
			System.Diagnostics.Debug.Assert (db_access.Provider != null);
			
			foreach (IDbAbstractionFactory db_factory in DbFactory.DbFactories)
			{
				if (db_factory.ProviderName == db_access.Provider)
				{
					try
					{
						return db_factory.NewDbAbstraction (db_access);
					}
					catch (DbFactoryException)
					{
						//	mange l'exception
					}
				}
			}
			
			return null;
		}
		
		public static IDbAbstractionFactory FindDbAbstractionFactory(string provider)
		{
			System.Diagnostics.Debug.Assert (provider != null);
			
			foreach (IDbAbstractionFactory db_factory in DbFactory.DbFactories)
			{
				if (db_factory.ProviderName == provider)
				{
					return db_factory;
				}
			}
			
			return null;
		}
		
		public static void DebugDumpRegisteredDbAbstractions()
		{
			foreach (IDbAbstractionFactory entry in DbFactory.DbFactories)
			{
				System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
				
				buffer.Append (entry.ProviderName);
				buffer.Append (": type=");
				buffer.Append (entry.GetType ().FullName);
				buffer.Append (", assembly=");
				buffer.Append (entry.GetType ().Assembly.FullName);
				
				System.Diagnostics.Debug.WriteLine (buffer.ToString ());
			}
		}
		
		public static void RegisterDbAbstraction(IDbAbstractionFactory db_factory)
		{
			System.Diagnostics.Debug.Assert (db_factory != null);
			System.Diagnostics.Debug.Assert (db_factory.ProviderName != null);
			System.Diagnostics.Debug.Assert (DbFactory.DbFactories.Contains (db_factory) == false);
			
			DbFactory.DbFactories.Add (db_factory);
		}
		
		
		private static System.Collections.ArrayList		DbFactories = new System.Collections.ArrayList ();
		private static bool								Initialised = false;
	}
}
