//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 10/05/2004

namespace Epsitec.Common.Support
{
	using CultureInfo = System.Globalization.CultureInfo;
	
	/// <summary>
	/// La classe ResourceBundleCollection permet de stocker ensemble toutes les variantes
	/// d'un bundle (Default, Localised, Customised).
	/// </summary>
	public class ResourceBundleCollection : System.Collections.ICollection, System.IDisposable, System.Collections.IList
	{
		public ResourceBundleCollection()
		{
			this.list = new System.Collections.ArrayList ();
		}
		
		public ResourceBundleCollection(string prefix, string[] ids) : this ()
		{
			this.LoadBundles (prefix, ids);
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
				return this[level, Resources.Culture];
			}
		}
		
		public ResourceBundle					this[ResourceLevel level, CultureInfo culture]
		{
			get
			{
				foreach (ResourceBundle bundle in this.list)
				{
					if ((bundle.ResourceLevel == level) &&
						((bundle.Culture == culture) || (level == ResourceLevel.Default)))
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
				
				Resources.MapFromSuffix (suffix, out level, out culture);
				
				return this[level, culture];
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
					
					string suffix;
					
					Resources.MapToSuffix (level, culture, out suffix);
					
					list.Add (suffix);
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
			this.Clear ();
			
			foreach (string id in ids)
			{
				string suffix = Resources.ExtractSuffix (id);
				string name   = Resources.StripSuffix (id);
				
				ResourceLevel level;
				CultureInfo   culture;
				
				Resources.MapFromSuffix (suffix, out level, out culture);
				
				System.Diagnostics.Debug.Assert (level != ResourceLevel.None);
				System.Diagnostics.Debug.Assert (culture != null || level == ResourceLevel.Default);
				
				ResourceBundle bundle = Resources.GetBundle (Resources.MakeFullName (prefix, name), level, culture);
				
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
		
		
		private void HandleBundleFieldsChanged(object sender)
		{
			this.OnFieldsChanged ();
		}
		
		
		
		public event EventHandler				FieldsChanged;
		
		
		private System.Collections.ArrayList	list;
	}
}
