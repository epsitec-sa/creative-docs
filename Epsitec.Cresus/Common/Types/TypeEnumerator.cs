//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Reflection;

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>TypeEnumerator</c> class is used to enumerate all types found in all loaded
	/// assemblies. This class maintains a cache, which helps in speeding up subsequent
	/// queries. The cache is automatically updated if new assemblies get loaded.
	/// </summary>
	public sealed class TypeEnumerator
	{
		public TypeEnumerator()
		{
			this.types      = new List<System.Type> ();
			this.assemblies = new List<System.Reflection.Assembly> ();
			this.typeNames  = new HashSet<string> ();

			System.AppDomain.CurrentDomain.AssemblyLoad += new System.AssemblyLoadEventHandler (this.HandleCurrentDomainAssemblyLoad);

			foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies ())
			{
				this.AnalyseAssembly (assembly);
			}
		}

		public static TypeEnumerator			Instance
		{
			get
			{
				return TypeEnumerator.instance;
			}
		}

		/// <summary>
		/// Gets all currently loaded types. This method is thread safe.
		/// </summary>
		/// <returns>The collection of all currently loaded types.</returns>
		public IEnumerable<System.Type> GetAllTypes()
		{
			int index = 0;

			while (true)
			{
				System.Type type;

				lock (this.types)
				{
					if (index >= this.types.Count)
					{
						yield break;
					}

					type = this.types[index++];
				}

				yield return type;
			}
		}

		private void HandleCurrentDomainAssemblyLoad(object sender, System.AssemblyLoadEventArgs args)
		{
			//	An additional assembly was just loaded; analyze it and update the cache
			//	accordinly :

			this.AnalyseAssembly (args.LoadedAssembly);
		}

		private void AnalyseAssembly(System.Reflection.Assembly assembly)
		{
#if DOTNET35
			if (!assembly.ReflectionOnly)
#else
			if ((!assembly.IsDynamic) &&
				(!assembly.ReflectionOnly))
#endif
			{
				lock (this.assemblies)
				{
					this.assemblies.Add (assembly);
				}

				var types = assembly.GetTypes ();

				lock (this.types)
				{
					this.types.AddRange (types);
				}
			}
		}

		private static readonly TypeEnumerator	instance = new TypeEnumerator ();

		private readonly List<System.Type>		types;
		private readonly List<Assembly>			assemblies;
		private readonly HashSet<string>		typeNames;
	}
}
