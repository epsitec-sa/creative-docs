//	Copyright © 2006-2013, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Types
{
	using Assembly=System.Reflection.Assembly;
	using Epsitec.Common.Support;
	
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
				var types           = new List<System.Type> ();
				var designerVisible = typeof (DesignerVisibleAttribute).FullName;

				foreach (System.Type type in EnumLister.GetPublicEnums ())
				{
					if (type.GetCustomAttributes (false).Any (x => x.GetType ().FullName == designerVisible))
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
			EnumLister.types = new Dictionary<string, Record> ();

			Assembly[] assemblies = TypeEnumerator.Instance.GetLoadedAssemblies ().ToArray ();

			AssemblyLoader.AssemblyLoaded += EnumLister.HandleDomainAssemblyLoaded;

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

		private static void HandleDomainAssemblyLoaded(object sender, System.AssemblyLoadEventArgs args)
		{
			if (!args.LoadedAssembly.ReflectionOnly)
			{
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

		private static readonly Dictionary<string, Record> types;
		private static int generation;

		[System.ThreadStatic]
		private static System.Type[]			publicEnumCache;
		[System.ThreadStatic]
		private static int						publicEnumCacheGeneration;
		
		[System.ThreadStatic]
		private static System.Type[]			designerVisibleEnumCache;
		[System.ThreadStatic]
		private static int						designerVisibleEnumCacheGeneration;
	}
}
