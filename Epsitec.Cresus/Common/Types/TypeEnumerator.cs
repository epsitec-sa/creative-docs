//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Reflection;
using System.Linq;

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
			this.assemblies    = new List<Assembly> ();
			this.typeNames     = new HashSet<string> ();
			this.typeMap       = new Dictionary<string, List<System.Type>> ();
			this.assemblyNames = new HashSet<string> ();
			
			this.namespaceShortcutsShortToFull = new ConcurrentDictionary<string, string> ();
			this.namespaceShortcutsFullToShort = new ConcurrentDictionary<string, string> ();

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
			lock (this.types)
			{
				List<System.Type> list;

				if (this.typeMap.TryGetValue (typeName, out list))
				{
					return list.ToArray ();
				}
				else
				{
					return Epsitec.Common.Types.Collections.EmptyEnumerable<System.Type>.Instance;
				}
			}
		}

		/// <summary>
		/// Finds the type for the given name. If the type cannot be resolved unambiguously,
		/// an exception will be thrown.
		/// </summary>
		/// <param name="typeName">Name of the type.</param>
		/// <returns>The type.</returns>
		/// <exception cref="System.ArgumentException">If type is ambiguous or cannot be resolved.</exception>
		public System.Type FindType(string typeName)
		{
			var types = this.GetTypesFromName (typeName);
			int count = types.Count ();

			if (count == 1)
			{
				return types.First ();
			}

			if (count == 0)
			{
				throw new System.ArgumentException ("Cannot resolve type name");
			}
			else
			{
				throw new System.ArgumentException (string.Format ("Ambiguous type name: found {0} matches", count));
			}
		}

		/// <summary>
		/// Gets the collection of loaded assemblies.
		/// </summary>
		/// <returns>The collection of loaded assemblies.</returns>
		public IEnumerable<Assembly> GetLoadedAssemblies()
		{
			lock (this.assemblies)
			{
				return this.assemblies.ToArray ();
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


		/// <summary>
		/// Shrinks the name of the type, by replacing part of the assembly name by its
		/// shortcut, if one has been defined using an <see cref="NamespaceShortcutAttribute"/>.
		/// </summary>
		/// <param name="name">The full type name.</param>
		/// <returns>The shortened name.</returns>
		public string ShrinkTypeName(string name)
		{
			var keys = this.namespaceShortcutsFullToShort.Keys.ToArray ();
			
			int    length = 0;
			string shortName = "";

			foreach (var key in keys)
			{
				if (name.StartsWith (key))
				{
					if ((key.Length > length) &&
						(name.Length == key.Length || name[key.Length] == '.'))
					{
						length = key.Length;
						shortName = this.namespaceShortcutsFullToShort[key];
					}
				}
			}

			if (name.Length == length)
			{
				return "@" + shortName;
			}
			else if (shortName.Length == 0)
			{
				return name;
			}
			else
			{
				return string.Concat ("@", shortName, name.Substring (length));
			}
		}

		/// <summary>
		/// Unshrinks the name of the type, if it was shortened by <see cref="ShrinkTypeName"/>.
		/// </summary>
		/// <param name="name">The (possibly shortened) name.</param>
		/// <returns>The full name.</returns>
		public string UnshrinkTypeName(string name)
		{
			if (string.IsNullOrEmpty (name))
			{
				return name;
			}

			if (name[0] != '@')
			{
				return name;
			}

			int pos = name.IndexOf ('.');

			if (pos < 0)
			{
				pos = name.Length;
			}

			var shortName = name.Substring (1, pos-1);
			var otherName = name.Substring (pos);
			var fullName  = this.namespaceShortcutsShortToFull[shortName];

			return fullName + otherName;
		}


		private void HandleCurrentDomainAssemblyLoaded(object sender, System.AssemblyLoadEventArgs args)
		{
			//	An additional assembly was just loaded; analyze it and update the cache
			//	accordingly :

			this.AnalyseAssembly (args.LoadedAssembly);
		}

		/// <summary>
		/// Analyses the assembly.
		/// </summary>
		/// <param name="assembly">The assembly.</param>
		private void AnalyseAssembly(Assembly assembly)
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

				var shortcuts = assembly.GetCustomAttributes<NamespaceShortcutAttribute> ();

				lock (this.namespaceShortcutsShortToFull)
				{
					foreach (var shortcut in shortcuts)
					{
						string shortName;
						string fullName;

						if (this.namespaceShortcutsFullToShort.TryGetValue (shortcut.FullName, out shortName))
						{
							System.Diagnostics.Debug.Assert (shortName == shortcut.ShortName);
						}
						if (this.namespaceShortcutsShortToFull.TryGetValue (shortcut.ShortName, out fullName))
						{
							System.Diagnostics.Debug.Assert (fullName == shortcut.FullName);
						}

						this.namespaceShortcutsShortToFull[shortcut.ShortName] = shortcut.FullName;
						this.namespaceShortcutsFullToShort[shortcut.FullName]  = shortcut.ShortName;
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
		private readonly ConcurrentDictionary<string, string> namespaceShortcutsShortToFull;
		private readonly ConcurrentDictionary<string, string> namespaceShortcutsFullToShort;
	}
}
