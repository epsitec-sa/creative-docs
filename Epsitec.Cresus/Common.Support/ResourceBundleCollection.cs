//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Support
{
	using CultureInfo = System.Globalization.CultureInfo;
	
	/// <summary>
	/// La classe ResourceBundleCollection permet de stocker ensemble toutes les variantes
	/// d'un bundle (Default, Localised, Customised).
	/// </summary>
	public class ResourceBundleCollection : System.Collections.ICollection, System.IDisposable, System.Collections.IList
	{
		public ResourceBundleCollection(ResourceManager resource_manager)
		{
			this.list    = new System.Collections.ArrayList ();
			this.manager = resource_manager;
		}
		
		
		public ResourceBundle					this[int index]
		{
			get
			{
				if ((index < 0) ||
					(index >= this.list.Count))
				{
					return null;
				}
				
				return this.list[index] as ResourceBundle;
			}
		}
		
		public ResourceBundle					this[ResourceLevel level]
		{
			get
			{
				return this[level, this.manager.DefaultCulture];
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
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}
		
		public string							Prefix
		{
			get
			{
				return Resources.ExtractPrefix (this.name);
			}
		}
		
		public string[]							Suffixes
		{
			get
			{
				System.Collections.ArrayList list = new System.Collections.ArrayList ();
				
				foreach (ResourceBundle bundle in this.list)
				{
					ResourceLevel level   = bundle.ResourceLevel;
					CultureInfo   culture = bundle.Culture;
					
					list.Add (this.manager.MapToSuffix (level, culture));
				}
				
				string[] suffixes = new string[list.Count];
				list.CopyTo (suffixes);
				
				return suffixes;
			}
		}
		
		
		public int Add(ResourceBundle bundle)
		{
			if (bundle != null)
			{
				this.Attach (bundle);
				return this.list.Add (bundle);
			}
			
			return -1;
		}
		
		public void Remove(ResourceBundle bundle)
		{
			int index = this.list.IndexOf (bundle);
			
			if (index < 0)
			{
				throw new System.ArgumentException ("Resource bundle not found in collection.");
			}
			
			this.Detach (bundle);
			this.list.RemoveAt (index);
		}
		
		
		public void LoadBundles(string prefix, string[] ids)
		{
			System.Diagnostics.Debug.Assert (this.FullName != null, "FullName should be defined first.");
			System.Diagnostics.Debug.Assert (this.Prefix == prefix, string.Format ("Prefix mismatch, expecting '{0}', got '{1}'.", this.Prefix, prefix));
			
			this.Clear ();
			
			foreach (string id in ids)
			{
				string suffix = Resources.ExtractSuffix (id);
				string name   = Resources.StripSuffix (id);
				
				ResourceLevel level;
				CultureInfo   culture;
				
				this.manager.MapFromSuffix (suffix, out level, out culture);
				
				System.Diagnostics.Debug.Assert (level != ResourceLevel.None);
				System.Diagnostics.Debug.Assert (culture != null || level == ResourceLevel.Default);
				
				ResourceBundle bundle = this.manager.GetBundle (this.manager.MakeFullName (prefix, name), level, culture);
				
				System.Diagnostics.Debug.Assert (bundle != null);
				
				this.Add (bundle);
			}
		}
		
		
		#region ICollection Members
		public bool								IsSynchronized
		{
			get
			{
				return this.list.IsSynchronized;
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
			this.list.CopyTo (array, index);
		}
		
		public object							SyncRoot
		{
			get
			{
				return this.list.SyncRoot;
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
		
		
		object			System.Collections.IList.this[int index]
		{
			get
			{
				return this[index];
			}
			set
			{
				if (this[index] != value)
				{
					throw new System.InvalidOperationException ("ResourceBundle may not be replaced.");
				}
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
		
		int System.Collections.IList.Add(object value)
		{
			return this.Add (value as ResourceBundle);
		}
		
		void System.Collections.IList.Remove(object value)
		{
			this.Remove (value as ResourceBundle);
		}
		
		public bool Contains(object value)
		{
			return this.list.Contains (value);
		}
		
		public void Clear()
		{
			ResourceBundle[] bundles = new ResourceBundle[this.list.Count];
			this.list.CopyTo (bundles);
			
			foreach (ResourceBundle bundle in bundles)
			{
				this.Remove (bundle);
			}
			
			System.Diagnostics.Debug.Assert (this.list.Count == 0);
		}

		public int IndexOf(object value)
		{
			return this.list.IndexOf (value);
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
		}
		
		protected virtual void Detach(ResourceBundle bundle)
		{
			bundle.FieldsChanged -= new EventHandler (this.HandleBundleFieldsChanged);
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
				this.merged = new System.Collections.ArrayList ();
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
			ResourceBundle model_localised  = this[ResourceLevel.Localised, culture];
			ResourceBundle model_customised = this[ResourceLevel.Customised, culture];
			
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
		
		
		private void HandleBundleFieldsChanged(object sender)
		{
			this.merged = null;
			this.OnFieldsChanged ();
		}
		
		
		
		public event EventHandler				FieldsChanged;
		
		private ResourceManager					manager;
		private System.Collections.ArrayList	merged;
		private System.Collections.ArrayList	list;
		private string							name;
	}
}
