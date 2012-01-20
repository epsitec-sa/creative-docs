//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
			this.types         = new List<System.Type> ();
			this.assemblies    = new List<System.Reflection.Assembly> ();
			this.typeNames     = new HashSet<string> ();
			this.typeMap       = new Dictionary<string, List<System.Type>> ();
			this.assemblyNames = new HashSet<string> ();

			Epsitec.Common.Support.AssemblyLoader.AssemblyLoaded += this.HandleCurrentDomainAssemblyLoaded;

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
		/// Gets the types which match the specified short type name (for instance <c>"System.Object"</c>).
		/// There will be more than one type if the same type was declared in different assemblies.
		/// </summary>
		/// <param name="typeName">Name of the type.</param>
		/// <returns>The collection of types, which will be empty if none was found.</returns>
		public IEnumerable<System.Type> GetTypesFromName(string typeName)
		{
			List<System.Type> list;

			if (this.typeMap.TryGetValue (typeName, out list))
			{
				return list.AsReadOnly ();
			}
			else
			{
				return Epsitec.Common.Types.Collections.EmptyEnumerable<System.Type>.Instance;
			}
		}

		/// <summary>
		/// Gets the collection of loaded assemblies.
		/// </summary>
		/// <returns>The collection of loaded assemblies.</returns>
		public IEnumerable<Assembly> GetLoadedAssemblies()
		{
			return this.assemblies.AsReadOnly ();
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

		private void HandleCurrentDomainAssemblyLoaded(object sender, System.AssemblyLoadEventArgs args)
		{
			//	An additional assembly was just loaded; analyze it and update the cache
			//	accordingly :

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
					if (this.assemblyNames.Add (assembly.FullName))
					{
						System.Diagnostics.Debug.WriteLine ("TypeEnumerator: analyzing assembly " + assembly.FullName);
						this.assemblies.Add (assembly);
					}
					else
					{
						System.Diagnostics.Debug.WriteLine ("TypeEnumerator: skipping assembly " + assembly.FullName);
						return;
					}
				}

				var types = assembly.GetTypes ();

				lock (this.types)
				{
					foreach (var type in types)
					{
						var name = type.FullName;

						if (this.typeNames.Add (type.AssemblyQualifiedName))
						{
							this.types.Add (type);

							List<System.Type> list;
							
							if (this.typeMap.TryGetValue (name, out list))
							{
							}
							else
							{
								list = new List<System.Type> ();
								this.typeMap[name] = list;
							}

							list.Add (type);
						}
						else
						{
							System.Diagnostics.Debug.Fail (string.Format ("TypeEnumerator: found duplicate type '{0}'", name));
						}
					}
				}
			}
		}

		private static readonly TypeEnumerator	instance = new TypeEnumerator ();

		private readonly List<System.Type>		types;
		private readonly List<Assembly>			assemblies;
		private readonly HashSet<string>		assemblyNames;
		private readonly HashSet<string>		typeNames;
		private readonly Dictionary<string, List<System.Type>> typeMap;
	}
}
