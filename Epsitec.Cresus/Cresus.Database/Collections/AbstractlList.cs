//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections;
using Epsitec.Common.Support;

namespace Epsitec.Cresus.Database.Collections
{
	/// <summary>
	/// Summary description for AbstractList.
	/// </summary>
	public abstract class AbstractList : AbstractBase
	{
		public AbstractList()
		{
		}
		
		
		protected override ArrayList			List
		{
			get { return this.list; }
		}
		
		
		public virtual void Remove(string name)
		{
			int index = this.IndexOf (name);
			
			if (index != -1)
			{
				this.RemoveAt (index);
			}
		}
		
		public virtual void RemoveAt(int index)
		{
			this.OnRemoving (this.list[index]);
			this.list.RemoveAt (index);
			this.OnChanged ();
		}
		
		public virtual bool Contains(string name)
		{
			return this.IndexOf (name) != -1;
		}
		
		public virtual int IndexOf(string name)
		{
			//	Cette méthode retourne toujours -1, car on ne sait pas comment chercher
			//	selon un nom. Par contre, les classes qui héritent de AbstractList
			//	fournissent leur propre implémentation.
			
			return -1;
		}
		
		public virtual void Clear()
		{
			this.list.Clear ();
			this.OnChanged ();
		}
		
		
		protected void InternalAdd(object element)
		{
			this.list.Add (element);
			this.OnInserted (element);
			this.OnChanged ();
		}
		
		protected void InternalAddRange(object[] elements)
		{
			if (elements == null)
			{
				return;
			}
			
			this.list.AddRange (elements);
			foreach (object element in elements)
			{
				this.OnInserted (element);
			}
			this.OnChanged ();
		}
		
		protected void InternalRemove(object element)
		{
			this.OnRemoving (element);
			this.list.Remove (element);
			this.OnChanged ();
		}
		
		
		protected virtual void OnChanged()
		{
			if (this.Changed != null)
			{
				this.Changed (this);
			}
		}
		
		protected virtual void OnInserted(object element)
		{
			if (this.Inserted != null)
			{
				this.Inserted (this, element);
			}
		}
		
		protected virtual void OnRemoving(object element)
		{
			if (this.Removing != null)
			{
				this.Removing (this, element);
			}
		}
		
		
		public event ArgEventHandler			Removing;
		public event ArgEventHandler			Inserted;
		public event EventHandler				Changed;
		
		protected ArrayList						list = new System.Collections.ArrayList ();
	}
}
