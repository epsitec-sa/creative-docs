//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Support
{
	using CultureInfo = System.Globalization.CultureInfo;
	
	/// <summary>
	/// La classe ResourceBundleCollection permet de stocker ensemble toutes les variantes
	/// d'un bundle (Default, Localized, Customized).
	/// </summary>
	public class ResourceBundleCollection : ICollection<ResourceBundle>, IList<ResourceBundle>
	{
		public ResourceBundleCollection(ResourceManager resourceManager)
		{
			this.list    = new List<ResourceBundle> ();
			this.manager = resourceManager;
		}

		public ResourceBundle					this[int index]
		{
			get
			{
				return this.list[index];
			}
			set
			{
				if (this[index] != value)
				{
					throw new System.InvalidOperationException ("ResourceBundle may not be replaced.");
				}
			}
		}
		public ResourceBundle					this[ResourceLevel level]
		{
			get
			{
				return this[level, this.manager.ActiveCulture];
			}
		}
		public ResourceBundle					this[ResourceLevel level, CultureInfo culture]
		{
			get
			{
				if (level == ResourceLevel.Merged)
				{
					//	L'appelant aimerait avoir la version fusionnée des ressources.
					//	Vu que le ResourceManager maintient un cache actif, il suffit
					//	d'utiliser ce dernier :
					
					return this.manager.GetBundle (this.FullName, level, culture);
				}
				
				foreach (ResourceBundle bundle in this.list)
				{
					if (Resources.EqualCultures (bundle.ResourceLevel, bundle.Culture, level, culture))
					{
						return bundle;
					}
				}
				
				return null;
			}
		}
		public ResourceBundle					this[string suffix]
		{
			get
			{
				ResourceLevel level;
				CultureInfo   culture;
				
				this.manager.MapFromSuffix (suffix, out level, out culture);
				
				return this[level, culture];
			}
		}
		
		public string							FullName
		{
			get
			{
				return this.manager.NormalizeFullId (this.prefix, this.name);
			}
		}
		public string							Name
		{
			get
			{
				return this.name;
			}
		}
		public string							Prefix
		{
			get
			{
				return this.prefix;
			}
		}
		public string[]							Suffixes
		{
			get
			{
				List<string> list = new List<string> ();
				
				foreach (ResourceBundle bundle in this.list)
				{
					ResourceLevel level   = bundle.ResourceLevel;
					CultureInfo   culture = bundle.Culture;
					
					list.Add (this.manager.MapToSuffix (level, culture));
				}
				
				return list.ToArray ();
			}
		}

		public void LoadBundles(string prefix, string[] ids)
		{
			if (ids.Length > 0)
			{
				string name = Resources.StripSuffix (Resources.ExtractName (ids[0]));
				string[] suffixes = new string[ids.Length];
				suffixes[0] = Resources.ExtractSuffix (ids[0]);
				
				for (int i = 1; i < ids.Length; i++)
				{
					if (Resources.StripSuffix (Resources.ExtractName (ids[i])) != name)
					{
						throw new System.ArgumentException (string.Format ("Invalid name '{0}' in argument ids[{1}]", Resources.StripSuffix (Resources.ExtractName (ids[i])), i));
					}
					
					suffixes[i] = Resources.ExtractSuffix (ids[i]);
				}

				this.LoadBundles (prefix, name, suffixes);
			}
		}
		public void LoadBundles(string prefix, string name, string[] suffixes)
		{
			this.Clear ();

			this.name   = name;
			this.prefix = prefix;

			string fullName = this.FullName;
			
			foreach (string suffix in suffixes)
			{
				ResourceLevel level;
				CultureInfo   culture;
				
				this.manager.MapFromSuffix (suffix, out level, out culture);
				
				System.Diagnostics.Debug.Assert (level != ResourceLevel.None);
				System.Diagnostics.Debug.Assert (culture != null || level == ResourceLevel.Default);
				
				ResourceBundle bundle = this.manager.GetBundle (fullName, level, culture);
				
				System.Diagnostics.Debug.Assert (bundle != null);
				
				this.Add (bundle);
			}
		}

		/// <summary>
		/// Gets the two letter ISO language names for all loaded bundles in
		/// this bundle collection.
		/// </summary>
		/// <returns>A collection of two letter ISO language names.</returns>
		public IList<string> GetTwoLetterISOLanguageNames()
		{
			string[] names = new string[this.list.Count];

			for (int i = 0; i < this.list.Count; i++)
			{
				names[i] = Resources.GetTwoLetterISOLanguageName (this.list[i].Culture);
			}

			return names;
		}

		#region ICollection<ResourceBundle> Members
		
		public void Add(ResourceBundle item)
		{
			if (item == null)
			{
				throw new System.ArgumentNullException ();
			}

			this.Attach (item);
			this.list.Add (item);
		}
		public bool Remove(ResourceBundle item)
		{
			int index = this.list.IndexOf (item);

			if (index < 0)
			{
				return false;
			}

			this.Detach (item);
			this.list.RemoveAt (index);

			return true;
		}

		public bool Contains(ResourceBundle item)
		{
			return this.list.Contains (item);
		}

		public void CopyTo(ResourceBundle[] array, int arrayIndex)
		{
			this.list.CopyTo (array, arrayIndex);
		}

		#endregion

		#region IEnumerable<ResourceBundle> Members

		IEnumerator<ResourceBundle> IEnumerable<ResourceBundle>.GetEnumerator()
		{
			return this.list.GetEnumerator ();
		}

		#endregion

		#region IList<ResourceBundle> Members

		public int IndexOf(ResourceBundle item)
		{
			return this.list.IndexOf (item);
		}

		public void Insert(int index, ResourceBundle item)
		{
			if (item == null)
			{
				throw new System.ArgumentNullException ();
			}

			this.Attach (item);
			this.list.Insert (index, item);
		}
		#endregion
		
		#region ICollection Members
		public bool								IsSynchronized
		{
			get
			{
				return false;
			}
		}
		
		public int								Count
		{
			get
			{
				return this.list.Count;
			}
		}
		
		public void CopyTo(System.Array array, int index)
		{
			this.list.ToArray ().CopyTo (array, index);
		}
		
		public object							SyncRoot
		{
			get
			{
				return this.list;
			}
		}
		#endregion
		
		#region IList Members
		public bool								IsReadOnly
		{
			get
			{
				return true;
			}
		}

		public bool								IsFixedSize
		{
			get
			{
				return false;
			}
		}

		
		public void RemoveAt(int index)
		{
			this.Remove (this[index]);
		}

		public void Insert(int index, object value)
		{
			ResourceBundle bundle = value as ResourceBundle;
			
			if (bundle != null)
			{
				this.Attach (bundle);
				this.list.Insert (index, bundle);
			}
		}
		
		public bool Contains(object value)
		{
			return this.list.Contains (value as ResourceBundle);
		}
		
		public void Clear()
		{
			ResourceBundle[] bundles = this.list.ToArray ();
			
			foreach (ResourceBundle bundle in bundles)
			{
				this.Remove (bundle);
			}
			
			System.Diagnostics.Debug.Assert (this.list.Count == 0);
		}
		#endregion
		
		#region IEnumerable Members
		public System.Collections.IEnumerator GetEnumerator()
		{
			return this.list.GetEnumerator ();
		}
		#endregion
		
		#region IDisposable Members
		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}
		#endregion
		
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.list.Count > 0)
				{
					this.Clear ();
				}
				
				System.Diagnostics.Debug.Assert (this.list.Count == 0);
				
				this.list = null;
			}
		}
		
		protected virtual void Attach(ResourceBundle bundle)
		{
			bundle.FieldsChanged += this.HandleBundleFieldsChanged;
			this.ClearMergedBundles ();
		}
		protected virtual void Detach(ResourceBundle bundle)
		{
			bundle.FieldsChanged -= this.HandleBundleFieldsChanged;
			this.ClearMergedBundles ();
		}

		protected virtual void ClearMergedBundles()
		{
			this.manager.ClearMergedBundlesFromBundleCache ();
		}
		
		protected virtual void OnFieldsChanged()
		{
			if (this.FieldsChanged != null)
			{
				this.FieldsChanged (this);
			}
		}

		private void HandleBundleFieldsChanged(object sender)
		{
			this.ClearMergedBundles ();
			this.OnFieldsChanged ();
		}
		
		public event EventHandler				FieldsChanged;
		
		private ResourceManager					manager;
		private List<ResourceBundle>			list;
		private string							name;
		private string							prefix;
	}
}
