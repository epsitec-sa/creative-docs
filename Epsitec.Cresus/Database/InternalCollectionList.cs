namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// Summary description for InternalCollectionList.
	/// </summary>
	public abstract class InternalCollectionList : InternalCollectionBase
	{
		public InternalCollectionList()
		{
		}
		
		
		public override System.Collections.ArrayList	List
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
			return -1;
		}
		
		public virtual void Clear()
		{
			this.list.Clear ();
		}
		
		
		protected System.Collections.ArrayList	list	= new System.Collections.ArrayList ();
	}
}
