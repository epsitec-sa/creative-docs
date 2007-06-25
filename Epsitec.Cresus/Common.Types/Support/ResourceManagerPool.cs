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

		
		public string							PoolName
		{
			get
			{
				return this.name;
			}
		}

		public string							DefaultPrefix
		{
			get
			{
				return this.defaultPrefix;
			}
			set
			{
				this.defaultPrefix = value;
			}
		}

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

		public IEnumerable<ResourceModuleInfo>	Modules
		{
			get
			{
				return this.modules.Values;
			}
		}


		public void AddModuleRootPath(string name, string path)
		{
			System.Diagnostics.Debug.Assert (!string.IsNullOrEmpty (name));
			System.Diagnostics.Debug.Assert (name.StartsWith ("%"));
			System.Diagnostics.Debug.Assert (name.EndsWith ("%"));

			if (string.IsNullOrEmpty (path))
			{
				this.moduleRoots.Remove (name);
			}
			else
			{
				this.moduleRoots[name] = path;
			}
		}

		public string GetRootRelativePath(string path)
		{
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

		public string GetRootAbsolutePath(string path)
		{
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

		public void ScanForModules(string rootPath)
		{
			foreach (string path in ResourceModule.FindModulePaths (this.GetRootAbsolutePath (rootPath)))
			{
				this.GetModuleInfo (path);
			}
		}
		
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

		public ResourceModuleInfo FindModuleInfo(string modulePath)
		{
			modulePath = this.GetRootRelativePath (modulePath);

			ResourceModuleInfo info;
			this.modules.TryGetValue (modulePath, out info);
			return info;
		}

		
		public IList<ResourceModuleInfo> FindModuleInfos(string name)
		{
			return this.FindModuleInfos (delegate (ResourceModuleInfo info) { return info.FullId.Name == name; });
		}
		
		public IList<ResourceModuleInfo> FindModuleInfos(int id)
		{
			return this.FindModuleInfos (delegate (ResourceModuleInfo info) { return info.FullId.Id == id; });
		}

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

		string									name;
		string									defaultPrefix;
		Dictionary<string, ResourceBundle>		bundles = new Dictionary<string, ResourceBundle> ();
		List<Weak<ResourceManager>>				managers = new List<Weak<ResourceManager>> ();
		int										mergedBundlesCount;
		Dictionary<string, ResourceModuleInfo>	modules = new Dictionary<string, ResourceModuleInfo> ();
		Dictionary<string, string>				moduleRoots = new Dictionary<string, string> ();
	}
}
