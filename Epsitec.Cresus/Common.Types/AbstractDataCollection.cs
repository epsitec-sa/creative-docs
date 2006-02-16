//	Copyright � 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	/// <summary>
	/// La classe AbstractDataCollection d�crit une collection de donn�es IDataItem
	/// et sert de base � UI.Data.Record et � bien d'autres classes.
	/// </summary>
	public abstract class AbstractDataCollection : IDataCollection, System.ICloneable
	{
		protected AbstractDataCollection()
		{
		}
		
		
		#region IDataCollection Members
		public IDataItem						this[string name]
		{
			get
			{
				//	TODO: on pourrait utiliser une table de hachage pour acc�l�rer la recherche
				//	par les noms; � partir de combien d'�l�ments ceci deviendrait-il utile ?
				
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

		#region IEnumerable<IDataItem> Members
		public IEnumerator<IDataItem> GetEnumerator()
		{
			return this.list.GetEnumerator ();
		}
		#endregion

		#region IEnumerable Members
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator ();
		}
		#endregion

		#region ICollection<IDataItem> Members
		public void Clear()
		{
			if (this.list.Count > 0)
			{
				this.list.Clear ();
				this.ClearCachedItemArray ();
			}
		}

		public bool Contains(IDataItem item)
		{
			return this.list.Contains (item);
		}

		public void CopyTo(IDataItem[] array, int arrayIndex)
		{
			this.list.CopyTo (array, arrayIndex);
		}

		public int Count
		{
			get
			{
				return this.list.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public void Add(IDataItem item)
		{
			this.list.Add (item);
			this.ClearCachedItemArray ();
		}

		public bool Remove(IDataItem item)
		{
			if (this.list.Remove (item))
			{
				this.ClearCachedItemArray ();
				return true;
			}
			else
			{
				return false;
			}
		}
		#endregion
		
		#region ICloneable Members
		object System.ICloneable.Clone()
		{
			return this.CloneCopyToNewObject (this.CloneNewObject ());
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
			//	TODO: le jour o� un acc�s aux noms est impl�ment� via une table de hachage, il
			//	faudra purger la table ici...
			
			this.is_dirty = true;
		}
		
		
		protected abstract object CloneNewObject();
		protected virtual object CloneCopyToNewObject(object o)
		{
			//	Les classes qui d�rivent de celle-ci devraient surcharger cette m�thode
			//	pour copier d'�ventuels autres champs importants.
			
			AbstractDataCollection that = o as AbstractDataCollection;
			
			foreach (IDataItem item in this.list)
			{
				that.Add (item.Clone () as IDataItem);
			}
			
			return that;
		}
		
		
		private List<IDataItem>					list = new List<IDataItem> ();
		private bool							is_dirty;
	}
}
