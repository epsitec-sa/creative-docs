//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 07/10/2003

namespace Epsitec.Cresus.Database
{
	using System.Collections;
	using Epsitec.Common.Support;
	
	/// <summary>
	/// Summary description for InternalCollectionList.
	/// </summary>
	public abstract class InternalCollectionList : InternalCollectionBase
	{
		public InternalCollectionList()
		{
		}
		
		
		protected override ArrayList		List
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
			//	selon un nom. Par contre, les classes qui héritent de InternalCollectionList
			//	fournissent leur propre implémentation.
			
			return -1;
		}
		
		public virtual void Clear()
		{
			this.list.Clear ();
			this.OnChanged ();
		}
		
		
		
		protected virtual void OnChanged()
		{
			if (this.Changed != null)
			{
				this.Changed (this);
			}
		}
		
		public event EventHandler			Changed;
		
		protected ArrayList					list = new System.Collections.ArrayList ();
	}
}
