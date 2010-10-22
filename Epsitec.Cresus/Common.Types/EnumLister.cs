//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
			if ((EnumLister.publicEnumCache == null) ||
				(EnumLister.publicEnumCacheGeneration != EnumLister.generation))
			{
				EnumLister.publicEnumCache = new System.Type[EnumLister.types.Count];
				string[] names = new string[EnumLister.types.Count];

				EnumLister.types.Keys.CopyTo (names, 0);
				System.Array.Sort (names);

				for (int i = 0; i < names.Length; i++)
				{
					EnumLister.publicEnumCache[i] = EnumLister.types[names[i]].Type;
				}

				EnumLister.publicEnumCacheGeneration = EnumLister.generation;
			}

			return EnumLister.publicEnumCache;
		}

		public static IEnumerable<System.Type> GetDesignerVisibleEnums()
		{
			if ((EnumLister.designerVisibleEnumCache == null) ||
				(EnumLister.designerVisibleEnumCacheGeneration != EnumLister.generation))
			{
				List<System.Type> types = new List<System.Type> ();

				foreach (System.Type type in EnumLister.GetPublicEnums ())
				{
					object[] attributes = type.GetCustomAttributes (typeof (DesignerVisibleAttribute), false);
					
					if (attributes.Length > 0)
					{
						types.Add (type);
					}
				}

				EnumLister.designerVisibleEnumCache = types.ToArray ();
				EnumLister.designerVisibleEnumCacheGeneration = EnumLister.generation;
			}

			return EnumLister.designerVisibleEnumCache;
		}

		#region Setup and Run-Time Analysis Methods

		static EnumLister()
		{
			EnumLister.domain     = System.AppDomain.CurrentDomain;
			EnumLister.assemblies = new List<Assembly> ();
			EnumLister.types      = new Dictionary<string, Record> ();

			Assembly[] assemblies = EnumLister.domain.GetAssemblies ();

			EnumLister.domain.AssemblyLoad += EnumLister.HandleDomainAssemblyLoad;

			foreach (Assembly assembly in assemblies)
			{
				EnumLister.Analyze (assembly);
			}
		}

		private static void Analyze(Assembly assembly)
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
						EnumLister.generation++;
					}
				}
			}
		}

		private static void HandleDomainAssemblyLoad(object sender, System.AssemblyLoadEventArgs args)
		{
			if (!args.LoadedAssembly.ReflectionOnly)
			{
				EnumLister.assemblies.Add (args.LoadedAssembly);
				EnumLister.Analyze (args.LoadedAssembly);
			}
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
		private static int generation;

		[System.ThreadStatic]
		private static System.Type[] publicEnumCache;
		[System.ThreadStatic]
		private static int publicEnumCacheGeneration;
		
		[System.ThreadStatic]
		private static System.Type[] designerVisibleEnumCache;
		[System.ThreadStatic]
		private static int designerVisibleEnumCacheGeneration;
	}
}
