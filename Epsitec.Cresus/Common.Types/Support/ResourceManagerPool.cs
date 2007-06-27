//	Copyright © 2006-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Common.Support
{
	/// <summary>
	/// The <c>ResourceManagerPool</c> class manages a pool of <see cref="ResourceManager"/>
	/// instances which share the same cache. This ensures that if a resource
	/// bundle is modified by one manager, that modification will be visible to
	/// all other managers in the pool.
	/// </summary>
	public sealed class ResourceManagerPool
	{
		public ResourceManagerPool()
			: this (null)
		{
		}

		public ResourceManagerPool(string name)
		{
			this.name = name;
			this.defaultPrefix = "file";
		}


		/// <summary>
		/// Gets the name of the pool.
		/// </summary>
		/// <value>The name of the pool.</value>
		public string							PoolName
		{
			get
			{
				return this.name;
			}
		}

		/// <summary>
		/// Gets or sets the default prefix for the associated resource
		/// managers.
		/// </summary>
		/// <value>The default prefix.</value>
		public string							DefaultPrefix
		{
			get
			{
				return this.defaultPrefix;
			}
			set
			{
				if (this.defaultPrefix != value)
				{
					if (this.defaultPrefix != null)
					{
						throw new System.InvalidOperationException ("The default prefix may not be changed");
					}

					this.defaultPrefix = value;
				}
			}
		}

		/// <summary>
		/// Gets the managers.
		/// </summary>
		/// <value>The managers.</value>
		public IEnumerable<ResourceManager>		Managers
		{
			get
			{
				Weak<ResourceManager>[] managers = this.managers.ToArray ();

				foreach (Weak<ResourceManager> item in managers)
				{
					ResourceManager manager = item.Target;
					
					if (manager != null)
					{
						yield return manager;
					}
				}
			}
		}

		/// <summary>
		/// Gets the known modules. Call <see cref="ScanForModules"/> to load
		/// all accessible module informations.
		/// </summary>
		/// <value>The modules.</value>
		public IEnumerable<ResourceModuleInfo>	Modules
		{
			get
			{
				return this.modules.Values;
			}
		}

		/// <summary>
		/// Gets the module roots.
		/// </summary>
		/// <value>The module roots, which consist of a symbolic name and an
		/// absolute path.</value>
		public IEnumerable<KeyValuePair <string, string>> ModuleRoots
		{
			get
			{
				return this.moduleRoots;
			}
		}


		public void SetupDefaultRootPaths()
		{
			//	TODO: définir les chemins...

			string execPath = Epsitec.Common.Support.Globals.Directories.Executable;
			
			string debugSuffix   = @"\bin\Debug";
			string releaseSuffix = @"\bin\Release";

			if (execPath.EndsWith (debugSuffix))
			{
				execPath = execPath.Substring (0, execPath.Length - debugSuffix.Length);
			}
			else if (execPath.EndsWith (releaseSuffix))
			{
				execPath = execPath.Substring (0, execPath.Length - releaseSuffix.Length);
			}
			
			this.AddModuleRootPath (ResourceManagerPool.SymbolicNames.Application, execPath);
			this.AddModuleRootPath (ResourceManagerPool.SymbolicNames.Library, "");
			this.AddModuleRootPath (ResourceManagerPool.SymbolicNames.Patches, "");
			this.AddModuleRootPath (ResourceManagerPool.SymbolicNames.Custom, "");
		}

		
		/// <summary>
		/// Adds the named module root path: this records a link between a symbolic
		/// name (such as <c>%app%</c>) and a file system path.
		/// </summary>
		/// <param name="symbolicName">The symbolic name.</param>
		/// <param name="path">The path.</param>
		public void AddModuleRootPath(string symbolicName, string path)
		{
			System.Diagnostics.Debug.Assert (!string.IsNullOrEmpty (symbolicName));
			System.Diagnostics.Debug.Assert (symbolicName.StartsWith ("%"));
			System.Diagnostics.Debug.Assert (symbolicName.EndsWith ("%"));

			if (string.IsNullOrEmpty (path))
			{
				this.moduleRoots.Remove (symbolicName);
			}
			else
			{
				this.moduleRoots[symbolicName] = path;
			}
		}

		/// <summary>
		/// Gets the symbolic root relative path. If a part of the path maps to
		/// one of the symbolic module root paths, then it is replaced with the
		/// symbolic name (e.g. <c>%app%</c>).
		/// </summary>
		/// <param name="path">The absolute path.</param>
		/// <returns>The relative path, possibly with a symbolic path name prefix.</returns>
		public string GetRootRelativePath(string path)
		{
			if (string.IsNullOrEmpty (path))
			{
				return path;
			}

			string root   = "";
			string prefix = "";
			string result = path;

			foreach (KeyValuePair<string, string> item in this.moduleRoots)
			{
				if ((item.Value.Length > root.Length) &&
					(path.StartsWith (item.Value)))
				{
					root   = item.Value;
					prefix = item.Key;
					result = path.Substring (root.Length);
				}
			}

			return string.Concat (prefix, result);
		}

		/// <summary>
		/// Gets the absolute path for a symbolic root relative path. If the path
		/// starts with a symbolic module path name (such as <c>%app%</c>), it
		/// will be expanded to the real file system path.
		/// </summary>
		/// <param name="path">The relative path.</param>
		/// <returns>The absolute path.</returns>
		public string GetRootAbsolutePath(string path)
		{
			if (string.IsNullOrEmpty (path))
			{
				return path;
			}

			string root   = "";
			string prefix = "";
			string result = path;

			foreach (KeyValuePair<string, string> item in this.moduleRoots)
			{
				if ((item.Key.Length > root.Length) &&
					(path.StartsWith (item.Key)))
				{
					root   = item.Key;
					prefix = item.Value;
					result = path.Substring (root.Length);
				}
			}

			return string.Concat (prefix, result);
		}

		
		/// <summary>
		/// Scans for modules in the specified path and all its subfolders.
		/// </summary>
		/// <param name="rootPath">The path.</param>
		public void ScanForModules(string rootPath)
		{
			foreach (string path in ResourceModule.FindModulePaths (this.GetRootAbsolutePath (rootPath)))
			{
				this.GetModuleInfo (path);
			}
		}

		/// <summary>
		/// Scans for all modules, using the module root paths as the starting
		/// points for the recursive exploration.
		/// </summary>
		public void ScanForAllModules()
		{
			foreach (KeyValuePair<string, string> root in this.ModuleRoots)
			{
				if ((string.IsNullOrEmpty (root.Key)) ||
					(string.IsNullOrEmpty (root.Value)))
				{
					continue;
				}

				this.ScanForModules (root.Key);
			}
		}


		/// <summary>
		/// Gets the module info for the specified path. The path can be absolute
		/// or start with a symbolic module root path (such as <c>%app%</c>). If
		/// the information is not in the cache, it will be loaded and the cache
		/// will be updated.
		/// </summary>
		/// <param name="modulePath">The module path.</param>
		/// <returns>The module information or <c>null</c>.</returns>
		public ResourceModuleInfo GetModuleInfo(string modulePath)
		{
			ResourceModuleInfo info;

			modulePath = this.GetRootRelativePath (modulePath);

			if (this.modules.TryGetValue (modulePath, out info))
			{
				return info;
			}

			info = ResourceModule.LoadManifest (this.GetRootAbsolutePath (modulePath));

			if (info != null)
			{
				this.modules[modulePath] = info;
			}
			
			return info;
		}

		/// <summary>
		/// Finds the module info for the specified path. The path can be absolute
		/// or start with a symbolic module root path (such as <c>%app%</c>). If
		/// the information is not in the cache, returns <c>null</c>.
		/// </summary>
		/// <param name="modulePath">The module path.</param>
		/// <returns>The module information or <c>null</c>.</returns>
		public ResourceModuleInfo FindModuleInfo(string modulePath)
		{
			modulePath = this.GetRootRelativePath (modulePath);

			ResourceModuleInfo info;
			this.modules.TryGetValue (modulePath, out info);
			return info;
		}


		/// <summary>
		/// Finds the module infos for the specified module.
		/// </summary>
		/// <param name="name">The module name.</param>
		/// <returns>A (possibly empty) list of <see cref="ResourceModuleInfo"/> instances.</returns>
		public IList<ResourceModuleInfo> FindModuleInfos(string name)
		{
			return this.FindModuleInfos (delegate (ResourceModuleInfo info) { return info.FullId.Name == name; });
		}

		/// <summary>
		/// Finds the module infos for the specified module.
		/// </summary>
		/// <param name="id">The module id.</param>
		/// <returns>
		/// A (possibly empty) list of <see cref="ResourceModuleInfo"/> instances.
		/// </returns>
		public IList<ResourceModuleInfo> FindModuleInfos(int id)
		{
			return this.FindModuleInfos (delegate (ResourceModuleInfo info) { return info.FullId.Id == id; });
		}

		/// <summary>
		/// Finds the module infos for the specified module.
		/// </summary>
		/// <param name="predicate">The search predicate.</param>
		/// <returns>
		/// A (possibly empty) list of <see cref="ResourceModuleInfo"/> instances.
		/// </returns>
		public IList<ResourceModuleInfo> FindModuleInfos(System.Predicate<ResourceModuleInfo> predicate)
		{
			List<ResourceModuleInfo> infos = new List<ResourceModuleInfo> ();

			foreach (ResourceModuleInfo info in this.Modules)
			{
				if (predicate (info))
				{
					infos.Add (info);
				}
			}
			
			return infos;
		}

		/// <summary>
		/// Finds the reference modules. This excludes all patch modules.
		/// </summary>
		/// <returns>The list of reference modules.</returns>
		public IList<ResourceModuleInfo> FindReferenceModules()
		{
			return this.FindModuleInfos (delegate (ResourceModuleInfo info) { return string.IsNullOrEmpty (info.ReferenceModulePath); });
		}

		/// <summary>
		/// Finds all the patch modules for the specified reference module. A patch
		/// module specifies a <see cref="ResourceModuleInfo.ReferenceModulePath"/>
		/// which points to the reference module and shares its name and id.
		/// </summary>
		/// <param name="referenceModule">The reference module.</param>
		/// <returns>The (possibly empty) list of patch modules.</returns>
		public IList<ResourceModuleInfo> FindPatchModuleInfos(ResourceModuleInfo referenceModule)
		{
			string path = this.GetRootRelativePath (referenceModule.FullId.Path);

			if (string.IsNullOrEmpty (path))
			{
				return Types.Collections.EmptyList<ResourceModuleInfo>.Instance;
			}
			else
			{
				return this.FindModuleInfos (
					delegate (ResourceModuleInfo info)
					{
						return (info.FullId.Id == referenceModule.FullId.Id)
							&& (info.FullId.Name == referenceModule.FullId.Name)
							&& (this.GetRootRelativePath (info.ReferenceModulePath) == path);
					});
			}
		}


		#region Internal Methods

		internal void Register(ResourceManager manager)
		{
			manager.SetPool (this);
			this.managers.Add (new Weak<ResourceManager> (manager));
		}

		internal ResourceBundle FindBundle(string key)
		{
			ResourceBundle bundle;

			lock (this.bundles)
			{
				if (this.bundles.TryGetValue (key, out bundle))
				{
					return bundle;
				}
			}
			
			return null;
		}

		internal void AddBundle(string key, ResourceBundle bundle)
		{
			lock (this.bundles)
			{
				if (bundle.ResourceLevel == ResourceLevel.Merged)
				{
					this.mergedBundlesCount++;
				}

				this.bundles[key] = bundle;
			}
		}

		internal void RemoveBundle(string key)
		{
			lock (this.bundles)
			{
				this.bundles.Remove (key);
			}
			
			this.SyncBundleRelatedCaches ();
		}

		internal void Clear()
		{
			lock (this.bundles)
			{
				this.bundles.Clear ();
				this.mergedBundlesCount = 0;
			}

			this.SyncBundleRelatedCaches ();
		}

		internal void ClearMergedBundles()
		{
			if (this.mergedBundlesCount > 0)
			{
				List<string> clear = new List<string> ();

				lock (this.bundles)
				{
					foreach (KeyValuePair<string, ResourceBundle> pair in this.bundles)
					{
						string key    = pair.Key;
						ResourceBundle bundle = pair.Value;

						if (bundle.ResourceLevel == ResourceLevel.Merged)
						{
							clear.Add (key);
						}
					}

					foreach (string key in clear)
					{
						this.bundles.Remove (key);
					}

					this.mergedBundlesCount = 0;
				}
			}

			this.SyncBundleRelatedCaches ();
		}

		internal void RefreshBundle(string key, ResourceBundle bundle)
		{
			this.AddBundle (key, bundle);
			this.SyncBundleRelatedCaches ();
		}

		#endregion

		public override string ToString()
		{
			return string.Format ("{0}:{1}:{2}", this.name ?? "<auto>", this.bundles.Count, this.managers.Count);
		}

		private void SyncBundleRelatedCaches()
		{
			List<ResourceManager> managers = new List<ResourceManager> ();

			lock (this.managers)
			{
				foreach (Weak<ResourceManager> weak in this.managers)
				{
					ResourceManager manager = weak.Target;

					if (manager != null)
					{
						managers.Add (manager);
					}
				}

				if (this.managers.Count != managers.Count)
				{
					this.managers.Clear ();

					foreach (ResourceManager manager in managers)
					{
						this.managers.Add (new Weak<ResourceManager> (manager));
					}
				}
			}

			foreach (ResourceManager manager in managers)
			{
				manager.SyncBundleRelatedCache ();
			}
		}

		
		public static class SymbolicNames
		{
			public const string Application = "%app%";
			public const string Library		= "%lib%";
			public const string Patches		= "%patches%";
			public const string Custom		= "%custom%";
		}
		
		string									name;
		string									defaultPrefix;
		Dictionary<string, ResourceBundle>		bundles = new Dictionary<string, ResourceBundle> ();
		List<Weak<ResourceManager>>				managers = new List<Weak<ResourceManager>> ();
		int										mergedBundlesCount;
		Dictionary<string, ResourceModuleInfo>	modules = new Dictionary<string, ResourceModuleInfo> ();
		Dictionary<string, string>				moduleRoots = new Dictionary<string, string> ();
	}
}
