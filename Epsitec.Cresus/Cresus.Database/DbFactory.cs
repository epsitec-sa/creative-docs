//	Copyright � 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 07/10/2003

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// La classe DbFactory permet d'obtenir une r�f�rence � une instance du gestionnaire
	/// de donn�es universel (IDbAbstraction), � partir d'une d�finition d'acc�s DbAccess.
	/// </summary>
	public class DbFactory
	{
		private DbFactory()
		{
			//	instanciation impossible
		}
		
		static DbFactory()
		{
			DbFactory.Initialise ();
		}
		
		
		protected static void Initialise()
		{
			System.Reflection.Assembly assembly = System.Reflection.Assembly.LoadWithPartialName ("Cresus.Database.Implementation");
			System.Type[] types_in_assembly = assembly.GetTypes ();
			
			foreach (System.Type type in types_in_assembly)
			{
				if (type.GetInterface ("IDbAbstractionFactory") != null)
				{
					System.Activator.CreateInstance (type);
				}
			}
		}
		
		
		public static IDbAbstraction FindDbAbstraction(DbAccess db_access)
		{
			System.Diagnostics.Debug.Assert (db_access.Provider != null);
			
			foreach (IDbAbstractionFactory db_factory in DbFactory.db_factories)
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
			
			foreach (IDbAbstractionFactory db_factory in DbFactory.db_factories)
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
			foreach (IDbAbstractionFactory entry in DbFactory.db_factories)
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
			System.Diagnostics.Debug.Assert (DbFactory.db_factories.Contains (db_factory) == false);
			
			DbFactory.db_factories.Add (db_factory);
		}
		
		
		protected static System.Collections.ArrayList	db_factories = new System.Collections.ArrayList ();
	}
}
