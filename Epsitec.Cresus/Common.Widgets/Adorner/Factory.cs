namespace Epsitec.Common.Widgets.Adorner
{
	/// <summary>
	/// La classe Adorner.Factory donne acc�s � l'interface IAdorner actuellement
	/// active. De plus, elle liste et cr�e automatiquement des instances de chaque
	/// classe impl�mentant IAdorner dans l'assembly actuelle...
	/// </summary>
	public class Factory
	{
		Factory()
		{
			//	On ne peut pas instancier Factory !
		}
		
		static Factory()
		{
			Factory.adorner_table = new System.Collections.Hashtable ();
			Factory.adorner_list  = new System.Collections.ArrayList ();
			
			System.Reflection.Assembly assembly = System.Reflection.Assembly.GetAssembly (typeof (Factory));
			
			Factory.AnalyseAssembly (assembly);
			
			Factory.SetActive ("Default");
			
			System.Diagnostics.Debug.Assert (Factory.adorner_table.ContainsKey ("Default"));
			System.Diagnostics.Debug.Assert (Factory.active_adorner != null);
		}
		
		
		internal static int AnalyseAssembly(System.Reflection.Assembly assembly)
		{
			int n = 0;
			
			System.Type[] all_types_in_assembly = assembly.GetTypes ();
			System.Type   i_adorner_type        = typeof (IAdorner);
				
			//	Cherche dans tous les types connus les classes qui impl�mentent l'interface
			//	IAdorner, et cr�e une instance unique de chacune de ces classes.
			
			foreach (System.Type type in all_types_in_assembly)
			{
				if (type.IsClass && type.IsPublic)
				{
					System.Type[] interfaces = type.GetInterfaces ();
					
					if (System.Array.IndexOf (interfaces, i_adorner_type) >= 0)
					{
						if (! Factory.adorner_list.Contains (type.Name))
						{
							Factory.adorner_list.Add (type.Name);
							Factory.adorner_table[type.Name] = System.Activator.CreateInstance (type);
							n++;
						}
					}
				}
			}
			
			return n;
		}
		
		
		public static IAdorner			Active
		{
			get { return Factory.active_adorner; }
		}
		
		public static string[]			AdornerNames
		{
			get
			{
				string[] names = new string[Factory.adorner_list.Count];
				Factory.adorner_list.CopyTo (names);
				return names;
			}
		}
		

		public static bool SetActive(string name)
		{
			IAdorner adorner = Factory.adorner_table[name] as IAdorner;
			
			if (adorner == null)
			{
				return false;
			}
			
			Factory.active_adorner = adorner;
			
			return true;
		}
		
		
		private static IAdorner						active_adorner;
		private static System.Collections.Hashtable	adorner_table;
		private static System.Collections.ArrayList	adorner_list;
	}
}
