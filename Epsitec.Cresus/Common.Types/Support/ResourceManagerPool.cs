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
	public class ResourceManagerPool
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

		private string								name;
		private string								defaultPrefix;
		private Dictionary<string, ResourceBundle>	bundles = new Dictionary<string, ResourceBundle> ();
		private List<Weak<ResourceManager>>			managers = new List<Weak<ResourceManager>> ();
		private int									mergedBundlesCount;
	}
}
