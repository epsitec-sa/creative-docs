//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	using Assembly=System.Reflection.Assembly;
	
	/// <summary>
	/// 
	/// </summary>
	public static class EnumLister
	{
		public static void Setup()
		{
		}

		public static IEnumerable<System.Type> GetPublicEnums()
		{
			if (EnumLister.cache == null)
			{
				EnumLister.cache = new System.Type[EnumLister.types.Count];
				string[] names = new string[EnumLister.types.Count];

				EnumLister.types.Keys.CopyTo (names, 0);
				System.Array.Sort (names);

				for (int i = 0; i < names.Length; i++)
				{
					EnumLister.cache[i] = EnumLister.types[names[i]].Type;
				}
			}

			return EnumLister.cache;
		}

		#region Setup and Run-Time Analysis Methods

		static EnumLister()
		{
			EnumLister.domain     = System.AppDomain.CurrentDomain;
			EnumLister.assemblies = new List<Assembly> ();
			EnumLister.types      = new Dictionary<string, Record> ();

			Assembly[] assemblies = EnumLister.domain.GetAssemblies ();

			EnumLister.domain.AssemblyLoad += new System.AssemblyLoadEventHandler (EnumLister.HandleDomainAssemblyLoad);

			foreach (Assembly assembly in assemblies)
			{
				EnumLister.Analyse (assembly);
			}
		}

		private static void Analyse(Assembly assembly)
		{
			foreach (System.Type type in assembly.GetTypes ())
			{
				if (type.IsEnum)
				{
					object[] hiddenAttributes = type.GetCustomAttributes (typeof (HiddenAttribute), false);

					if (hiddenAttributes.Length == 0)
					{
						string name = type.FullName;
						Record record = new Record (type);
						EnumLister.types[name] = record;
						EnumLister.cache = null;
					}
				}
			}
		}

		private static void HandleDomainAssemblyLoad(object sender, System.AssemblyLoadEventArgs args)
		{
			EnumLister.assemblies.Add (args.LoadedAssembly);
			EnumLister.Analyse (args.LoadedAssembly);
		}

		#endregion

		#region Private Record Structure

		private struct Record
		{
			public Record(System.Type type)
			{
				this.type = type;
			}

			public System.Type Type
			{
				get
				{
					return this.type;
				}
			}
			
			private System.Type type;
		}

		#endregion

		private static System.AppDomain domain;
		private static List<Assembly> assemblies;
		private static Dictionary<string, Record> types;
		
		[System.ThreadStatic]
		private static System.Type[] cache;
	}
}
