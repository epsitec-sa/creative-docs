//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

namespace Epsitec.Cresus.UserInterface
{
	/// <summary>
	/// La classe BinderFactory permet d'obtenir l'interface IBinder nécessaire
	/// à la création de la "glu" entre des éléments d'interface graphique et
	/// les données. IBinder est utilisé par DataBinder.
	/// </summary>
	public class BinderFactory
	{
		private BinderFactory()
		{
			//	instanciation impossible
		}
		
		static BinderFactory()
		{
			BinderFactory.Initialise ();
		}
		
		
		protected static void Initialise()
		{
			System.Reflection.Assembly assembly = typeof (BinderFactory).Assembly;
			System.Type[] types_in_assembly = assembly.GetTypes ();
			
			foreach (System.Type type in types_in_assembly)
			{
				if (type.GetInterface ("IBinder") != null)
				{
					System.Activator.CreateInstance (type);
				}
			}
		}
		
		
		public static void RegisterBinder(string name, IBinder binder)
		{
			System.Diagnostics.Debug.Assert (binder != null);
			System.Diagnostics.Debug.Assert (name != null);
			
			BinderFactory.binders[name] = binder;
		}
		
		
		public static IBinder FindBinder(string name)
		{
			System.Diagnostics.Debug.WriteLine (string.Format ("Searching a binder for {0}...", name));
			System.Diagnostics.Debug.Assert (name != null);
			return BinderFactory.binders[name] as IBinder;
		}
		
		public static IBinder FindBinder(DataLayer.DataType type)
		{
			if (type != null)
			{
				return BinderFactory.FindBinder (type.BinderName);
			}
			
			return null;
		}
		
		
		protected static System.Collections.Hashtable	binders = new System.Collections.Hashtable ();
	}
}
