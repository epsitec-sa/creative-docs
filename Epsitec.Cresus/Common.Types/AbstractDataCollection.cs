//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 26/04/2004

namespace Epsitec.Common.Types
{
	/// <summary>
	/// La classe Record décrit un ensemble de champs utilisés pour échanger des
	/// données entre une application et son interface via mapper/binder/...
	/// </summary>
	public abstract class AbstractDataCollection : IDataCollection
	{
		public AbstractDataCollection()
		{
		}
		
		
		#region IDataCollection Members
		public IDataItem						this[string name]
		{
			get
			{
				//	TODO: on pourrait utiliser une table de hachage pour accélérer la recherche
				//	par les noms; à partir de combien d'éléments ceci deviendrait-il utile ?
				
				IDataItem[] items = this.CachedItemArray;
				
				for (int i = 0; i < items.Length; i++)
				{
					if (items[i].Name == name)
					{
						return items[i];
					}
				}
				
				return null;
			}
		}

		public IDataItem						this[int index]
		{
			get
			{
				IDataItem[] items = this.CachedItemArray;
				return items[index];
			}
		}

		public int IndexOf(IDataItem item)
		{
			IDataItem[] items = this.CachedItemArray;
			
			for (int i = 0; i < items.Length; i++)
			{
				if (items[i] == item)
				{
					return i;
				}
			}
			
			return -1;
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
		
		public object							SyncRoot
		{
			get
			{
				return this;
			}
		}
		
		public void CopyTo(System.Array array, int index)
		{
			this.list.CopyTo (array, index);
		}
		#endregion
		
		#region IEnumerable Members
		public System.Collections.IEnumerator GetEnumerator()
		{
			return this.list.GetEnumerator ();
		}
		#endregion
		
		protected IDataItem[]					CachedItemArray
		{
			get
			{
				if (this.is_dirty)
				{
					this.UpdateCachedItemArray ();
				}
				
				return this.GetCachedItemArray ();
			}
		}
		
		
		protected abstract IDataItem[] GetCachedItemArray();
		
		protected virtual void UpdateCachedItemArray()
		{
			this.is_dirty = false;
		}
		
		protected virtual void ClearCachedItemArray()
		{
			//	TODO: le jour où un accès aux noms est implémenté via une table de hachage, il
			//	faudra purger la table ici...
			
			this.is_dirty = true;
		}
		
		
		protected System.Collections.ArrayList	list = new System.Collections.ArrayList ();
		private bool							is_dirty;
	}
}
