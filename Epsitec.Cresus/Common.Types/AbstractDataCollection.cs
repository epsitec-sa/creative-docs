//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	/// <summary>
	/// La classe AbstractDataCollection décrit une collection de données IDataItem
	/// et sert de base à UI.Data.Record et à bien d'autres classes.
	/// </summary>
	public abstract class AbstractDataCollection : IDataCollection, System.ICloneable
	{
		protected AbstractDataCollection()
		{
		}

		public virtual void AddRange(System.Collections.ICollection items)
		{
			foreach (IDataItem item in items)
			{
				this.list.Add (item);
			}
			
			this.ClearCachedItemArray ();
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
		public virtual void Clear()
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

		public virtual void Add(IDataItem item)
		{
			this.list.Add (item);
			this.ClearCachedItemArray ();
		}

		public virtual bool Remove(IDataItem item)
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
				if (this.isDirty)
				{
					this.UpdateCachedItemArray ();
				}
				
				return this.GetCachedItemArray ();
			}
		}
		
		
		protected abstract IDataItem[] GetCachedItemArray();
		
		protected virtual void UpdateCachedItemArray()
		{
			this.isDirty = false;
		}
		
		protected virtual void ClearCachedItemArray()
		{
			//	TODO: le jour où un accès aux noms est implémenté via une table de hachage, il
			//	faudra purger la table ici...
			
			this.isDirty = true;
		}
		
		
		protected abstract object CloneNewObject();
		protected virtual object CloneCopyToNewObject(object o)
		{
			//	Les classes qui dérivent de celle-ci devraient surcharger cette méthode
			//	pour copier d'éventuels autres champs importants.
			
			AbstractDataCollection that = o as AbstractDataCollection;
			
			foreach (IDataItem item in this.list)
			{
				that.Add (item.Clone () as IDataItem);
			}
			
			return that;
		}
		
		
		private List<IDataItem>					list = new List<IDataItem> ();
		private bool							isDirty;
	}
}
