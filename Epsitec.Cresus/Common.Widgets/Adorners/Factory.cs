//	Copyright © 2003-2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets.Adorners
{
	/// <summary>
	/// La classe Adorners.Factory donne accès à l'interface IAdorner actuellement
	/// active. De plus, elle liste et crée automatiquement des instances de chaque
	/// classe implémentant IAdorner dans l'assembly actuelle...
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
				
			//	Cherche dans tous les types connus les classes qui implémentent l'interface
			//	IAdorner, et crée une instance unique de chacune de ces classes.
			
			foreach (System.Type type in all_types_in_assembly)
			{
				if (type.IsClass && type.IsPublic && !type.IsAbstract)
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
		
		public static string			ActiveName
		{
			get
			{
				return Factory.Active.GetType ().Name;
			}
		}
		
		public static string[]			AdornerNames
		{
			get
			{
				string[] names = new string[Factory.adorner_list.Count];
				Factory.adorner_list.CopyTo (names);
				System.Array.Sort (names);
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
			
			if (Factory.active_adorner != adorner)
			{
				Factory.active_adorner = adorner;
				
				Window.InvalidateAll (Window.InvalidateReason.AdornerChanged);
			}
			
			return true;
		}
		
		
		private static IAdorner						active_adorner;
		private static System.Collections.Hashtable	adorner_table;
		private static System.Collections.ArrayList	adorner_list;
	}
}
