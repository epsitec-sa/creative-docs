//	Copyright � 2003-2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Widgets.Adorners
{
	/// <summary>
	/// La classe Adorners.Factory donne acc�s � l'interface IAdorner actuellement
	/// active. De plus, elle liste et cr�e automatiquement des instances de chaque
	/// classe impl�mentant IAdorner dans l'assembly actuelle...
	/// </summary>
	public static class Factory
	{
		static Factory()
		{
			Factory.adornerTable = new Dictionary<string, IAdorner> ();
			
			System.Reflection.Assembly assembly = System.Reflection.Assembly.GetAssembly (typeof (Factory));
			
			Factory.AnalyseAssembly (assembly);
			
			Factory.SetActive ("Default");
			
			System.Diagnostics.Debug.Assert (Factory.adornerTable.ContainsKey ("Default"));
			System.Diagnostics.Debug.Assert (Factory.activeAdorner != null);
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
				if (type.IsClass && type.IsPublic && !type.IsAbstract)
				{
					System.Type[] interfaces = type.GetInterfaces ();
					
					if (System.Array.IndexOf (interfaces, i_adorner_type) >= 0)
					{
						if (! Factory.adornerTable.ContainsKey (type.Name))
						{
							Factory.adornerTable[type.Name] = (IAdorner) System.Activator.CreateInstance (type);
							n++;
						}
					}
				}
			}
			
			return n;
		}


		public static IAdorner					Active
		{
			get
			{
				return Factory.activeAdorner;
			}
		}
		
		public static string					ActiveName
		{
			get
			{
				return Factory.Active.GetType ().Name;
			}
		}
		
		public static string[]					AdornerNames
		{
			get
			{
				string[] names = new string[Factory.adornerTable.Count];
				Factory.adornerTable.Keys.CopyTo (names, 0);
				System.Array.Sort (names);
				return names;
			}
		}


		public static bool SetActive(string name)
		{
			IAdorner adorner;

			if (Factory.adornerTable.TryGetValue (name, out adorner))
			{
				if (Factory.activeAdorner != adorner)
				{
					Factory.activeAdorner = adorner;
					
					Drawing.Color.DefineCaptionColor (adorner.ColorCaption);

					Window.InvalidateAll (Window.InvalidateReason.AdornerChanged);
				}

				return true;
			}
			else
			{
				return false;
			}
		}
		
		
		static IAdorner							activeAdorner;
		static Dictionary<string, IAdorner>		adornerTable;
	}
}
