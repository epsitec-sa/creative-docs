namespace Epsitec.Common.Widgets.Feel
{
	/// <summary>
	/// La classe Feel.Factory donne accès à l'interface IFeel actuellement active.
	/// De plus, elle liste et crée automatiquement des instances de chaque classe
	/// implémentant IFeel dans l'assembly actuelle...
	/// </summary>
	public class Factory
	{
		Factory()
		{
			//	On ne peut pas instancier Factory !
		}
		
		static Factory()
		{
			Factory.feel_table = new System.Collections.Hashtable ();
			Factory.feel_list  = new System.Collections.ArrayList ();
			
			System.Reflection.Assembly assembly = System.Reflection.Assembly.GetAssembly (typeof (Factory));
			
			Factory.AnalyseAssembly (assembly);
			
			Factory.SetActive ("Default");
			
			System.Diagnostics.Debug.Assert (Factory.feel_table.ContainsKey ("Default"));
			System.Diagnostics.Debug.Assert (Factory.active_feel != null);
		}
		
		
		internal static int AnalyseAssembly(System.Reflection.Assembly assembly)
		{
			int n = 0;
			
			System.Type[] all_types_in_assembly = assembly.GetTypes ();
			System.Type   i_feel_type           = typeof (IFeel);
				
			//	Cherche dans tous les types connus les classes qui implémentent l'interface
			//	IFeel, et crée une instance unique de chacune de ces classes.
			
			foreach (System.Type type in all_types_in_assembly)
			{
				if (type.IsClass && type.IsPublic)
				{
					System.Type[] interfaces = type.GetInterfaces ();
					
					if (System.Array.IndexOf (interfaces, i_feel_type) >= 0)
					{
						if (! Factory.feel_list.Contains (type.Name))
						{
							Factory.feel_list.Add (type.Name);
							Factory.feel_table[type.Name] = System.Activator.CreateInstance (type);
							n++;
						}
					}
				}
			}
			
			return n;
		}
		
		
		public static IFeel				Active
		{
			get { return Factory.active_feel; }
		}
		
		public static string			ActiveName
		{
			get
			{
				return Factory.Active.GetType ().Name;
			}
		}
		
		public static string[]			FeelNames
		{
			get
			{
				string[] names = new string[Factory.feel_list.Count];
				Factory.feel_list.CopyTo (names);
				return names;
			}
		}
		

		public static bool SetActive(string name)
		{
			IFeel feel = Factory.feel_table[name] as IFeel;
			
			if (feel == null)
			{
				return false;
			}
			
			if (Factory.active_feel != feel)
			{
				Factory.active_feel = feel;
			}
			
			return true;
		}
		
		
		private static IFeel						active_feel;
		private static System.Collections.Hashtable	feel_table;
		private static System.Collections.ArrayList	feel_list;
	}
}
