//	Copyright � 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 07/10/2003

namespace Epsitec.Cresus.Database
{
	using System.Collections;
	
	/// <summary>
	/// Summary description for InternalCollectionList.
	/// </summary>
	public abstract class InternalCollectionList : InternalCollectionBase
	{
		public InternalCollectionList()
		{
		}
		
		
		public override ArrayList	List
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
		}
		
		public virtual bool Contains(string name)
		{
			return this.IndexOf (name) != -1;
		}
		
		public virtual int IndexOf(string name)
		{
			//	Cette m�thode retourne toujours -1, car on ne sait pas comment chercher
			//	selon un nom. Par contre, les classes qui h�ritent de InternalCollectionList
			//	fournissent leur propre impl�mentation.
			
			return -1;
		}
		
		public virtual void Clear()
		{
			this.list.Clear ();
		}
		
		
		protected ArrayList	list		= new System.Collections.ArrayList ();
	}
}
