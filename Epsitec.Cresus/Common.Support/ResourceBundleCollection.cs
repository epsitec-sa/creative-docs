//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Support
{
	using CultureInfo = System.Globalization.CultureInfo;
	
	/// <summary>
	/// La classe ResourceBundleCollection permet de stocker ensemble toutes les variantes
	/// d'un bundle (Default, Localized, Customized).
	/// </summary>
	public class ResourceBundleCollection : System.Collections.Generic.ICollection<ResourceBundle>, System.Collections.Generic.IList<ResourceBundle>
	{
		public ResourceBundleCollection(ResourceManager resource_manager)
		{
			this.list    = new List<ResourceBundle> ();
			this.manager = resource_manager;
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
					//	L'appelant aimerait avoir la version fusionnée des ressources. Nous
					//	fabriquons ce bundle à la volée si besoin...
					
					return this.GetMerged (culture);
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
			bundle.FieldsChanged += new EventHandler (this.HandleBundleFieldsChanged);
			this.ClearMergedCache ();
		}
		protected virtual void Detach(ResourceBundle bundle)
		{
			bundle.FieldsChanged -= new EventHandler (this.HandleBundleFieldsChanged);
			this.ClearMergedCache ();
		}
		
		protected virtual void OnFieldsChanged()
		{
			if (this.FieldsChanged != null)
			{
				this.FieldsChanged (this);
			}
		}
		
		protected virtual ResourceBundle GetMerged(CultureInfo culture)
		{
			//	Retrouve la version "fusionnée" des divers bundles pour la culture
			//	spécifiée. Si celle-ci a déjà été synthétisée auparavant, on utilise
			//	la version cachée, sinon il faut la générer...
			
			if (this.merged == null)
			{
				this.merged = new List<ResourceBundle> ();
			}
			
			foreach (ResourceBundle bundle in this.merged)
			{
				if (Resources.EqualCultures (bundle.Culture, culture))
				{
					return bundle;
				}
			}
			
			//	Rien trouvé en cache pour cette culture : génère la version fusionnée
			//	par nos propres moyens.
			
			ResourceBundle merged = this.CreateMerged (culture);
			
			if (merged != null)
			{
				this.merged.Add (merged);
			}
			
			return merged;
		}
		protected virtual ResourceBundle CreateMerged(CultureInfo culture)
		{
			System.Diagnostics.Debug.WriteLine (string.Format ("Merging ressource {0} for culture '{1}'.", this.FullName, culture.TwoLetterISOLanguageName));
			
			ResourceBundle model_default    = this[ResourceLevel.Default, culture];
			ResourceBundle model_localised  = this[ResourceLevel.Localized, culture];
			ResourceBundle model_customised = this[ResourceLevel.Customized, culture];
			
			if (model_default == null)
			{
				return null;
			}
			
			ResourceBundle bundle = model_default.Clone ();
			
			bundle.DefineCulture (culture);
			
			if (model_localised != null)
			{
				bundle.Compile (model_localised.CreateXmlAsData ());
			}
			
			if (model_customised != null)
			{
				bundle.Compile (model_customised.CreateXmlAsData ());
			}
			
			return bundle;
		}

		protected virtual void ClearMergedCache()
		{
			this.merged = null;
		}
		
		private void HandleBundleFieldsChanged(object sender)
		{
			this.ClearMergedCache ();
			this.OnFieldsChanged ();
		}

		
		public event EventHandler				FieldsChanged;
		
		private ResourceManager					manager;
		private List<ResourceBundle>			merged;
		private List<ResourceBundle>			list;
		private string							name;
		private string							prefix;
	}
}
