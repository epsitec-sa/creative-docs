namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// La classe InternalCollectionBase sert de base aux collections propres à la
	/// base de données.
	/// </summary>
	public abstract class InternalCollectionBase : System.Collections.IEnumerable, System.Collections.ICollection
	{
		public InternalCollectionBase()
		{
		}
		
		
		public virtual System.Collections.ArrayList		List
		{
			get { return null; }
		}
		
		
		public bool										IsSynchronized
		{
			get { return this.List.IsSynchronized; }
		}

		public int										Count
		{
			get { return this.List.Count; }
		}

		public object									SyncRoot
		{
			get { return this.List.SyncRoot; }
		}
		
		
		public System.Collections.IEnumerator GetEnumerator()
		{
			return this.List.GetEnumerator ();
		}
		
		public void CopyTo(System.Array array, int index)
		{
			this.List.CopyTo (array, index);
		}
	}
}
